using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Text;

namespace TrifleJS
{
    /// <summary>
    /// Various tools to make life easier
    /// </summary>
    public class Utils
    {
        public static void Debug(object message, params object[] args) {
            if (Program.verbose)
            {
                ConsoleColor normalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (args != null)
                {
                    Console.WriteLine(String.Format(message as string, args));
                }
                else {
                    Console.WriteLine(message);
                }
                Console.ForegroundColor = normalColor;
            }
        }

        public static bool TryWriteRegistryKey(string path, string name, object value, RegistryValueKind kind)
        {
            try
            {
                // Use current user, no need for admin permissions
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                if (key == null) {
                    key = CreateRegistryPath(Registry.CurrentUser, path);
                }
                key.SetValue(name, value, kind);
                return true;
            }
            catch {
                return false;
            }
        }

        private static RegistryKey CreateRegistryPath(RegistryKey key, string path) {
            foreach (string name in path.Split('\\'))
            {
                RegistryKey subkey = key.OpenSubKey(name, true);
                if (subkey == null)
                {
                    key.CreateSubKey(name);
                    subkey = key.OpenSubKey(name, true);
                }
                key = subkey;
            };
            return key;
        }


        public static object[] COMToArray(object comObject) {
            List<object> output = new List<object>();
            int? length = (int?)(comObject.GetType()).InvokeMember("length", System.Reflection.BindingFlags.GetProperty, null, comObject, null);
            for (int i = 0; i < length; i++) {
                output.Add(comObject.GetType().InvokeMember(String.Format("{0}", i), BindingFlags.GetProperty, null, comObject, null));
            }
            return output.ToArray();
        }

        public static Dictionary<string, object> COMToErrorObject(object comObject) {
            Dictionary<string, object> output = new Dictionary<string, object>();
            PropertyInfo[] props = comObject.GetType().GetProperties();
            foreach (PropertyInfo prop in props) {
                output.Add(prop.Name, comObject.GetType().InvokeMember(prop.Name, BindingFlags.GetProperty, null, comObject, null));
            }
            return output;
        }
    }
}
