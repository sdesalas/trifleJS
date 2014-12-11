using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;
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

        /// <summary>
        /// Emulate a version of IE using the relevant registry keys
        /// @see http://www.west-wind.com/weblog/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version
        /// @see http://www.cyotek.com/blog/configuring-the-emulation-mode-of-an-internet-explorer-webbrowser-control
        /// @see http://blogs.msdn.com/b/ie/archive/2009/03/10/more-ie8-extensibility-improvements.aspx
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
                    case "IE9":
                        dWord = 9999; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE8":
                        dWord = 8888; // "Forced" (ie IGNORE_DOCTYPE mode)
                        break;
                    case "IE7":
                        dWord = 7000; 
                        break;
                    default:
                        throw new Exception("Incorrect IE version: " + ieVersion);
                }
                Utils.Debug("Setting Version to " + ieVersion);
#if DEBUG
                Utils.TryWriteRegistryKey(IEEmulationPathx32, "TrifleJS.vshost.exe", dWord, RegistryValueKind.DWord);
                Utils.TryWriteRegistryKey(IEEmulationPathx64, "TrifleJS.vshost.exe", dWord, RegistryValueKind.DWord);
#else 
                Utils.TryWriteRegistryKey(IEEmulationPathx32, "TrifleJS.exe", dWord, RegistryValueKind.DWord);
                Utils.TryWriteRegistryKey(IEEmulationPathx64, "TrifleJS.exe", dWord, RegistryValueKind.DWord);
#endif
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
        /// Navigates to a URI, using a specific HTTP method and posted data or headers.
        /// </summary>
        /// <param name="uri">the Uri to navigate to</param>
        /// <param name="method">HTTP method (GET or POST)</param>
        /// <param name="data">data being sent in POST request</param>
        /// <param name="customHeaders">custom header string</param>
        public void Navigate(Uri uri, string method, string data, string customHeaders) {
            // Use HTTP method, currently only POST and GET are supported
            switch (method.ToUpper())
            {
                case "POST":
                    // We must have some sort of payload for a POST request. 
                    // Create one if empty
                    if (String.IsNullOrEmpty(data))
                    {
                        data = " ";
                    }
                    base.Navigate(uri.AbsoluteUri, "", Encoding.UTF8.GetBytes(data), customHeaders);
                    break;
                case "GET":
                    base.Navigate(uri.AbsoluteUri, "", null, customHeaders);
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
                Render(fileName);
                Console.WriteLine("Screenshot rendered to file: " + fileName);
            };
        }

        /// <summary>
        /// Takes a screenshot and saves into a file
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        public void Render(string filename) {
            using (var pic = this.Render())
            {
                pic.Save(filename);
            }
        }

        /// <summary>
        /// Takes a screenshot and saves into a file at a specific zoom ratio
        /// </summary>
        /// <param name="filename">path where the screenshot is saved</param>
        /// <param name="ratio">zoom ratio</param>
        public void Render(string filename, double ratio)
        {
            using (var pic = this.Render(ratio))
            {
                pic.Save(filename);
            }
        }

        /// <summary>
        /// Takes a screenshot and saves into a Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap Render()
        {
            return Render(this.Width, this.Height);
        }

        /// <summary>
        /// Takes a screenshot and saves into a Bitmap at a specific zoom ratio
        /// </summary>
        /// <param name="ratio">zoom ratio</param>
        /// <returns></returns>
        public Bitmap Render(double ratio)
        {
            return Render(Convert.ToInt32(this.Width * ratio), Convert.ToInt32(this.Height * ratio));
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
            Uri uri;
            try { uri = new Uri(url); }
            catch { return null; }
            return uri;
        }

    }
}
