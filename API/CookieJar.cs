using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TrifleJS.API
{
    public class CookieJar
    {
        public bool Enabled { get; set; }
        public Dictionary<string, List<Cookie>> cookieList = new Dictionary<string, List<Cookie>>();

        public CookieJar() { }

        public CookieJar(string file) {
            throw new NotImplementedException();
        }

        public bool addCookiesFromMap(List<Dictionary<string, object>> cookies, string url)
        {
            if (!Enabled) return false;
            bool result = true;
            foreach (var cookie in cookies)
            {
                if (addCookieFromMap(cookie, url) == false)
                    result = false;
            }
            return result;
        }

        public bool addCookieFromMap(Dictionary<string, object> cookie, string url)
        {
            if (Enabled)
            {
                // Empty URL? Look for it in domain
                if (String.IsNullOrEmpty(url)) {
                    
                }
                if (cookieList.ContainsKey(url))
                {
                    cookieList[url].Add(new Cookie(cookie));
                }
                else
                {
                    cookieList.Add(url, new List<Cookie> { new Cookie(cookie) } );
                }
                return true;
            }
            return false;
        }

        public class Cookie 
        {
            public string Name;
            public string Value;
            public string Domain;
            public string Path;
            public DateTime Expires;
            public bool HttpOnly;
            public bool Secure;

            public Cookie(Dictionary<string, object> data) { 
                
            }

            public Dictionary<string, object> ToDictionary() {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("name", Name);
                data.Add("value", Value);
                data.Add("domain", Domain);
                data.Add("path", Path);
                data.Add("expires", Expires);
                data.Add("httpOnly", HttpOnly);
                data.Add("secure", Secure);
                return data;
            }
        }
    }
}
