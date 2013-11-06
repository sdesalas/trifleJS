using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Web.Script.Serialization;
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

        public static string Serialize(object obj)
        {
            string result = new JavaScriptSerializer().Serialize(obj);
            return FormatJson(result);
        }

        private const string INDENT_STRING = "  ";

        private static string FormatJson(string str)
        {
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && str[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }

    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }
}
