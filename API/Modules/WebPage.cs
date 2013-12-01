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
            this.browser.Size = new Size(1024, 800);
            this.browser.ScrollBarsEnabled = false;
            this.browser.ObjectForScripting = new Callback.External(this);
            Open("about:blank", null);
        }

        /// <summary>
        /// Returns the title of the current page
        /// </summary>
        public string Title {
            get { return (this.browser != null) ? this.browser.DocumentTitle : String.Empty; }
        }

        /// <summary>
        /// Returns the current url
        /// </summary>
        public string Url {
            get { return (this.browser != null && this.browser.Url != null) ? this.browser.Url.AbsoluteUri : String.Empty; }
        }

        /// <summary>
        /// Gets the HTML content of the document
        /// </summary>
        public string Content {
            get { return (this.browser != null) ? this.browser.DocumentText : String.Empty; }
        }

        /// <summary>
        /// Gets the Plain Text content of the document
        /// </summary>
        public string PlainText {
            get { return (this.browser != null 
                        && this.browser.Document != null 
                        && this.browser.Document.Body != null 
                        && this.browser.Document.Body.Parent != null) ?
                            this.browser.Document.Body.Parent.OuterText:
                            String.Empty; }
        }

        /// <summary>
        /// List of key/value pairs for custom headers to send to the server
        /// </summary>
        public string CustomHeaders { get; set; }

        /// <summary>
        /// Opens a url using GET request and executes a callback
        /// </summary>
        /// <param name="url">URL location</param>
        /// <param name="callbackId">id of the callback to execute</param>
        public void Open(string url, string callbackId) {
            Open(url, "GET", null, callbackId);
        }

        /// <summary>
        /// Opens a URL using a specific HTTP method with a corresponding payload
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="data"></param>
        /// <param name="callbackId"></param>
        public void Open(string url, string method, string data, string callbackId) {
            Console.log("Opening " + url);
            // Check the URL
            Uri uri = Browser.TryParse(url);
            if (uri != null)
            {
                // Use HTTP method, currently only POST and GET are supported
                switch (method) { 
                    case "POST":
                        // We must have some sort of payload for a POST request. 
                        // Create one if empty
                        if (String.IsNullOrEmpty(data)) {
                            data = " ";
                        }
                        browser.Navigate(uri, "", Utils.GetBytes(data), CustomHeaders);
                        break;
                    case "GET":
                        browser.Navigate(uri, "", null, CustomHeaders);
                        break;
                    default:
                        throw new Exception("WebPage.open(url, method), only POST and GET methods allowed.");
                }
                // Define what happens when browser finishes loading the page
                browser.DocumentCompleted += delegate
                {
                    // Add toolset
                    EvaluateJavaScript(TrifleJS.Properties.Resources.ie_json2);
                    EvaluateJavaScript(TrifleJS.Properties.Resources.ie_tools);
                    // Track unhandled errors
                    this.browser.Document.Window.Error += delegate(object obj, HtmlElementErrorEventArgs e)
                    {
                        Handle(e.Description, e.LineNumber, e.Url);
                        e.Handled = true;
                    };
                    // Continue with callback
                    Callback.ExecuteOnce(callbackId, "success");
                };
                // Continue with Forms application logic until ready
                while (browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            else
            {
                Console.log("Error opening url: " + url);
            }
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
        public void Close() {
            this.browser.Dispose();
            this.browser = null;
        }

        /// <summary>
        /// Evaluates (executes) javascript on the currently open window
        /// @see http://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
        /// </summary>
        /// <param name="code">code to inject into browser window</param>
        public void EvaluateJavaScript(string code)
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
        public object Evaluate(string function, object[] args)
        {
            string[] input;
            if (args == null) { input = new string[] {}; }
            else { input = Callback.Parse(args); }
            string guid = "__" + (Guid.NewGuid()).ToString().Replace("-", "");
            string script = String.Format("function {0}() {{ return ({1})({2}); }}", guid, function, String.Join(",", input));
            EvaluateJavaScript(script);
            object result = browser.Document.InvokeScript(guid);
            return result;
        }

        /// <summary>
        /// Includes a JavaScript file from external URL and executes a 
        /// callback when ready
        /// </summary>
        /// <param name="url">URL of javascript file</param>
        /// <param name="callbackId"></param>
        public void IncludeJs(string url, string callbackId)
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
        public void InjectJs(string filename) {
            if (File.Exists(filename)) {
                EvaluateJavaScript(File.ReadAllText(filename));
            }
        }

        /// <summary>
        /// Takes a screenshot and saves into a file path
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        public void Render(string filename)
        {
            browser.Render(filename);
        }

        /// <summary>
        /// Takes a screenshot and returns Base64 encoding
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string RenderBase64(string format)
        {
            using (var pic = browser.Render())
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
        /// Get size of the page for the layout process.
        /// </summary>
        public Dictionary<string, int> GetViewportSize()
        {
            return new Dictionary<string, int>() { 
                {"width", browser.Size.Width},
                {"height", browser.Size.Height}
            };
        }

        /// <summary>
        /// Sets the viewport size for the layout process
        /// </summary>
        /// <param name="size"></param>
        public void SetViewportSize(int width, int height)
        {
            try
            {
                width = width > 0 ? width : browser.Size.Width;
                height = height > 0 ? height : browser.Size.Height;
                if (width != browser.Size.Width || height != browser.Size.Height)
                {
                    browser.Size = new Size(width, height);
                }
            }
            catch { }
        }

    }
}
