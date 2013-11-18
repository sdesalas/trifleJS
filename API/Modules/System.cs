using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace TrifleJS.API.Modules
{
    /// <summary>
    /// Defines a set of system tools 
    /// </summary>
    public class System
    {
        /// <summary>
        /// Returns the arguments passed when executing triflejs.exe in the console
        /// </summary>
        public static string[] Args
        {
            get { return Program.args; }
        }

        /// <summary>
        /// Returns the Thread ID for the currently executing OS process.
        /// </summary>
        public static string PID {
            get { return AppDomain.GetCurrentThreadId().ToString(); }
        }

        /// <summary>
        /// Returns a list of environmental variables
        /// </summary>
        public static IDictionary Env {
            get {
                return Environment.GetEnvironmentVariables();
            }
        }

        /// <summary>
        /// Gets OS architecture (32/64 bit)
        /// </summary>
        public string Architecture {
            get { return GetArchitecture(); }
        }

        /// <summary>
        /// Gets OS name (windows)
        /// </summary>
        public string Name {
            get { return "windows"; }
        }

        /// <summary>
        /// Gets OS Version (XP/Vista/7 etc)
        /// </summary>
        public string Version {
            get { return GetVersion(); }
        }

        private string GetArchitecture() { 
            // Clearly if this is a 64-bit process we must be on a 64-bit OS.
            if (IntPtr.Size == 8) return "64bit";
            // Ok, so we are a 32-bit process, but is the OS 64-bit?
            // If we are running under Wow64 than the OS is 64-bit.
            bool isWow64;
            if (ModuleContainsFunction("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), out isWow64) && isWow64)
            {
                return "64bit";
            }
            else {
                return "32bit";
            }
        }
        private bool ModuleContainsFunction(string moduleName, string methodName)
        {
            IntPtr hModule = GetModuleHandle(moduleName);
            if (hModule != IntPtr.Zero)
                return GetProcAddress(hModule, methodName) != IntPtr.Zero;
            return false;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern static bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool isWow64);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        extern static IntPtr GetCurrentProcess();
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        extern static IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        extern static IntPtr GetProcAddress(IntPtr hModule, string methodName);

        /// <summary>
        /// Gets the system version (XP, Vista, 7 etc)
        /// </summary>
        /// <returns></returns>
        private string GetVersion() {

            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        return "95";
                    case 10:
                        return "98";
                    case 90:
                        return "me";
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                    case 4:
                        return "nt";
                    case 5:
                        if (vs.Minor == 0)
                            return "2000";
                        else
                            return "xp";
                    case 6:
                        switch (vs.Minor)
                        {
                            case 0:
                                return "vista";
                            case 1:
                                return "7";
                            case 2:
                                return "8";
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            return "unknown";
        }
    }

}
