using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Text;

namespace TrifleJS
{
    /// <summary>
    /// Various tools to make life easier
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Debug message to console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Debug(object message, params object[] args)
        {
            if (Program.Verbose)
            {
                ConsoleColor normalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                if (args != null)
                {
                    Console.WriteLine(String.Format(message as string, args));
                }
                else
                {
                    Console.WriteLine(message);
                }
                Console.ForegroundColor = normalColor;
            }
        }

        /// <summary>
        /// Tries reading a registry key for the current user, returns null if not found
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object TryReadRegistryKey(string path, string name)
        {
            return TryReadRegistryKey(Registry.CurrentUser, path, name);
        }

        /// <summary>
        /// Tries reading a registry key using a specific path, returns null if not found
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object TryReadRegistryKey(RegistryKey root, string path, string name)
        {
            try
            {
                // Use current user, no need for admin permissions
                RegistryKey key = root.OpenSubKey(path);
                if (key != null)
                {
                    return key.GetValue(name);
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Tries to write a key to registry catching any exceptions, returns true if the key was written successfully
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static bool TryWriteRegistryKey(string path, string name, object value, RegistryValueKind kind)
        {
            try
            {
                // Use current user, no need for admin permissions
                RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                if (key == null)
                {
                    key = CreateRegistryPath(Registry.CurrentUser, path);
                }
                key.SetValue(name, value, kind);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a path in the registry, including all necessary parent paths
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static RegistryKey CreateRegistryPath(RegistryKey key, string path)
        {
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

        /// <summary>
        /// Converts a COM object to an array of objects
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns></returns>
        public static object[] COMToArray(object comObject)
        {
            List<object> output = new List<object>();
            int? length = (int?)(comObject.GetType()).InvokeMember("length", System.Reflection.BindingFlags.GetProperty, null, comObject, null);
            for (int i = 0; i < length; i++)
            {
                output.Add(comObject.GetType().InvokeMember(String.Format("{0}", i), BindingFlags.GetProperty, null, comObject, null));
            }
            return output.ToArray();
        }

        /// <summary>
        /// Converts a COM object to dictionary (useful for outputting error information)
        /// </summary>
        /// <param name="comObject"></param>
        /// <returns></returns>
        public static Dictionary<string, object> COMToErrorObject(object comObject)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            PropertyInfo[] props = comObject.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                output.Add(prop.Name, comObject.GetType().InvokeMember(prop.Name, BindingFlags.GetProperty, null, comObject, null));
            }
            return output;
        }

        /// <summary>
        /// Serialises an object to JSON string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Deserialises a JSON string into an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Deserialize(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        /// <summary>
        /// Returns a unique ID string (16^8 = 4.3 trillion combinations)
        /// </summary>
        /// <returns></returns>
        public static string newUid()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
