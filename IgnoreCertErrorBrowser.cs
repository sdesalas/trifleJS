using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using mshtml;
using System.Text;

namespace TrifleJS.IgnoreSSL
{
    /// <summary>
    /// @see http://jiangsheng.net/2013/07/17/howto-ignoring-web-browser-certificate-errors-in-webbrowser-host/
    /// @see http://stackoverflow.com/questions/17698002/ignoring-web-browser-security-alerts-in-console-application
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            //InitializeComponent();
        }
        private void webBrowser1_DocumentCompleted(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString() == "about:blank")
            {
                //create a certificate mismatch
                //webBrowser1.Navigate("https://74.125.225.229:8243/");
            }
        }
    }
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface UCOMIServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryService(
            [In] ref Guid guidService,
            [In] ref Guid riid,
            [Out] out IntPtr ppvObject);
    }
    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWindowForBindingUI
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow(
            [In] ref Guid rguidReason,
            [In, Out] ref IntPtr phwnd);
    }
    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IHttpSecurity
    {
        //derived from IWindowForBindingUI
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow(
            [In] ref Guid rguidReason,
            [In, Out] ref IntPtr phwnd);
        [PreserveSig]
        int OnSecurityProblem(
            [In, MarshalAs(UnmanagedType.U4)] uint dwProblem);
    }
    public class MyWebBrowser : WebBrowser
    {
        public static Guid IID_IHttpSecurity
            = new Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b");
        public static Guid IID_IWindowForBindingUI
            = new Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b");
        public const int S_OK = 0;
        public const int S_FALSE = 1;
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
        public const int RPC_E_RETRY = unchecked((int)0x80010109);
        protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new MyWebBrowserSite(this);
        }
        class MyWebBrowserSite : WebBrowserSite, UCOMIServiceProvider, IHttpSecurity, IWindowForBindingUI
        {
            private MyWebBrowser myWebBrowser;

            public MyWebBrowserSite(MyWebBrowser myWebBrowser):base(myWebBrowser)
            {
                this.myWebBrowser = myWebBrowser;
            }

            public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
            {
                if (riid == IID_IHttpSecurity)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this
                        , typeof(IHttpSecurity));
                    return S_OK;
                }
                if (riid == IID_IWindowForBindingUI)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this
                        , typeof(IWindowForBindingUI));
                    return S_OK;
                }
                ppvObject = IntPtr.Zero;
                return E_NOINTERFACE;
            }

            public int GetWindow(ref Guid rguidReason, ref IntPtr phwnd)
            {
                if (rguidReason == IID_IHttpSecurity
                    || rguidReason == IID_IWindowForBindingUI)
                {
                    phwnd = myWebBrowser.Handle;
                    return S_OK;
                }
                else
                {
                    phwnd = IntPtr.Zero;
                    return S_FALSE;
                }
            }

            public int OnSecurityProblem(uint dwProblem)
            {
                //ignore errors
                //undocumented return code, does not work on IE6
                return S_OK;
            }
        }
    }
}
