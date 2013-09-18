using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using mshtml;

namespace TrifleJS
{
    public class WebPage
    {
        private Browser browser;

        public WebPage() {
            this.browser = new Browser();
            this.browser.Size = new Size(1024, 700);
            this.browser.ScrollBarsEnabled = false;
        }

        public void Render(string filename)
        {
            using (var pic = new Bitmap(browser.Width, browser.Height))
            {
                NativeMethods.GetImage(browser.ActiveXInstance, pic, Color.White);
                pic.Save(filename);
            }
        }

        public string RenderBase64(string format) { 
            using (var pic = new Bitmap(browser.Width, browser.Height))
            {
                NativeMethods.GetImage(browser.ActiveXInstance, pic, Color.White);
                MemoryStream stream = new MemoryStream();
                switch (format.ToUpper()) { 
                    case "JPG":
                        pic.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case "GIF":
                        pic.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    default:
                        pic.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                }
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        
        public void Open(string url, string callbackId) {
            Console.WriteLine("Opening " + url);
            // Check the URL
            if (TryParse(url) != null)
            {
                browser.Navigate(url);
                browser.DocumentCompleted += delegate
                {
                    Callback.execute(callbackId, "success");
                };
                while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            else {
                Console.WriteLine("Error opening url: " + url);
            }
        }

        public Uri TryParse(string url) {
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch
            {
                return null;
            }
            return uri;
        }

        // @see http://stackoverflow.com/questions/153748/how-to-inject-javascript-in-webbrowser-control
        //public object EvaluateJavaScript(string code)
        //{
        //    HtmlElementCollection head = browser.Document.GetElementsByTagName("head");
        //    if (head != null)
        //    {
        //        HtmlElement scriptEl = browser.Document.CreateElement("script");
        //        IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
        //        string guid = "__" + (Guid.NewGuid()).ToString().Replace("-", "");
        //        element.text = String.Format("function {0}() {{ {1} }}", guid, code);
        //        head[0].AppendChild(scriptEl);
        //        return browser.Document.InvokeScript(guid);
        //    }
        //    return null;
        //}

        public object EvaluateJavaScript(string code)
        {
            HtmlElementCollection head = browser.Document.GetElementsByTagName("head");
            if (head != null)
            {
                HtmlElement scriptEl = browser.Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                element.text = code;
                head[0].AppendChild(scriptEl);
            }
            return null;
        }

        public object Evaluate(string function, object[] args)
        {
            string[] input = Callback.parse(args);
            string guid = "__" + (Guid.NewGuid()).ToString().Replace("-", "");
            string script = String.Format("function {0}() {{ return ({1})({2}); }}", guid, function, String.Join(",", input));
            EvaluateJavaScript(script);
            return browser.Document.InvokeScript(guid);
        }

        public void IncludeJs(string url, string callbackId)
        {
            Uri uri = TryParse(url);
            if (uri != null)
            {
                // Fetch remotely
                WebClient client = new WebClient();
                client.DownloadStringCompleted += (sender, e) =>
                {
                    EvaluateJavaScript(e.Result);
                    Callback.execute(callbackId);
                };
                client.DownloadStringAsync(uri);
            }

        }

        public void InjectJs(string filename) {
            if (File.Exists(filename)) {
                EvaluateJavaScript(File.ReadAllText(filename));
            }
        }

    }
}
