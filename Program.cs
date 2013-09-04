using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TrifleJS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length < 1) {
                Usage();
                return;
            }

            string url = args[0];
            Console.WriteLine("Opening " + url + "...");
            using (var browser = new Browser())
            {

                browser.Size = new Size(1024, 700);
                browser.Navigate(url); //a file or a url
                browser.ScrollBarsEnabled = false;
                browser.Render("url.png");

                while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        static void Usage() {
            Console.WriteLine("Usage: triflejs.exe [url]");
        }
    }
}
