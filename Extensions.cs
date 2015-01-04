using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace TrifleJS
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a unix timestamp
        /// </summary>
        /// <returns></returns>
        public static int ToUnixTimestamp(this DateTime date) {
            if (date >= new DateTime(1970, 1, 1, 0, 0, 0, 0))
            {
                TimeSpan span = (date - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                return (int)span.TotalSeconds;
            }
            return -1;
        }

        /// <summary>
        /// Gets an entry in a dictionary by specifying its type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this Dictionary<string, object> dictionary, string key)
        {
            try
            {
                if (dictionary.ContainsKey(key)) {
                    return (T)Convert.ChangeType(dictionary[key], typeof(T));
                }
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Gets an entry in a dictionary as an object
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(this Dictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            return null;
        }

        /// <summary>
        /// Gets all ancestor frames in a HtmlWindow
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static List<HtmlWindow> GetAllFrames(this HtmlWindow window) {
            List<HtmlWindow> ancestors = new List<HtmlWindow>();
            foreach (HtmlWindow child in window.Frames) { ancestors.Add(child); }
            bool added;
            do
            {
                // Keep recursing until we no longer 
                // have anything else to add.
                added = false;
                foreach (HtmlWindow ancestor in ancestors.ToArray())
                {
                    foreach (HtmlWindow frame in ancestor.Frames)
                    {
                        if (!ancestors.Contains(frame))
                        {
                            ancestors.Add(frame);
                            added = true;
                        }
                    }
                }
            } while (added);
            return ancestors;
        }

        /// <summary>
        /// Gets the currently focused frame in all ancestor frames
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static HtmlWindow GetCurrentFrame(this HtmlWindow window) {
            foreach (HtmlWindow frame in window.GetAllFrames()) {
                if (frame.Document != null && frame.Document.Focused) {
                    return frame;
                }
            }
            return window;
        }

    }
}
