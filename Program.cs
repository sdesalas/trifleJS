using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using Noesis.Javascript;

namespace TrifleJS
{
    class Program
    {
        public static JavascriptContext context;

        [STAThread]
        static void Main(string[] args)
        {
            // Usage
            if (args.Length < 1) {
                Usage();
                return;
            }

            // Self test
            if (args[0] == "--test") {
                Test();
                return;
            }

            // Render
            if (args[0].StartsWith("--render:")) {
                string url = args[0].Replace("--render:", "");
                Render(url);
            }

            // Execute
            if (args[0].StartsWith("--open:"))
            {
                string filename = args[0].Replace("--open:", "");
                Open(filename);
            }

            Console.Read();

        }

        static void Usage() {
            Console.WriteLine("Usage: triflejs.exe [options]");
            Console.WriteLine();
            Console.WriteLine("Options: ");
            Console.WriteLine("  --test          Runs a System Test.");
            Console.WriteLine("  --render:url    Opens a url and renders into a file.");
            Console.WriteLine("  --open:file     Opens a file and executes in V8 engine API.");

        }

        static void Test() { 
        
        }

        static void Render(string url) {
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

            // Continue if ok
            using (var browser = new Browser())
            {
                browser.Size = new Size(1024, 700);
                browser.Navigate(url); //a file or a url
                browser.ScrollBarsEnabled = false;
                browser.Render(uri.Host + ".png");

                while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        static void Open(string filename) {
            if (!File.Exists(filename))
            {
                Console.WriteLine(String.Format("File does not exist: {0}", filename));
                return;
            }

            //Initialize a context
            using (Program.context = new JavascriptContext()) {

                // Setting external parameters for the context
                context.SetParameter("console", new Host.console());
                context.SetParameter("phantom", new Host.phantom());
                context.SetParameter("_interop", new Host._interop());

                // Initialise host env
                context.Run(TrifleJS.Properties.Resources.triflejs_core);
                context.Run(TrifleJS.Properties.Resources.triflejs_modules);
                context.Run(TrifleJS.Properties.Resources.initialize);

                // Script
                string script = File.ReadAllText(filename);

                // Running the script
                try
                {
                    context.Run(script);
                }
                catch (Noesis.Javascript.JavascriptException ex) {
                    // Remove refs to Host environment
                    string message = ex.Message.Replace("TrifleJS.Host+", "");
                    Console.WriteLine(String.Format("Line {0}: {1}", ex.Line, message));
                }

                // Getting a parameter
                //Console.WriteLine("number: " + context.GetParameter("number"));
            }
        }
    }
}
