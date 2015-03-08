using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using mshtml;

namespace TrifleJS.Native.Deprecated
{
    /// <summary>
    /// This class enhances the WebBrowser control by automating certain OLE dialogs
    /// (Error, Authentication, Security etc).
    /// @see http://www.journeyintocode.com/2013/08/c-webbrowser-control-proxy.html
    /// @see http://social.msdn.microsoft.com/Forums/ie/en-US/8b0712ca-0b92-4e3d-a243-27af57a57213/idochostshowui-problem-c-webbrowser?forum=ieextensiondevelopment
    /// @see http://jiangsheng.net/2013/07/17/howto-ignoring-web-browser-certificate-errors-in-webbrowser-host/
    /// </summary>
    public class EnhancedBrowser : Browser, IOleDocumentSite, IOleClientSite, IServiceProvider, IAuthenticate
    {
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
            IntPtr lpBuffer, int lpdwBufferLength);
        internal static Guid IID_IAuthenticate = new Guid("79eac9d0-baf9-11ce-8c82-00aa004ba90b");
        internal static Guid IID_IHttpSecurity = new Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b");
        internal static Guid IID_IWindowForBindingUI = new Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b");
        private const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);
        private const int RPC_E_RETRY = unchecked((int)0x80010109);
        private const int S_FALSE = 1;
        private const int S_OK = unchecked((int)0x00000000);

        /// <summary>
        /// Add OLE objects necessary for bypassing prompt dialogs
        /// </summary>
        public void InitialiseOLE()
        {
            object obj = this.ActiveXInstance;
            IOleObject oc = obj as IOleObject;
            oc.SetClientSite(this as IOleClientSite);
            // Add Support for bypassing Proxy Authentication dialog
            AuthenticateProxy += delegate(object sender, EnhancedBrowser.AthenticateProxyEventArgs e)
            {
                e.Username = Proxy.Username;
                e.Password = Proxy.Password;
            };
        }

        #region IOleClientSite Members

        public void SaveObject() { }

        public void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk) { }

        public void GetContainer(object ppContainer)
        {
            ppContainer = this.Site.Container;
        }

        public void ShowObject() { }

        public void OnShowWindow(bool fShow) { }

        public void RequestNewObjectLayout() { }

        #endregion

        #region IServiceProvider Members

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
            if (guidService == IID_IAuthenticate && riid == IID_IAuthenticate)
            {
                ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IAuthenticate));
                return S_OK;
            }
            ppvObject = IntPtr.Zero;
            return E_NOINTERFACE;
            /*
            int nRet = guidService.CompareTo(IID_IAuthenticate);
            if (nRet == 0)
            {
                nRet = riid.CompareTo(IID_IAuthenticate);
                if (nRet == 0)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IAuthenticate));
                    return S_OK;
                }
            }

            ppvObject = new IntPtr();
            return INET_E_DEFAULT_ACTION;*/
        }

        #endregion

        #region IAuthenticate Members

        /// <summary>
        /// Implementing this method bypasses the security dialog for proxy authentication.
        /// </summary>
        /// <param name="phwnd"></param>
        /// <param name="pszUsername"></param>
        /// <param name="pszPassword"></param>
        /// <returns></returns>
        public int Authenticate(ref IntPtr phwnd, ref IntPtr pszUsername, ref IntPtr pszPassword)
        {
            var e = new AthenticateProxyEventArgs(null, null);
            this.OnAuthenticateProxy(e);

            IntPtr sUser = Marshal.StringToCoTaskMemAuto(e.Username);
            IntPtr sPassword = Marshal.StringToCoTaskMemAuto(e.Password);

            pszUsername = sUser;
            pszPassword = sPassword;
            return S_OK;
        }

        #endregion

        #region IOleDocumentSite methods

        public void ActivateMe(ref object pViewToActivate) { }

        #endregion

        #region Events

        #region OnShowMessage

        public class ShowMessageEventArgs : EventArgs
        {
            public ShowMessageEventArgs(string text, string caption, uint type, string helpFile, uint helpContext)
            {
            }

            public bool Handled { get; set; }
            public int Result { get; set; }
            public uint Type { get; private set; }
            public uint HelpContext { get; private set; }
            public string Text { get; private set; }
            public string Caption { get; private set; }
            public string HelpFile { get; private set; }
        }

        protected virtual void OnShowMessage(ShowMessageEventArgs e)
        {
            var handler = this.Events["ShowMessage"] as EventHandler<ShowMessageEventArgs>;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ShowMessageEventArgs> ShowMessage
        {
            add { this.Events.AddHandler("ShowMessage", value); }
            remove { this.Events.RemoveHandler("ShowMessage", value); }
        }

        #endregion

        #region OnAuthenticateProxy

        public class AthenticateProxyEventArgs : EventArgs
        {
            public AthenticateProxyEventArgs(string username, string password)
            {
            }

            public string Username { get; set; }
            public string Password { get; set; }
        }

        protected virtual void OnAuthenticateProxy(AthenticateProxyEventArgs e)
        {
            var handler = this.Events["AuthenticateProxy"] as EventHandler<AthenticateProxyEventArgs>;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<AthenticateProxyEventArgs> AuthenticateProxy
        {
            add { this.Events.AddHandler("AuthenticateProxy", value); }
            remove { this.Events.RemoveHandler("AuthenticateProxy", value); }
        }

        #endregion

        #endregion

        #region EnhancedWebBrowserSite

        protected override System.Windows.Forms.WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new EnhancedWebBrowserSite(this);
        }

        protected class EnhancedWebBrowserSite : WebBrowserSite, IServiceProvider, IDocHostShowUI, IHttpSecurity, IWindowForBindingUI
        {
            private EnhancedBrowser host;

            public EnhancedWebBrowserSite(EnhancedBrowser host)
                : base(host)
            {
                this.host = host;
            }

            #region IDocHostShowUI Members

            int IDocHostShowUI.ShowMessage(IntPtr hwnd, string lpstrText,
                string lpstrCaption, uint dwType,
                string lpstrHelpFile, uint dwHelpContext, ref int lpResult)
            {
                //Initially
                //lpResult is set 0 //S_OK
                API.Console.warn("ShowMessage: " + lpstrCaption);
                //Host did not display its UI. MSHTML displays its message box.
                var e = new ShowMessageEventArgs(lpstrText, lpstrCaption, dwType, lpstrHelpFile, dwHelpContext);
                this.host.OnShowMessage(e);

                lpResult = (e.Handled) ? e.Result : 0;

                return Hresults.S_OK;
            }

            int IDocHostShowUI.ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, object pDispatchObjectHit)
            {
                return Hresults.E_NOTIMPL;
            }

            #endregion

            #region IServiceProvider Members

            public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
            {
                if (riid == IID_IHttpSecurity)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IHttpSecurity));
                    return S_OK;
                }
                if (riid == IID_IWindowForBindingUI)
                {
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IWindowForBindingUI));
                    return S_OK;
                }
                ppvObject = IntPtr.Zero;
                return E_NOINTERFACE;
            }

            #endregion

            #region IHttpSecurity

            public int GetWindow(ref Guid rguidReason, ref IntPtr phwnd)
            {
                if (rguidReason == IID_IHttpSecurity
                    || rguidReason == IID_IWindowForBindingUI)
                {
                    phwnd = this.host.Handle;
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

            #endregion

        }

        #endregion

        #region Hresults

        public sealed class Hresults
        {
            public const int NOERROR = 0;
            public const int S_OK = 0;
            public const int S_FALSE = 1;
            public const int E_PENDING = unchecked((int)0x8000000A);
            public const int E_HANDLE = unchecked((int)0x80070006);
            public const int E_NOTIMPL = unchecked((int)0x80004001);
            public const int E_NOINTERFACE = unchecked((int)0x80004002);
            //ArgumentNullException. NullReferenceException uses COR_E_NULLREFERENCE
            public const int E_POINTER = unchecked((int)0x80004003);
            public const int E_ABORT = unchecked((int)0x80004004);
            public const int E_FAIL = unchecked((int)0x80004005);
            public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
            public const int E_ACCESSDENIED = unchecked((int)0x80070005);
            public const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
            public const int E_FLAGS = unchecked((int)0x1000);
            public const int E_INVALIDARG = unchecked((int)0x80070057);

            //Wininet
            public const int ERROR_SUCCESS = 0;
            public const int ERROR_FILE_NOT_FOUND = 2;
            public const int ERROR_ACCESS_DENIED = 5;
            public const int ERROR_INSUFFICIENT_BUFFER = 122;
            public const int ERROR_NO_MORE_ITEMS = 259;

            //Ole Errors
            public const int OLE_E_FIRST = unchecked((int)0x80040000);
            public const int OLE_E_LAST = unchecked((int)0x800400FF);
            public const int OLE_S_FIRST = unchecked((int)0x00040000);
            public const int OLE_S_LAST = unchecked((int)0x000400FF);
            //OLECMDERR_E_FIRST = 0x80040100
            public const int OLECMDERR_E_FIRST = unchecked((int)(OLE_E_LAST + 1));
            public const int OLECMDERR_E_NOTSUPPORTED = unchecked((int)(OLECMDERR_E_FIRST));
            public const int OLECMDERR_E_DISABLED = unchecked((int)(OLECMDERR_E_FIRST + 1));
            public const int OLECMDERR_E_NOHELP = unchecked((int)(OLECMDERR_E_FIRST + 2));
            public const int OLECMDERR_E_CANCELED = unchecked((int)(OLECMDERR_E_FIRST + 3));
            public const int OLECMDERR_E_UNKNOWNGROUP = unchecked((int)(OLECMDERR_E_FIRST + 4));

            public const int OLEOBJ_E_NOVERBS = unchecked((int)0x80040180);
            public const int OLEOBJ_S_INVALIDVERB = unchecked((int)0x00040180);
            public const int OLEOBJ_S_CANNOT_DOVERB_NOW = unchecked((int)0x00040181);
            public const int OLEOBJ_S_INVALIDHWND = unchecked((int)0x00040182);

            public const int DV_E_LINDEX = unchecked((int)0x80040068);
            public const int OLE_E_OLEVERB = unchecked((int)0x80040000);
            public const int OLE_E_ADVF = unchecked((int)0x80040001);
            public const int OLE_E_ENUM_NOMORE = unchecked((int)0x80040002);
            public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
            public const int OLE_E_NOCONNECTION = unchecked((int)0x80040004);
            public const int OLE_E_NOTRUNNING = unchecked((int)0x80040005);
            public const int OLE_E_NOCACHE = unchecked((int)0x80040006);
            public const int OLE_E_BLANK = unchecked((int)0x80040007);
            public const int OLE_E_CLASSDIFF = unchecked((int)0x80040008);
            public const int OLE_E_CANT_GETMONIKER = unchecked((int)0x80040009);
            public const int OLE_E_CANT_BINDTOSOURCE = unchecked((int)0x8004000A);
            public const int OLE_E_STATIC = unchecked((int)0x8004000B);
            public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C);
            public const int OLE_E_INVALIDRECT = unchecked((int)0x8004000D);
            public const int OLE_E_WRONGCOMPOBJ = unchecked((int)0x8004000E);
            public const int OLE_E_INVALIDHWND = unchecked((int)0x8004000F);
            public const int OLE_E_NOT_INPLACEACTIVE = unchecked((int)0x80040010);
            public const int OLE_E_CANTCONVERT = unchecked((int)0x80040011);
            public const int OLE_E_NOSTORAGE = unchecked((int)0x80040012);
            public const int RPC_E_RETRY = unchecked((int)0x80010109);
        }
        #endregion
    }

    #region COM Interfaces

    [ComImport, Guid("00000112-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleObject
    {
        void SetClientSite(IOleClientSite pClientSite);
        void GetClientSite(IOleClientSite ppClientSite);
        void SetHostNames(object szContainerApp, object szContainerObj);
        void Close(uint dwSaveOption);
        void SetMoniker(uint dwWhichMoniker, object pmk);
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
        void InitFromData(IDataObject pDataObject, bool
            fCreation, uint dwReserved);
        void GetClipboardData(uint dwReserved, IDataObject ppDataObject);
        void DoVerb(uint iVerb, uint lpmsg, object pActiveSite,
            uint lindex, uint hwndParent, uint lprcPosRect);
        void EnumVerbs(object ppEnumOleVerb);
        void Update();
        void IsUpToDate();
        void GetUserClassID(uint pClsid);
        void GetUserType(uint dwFormOfType, uint pszUserType);
        void SetExtent(uint dwDrawAspect, uint psizel);
        void GetExtent(uint dwDrawAspect, uint psizel);
        void Advise(object pAdvSink, uint pdwConnection);
        void Unadvise(uint dwConnection);
        void EnumAdvise(object ppenumAdvise);
        void GetMiscStatus(uint dwAspect, uint pdwStatus);
        void SetColorScheme(object pLogpal);
    }

    [ComImport, Guid("00000118-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleClientSite
    {
        void SaveObject();
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
        void GetContainer(object ppContainer);
        void ShowObject();
        void OnShowWindow(bool fShow);
        void RequestNewObjectLayout();
    }

    [ComImport, GuidAttribute("6d5140c1-7436-11ce-8034-00aa006009fa"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
    ComVisible(false)]
    public interface IServiceProvider
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
    }

    [ComImport, GuidAttribute("79EAC9D0-BAF9-11CE-8C82-00AA004BA90B"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown),
    ComVisible(false)]
    public interface IAuthenticate
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Authenticate(ref IntPtr phwnd, ref IntPtr pszUsername, ref IntPtr pszPassword);
    }

    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d5-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IWindowForBindingUI
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetWindow([In] ref Guid rguidReason, [In, Out] ref IntPtr phwnd);
    }

    [ComImport()]
    [ComVisible(true)]
    [Guid("79eac9d7-bafa-11ce-8c82-00aa004ba90b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IHttpSecurity : IWindowForBindingUI
    {
        [PreserveSig]
        int OnSecurityProblem([In, MarshalAs(UnmanagedType.U4)] uint dwProblem);
    }


    [ComImport, GuidAttribute("b722bcc7-4e68-101b-a2bc-00aa00404770"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleDocumentSite
    {
        void ActivateMe(ref object pViewToActivate);
    }

    [ComImport, ComVisible(true)]
    [Guid("C4D244B0-D43E-11CF-893B-00AA00BDCE1A")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDocHostShowUI
    {
        //MIDL_INTERFACE("c4d244b0-d43e-11cf-893b-00aa00bdce1a")
        //IDocHostShowUI : public IUnknown
        //{
        //public:
        //    virtual HRESULT STDMETHODCALLTYPE ShowMessage( 
        //        /* [in] */ HWND hwnd,
        //        /* [in] */ LPOLESTR lpstrText,
        //        /* [in] */ LPOLESTR lpstrCaption,
        //        /* [in] */ DWORD dwType,
        //        /* [in] */ LPOLESTR lpstrHelpFile,
        //        /* [in] */ DWORD dwHelpContext,
        //        /* [out] */ LRESULT *plResult) = 0;

        //    virtual HRESULT STDMETHODCALLTYPE ShowHelp( 
        //        /* [in] */ HWND hwnd,
        //        /* [in] */ LPOLESTR pszHelpFile,
        //        /* [in] */ UINT uCommand,
        //        /* [in] */ DWORD dwData,
        //        /* [in] */ POINT ptMouse,
        //        /* [out] */ IDispatch *pDispatchObjectHit) = 0;

        //};
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowMessage(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrText,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrCaption,
            [MarshalAs(UnmanagedType.U4)] uint dwType,
            [MarshalAs(UnmanagedType.LPWStr)] string lpstrHelpFile,
            [MarshalAs(UnmanagedType.U4)] uint dwHelpContext,
            [In, Out] ref int lpResult);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowHelp(
            IntPtr hwnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszHelpFile,
            [MarshalAs(UnmanagedType.U4)] uint uCommand,
            [MarshalAs(UnmanagedType.U4)] uint dwData,
            [In, MarshalAs(UnmanagedType.Struct)] tagPOINT ptMouse,
            [Out, MarshalAs(UnmanagedType.IDispatch)] object pDispatchObjectHit);
    }

    #endregion

}