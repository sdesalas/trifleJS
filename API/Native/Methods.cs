using System;
using System.Net;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using IWshRuntimeLibrary;

namespace TrifleJS.API.Native
{
    // @see http://stackoverflow.com/questions/5006825/converting-webbrowser-document-to-a-bitmap
    internal static class Methods
    {
        #region Screenshots

        [ComImport]
        [Guid("0000010D-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IViewObject
        {
            void Draw([MarshalAs(UnmanagedType.U4)] uint dwAspect, int lindex, IntPtr pvAspect, [In] IntPtr ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, [MarshalAs(UnmanagedType.Struct)] ref RECT lprcBounds, [In] IntPtr lprcWBounds, IntPtr pfnContinue, [MarshalAs(UnmanagedType.U4)] uint dwContinue);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void GetImage(object obj, Image destination, Color backgroundColor)
        {
            using (Graphics graphics = Graphics.FromImage(destination))
            {
                IntPtr deviceContextHandle = IntPtr.Zero;
                RECT rectangle = new RECT();

                rectangle.Right = destination.Width;
                rectangle.Bottom = destination.Height;

                graphics.Clear(backgroundColor);

                try
                {
                    deviceContextHandle = graphics.GetHdc();

                    IViewObject viewObject = obj as IViewObject;
                    viewObject.Draw(1, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, deviceContextHandle, ref rectangle, IntPtr.Zero, IntPtr.Zero, 0);
                }
                finally
                {
                    if (deviceContextHandle != IntPtr.Zero)
                    {
                        graphics.ReleaseHdc(deviceContextHandle);
                    }
                }
            }
        }

        #endregion

        #region WindowsScriptingHost

        /// <summary>
        /// Follows a shortcut file programmatically
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResolveLink(string path) {
            try
            {
                IWshShell wsh = new WshShellClass();
                IWshShortcut sc = (IWshShortcut)wsh.CreateShortcut(path);
                return sc.TargetPath;
            }
            catch { }
            return "";
        }

        #endregion

        #region InternetCookie

        internal const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool InternetSetOption(IntPtr hInternet, int dwOption, string lpBuffer, int dwBufferLength);

        /// <summary>
        /// Resets the browser session
        /// @see http://stackoverflow.com/questions/1688991/how-to-set-and-delete-cookies-from-webbrowser-control-for-arbitrary-domains
        /// </summary>
        /// <returns></returns>
        internal static bool ResetBrowserSession(IntPtr handle) {
            try
            {
                InternetSetOption(handle, INTERNET_OPTION_END_BROWSER_SESSION, null, 0);
            }
            catch { }
            return false;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", SetLastError = true)]
        internal static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            Int32 dwFlags,
            IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        /// <summary>
        /// Gets the URI cookie container.
        /// @see http://stackoverflow.com/questions/3382498/is-it-possible-to-transfer-authentication-from-webbrowser-to-webrequest
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        #endregion
    }
}
