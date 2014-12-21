using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrifleJS.API
{
    public class CookieJar
    {
        public static CookieJar Current = new CookieJar();

        public bool Enabled { get; set; }
        public Dictionary<string, List<Cookie>> content = new Dictionary<string, List<Cookie>>();

        public CookieJar() { }

        public CookieJar(string file) {
            throw new NotImplementedException();
        }

        public bool Add(Dictionary<string, object> data)
        {
            return Add(data, null);
        }

        public bool Add(Dictionary<string, object> data, string url)
        {
            if (Enabled && data != null)
            {
                Cookie cookie = new Cookie();
                cookie.Load(data);
                if (String.IsNullOrEmpty(url))
                {
                    // Empty URL? Look for it in domain
                    url = cookie.GetUrl();
                }
                if (Browser.TryParse(url ?? "") != null) {
                    if (cookie.Save())
                    {
                        if (content.ContainsKey(url))
                        {
                            content[url].Add(cookie);
                        }
                        else
                        {
                            content.Add(url, new List<Cookie> { cookie });
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clears cookies from all browser sessions
        /// </summary>
        public void ClearAll() {
            content = new Dictionary<string, List<Cookie>>();
            API.Native.Methods.ResetBrowserSession(IntPtr.Zero);
        }

        /// <summary>
        /// Clears cookies for a specific URI
        /// </summary>
        /// <param name="uri"></param>
        public void Clear(Uri targetUri) {
            if (targetUri != null)
            {
                foreach (string url in content.Keys) {
                    Uri uri = Browser.TryParse(url);
                    if (uri.Host == targetUri.Host) {
                        content.Remove(url);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Extensions to .NET Cookie class for Cookie Jar functionality
    /// </summary>
    public static class CookieExtensions
    {
        /// <summary>
        /// Loads the contents of a cookie dictionary into a system cookie
        /// </summary>
        /// <param name="cookie"></param>
        /// <param name="data"></param>
        public static void Load(this Cookie cookie, Dictionary<string, object> data)
        {
            cookie.Name = data.Get<string>("name");
            cookie.Value = data.Get<string>("value");
            cookie.Secure = data.Get<bool>("secure");
            cookie.Domain = data.Get<string>("domain");
            cookie.Path = String.IsNullOrEmpty(data.Get<string>("path")) ? "/" : data.Get<string>("path");
            cookie.Expires = data.Get<DateTime>("expires");
            cookie.HttpOnly = data.Get<bool>("httpOnly");
        }

        /// <summary>
        /// Converts a cookie into a dictionary for display
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this Cookie cookie)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("name", cookie.Name);
            data.Add("value", cookie.Value);
            data.Add("secure", cookie.Secure);
            data.Add("domain", cookie.Domain);
            data.Add("path", cookie.Path);
            if (cookie.Expires > DateTime.Now) { data.Add("expires", cookie.Expires); }
            data.Add("httpOnly", cookie.HttpOnly);
            return data;
        }

        /// <summary>
        /// Returns url for the cookie (using cookie domain and path)
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string GetUrl(this Cookie cookie)
        {
            if (!String.IsNullOrEmpty(cookie.Domain))
            {
                return String.Format("http{0}://{1}{2}{3}",
                            cookie.Secure ? "s" : "",
                            cookie.Domain.StartsWith(".") ? "www" : "",
                            cookie.Domain,
                            String.IsNullOrEmpty(cookie.Path) ? "/" : cookie.Path);
            }
            return null;
        }

        /// <summary>
        /// Saves a cookie for use in .NET WebBrowser (via Windows API)
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static bool Save(this Cookie cookie)
        {
            // Make sure we only set cookie for duration of the session
            // @see http://stackoverflow.com/questions/1780469/how-do-i-set-cookie-expiration-to-session-in-c
            DateTime expires = cookie.Expires;
            cookie.Expires = DateTime.MinValue;
            bool success = false;
            try
            {
                string url = cookie.GetUrl();
                string cookieString = cookie.ToString();
                API.Native.Methods.InternetSetCookie(cookie.GetUrl(), null, cookie.ToString());
                success = true;
            }
            catch { }
            finally
            {
                cookie.Expires = expires;
            }
            return success;
        }
    }
}
