using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace TrifleJS.API.Modules
{

    public class ChildProcess 
    {
        /// <summary>
        /// Invokes process and listens on stdout/stderr
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <returns></returns>
        public Context _spawn(string cmd, string[] args, Dictionary<string, object> opts, string exitCallbackId, string stdOutCallbackId, string stdErrCallbackId)
        {
            Context context = new Context();
            if (opts != null)
            {
                context.setEncoding(opts.Get<string>("encoding"));
            }
            context.exitCallbackId = exitCallbackId;
            context.outputCallbackId = stdOutCallbackId;
            context.errorCallbackId = stdErrCallbackId;
            context.start(cmd, args);
            return context;
        }

        /// <summary>
        /// Executes s file and runs a callback
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <param name="callbackId"></param>
        /// <returns></returns>
        public Context _execFile(string cmd, string[] args, Dictionary<string, object> opts, string exitCallbackId)
        { 
            Context context = new Context();
            if (opts != null) {
                context.setEncoding(opts.Get<string>("encoding"));
            }
            context.exitCallbackId = exitCallbackId;
            context.start(cmd, args);
            return context;
        }

        /// <summary>
        /// Executes command synchronously
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Context _execSync(string cmd, string[] args, Dictionary<string, object> opts)
        {
            Context context = new Context();
            if (opts != null)
            {
                context.setEncoding(opts.Get<string>("encoding"));
            }
            context.execute(cmd, args);
            return context;
        }

        /// <summary>
        /// Running list of processes
        /// </summary>
        public static Dictionary<string, Context> runningProceses = new Dictionary<string, Context>();

        /// <summary>
        /// Closes all running processes
        /// </summary>
        public void terminate() { 
            foreach(Context ctx in runningProceses.Values) {
                ctx.kill();
            }
        }

        /// <summary>
        /// Returns a context from list of running processes
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns></returns>
        public Context _findContext(string contextId) {
            if (runningProceses.ContainsKey(contextId))
            {
                return runningProceses[contextId];
            }
            return null;
        }

        /// <summary>
        /// Context class used by ChildProcess module. It is usually returned 
        /// to the V8 runtime to allow manipulation and event handling of
        /// a child process.
        /// </summary>
        public class Context
        {
            private Process process;
            private string id;

            internal string exitCallbackId;
            internal string outputCallbackId;
            internal string errorCallbackId;

            public string output { get; set; }
            public string errorOutput { get; set; }

            public bool exited { get; set; }
            public int? exitCode { get; set; }

            /// <summary>
            /// Creates a new execution context for child process
            /// </summary>
            public Context()
            {
                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        WorkingDirectory = Phantom.libraryPath
                    }
                };
                // Give the context a UID so we can find it 
                // when returning to the V8 context
                this.id = Utils.NewUid();
            }

            /// <summary>
            /// Sets encoding
            /// </summary>
            /// <param name="encoding"></param>
            internal void setEncoding(string encoding)
            {
                if (process != null && !String.IsNullOrEmpty(encoding))
                {
                    process.StartInfo.StandardErrorEncoding = Utils.GetEncoding(encoding);
                    process.StartInfo.StandardOutputEncoding = Utils.GetEncoding(encoding); ;
                }
            }

            /// <summary>
            /// Starts an asynchronous process and listens on stdout/stderr
            /// </summary>
            /// <param name="cmd"></param>
            /// <param name="args"></param>
            public bool start(string cmd, string[] args)
            {
                if (process != null && !exited)
                {
                    process.StartInfo.FileName = cmd;
                    process.StartInfo.Arguments = String.Join(" ", args);
                    process.EnableRaisingEvents = true;
                    process.Exited += Exited;
                    process.OutputDataReceived += OutputDataReceived;
                    process.ErrorDataReceived += ErrorDataReceived;
                    process.Start();
                    output = "";
                    errorOutput = "";
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    ChildProcess.runningProceses.Add(this.id, this);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Runs a process syncronously and waits for execution
            /// </summary>
            /// <param name="cmd"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public bool execute(string cmd, string[] args)
            {
                if (process != null && !exited && !String.IsNullOrEmpty(cmd))
                {
                    //process.StartInfo.FileName = cmd;
                    //process.StartInfo.Arguments = String.Join(" ", args);
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = String.Format("/C \"{0} {1}\"", cmd, String.Join(" ", args));
                    process.Start();
                    process.WaitForExit();
                    output = process.StandardOutput.ReadToEnd();
                    errorOutput = process.StandardError.ReadToEnd();
                    exitCode = (int?)process.ExitCode;
                    exited = true;
                    process.Dispose();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Kills and disposes of the currently executing process
            /// </summary>
            /// <returns></returns>
            public bool kill()
            {
                if (process != null)
                {
                    try
                    {
                        process.Kill();
                        process.Dispose();
                        return true;
                    }
                    catch { }
                }
                return false;
            }

            /// <summary>
            /// Event handler for Exit.
            /// NOTE: This executes on a separate thread 
            /// (the thread of the child process).
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void Exited(object sender, EventArgs args)
            {
                exited = true;
                exitCode = (process != null) ? (int?)process.ExitCode : null;
                if (!String.IsNullOrEmpty(exitCallbackId))
                {
                    // We need to ensure that the callback to the V8 API
                    // is made on the parent thread, otherwise the context 
                    // will be different and we wont be able to continue.
                    // SOLUTION: Queue up the callback ID (+ any arguments)
                    // and wait for main thread to pick it up.
                    Callback.Queue(exitCallbackId, true, this.id, exitCode);
                }
            }

            /// <summary>
            /// Event handler for STDOUT.
            /// NOTE: this executes on a separate thread.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void OutputDataReceived(object sender, DataReceivedEventArgs args)
            {
                output = String.Format("{0}{1}", output, args.Data);
                if (!String.IsNullOrEmpty(outputCallbackId) && args.Data != null)
                {
                    Callback.Queue(outputCallbackId, false, args.Data);
                }
            }

            /// <summary>
            /// Event handler for STDERR.
            /// NOTE: this executes on a separate thread.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private void ErrorDataReceived(object sender, DataReceivedEventArgs args)
            {
                errorOutput = String.Format("{0}{1}", errorOutput, args.Data);
                if (!String.IsNullOrEmpty(errorCallbackId) && args.Data != null)
                {
                    Callback.Queue(errorCallbackId, false, args.Data);
                }
            }
        }
    }
}
