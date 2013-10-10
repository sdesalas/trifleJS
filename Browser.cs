using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;

namespace TrifleJS
{
    /// <summary>
    /// Browser class that represents an IE window
    /// </summary>
    public class Browser : System.Windows.Forms.WebBrowser
    {
        private const string IEEmulationPathx32 = @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
        private const string IEEmulationPathx64 = @"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";

        public static class Versions
        {
            public const System.UInt32 IE10_IgnoreDoctype = 0x2711u;
            public const System.UInt32 IE10 = 0x02710u;
            public const System.UInt32 IE9_IgnoreDoctype = 0x270Fu;
            public const System.UInt32 IE9 = 0x2328u;
            public const System.UInt32 IE8_IgnoreDoctype = 0x22B8u;
            public const System.UInt32 IE8 = 0x1F40u;
            public const System.UInt32 IE7 = 0x1B58u;
        }

        public static void SetIE7()
        {
            Utils.Debug("Setting Version to IE7");
            Set(Versions.IE7);
        }

        public static void SetIE8()
        {
            Utils.Debug("Setting Version to IE8");
            Set(Versions.IE8);
        }

        public static void SetIE9()
        {
            Utils.Debug("Setting Version to IE9");
            Set(Versions.IE9);
        }

        public static void SetIE10()
        {
            Utils.Debug("Setting Version to IE10");
            Set(Versions.IE10);
        }

        /// <summary>
        /// Sets the Version of IE using the relevant registry keys
        /// </summary>
        /// <param name="version"></param>
        private static void Set(System.UInt32 version)
        {
            Utils.TryWriteRegistryKey(IEEmulationPathx32, "TrifleJS.exe", version, RegistryValueKind.DWord);
            Utils.TryWriteRegistryKey(IEEmulationPathx64, "TrifleJS.exe", version, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Waits until window finishes loading and then takes a screenshot
        /// </summary>
        /// <param name="fileName">path where the screenshot is saved</param>
        public void RenderOnLoad(string fileName)
        {
            this.DocumentCompleted += delegate
            {
                Console.WriteLine("WebBrowser#DocumentCompleted");
                this.Size = this.Document.Window.Size;
                this.ScrollBarsEnabled = false;
                Render(fileName);
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
        /// Takes a screenshot and saves into a Bitmap of specific dimensions
        /// </summary>
        /// <param name="width">bitmap width</param>
        /// <param name="height">bitmap height</param>
        /// <returns></returns>
        public Bitmap Render(int width, int height) {
            Size originalSize = this.Size;
            this.Size = new Size(width, height);
            Bitmap output = this.Render();
            this.Size = originalSize;
            return output;
        }

        /// <summary>
        /// Takes a screenshot and saves into a Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap Render() {
            Bitmap output = new Bitmap(this.Width, this.Height);
            NativeMethods.GetImage(this.ActiveXInstance, output, Color.White);
            return output;
        }

    }
}
