using System;
using System.Net;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Windows.Forms;
using TrifleJS.Native;
using SHDocVw;
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
        private SkipDialogBrowser browser;

        /// <summary>
        /// Creates a webpage module
        /// </summary>
        public WebPage() {
            this.uuid = Utils.NewUid();
            this.browser = new SkipDialogBrowser();
            this.browser.Size = new Size(400, 300);
            // Add events
            this.browser.ActiveXBrowser.BeforeNavigate2 += BeforeNavigate2;
            this.browser.Navigated += Navigated;
            this.browser.DocumentCompleted += DocumentCompleted;
            this.browser.NewWindow += NewWindow;
            this.browser.Navigate("about:blank");
            while (loading)
            {
                Application.DoEvents();
            }
            //this.browser.InitialiseOLE();
            // Add WebBrowser external scripting support
            this.browser.ObjectForScripting = new Callback.External(this);
            // Initialize properties
            this.libraryPath = Phantom.libraryPath;
            this.customHeaders = new Dictionary<string, object>();
            this.zoomFactor = 1;
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
            set
            {
                if (this.browser != null)
                {
                    this.browser.Navigate("about:blank");
                    Application.DoEvents();
                    this.browser.Document.Write(value);
                    switchToMainFrame();
                }
            }
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
                      && this.browser.Document.Body.InnerText != null) ?
                          this.browser.Document.Body.InnerText :
                          String.Empty;
            }
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
        /// Pointer to currently selected frame used by PhantomJS
        /// </summary>
        private HtmlWindow CurrentFrame {get; set;}

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
            get { return (CurrentFrame != null && CurrentFrame.Frames != null) ? CurrentFrame.Frames.Count : 0; }
        }

        /// <summary>
        /// A list of names for child frames of browser window
        /// </summary>
        public List<string> framesName {
            get {
                List<string> result = new List<string>();
                if (CurrentFrame != null && CurrentFrame.Frames != null)
                {
                    foreach (HtmlWindow window in CurrentFrame.Frames) { result.Add(window.Name); }
                }
                return result;
            }
        }

        /// <summary>
        /// Returns the name of the selected frame
        /// </summary>
        public string frameName {
            get
            {
                if (CurrentFrame != null)
                {
                    return CurrentFrame.Name ?? String.Empty;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the title of the selected frame
        /// </summary>
        public string frameTitle
        {
            get
            {
                if (CurrentFrame != null && CurrentFrame.Document != null)
                {
                    return CurrentFrame.Document.Title ?? String.Empty;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the URL of the selected frame
        /// </summary>
        public string frameUrl {
            get {
                if (CurrentFrame != null && CurrentFrame.Url != null) {
                    return CurrentFrame.Url.AbsoluteUri;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the HTML of the selected frame
        /// </summary>
        public string frameContent {
            get {
                if (CurrentFrame != null && CurrentFrame.Document != null)
                {
                    mshtml.HTMLDocumentClass doc = CurrentFrame.Document.DomDocument as mshtml.HTMLDocumentClass;
                    if (doc != null && doc.documentElement != null)
                    {
                        return doc.documentElement.outerHTML ?? String.Empty;
                    }
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the plain text content of the frame
        /// </summary>
        public string framePlainText { 
            get {
                if (CurrentFrame != null && CurrentFrame.Document != null && CurrentFrame.Document.Body != null)
                {
                    return CurrentFrame.Document.Body.InnerText ?? String.Empty;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns the name of frame IE is currently focused on.
        /// </summary>
        public string focusedFrameName {
            get {
                if (this.browser != null && this.browser.FocusedFrame != null) {
                    return this.browser.FocusedFrame.Name ?? String.Empty;
                }
                return String.Empty;
            }
        }

        /// <summary>
        /// Switches current frame by name
        /// </summary>
        /// <param name="name"></param>
        public bool switchToFrame(string name) {
            if (CurrentFrame != null && CurrentFrame.Frames != null)
            {
                foreach (HtmlWindow window in CurrentFrame.Frames)
                {
                    if (window.Name == name)
                    {
                        CurrentFrame = window;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Switches current frame to main window
        /// </summary>
        /// <returns></returns>
        public void switchToMainFrame() {
            CurrentFrame = this.Window;
        }

        /// <summary>
        /// Switches current frame to parent
        /// </summary>
        /// <returns></returns>
        public void switchToParentFrame() {
            if (CurrentFrame != null && CurrentFrame.Parent != null) {
                CurrentFrame = CurrentFrame.Parent;
            }
        }

        /// <summary>
        /// Switches current frame to the frame IE is focused on
        /// </summary>
        public void switchToFocusedFrame() {
            if (this.browser != null && this.browser.FocusedFrame != null) {
                CurrentFrame = this.browser.FocusedFrame;
            }
        }

        #endregion

        #region Scripting

        /// <summary>
        /// Stores the path used by WebPage#injectJs function to resolve script. 
        /// Initially set to location of script invoked by PhantomJS.
        /// </summary>
        public string libraryPath { get; set; }

        /// <summary>
        /// Evaluates (executes) javascript on the currently open window
        /// @see http://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
        /// </summary>
        /// <param name="code">code to inject into browser window</param>
        public void _evaluateJavaScript(string code)
        {
            if (CurrentFrame != null && CurrentFrame.Document != null)
            {
                HtmlElementCollection head = CurrentFrame.Document.GetElementsByTagName("head");
                if (head != null)
                {
                    try
                    {
                        HtmlElement scriptEl = CurrentFrame.Document.CreateElement("script");
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
        public string _evaluate(string function, object[] args)
        {
            if (CurrentFrame != null && CurrentFrame.Document != null)
            {
                string[] input;
                if (args == null) { input = new string[] { }; }
                else { input = Context.Parse(args); }
                string guid = "__" + (Guid.NewGuid()).ToString().Replace("-", "");
                string script = String.Format("function {0}() {{ return JSON.stringify(({1})({2})); }}", guid, function, String.Join(",", input));
                _evaluateJavaScript(script);
                object result = CurrentFrame.Document.InvokeScript(guid);
                // Before returning, clear any IE events
                // generated as a result of the script
                Application.DoEvents();
                return result as string;
            }
            return null;
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
            if (uri != null && CurrentFrame != null && CurrentFrame.Document != null)
            {
                HtmlElementCollection head = CurrentFrame.Document.GetElementsByTagName("head");
                if (head != null)
                {
                    HtmlElement scriptEl = CurrentFrame.Document.CreateElement("script");
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
            if (File.Exists(filename))
            {
                _evaluateJavaScript(File.ReadAllText(filename));
            }
            else
            {
                // Not there? Try page.libraryPath instead.
                string alternatePath = Path.Combine(libraryPath, filename);
                if (File.Exists(alternatePath))
                {
                    _evaluateJavaScript(File.ReadAllText(alternatePath));
                }
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
                // Set navigation type
                this.navigationType = "Other";
                // Navigate to URL and set handler for completion
                // Remove any DocumentCompleted listeners from last round
                browser.Navigate(uri, method, data, customHeaders);
                // Add callback to execution stack
                AddCallback(callbackId, "success");
            }
            else
            {
                throw new Exception("Error opening url: " + url);
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
            if (Window != null)
            {
                // DocumentCompleted is fired before window.onload and body.onload
                // @see http://stackoverflow.com/questions/18368778/getting-html-body-content-in-winforms-webbrowser-after-body-onload-event-execute/18370524#18370524
                Window.AttachEventHandler("onload", delegate
                {
                    // Fire onLoadFinished event
                    _fireEvent("loadfinished", new object[] { "success" });
                    // Execute callback at top of the stack
                    RemoveCallback();
                });
                // Reset navigation type
                this.navigationType = "Unknown";
            }
        }

        /// <summary>
        /// Event handler for actions needed after the webpage is created
        /// and the document is loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Navigated(object sender, WebBrowserNavigatedEventArgs args)
        {
            if (browser != null)
            {
                // Fire onUrlChanged event
                _fireEvent("urlchanged", new object[] { args.Url });
                // Set current widnow and add IE tools
                switchToMainFrame();
                _evaluateJavaScript(TrifleJS.Properties.Resources.ie_json2);
                _evaluateJavaScript(TrifleJS.Properties.Resources.ie_tools);
                // Fire onInitialized event
                _fireEvent("initialized");
            }
        }

        /// <summary>
        /// Event Handler for BeforeNavigate2 (equivalent of WebBrowser#Navigating but with more info)
        /// @see https://msdn.microsoft.com/en-us/library/aa768326(v=vs.85).aspx
        /// </summary>
        public void BeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        {
            if (browser != null)
            {
                IWebBrowser2 cie = (IWebBrowser2)pDisp;
                //Console.warn(cie.LocationURL + " navigates to " + URL + " target=" + TargetFrameName + " ...");
                string url = URL.ToString();
                string type = this.navigationType;
                try {
                    // TODO: Add unit tests for tracking navigation sources
                    switch ((int)Flags) { 
                        case 64:
                            type = "LinkClicked";
                            break;
                        case 256:
                            type = "Other";
                            break;
                        case 320:
                            if (type != "Reload")
                                type = "BackOrForward";
                            break;
                        default:
                            if (PostData != null)
                                type = "FormSubmitted";
                            break;
                    }
                } catch {}
                bool willNavigate = browser.AllowNavigation;
                bool main = (pDisp == browser.ActiveXBrowser);
                // Fire onNavigationRequested
                // @see https://github.com/ariya/phantomjs/wiki/API-Reference-WebPage/e17c0dd8a89831251efc9d79b887fae7d0f73f2c#onnavigationrequested
                _fireEvent("navigationrequested", url, type, willNavigate, main);
                // Clear event queue
                // TODO: Find out why frames test code (spec/webpage.js:400) 
                // is breaking when adding line below.
                // Trifle.Wait(1);
                // Fire onLoadStarted
                _fireEvent("loadstarted");
            }
        }

        /// <summary>
        /// Event handler for popup windows
        /// TODO: Use NewWindow3 to capture the child browser object
        /// @see https://msdn.microsoft.com/en-us/library/aa768337(v=vs.85).aspx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void NewWindow(object sender, CancelEventArgs args) {
            if (browser != null) {
                object result = _fireEvent("internalpagecreated");
                // Cancel popup if event returns false
                if (result != null && !Convert.ToBoolean(result)) args.Cancel = true;
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
                // Set navigation type
                this.navigationType = "BackOrForward";
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
                // Set navigation type
                this.navigationType = "BackOrForward";
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
                // Set navigation type
                this.navigationType = "BackOrForward";
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
                // Set navigation type
                this.navigationType = "Reload";
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
        public void _close()
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

        /// <summary>
        /// Type of navigation, as used in event "onNavigationRequested".
        /// (Undefined, LinkClicked, FormSubmitted, BackOrForward, Reload, FormResubmitted, Other)
        /// </summary>
        public string navigationType { get; set; }

        #endregion

        #region Event Handling

        /// <summary>
        /// Fires an event in the page object (V8 runtime) with some optional arguments
        /// </summary>
        /// <param name="shortname"></param>
        /// <param name="jsonArgs"></param>
        /// <returns></returns>
        public object _fireEvent(string nickname, params object[] args)
        {
            return _fireEvent(nickname, Utils.Serialize(args ?? new object[] { }));
        }

        /// <summary>
        /// Fires an event in the page object (V8 runtime) passing some arguments
        /// </summary>
        /// <param name="shortname"></param>
        /// <param name="jsonArgs"></param>
        /// <returns></returns>
        public object _fireEvent(string nickname, string jsonArgs)
        {
            try
            {
                if (Program.Context != null)
                {
                    // Execute in V8 engine and return result
                    object result = Program.Context.Run(
                        String.Format("WebPage.fireEvent('{0}', '{1}', {2});", nickname, this.uuid, jsonArgs ?? "[]"),
                        "WebPage.fireEvent('" + nickname + "')"
                    );
                    return result;
                }
            }
            catch (Exception ex)
            {
                API.Context.Handle(ex);
            }
            return null;
        }

        /// <summary>
        /// Sends a keyboard or mouse event to the browser.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void _sendEvent(string type, params object[] args) {
            HtmlElement element = null;
            object modifier = null;;
            if (browser != null && browser.Document != null)
            {
                switch (type)
                {
                    // Mouse events
                    case "click":
                    case "mousedown":
                    case "mouseup":
                    case "mousemove":
                    case "mousedoubleclick":
                        if (args != null)
                        {
                            switch (args.Length)
                            {
                                case 1:
                                    // Invoke mouse event by CSS selector (first match)
                                    foreach (HtmlElement result in browser.Document.GetElementFromSelector(args[0] as string)) {
                                        element = result;
                                        break;
                                    }
                                    break;
                                case 2:
                                    // Invoke mouse event using X/Y coordinates
                                    element = browser.Document.GetElementFromPoint(args[0], args[1]);
                                    break;
                            }
                            // Nothing found?
                            // Use document body
                            if (element == null) element = browser.Document.Body;
                            // Invoke mouse event
                            element.InvokeMember(type);
                        }
                        // Clear the event queue
                        Program.DoEvents();
                        break;
                    // Single Key events
                    // @see https://msdn.microsoft.com/en-us/library/system.windows.forms.sendkeys.send.aspx
                    case "keydown":
                    case "keyup":
                        if (!browser.Focused) Native.Methods.SetForegroundWindow(browser.Handle);
                        if (args != null && args.Length > 0)
                        {
                            string keys = args[0] as string;
                            if (!String.IsNullOrEmpty(keys))
                                SendKeys.Send(keys.Substring(0, 1));
                            else 
                                // Assume keycode
                                SendKeys.Send(FindKey(args[0]));
                        }
                        // Clear the event queue
                        Program.DoEvents();
                        break;
                    // Multiple Key events
                    case "keypress":
                        if (args != null && args.Length > 0) {
                            if (args.Length > 4) {
                                modifier = args[4];
                            }
                            string keys = args[0] as string;
                            if (!String.IsNullOrEmpty(keys))
                            {
                                // Loop through individual key events
                                foreach (char c in keys)
                                {
                                    _sendEvent("keydown", new object[] { c, modifier });
                                    _sendEvent("keyup", new object[] { c, modifier });
                                }
                            }
                            else
                            {
                                // Or try using a charcode
                                try
                                {
                                    string key = FindKey(args[0]);
                                    if (!String.IsNullOrEmpty(key))
                                    {
                                        _sendEvent("keydown", new object[] { key, modifier });
                                        _sendEvent("keyup", new object[] { key, modifier });
                                    }
                                }
                                catch { }
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a character using key codes
        /// TODO: Continue when "page.event.key" is ready
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        private string FindKey(object keyCode) {
            return String.Empty;
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
                    Native.Methods.ResetBrowserSession(browser.Handle);
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
                browser.Render(filename, zoomFactor);
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
        /// Scroll position of the page. Used for rendering with clipRect
        /// </summary>
        public Dictionary<string, object> scrollPosition {
            get
            {
                int top = 0; int left = 0;
                if (this.Window != null && this.Window.Position != null) {
                    left = this.Window.Position.X;
                    top = this.Window.Position.Y;
                }
                return new Dictionary<string, object>  {
                    {"top", top},
                    {"left", left}
                };
            }
            set {
                int top = 0; int left = 0;
                if (value.ContainsKey("top")) top = value.Get<int>("top");
                if (value.ContainsKey("left")) left = value.Get<int>("left");
                // Scroll and render
                this.Window.ScrollTo(left, top);
                Program.DoEvents();
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
                    // Check width + height
                    if (value.ContainsKey("width")) width = value.Get<int>("width");
                    if (value.ContainsKey("height")) height = value.Get<int>("height");
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
