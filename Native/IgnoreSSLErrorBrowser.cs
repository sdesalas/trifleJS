using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using mshtml;

namespace TrifleJS.Native
{
    /// <summary>
    /// This class enhances the WebBrowser control by automaticaly ignoring any SSL certificate errors
    /// @see http://jiangsheng.net/2013/07/17/howto-ignoring-web-browser-certificate-errors-in-webbrowser-host/
    /// </summary>
    public class IgnoreSSLBrowser : Browser
    {
        internal static bool IgnoreSSLErrors = false;
        internal static Guid IID_IHttpSecurity = new Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b");
        internal static Guid IID_IWindowForBindingUI = new Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b");

        #region IgnoreSSLErrorBrowserSite

        protected override System.Windows.Forms.WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new IgnoreSSLErrorBrowserSite(this);
        }

        protected class IgnoreSSLErrorBrowserSite : WebBrowserSite, Interface.UCOMIServiceProvider, Interface.IHttpSecurity, Interface.IWindowForBindingUI
        {
            private IgnoreSSLBrowser host;

            public IgnoreSSLErrorBrowserSite(IgnoreSSLBrowser host)
                : base(host)
            {
                this.host = host;
            }

            #region IServiceProvider Members

            public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
            {
                if (riid == IID_IHttpSecurity)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(Interface.IHttpSecurity));
                    return HResult.S_OK;
                }
                if (riid == IID_IWindowForBindingUI)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(Interface.IWindowForBindingUI));
                    return HResult.S_OK;
                }
                ppvObject = IntPtr.Zero;
                return HResult.E_NOINTERFACE;
            }

            #endregion

            #region IHttpSecurity

            public int GetWindow(ref Guid rguidReason, ref IntPtr phwnd)
            {
                if (rguidReason == IID_IHttpSecurity
                    || rguidReason == IID_IWindowForBindingUI)
                {
                    phwnd = this.host.Handle;
                    return HResult.S_OK;
                }
                else
                {
                    phwnd = IntPtr.Zero;
                    return HResult.S_FALSE;
                }
            }

            public int OnSecurityProblem(uint dwProblem)
            {
                if (IgnoreSSLErrors)
                {
                    return HResult.S_OK;
                }
                else
                {
                    return HResult.S_FALSE;
                }
            }

            #endregion

        }

        #endregion

    }



}