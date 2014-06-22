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
        private Browser browser;

        public WebPage() {
            this.browser = new Browser();
            this.browser.Size = new Size(800, 600); 
            this.browser.ScrollBarsEnabled = false;
            this.browser.ObjectForScripting = new Callback.External(this);
            if (this.url == "about:blank") {
                this.AddToolset();
            }
            // Initialize properties
            this.customHeaders = new Dictionary<string, object>();
            this.zoomFactor = 1;

        }

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

        /// <summary>
        /// Opens a URL using a specific HTTP method with a corresponding payload
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="callbackId"></param>
        public void _open(string url, string method, string data, string callbackId) {
            // Check the URL
            Uri uri = Browser.TryParse(url);
            if (uri != null)
            {
                // Navigate to URL
                browser.Navigate(uri, method, data, ParsedHeaders);
                // Define what happens when browser finishes loading the page
                browser.DocumentCompleted += delegate
                {
                    // Set size to document size
                    browser.Size = browser.Document.Window.Size; 
                    // DocumentCompleted is fired before window.onload and body.onload
                    // @see http://stackoverflow.com/questions/18368778/getting-html-body-content-in-winforms-webbrowser-after-body-onload-event-execute/18370524#18370524
                    browser.Document.Window.AttachEventHandler("onload", delegate
                    {
                        // Add IE Toolset
                        AddToolset();
                        // Track unhandled errors
                        this.browser.Document.Window.Error += delegate(object obj, HtmlElementErrorEventArgs e)
                        {
                            Handle(e.Description, e.LineNumber, e.Url);
                            e.Handled = true;
                        };
                        // Continue with callback
                        if (!String.IsNullOrEmpty(callbackId))
                        {
                            Callback.ExecuteOnce(callbackId, "success");
                        }
                    });

                };
                // Continue with Forms application logic until ready
                while (browser.ReadyState != WebBrowserReadyState.Complete || browser.StatusText != "Done")
                {
                    Program.DoEvents();
                }
            }
            else
            {
                Console.log("Error opening url: " + url);
            }
        }

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
                isHandled = Convert.ToBoolean(Program.context.Run(script, "WebPage.onError()"));
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
            this.browser.Dispose();
            this.browser = null;
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

        /// <summary>
        /// Takes a screenshot and saves into a file path
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        public void _render(string filename)
        {
            browser.Render(filename, zoomFactor);
        }

        /// <summary>
        /// Takes a screenshot and returns Base64 encoding
        /// </summary>
        /// <param name="format"></param>
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
        public double zoomFactor;

    }
}
