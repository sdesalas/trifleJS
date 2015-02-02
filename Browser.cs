using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using TrifleJS.API.Native;

namespace TrifleJS
{
    /// <summary>
    /// Browser class that represents an IE window
    /// </summary>
    public class Browser : WebBrowser
    {
        private const string IERootKeyx32 = @"SOFTWARE\Microsoft\Internet Explorer\";
        private const string IERootKeyx64 = @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\";
        private const string IEEmulationPath = @"MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
        private const string IEEmulationPathx32 = IERootKeyx32 + IEEmulationPath;
        private const string IEEmulationPathx64 = IERootKeyx64 + IEEmulationPath;

        public Browser()
            : base()
        {
#if !DEBUG
            // Suppress Javascript error popups (release only)
            this.ScriptErrorsSuppressed = true;
#endif
            // Make sure we track which frames IE is focused on as a result
            // of javascript or mouse/keyboard events.
            this.Navigated += delegate(object sender, WebBrowserNavigatedEventArgs e)
            {
                if (this.Document != null && this.Document.Window != null)
                {
                    FocusedFrame = this.Document.Window;
                    foreach (HtmlWindow window in FocusedFrame.GetAllFrames())
                    {
                        window.GotFocus += delegate(object sender2, HtmlElementEventArgs e2)
                        {
                            FocusedFrame = sender2 as HtmlWindow;
                        };
                    }
                }
            };
        }

        /// <summary>
        /// The frame IE is currently focused on
        /// </summary>
        public HtmlWindow FocusedFrame { get; set; }

        /// <summary>
        /// Emulate a version of IE using the relevant registry keys
        /// @see http://www.west-wind.com/weblog/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version
        /// @see http://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control
        /// @see http://blogs.msdn.com/b/ie/archive/2009/03/10/more-ie8-extensibility-improvements.aspx
        /// @see http://msdn.microsoft.com/en-us/library/ee330730(v=vs.85).aspx
        /// </summary>
        /// <param name="ieVersion">The version of IE to emulate (IE7, IE8, IE9 etc)</param>
        public static bool Emulate(string ieVersion)
        {
            try
            {
                System.UInt32 dWord;
                switch (ieVersion.ToUpper())
                {
                    case "IE11":
                        dWord = 11001; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE10":
                        dWord = 10001; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE10_STANDARDS":
                        dWord = 10000; // Default - Enables Auto-downgrade (aka. Doctype "Standards")
                        break;
                    case "IE9":
                        dWord = 9999; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE9_STANDARDS":
                        dWord = 9000; // Default - Enables Auto-downgrade (aka. Doctype "Standards")
                        break;
                    case "IE8":
                        dWord = 8888; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE8_STANDARDS":
                        dWord = 8000; // Default - Enables Auto-downgrade (aka. Doctype "Standards")
                        break;
                    case "IE7":
                        dWord = 7000; 
                        break;
                    default:
                        throw new Exception("Incorrect IE version: " + ieVersion);
                }
                Utils.Debug("Setting Version to " + ieVersion);
                API.Trifle.Emulation = ieVersion;

                Utils.TryWriteRegistryKey(IEEmulationPathx32, AppDomain.CurrentDomain.FriendlyName, dWord, RegistryValueKind.DWord);
                Utils.TryWriteRegistryKey(IEEmulationPathx64, AppDomain.CurrentDomain.FriendlyName, dWord, RegistryValueKind.DWord);
            }
            catch {
                Console.Error.WriteLine(String.Format("Unrecognized IE Version \"{0}\". Choose from \"IE7\", \"IE8\", \"IE9\", \"IE10\" or \"IE11\".", ieVersion));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the version of IE currently installed in the users machine
        /// @see http://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control
        /// </summary>
        /// <returns></returns>
        public static string InstalledVersion() {

            object value = Utils.TryReadRegistryKey(Registry.LocalMachine, IERootKeyx32, "svcVersion");
            value = value ?? Utils.TryReadRegistryKey(Registry.LocalMachine, IERootKeyx64, "svcVersion");
            value = value ?? Utils.TryReadRegistryKey(Registry.LocalMachine, IERootKeyx32, "Version");
            value = value ?? Utils.TryReadRegistryKey(Registry.LocalMachine, IERootKeyx64, "Version");
            if (value != null) { 
                string version = value.ToString();
                int separator = version.IndexOf('.');
                if (separator != -1) {
                    return "IE" + version.Substring(0, separator);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a list of headers as string
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string Build(Dictionary<string, object> headers) {
            List<string> output = new List<string>();
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    output.Add(String.Format("{0}: {1}", key, headers[key]));
                }
            }
            return String.Join("\r\n", output.ToArray());
        }

        /// <summary>
        /// Navigates to a URI, using a specific HTTP method and posted data or headers.
        /// </summary>
        /// <param name="uri">the Uri to navigate to</param>
        /// <param name="method">HTTP method (GET or POST)</param>
        /// <param name="data">data being sent in POST request</param>
        /// <param name="customHeaders">custom header string</param>
        public void Navigate(Uri uri, string method, string data, Dictionary<string, object> customHeaders) {
            // Use HTTP method, currently only POST and GET are supported
            string headers = Build(customHeaders);
            switch (method.ToUpper())
            {
                case "POST":
                    // We must have some sort of payload for a POST request. 
                    // Create one if empty
                    if (String.IsNullOrEmpty(data))
                    {
                        data = " ";
                    }
                    base.Navigate(uri.AbsoluteUri, "", Encoding.UTF8.GetBytes(data), headers);
                    break;
                case "GET":
                    base.Navigate(uri.AbsoluteUri, "", null, headers);
                    break;
                default:
                    throw new Exception("Browser.Navigate(), only POST and GET methods allowed.");
            }
        }

        /// <summary>
        /// Gets the history for the current document
        /// </summary>
        public HtmlHistory History {
            get { return (Document != null && Document.Window != null) ? Document.Window.History : null; }
        }

        /// <summary>
        /// Resets the current session
        /// </summary>
        public void ResetSession() {
            Methods.ResetBrowserSession(this.Handle);
        }

        /// <summary>
        /// Waits until window finishes loading and then takes a screenshot
        /// </summary>
        /// <param name="fileName">path where the screenshot is saved</param>
        public void RenderOnLoad(string fileName)
        {
            this.DocumentCompleted += delegate
            {
                Utils.Debug("WebBrowser#DocumentCompleted");
                this.Size = this.Document.Window.Size;
                this.ScrollBarsEnabled = false;
                Render(fileName, 1);
                Console.WriteLine("Screenshot rendered to file: " + fileName);
            };
        }

        /// <summary>
        /// Takes a screenshot and saves into a file at a specific zoom ratio
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        /// <param name="ratio">zoom ratio</param>
        public void Render(string filename, double ratio)
        {
            Render(filename, ratio, new Rectangle(0, 0, 0, 0));
        }

        /// <summary>
        /// Takes a screenshot and saves into a file at a specific zoom ratio
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        /// <param name="ratio">zoom ratio</param>
        /// <param name="clipRect">Part of the image captured</param>
        public void Render(string filename, double ratio, Rectangle clipRect)
        {
            using (var pic = this.Render(ratio, clipRect))
            {
                FileInfo file = new FileInfo(filename);
                try
                {
                    switch (file.Extension.Replace(".", "").ToUpper())
                    {
                        case "JPG":
                            pic.Save(filename, ImageFormat.Jpeg);
                            break;
                        case "GIF":
                            pic.Save(filename, ImageFormat.Gif);
                            break;
                        default:
                            pic.Save(filename, ImageFormat.Png);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


        /// <summary>
        /// Takes a screenshot and saves into a Bitmap at a specific zoom ratio
        /// </summary>
        /// <param name="ratio">zoom ratio</param>
        /// <returns></returns>
        public Bitmap Render(double ratio = 1.0)
        {
            return Render(ratio, new Rectangle(0, 0, 0, 0));
        }

        /// <summary>
        /// Takes a screenshot and saves into a Bitmap at a specific zoom ratio
        /// </summary>
        /// <param name="ratio">zoom ratio</param>
        /// <returns></returns>
        /// <param name="clipRect">Part of the imaeg captured - Only Height and Width are used right now</param>
        public Bitmap Render(double ratio, Rectangle clipRect)
        {
            // Resize to full page size before rendering
            Size oldSize = this.Size;
            this.Size = PageSize;
            Bitmap result = Render(Convert.ToInt32(this.Width * ratio), Convert.ToInt32(this.Height * ratio));
            Bitmap crop = null;

            // Crop image
            if (clipRect != null && clipRect.Width > 0 && clipRect.Height > 0)
            {
                // make sure we rquest a bitmap that fits inside the full size page
                int width = Convert.ToInt32(Math.Min(clipRect.Width, this.Size.Width - clipRect.Left) * ratio);
                int height = Convert.ToInt32(Math.Min(clipRect.Height, this.Size.Height - clipRect.Top) * ratio);
                int top = Convert.ToInt32(clipRect.Top * ratio);
                int left = Convert.ToInt32(clipRect.Left * ratio);

                crop = new Bitmap(width, height);
                Rectangle cropRect = new Rectangle(left, top, width, height);

                using(Graphics g = Graphics.FromImage(crop))
                {
                   g.DrawImage(result, new Rectangle(0, 0, width, height), cropRect, GraphicsUnit.Pixel);
                }
            }
            this.Size = oldSize;
            if (crop != null)
            {
                result.Dispose();
                return crop;
            }
                
            return result;
        }


        /// <summary>
        /// Full-height page size after rendering HTML
        /// </summary>
        private Size PageSize {
            get {
                if (this.Document != null && this.Document.Body != null && this.Document.Body.ScrollRectangle != null)
                {
                    // Add 50 pixels to the bottom of the screen
                    // to avoid scrolling bars.
                    Size size = this.Document.Body.ScrollRectangle.Size;
                    int height = size.Height;
                    int width = this.Size.Width;

                    if (this.Document.Body.ClientRectangle.Width < this.Document.Body.ScrollRectangle.Width)
                        width += 50;

                    if (this.Document.Body.ClientRectangle.Height < this.Document.Body.ScrollRectangle.Height)
                        height += 50;

                    return new Size(width, height);                                       
                }
                return this.Size;
            }
        }

        /// <summary>
        /// Takes a screenshot and saves into a Bitmap with specific width and height
        /// </summary>
        /// <returns></returns>
        public Bitmap Render(int width, int height) {
            Bitmap output = new Bitmap(width, height);
            Methods.GetImage(this.ActiveXInstance, output, Color.White);
            return output;
        }

        /// <summary>
        /// Tries to parse a URL, otherwise returns null
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Uri TryParse(string url)
        {
            Uri uri = null;
            try { uri = new Uri(url); }
            catch { }
            return uri;
        }

    }
}
