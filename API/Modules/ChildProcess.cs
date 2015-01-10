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
        /// Executes s file and runs a callback
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="opts"></param>
        /// <param name="callbackId"></param>
        /// <returns></returns>
        public Context _execFile(string cmd, string[] args, Dictionary<string, object> opts, string callbackId) { 
            Context context = new Context();
            if (opts != null) {
                context.setEncoding(opts.Get<string>("encoding"));
            }
            context.start(cmd, args);
            Callback.ExecuteOnce(callbackId, context.stdOut, context.stdErr);
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
        public static List<Context> runningProceses = new List<Context>();

        /// <summary>
        /// Closes all running processes
        /// </summary>
        public void terminate() { 
            foreach(Context ctx in runningProceses) {
                ctx.kill();
            }
        }

        /// <summary>
        /// Context class used by ChildProcess module. It is usually returned 
        /// to the V8 runtime to allow manipulation and event handling of
        /// a child process.
        /// </summary>
        public class Context
        {
            private Process process;

            internal string exitCallbackId;
            internal string outputCallbackId;
            internal string errorCallbackId;

            public string stdOut { get; set; }
            public string stdErr { get; set; }

            public bool exited { get; set; }

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
                ChildProcess.runningProceses.Add(this);
            }

            internal void setEncoding(string encoding)
            {
                if (process != null && !String.IsNullOrEmpty(encoding))
                {
                    process.StartInfo.StandardErrorEncoding = Utils.GetEncoding(encoding);
                    process.StartInfo.StandardOutputEncoding = Utils.GetEncoding(encoding); ;
                }
            }

            /// <summary>
            /// Runs a process asynchronously 
            /// </summary>
            /// <param name="cmd"></param>
            /// <param name="args"></param>
            public bool start(string cmd, string[] args)
            {
                if (process != null && !exited)
                {
                    process.StartInfo.FileName = cmd;
                    process.StartInfo.Arguments = String.Join(" ", args);
                    process.Exited += Exited;
                    process.OutputDataReceived += OutputDataReceived;
                    process.ErrorDataReceived += ErrorDataReceived;
                    process.Start();
                    stdOut = "";
                    stdErr = "";
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Runs a process syncronously
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
                    stdOut = process.StandardOutput.ReadToEnd();
                    stdErr = process.StandardError.ReadToEnd();
                    exited = true;
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
                try
                {
                    process.Kill();
                    process.Dispose();
                    return true;
                }
                catch { }
                return true;
            }

            private void Exited(object sender, EventArgs args)
            {
                exited = true;
                if (!String.IsNullOrEmpty(exitCallbackId))
                {
                    Callback.ExecuteOnce(exitCallbackId, this);
                }
            }

            private void OutputDataReceived(object sender, DataReceivedEventArgs args)
            {
                stdOut = String.Format("{0}{1}", stdOut, args.Data);
                if (!String.IsNullOrEmpty(outputCallbackId))
                {
                    Callback.Execute(outputCallbackId, args.Data);
                }
            }

            private void ErrorDataReceived(object sender, DataReceivedEventArgs args)
            {
                stdErr = String.Format("{0}{1}", stdOut, args.Data);
                if (!String.IsNullOrEmpty(errorCallbackId))
                {
                    Callback.Execute(errorCallbackId, args.Data);
                }
            }
        }
    }
}
