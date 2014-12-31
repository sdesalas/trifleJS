using System;
using System.Net;
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
        /// <summary>
        /// Internal IE WebBrowser (enhanced)
        /// </summary>
        private EnhancedBrowser browser;

        /// <summary>
        /// Creates a webpage module
        /// </summary>
        public WebPage() {
            this.browser = new EnhancedBrowser();
            this.browser.Size = new Size(400, 300);
            this.browser.ScrollBarsEnabled = false;
            // Add WebBrowser external scripting support
            this.browser.DocumentCompleted += DocumentCompleted;
            this.browser.Navigate("about:blank");
            while (loading)
            {
                Application.DoEvents();
            }
            this.browser.InitialiseOLE();
            this.browser.ObjectForScripting = new Callback.External(this);
            this.browser.ScriptErrorsSuppressed = true;
            // Initialize properties
            this.customHeaders = new Dictionary<string, object>();
            this.zoomFactor = 1;
            this.uuid = Utils.newUid();
            this.clipRect = new Dictionary<string, object>() {
                { "top", 0 },
                { "left", 0 },
                { "width", 0 },
                { "height", 0 }
            };
        }

        /// <summary>
        /// Unique ID to help identify page in V8 enviroment
        /// </summary>
        public string uuid { get; set; }

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
        /// Returns the current url as a parsed Uri
        /// </summary>
        public Uri uri {
            get { return Browser.TryParse(url); }
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
            get
            {
                return (this.browser != null
                      && this.browser.Document != null
                      && this.browser.Document.Body != null
                      && this.browser.Document.Body.OuterText != null
                      && this.browser.Document.Body.OuterText != null) ?
                          this.browser.Document.Body.OuterText :
                          String.Empty;
            }
        }

        /// <summary>
        /// The current userAgent string
        /// </summary>
        private string userAgent {
            get { return _evaluate("function() { return navigator.userAgent ;}", null) as string; }
        }

        /// <summary>
        /// List of key/value pairs for custom headers to send to the server
        /// </summary>
        public Dictionary<string, object> customHeaders { get; set; }

        #endregion

        #region Windows and Frames

        /// <summary>
        /// Current browser window object
        /// </summary>
        private HtmlWindow Window
        {
            get
            {
                return (this.browser != null
                       && this.browser.Document != null) ?
                       this.browser.Document.Window :
                       null;
            }
        }

        /// <summary>
        /// List of frames in the browser window object
        /// </summary>
        private HtmlWindowCollection Frames {
            get {
                return (this.Window != null) ? this.Window.Frames : null;
            }
        }

        /// <summary>
        /// Name of the window
        /// </summary>
        public string windowName
        {
            get { return (this.Window != null && this.Window.Name != null) ? this.Window.Name : String.Empty; }
        }

        /// <summary>
        /// Number of frames in browser window
        /// </summary>
        public int framesCount
        {
            get { return (this.Frames != null) ? this.Frames.Count : 0; }
        }

        /// <summary>
        /// A list of names for child frames of browser window
        /// </summary>
        public List<string> framesName {
            get {
                List<string> result = new List<string>();
                if (this.Frames != null) {
                    foreach (HtmlWindow window in this.Frames) { result.Add(window.Name); }
                }
                return result;
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
        /// Evaluates (executes) javascript on the currently open window
        /// @see http://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
        /// </summary>
        /// <param name="code">code to inject into browser window</param>
        public void _evaluateJavaScript(string code)
        {
            if (browser != null)
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
                    catch (Exception ex)
                    {
                        Utils.Debug(ex.Message);
                    }
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
            if (browser == null) return null;
            string[] input;
            if (args == null) { input = new string[] { }; }
            else { input = Context.Parse(args); }
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
            if (uri != null && browser != null)
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

        #endregion

        #region Navigation

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
            if (uri != null && browser != null)
            {
                // Navigate to URL and set handler for completion
                // Remove any DocumentCompleted listeners from last round
                browser.Navigate(uri, method, data, customHeaders);
                browser.DocumentCompleted -= DocumentCompleted;
                browser.DocumentCompleted += DocumentCompleted;
                // Add callback to execution stack
                AddCallback(callbackId, "success");

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
            if (browser != null)
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
        /// Reloads page for current URL
        /// </summary>
        public void reload()
        {
            if (browser != null)
            {
                browser.Refresh(WebBrowserRefreshOption.Completely);
                do
                {
                    // Run events while waiting
                    Trifle.Wait(50);
                } while (loading);
            }
        }

        /// <summary>
        /// Stops loading the current page.
        /// </summary>
        public void stop()
        {
            if (browser != null)
            {
                browser.Stop();
                do
                {
                    // Run events while waiting
                    Trifle.Wait(50);
                } while (loading);
            }
        }

        /// <summary>
        /// Closes the page and releases memory
        /// </summary>
        public void close()
        {
            callbackStack.Clear();
            if (browser != null)
            {
                browser.Dispose();
                browser = null;
            }
        }

        /// <summary>
        /// Returns true if the browser is loading
        /// </summary>
        public bool loading {
            get { return (browser != null) ? browser.ReadyState != WebBrowserReadyState.Complete : false; }
        }

        #endregion

        #region Cookies

        /// <summary>
        /// Add a Cookies to the page. 
        /// If the domains do not match, the Cookie will be ignored/rejected. 
        /// Returns true if successfully added, otherwise false.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool addCookie(Dictionary<string, object> data) {
            Cookie cookie = new Cookie();
            try
            {
                cookie.Load(data);
                if (cookie.Domain == uri.Host)
                {
                    return CookieJar.Current.Add(cookie);
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Get or set Cookies visible to the current URL 
        /// (though, for setting, use of WebPage#addCookie is preferred).
        /// This array will be pre-populated by any existing Cookie data 
        /// visible to this URL that is stored in the CookieJar, if any. 
        /// </summary>
        public object[] cookies
        {
            get
            {
                List<object> output = new List<object>();
                foreach (string url in CookieJar.Current.content.Keys)
                {
                    Uri uri = Browser.TryParse(url);
                    if (uri.Host == this.uri.Host)
                    {
                        foreach (var cookie in CookieJar.Current.content[url])
                        {
                            output.Add(cookie.ToDictionary());
                        }
                    }
                }
                return output.ToArray();
            }
            set
            {
                if (browser != null)
                {
                    API.Native.Methods.ResetBrowserSession(browser.Handle);
                    CookieJar.Current.Clear(uri);
                    if (value != null)
                    {
                        foreach (object data in value)
                        {
                            this.addCookie(data as Dictionary<string, object>);
                        }
                    }
                }
            }
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Takes a screenshot and saves into a file path
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        public void _render(string filename)
        {
            if (browser != null)
            {
                Size oldSize = browser.Size;
                int pageHeight = browser.Document.Body.ScrollRectangle.Height;
                browser.Size = new Size(browser.Size.Width, pageHeight);
                browser.Render(filename, zoomFactor);
                browser.Size = oldSize;
            }
        }

        /// <summary>
        /// Takes a screenshot and returns Base64 encoding
        /// </summary>
        /// <param name="format">image format (defaults to PNG)</param>
        /// <returns></returns>
        public string _renderBase64(string format)
        {
            if (browser != null)
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
            return String.Empty;
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
                try
                {
                    int top = 0, left = 0, width = 0, height = 0;
                    if (clipRect.ContainsKey("top")) Int32.TryParse(clipRect["top"].ToString(), out top);
                    if (clipRect.ContainsKey("left")) Int32.TryParse(clipRect["left"].ToString(), out left);
                    if (clipRect.ContainsKey("width")) Int32.TryParse(clipRect["width"].ToString(), out width);
                    if (clipRect.ContainsKey("height")) Int32.TryParse(clipRect["height"].ToString(), out height);
                    if (browser != null)
                    {
                        if (width == 0) width = browser.Document.Window.Size.Width;
                        if (height == 0) height = browser.Document.Window.Size.Height;
                    }
                    return new Rectangle(top, left, width, height);
                }
                catch { }
                return new Rectangle();
            }
        }

        #endregion

    }
}
