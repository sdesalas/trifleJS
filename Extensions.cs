using System;
using System.Net;
using System.Drawing;
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
            List<HtmlWindow> ancestors = new List<HtmlWindow> {window};
            bool added; int count = 0;
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
                            count++;
                        }
                    }
                }
            } while (added && count < 100); // Limit to 100 frames
            return ancestors;
        }

        /// <summary>
        /// Gets a DOM Element at specific X/Y coordinates. 
        /// Returns null if element not found.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static HtmlElement GetElementFromPoint(this HtmlDocument document, object x, object y)
        {
            try
            {
                if (x != null && y != null)
                {
                    Point point = new Point((int)x, (int)y);
                    return document.GetElementFromPoint(point);
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets a list of DOM elements using a CSS query selector.
        /// Only accepts simple selectors. (ie '.myclass' or '#myid' or 'mytag')
        /// </summary>
        /// <param name="document"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<HtmlElement> GetElementFromSelector(this HtmlDocument document, string selector)
        {
            List<HtmlElement> output = new List<HtmlElement>();
            selector = selector.Trim();
            if (selector.StartsWith("#"))
            {
                HtmlElement result = document.GetElementById(selector.Remove(1));
                if (result != null) output.Add(result);
            }
            else
            {
                if (selector.StartsWith("."))
                {
                    foreach (HtmlElement element in document.All)
                    {

                        if (element.GetAttribute("className") == selector.Remove(1))
                            output.Add(element);
                    }
                }
                else
                {
                    foreach (HtmlElement element in document.GetElementsByTagName(selector))
                        output.Add(element);
                }
            }
            return output;
        }

    }
}
