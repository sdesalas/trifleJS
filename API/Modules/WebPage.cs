using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Windows.Forms;
using mshtml;

namespace TrifleJS.API.Modules
{
    /// <summary>
    /// Encapsulates a webpage opened inside IE environment
    /// </summary>
    public class WebPage
    {
        private EnhancedBrowser browser;

        public WebPage() {
            this.browser = new EnhancedBrowser();
            this.browser.Size = new Size(400, 300); 
            this.browser.ScrollBarsEnabled = false;
            this.browser.ObjectForScripting = new Callback.External(this);
            if (this.url == "about:blank") {
                this.AddToolset();
            }
            // Wait for about blank
            while (loading) {
                Program.DoEvents();
            }
            // Initialize properties
            this.customHeaders = new Dictionary<string, object>();
            this.zoomFactor = 1;
            this.clipRect = new Dictionary<string, object>() {
                { "top", 0 },
                { "left", 0 },
                { "width", 0 },
                { "height", 0 }
            };
        }

        #region Main Properties (title, url etc)

        /// <summary>
        /// Returns the title of the current page
        /// </summary>
        public string title {
            get { return (this.browser != null) ? this.browser.DocumentTitle : String.Empty; }
        }

        /// <summary>
        /// Returns the current url
        /// </summary>
        public string url {
            get { return (this.browser != null && this.browser.Url != null) ? this.browser.Url.AbsoluteUri : String.Empty; }
        }

        /// <summary>
        /// Gets the HTML content of the document
        /// </summary>
        public string content {
            get { return (this.browser != null) ? this.browser.DocumentText : String.Empty; }
        }

        /// <summary>
        /// Gets the Plain Text content of the document
        /// </summary>
        public string plainText {
            get { return (this.browser != null 
                        && this.browser.Document != null 
                        && this.browser.Document.Body != null 
                        && this.browser.Document.Body.Parent != null
                        && this.browser.Document.Body.Parent.OuterText != null) ?
                            this.browser.Document.Body.Parent.OuterText:
                            String.Empty; }
        }

        /// <summary>
        /// List of key/value pairs for custom headers to send to the server
        /// </summary>
        public Dictionary<string, object> customHeaders { get; set; }

        /// <summary>
        /// A string of HTTP headers
        /// </summary>
        private string ParsedHeaders {
            get {
                StringBuilder output = new StringBuilder();
                if (customHeaders != null)
                {
                    foreach (string key in customHeaders.Keys)
                    {
                        output.AppendLine(String.Format("{0}:{1}\r\n", key, customHeaders[key]));
                    }
                }
                return output.ToString();
            }
        }

        #endregion

        #region Scripting

        /// <summary>
        /// Adds resources needed after loading page
        /// </summary>
        private void AddToolset()
        {
            // Add toolset
            _evaluateJavaScript(TrifleJS.Properties.Resources.ie_json2);
            _evaluateJavaScript(TrifleJS.Properties.Resources.ie_tools);
        }

        /// <summary>
        /// Handles an exception in the IE runtime
        /// </summary>
        /// <param name="ex"></param>
        private void Handle(string description, int line, Uri uri)
        {
            bool isHandled = false;
            // Check the page.onError() handler to see if we need to run it
            try
            {
                string script = String.Format("WebPage.onError(\"{0}\", {1}, \"{2}\");", description, line, uri);
                isHandled = Convert.ToBoolean(Program.Context.Run(script, "WebPage.onError()"));
            }
            catch (Exception ex2)
            {
                // Problems?
                API.Context.Handle(ex2);
                isHandled = true;
            }
            // Output to console if we havent handled it yet
            if (!isHandled)
            {
                Console.error(String.Format("{0}:{1}({2}): {3}", "IE", uri.AbsoluteUri, line, description));
            }
        }

        /// <summary>
        /// Closes the page and releases memory
        /// </summary>
        public void close() {
            throw new Exception("not implemented");
            callbackStack.Clear();
            browser.Dispose();
            browser = null;
        }

        /// <summary>
        /// Evaluates (executes) javascript on the currently open window
        /// @see http://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
        /// </summary>
        /// <param name="code">code to inject into browser window</param>
        public void _evaluateJavaScript(string code)
        {
            HtmlElementCollection head = browser.Document.GetElementsByTagName("head");
            if (head != null)
            {
                try
                {
                    HtmlElement scriptEl = browser.Document.CreateElement("script");
                    IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.text = code;
                    head[0].AppendChild(scriptEl);
                }
                catch (Exception ex){
                    Utils.Debug(ex.Message);
                }
            }
        }

        /// <summary>
        /// Evaluates (executes) a javascript function and returns result
        /// </summary>
        /// <param name="function">javascript function to execute</param>
        /// <param name="args">arguments to pass the the function</param>
        /// <returns></returns>
        public object _evaluate(string function, object[] args)
        {
            string[] input;
            if (args == null) { input = new string[] {}; }
            else { input = Callback.Parse(args); }
            string guid = "__" + (Guid.NewGuid()).ToString().Replace("-", "");
            string script = String.Format("function {0}() {{ return ({1})({2}); }}", guid, function, String.Join(",", input));
            _evaluateJavaScript(script);
            object result = browser.Document.InvokeScript(guid);
            return result;
        }

        /// <summary>
        /// Includes a JavaScript file from external URL and executes a 
        /// callback when ready
        /// </summary>
        /// <param name="url">URL of javascript file</param>
        /// <param name="callbackId"></param>
        public void _includeJs(string url, string callbackId)
        {
            Uri uri = Browser.TryParse(url);
            if (uri != null)
            {
                HtmlElementCollection head = browser.Document.GetElementsByTagName("head");
                if (head != null)
                {
                    HtmlElement scriptEl = browser.Document.CreateElement("script");
                    IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                    element.src = url;
                    element.type = "text/javascript";
                    head[0].AppendChild(scriptEl);
                    // Listen for readyState changes
                    // @see http://stackoverflow.com/questions/9110388/web-browser-control-how-to-capture-document-events
                    ((mshtml.HTMLScriptEvents2_Event)element).onreadystatechange += delegate
                    {
                        if (element.readyState == "complete" || element.readyState == "loaded")
                        {
                            Callback.ExecuteOnce(callbackId);
                        }
                    };
                }
            }
        }

        /// <summary>
        /// Injects a JavaScript file into current active window
        /// </summary>
        /// <param name="filename">path of the javascript file to inject</param>
        public void _injectJs(string filename) {
            if (File.Exists(filename)) {
                _evaluateJavaScript(File.ReadAllText(filename));
            }
        }

        #region Navigation

        private Stack<string> history = new Stack<string>();
        private int historyPosition = 0;

        /// <summary>
        /// Opens a URL using a specific HTTP method with a corresponding payload
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="callbackId"></param>
        public void _open(string url, string method, string data, string callbackId)
        {
            // Check the URL
            Uri uri = Browser.TryParse(url);
            if (uri != null)
            {
                // Navigate to URL and set handler for completion
                // Remove any DocumentCompleted listeners from last round
                browser.Navigate(uri, method, data, ParsedHeaders);
                browser.DocumentCompleted -= DocumentCompleted;
                browser.DocumentCompleted += DocumentCompleted;
                // Add callback to execution stack
                AddCallback(callbackId, "success");
                // Check if we are not already in the event loop
                if (!Program.InEventLoop)
                {
                    // Loading?
                    while (loading || browser.StatusText != "Done")
                    {
                        // Run events while waiting
                        Trifle.Wait(50);
                    }
                }
            }
            else
            {
                Console.log("Error opening url: " + url);
            }
        }

        /// <summary>
        /// A stack of callbacks in the V8 context that need to
        /// be executed (or are in the process of being executed)
        /// </summary>
        private static Stack<KeyValuePair<string, object[]>> callbackStack = new Stack<KeyValuePair<string, object[]>>();

        /// <summary>
        /// Adds a V8 callback to the execution stack
        /// </summary>
        /// <param name="id"></param>
        /// <param name="args"></param>
        private static void AddCallback(string id, params object[] args)
        {
            callbackStack.Push(new KeyValuePair<string, object[]>(id, args));
        }

        /// <summary>
        /// Executes a V8 callback at the top of the stack
        /// </summary>
        /// <param name="id"></param>
        private static void RemoveCallback()
        {
            if (callbackStack.Count > 0)
            {
                var callback = callbackStack.Pop();
                if (!String.IsNullOrEmpty(callback.Key))
                    Callback.ExecuteOnce(callback.Key, callback.Value);
            }
        }

        /// <summary>
        /// Event handler for actions needed after when the page completes loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs args)
        {
            // DocumentCompleted is fired before window.onload and body.onload
            // @see http://stackoverflow.com/questions/18368778/getting-html-body-content-in-winforms-webbrowser-after-body-onload-event-execute/18370524#18370524
            browser.Document.Window.AttachEventHandler("onload", delegate
            {
                // Add IE Toolset
                AddToolset();
                // Track unhandled errors
                browser.Document.Window.Error += delegate(object obj, HtmlElementErrorEventArgs e)
                {
                    Handle(e.Description, e.LineNumber, e.Url);
                    e.Handled = true;
                };
                // Execute callback at top of the stack
                RemoveCallback();
            });
        }

        /// <summary>
        /// Navigation (back)
        /// </summary>
        public bool canGoBack
        {
            get { return (this.browser != null) ? this.browser.CanGoBack : false; }
        }

        /// <summary>
        /// Navigation (forward)
        /// </summary>
        public bool canGoForward
        {
            get { return (this.browser != null) ? this.browser.CanGoForward : false; }
        }

        /// <summary>
        /// Goes back to previous page in history
        /// </summary>
        public void goBack() {
            if (browser.CanGoBack)
            {
                browser.GoBack();
                do
                {
                    // Run events while waiting
                    Trifle.Wait(50);
                } while (loading);
            }
        }

        /// <summary>
        /// Goes forward to next page in history
        /// </summary>
        public void goForward()
        {
            if (browser.CanGoForward)
            {
                browser.GoForward();
                do
                {
                    // Run events while waiting
                    Trifle.Wait(50);
                } while (loading);
            }
        }

        /// <summary>
        /// Goes back or forward through history 
        /// </summary>
        /// <param name="index"></param>
        public void go(int index)
        {
            if (index != 0 && browser.History != null)
            {
                browser.History.Go(index);
                do
                {
                    // Run events while waiting
                    Trifle.Wait(50);
                } while (loading);
            }
        }

        /// <summary>
        /// Returns true if the browser is loading
        /// </summary>
        public bool loading {
            get { return (browser != null) ? browser.ReadyState != WebBrowserReadyState.Complete : false; }
        }

        #endregion

        #endregion

        #region Rendering Functionality

        /// <summary>
        /// Takes a screenshot and saves into a file path
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        public void _render(string filename)
        {
            Size oldSize = browser.Size;
            int bottomPadding = 50;
            int pageHeight = browser.Document.Body.ScrollRectangle.Height + bottomPadding;
            browser.Size = new Size(browser.Size.Width, pageHeight);
            browser.Render(filename, zoomFactor);
            browser.Size = oldSize;
        }

        /// <summary>
        /// Takes a screenshot and returns Base64 encoding
        /// </summary>
        /// <param name="format">image format (defaults to PNG)</param>
        /// <returns></returns>
        public string _renderBase64(string format)
        {
            using (var pic = browser.Render(zoomFactor))
            {
                MemoryStream stream = new MemoryStream();
                switch (format.ToUpper())
                {
                    case "JPG":
                        pic.Save(stream, ImageFormat.Jpeg);
                        break;
                    case "GIF":
                        pic.Save(stream, ImageFormat.Gif);
                        break;
                    default:
                        pic.Save(stream, ImageFormat.Png);
                        break;
                }
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        /// <summary>
        /// Get/Set Viewport size for layout process
        /// </summary>
        public Dictionary<string, object> viewportSize {
            get {
                return new Dictionary<string, object>() { 
                    {"width", browser.Size.Width},
                    {"height", browser.Size.Height}
                };
            }
            set {
                try
                {
                    int width = browser.Size.Width;
                    int height = browser.Size.Height;
                    // Loop through input values and check width + height
                    foreach (string key in value.Keys)
                    {
                        try
                        {
                            switch (key)
                            {
                                case "width":
                                    width = Convert.ToInt32(value[key]);
                                    break;
                                case "height":
                                    height = Convert.ToInt32(value[key]);
                                    break;
                            }
                        }
                        catch { }
                    }
                    // Check if anythings changed
                    if (width != browser.Size.Width || height != browser.Size.Height)
                    {
                        browser.Size = new Size(width, height);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// The scaling factor for WebPage.Render() and WebPage.RenderBase64()
        /// </summary>
        public double zoomFactor {get; set;}

        /// <summary>
        /// Rectangle used for clipping
        /// </summary>
        public Dictionary<string, object> clipRect { get; set; }

        /// <summary>
        /// Turns clipRect property into a Rectangle type
        /// </summary>
        /// <returns></returns>
        private Rectangle ClipRect
        {
            get
            {
                int top = 0, left = 0, width = 0, height = 0;
                if (clipRect.ContainsKey("top")) Int32.TryParse(clipRect["top"].ToString(), out top);
                if (clipRect.ContainsKey("left")) Int32.TryParse(clipRect["left"].ToString(), out left);
                if (clipRect.ContainsKey("width")) Int32.TryParse(clipRect["width"].ToString(), out width);
                if (clipRect.ContainsKey("height")) Int32.TryParse(clipRect["height"].ToString(), out height);
                if (width == 0) width = browser.Document.Window.Size.Width;
                if (height == 0) height = browser.Document.Window.Size.Height;
                return new Rectangle(top, left, width, height);
            }
        }

        #endregion

    }
}
