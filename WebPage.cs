using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TrifleJS
{
    public class WebPage
    {
        private Browser browser;

        public WebPage() {
            this.browser = new Browser();
        }

        public void render(string filename)
        {
            using (var pic = new Bitmap(browser.Width, browser.Height))
            {
                browser.DrawToBitmap(pic, new Rectangle(0, 0, pic.Width, pic.Height));
                pic.Save(filename);
            }
        }

        public void open(string url) {
            open(url, null);
        }

        public void open(string url, object callback) {
            Console.WriteLine("Opening " + url + "...");
            // Check the URL
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch
            {
                Console.WriteLine("Unable to open url: " + url);
                return;
            }
            browser.Navigate(uri);
        }

    }
}
