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
        public static API.Context context;
        public static string[] args;
        public static bool verbose = false;

        [STAThread]
        static void Main(string[] args)
        {
            // Define environment
            bool isExecuted = false;
            bool isVersionSet = false;
            List<string> configLoop = new List<string>(args);
            List<string> executionLoop = new List<string>();
            Program.args = args;
#if DEBUG
            Program.verbose = true;
#endif
            
            // Usage
            if (args.Length < 1) {
                InteractiveMode();
                return;
            }

            // Config Loop (Set version etc)
            foreach (string arg in configLoop)
            {
                string[] parts = arg.Split(':');
                switch (parts[0])
                {
                    case "--help":
                        Help();
                        return;
                    case "--debug":
                        Program.verbose = true;
                        break;
                    case "--version":
                        var v = API.Trifle.Version;
                        Console.WriteLine("{0}.{1}.{2}", v["major"], v["minor"], v["patch"]);
                        return;
                    case "--emulate":
                        string version = arg.Replace("--emulate:", "");
                        try
                        {
                            Browser.Emulate(version.ToUpper());              
                            isVersionSet = true;
                        }
                        catch {
                            Console.Error.WriteLine(String.Format("Unrecognized IE Version \"{0}\". Choose from \"IE7\", \"IE8\", \"IE9\", \"IE10\".", version));
                        }
                        break;
                    default:
                        executionLoop.Add(arg);
                        break;
                }
            }

            // Default to IE9
            if (!isVersionSet)
            {
                Browser.Emulate("IE9");
            }

            // Execution Loop - Run Commands
            foreach (string arg in executionLoop)
            {
                string[] parts = arg.Split(':');
                switch (parts[0]) 
                { 
                    // Self test
                    case "--test":
                        Test();
                        return;
                    case "--render":
                        string url = arg.Replace("--render:", "");
                        Render(url);
                        return;
                    default:
                        // If no switch is defined then we are dealing 
                        // with javascript files that need executing
                        if (arg == parts[0] && !isExecuted)
                        {
                            Open(arg);
                            isExecuted = true;
                        }
                        else if (parts[0].StartsWith("--")) {
                            Help();
                            Exit(0);
                        }
                        break;
                }
            }

            Exit(0);

        }

        public static void Exit(int exitCode)
        {
#if DEBUG
            // Debugging? Wait for input
            Console.Read();
#endif
            Environment.Exit(exitCode);
        }

        static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("TrifleJS.exe");
            Console.WriteLine("http://triflejs.org/");
            Console.WriteLine();
            Console.WriteLine("A headless Internet Explorer with JavaScript API running on V8 engine.");
            Console.WriteLine();
            Console.WriteLine("(c) Steven de Salas 2013 - MIT Licence");
            Console.WriteLine();
            Console.WriteLine("Usage: triflejs.exe [options] somescript.js [arg1 [arg2 [...]]]..)");
            Console.WriteLine();
            Console.WriteLine("Options: ");
            Console.WriteLine("  --debug                   Prints additional warning and debug messages.");
            Console.WriteLine("  --render:<url>            Opens a url, renders into a file and quits.");
            Console.WriteLine("  --emulate:<version>       Emulates an earlier version of IE (IE7, IE8, IE9 etc).");
            Console.WriteLine();
            Console.WriteLine(" -h, --help                 Show this message and quits");
            Console.WriteLine(" -t, --test                 Runs a System Test and quits");
            Console.WriteLine(" -t, --version              Prints out TrifleJS version and quits");
            Console.WriteLine();
            Console.WriteLine("Without arguments, TrifleJS will launch in interactive mode (REPL)");
            Console.WriteLine();
        }

        static void Test() {
            Console.WriteLine();
            Console.WriteLine("System test to be implemented at a later stage.");
            Console.WriteLine();
        }

        /// <summary>
        /// Run TrifleJS in interactive mode
        /// </summary>
        static void InteractiveMode() {
            // Initialize and start console read loop;
            using (Program.context = Program.Initialise()) {
                Console.Write("triflejs> ");
                while (true)
                {
                    try
                    {
                        API.Console.log(context.Run(Console.ReadLine(), "REPL"));
                    }
                    catch (Exception ex) {
                        API.Context.Handle(ex);
                    }
                    Console.Write("triflejs> ");
                }
            }   
        }

        /// <summary>
        /// Renders a url
        /// </summary>
        /// <param name="url"></param>
        static void Render(string url) {
            Console.WriteLine("Rendering " + url + "...");

            // Check the URL
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch
            {
                Console.Error.WriteLine("Unable to open url: " + url);
                return;
            }

            // Continue if ok
            using (var browser = new Browser())
            {
                browser.Size = new Size(1024, 700);
                browser.Navigate(url); //a file or a url
                browser.ScrollBarsEnabled = false;
                browser.RenderOnLoad(uri.Host + ".png");

                while (browser.ReadyState != System.Windows.Forms.WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Opens a javascript file and executes in host environment
        /// </summary>
        /// <param name="filename">Path to a javascript file</param>
        static void Open(string filename) {
            if (!File.Exists(filename))
            {
                Console.Error.WriteLine(String.Format("File does not exist: {0}", filename));
                return;
            }

            //Initialize a context
            using (Program.context = Initialise())
            {
                // Set Library Path
                API.Trifle.LibraryPath = new FileInfo(filename).DirectoryName;

                try
                {
                    // Run the script
                    context.Run(filename);

                    // Keep running until told to stop
                    // This is to make sure asynchronous code gets executed
                    while (true) {
                        API.Window.CheckTimers();
                    }

                }
                catch (Exception ex) {
                    // Handle any exceptions
                    API.Context.Handle(ex);
                }
            }
        }

        private static API.Context Initialise()
        {

            // Create new context
            API.Context context = new API.Context();

            // Setting core global variables
            context.SetParameter("console", new API.Console());
            context.SetParameter("trifle", new API.Trifle());
            context.SetParameter("module", new API.Module());
            context.SetParameter("window", new API.Window());

            try
            {
                // Initialise host env
                context.Run(TrifleJS.Properties.Resources.triflejs_core, "triflejs.core.js");
                context.Run(TrifleJS.Properties.Resources.triflejs_modules, "triflejs.modules.js");
            }
            catch (Exception ex)
            {
                API.Context.Handle(ex);
            }

            // Return context
            return context;
        }

    }
}
