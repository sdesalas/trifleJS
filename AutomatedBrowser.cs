using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TrifleJS
{
    /// <summary>
    /// This class automates the WebBrowser control by bypassing 
    /// common dialogs (Prompt, Confirm, Authentication, Security etc)
    /// @see http://www.journeyintocode.com/2013/08/c-webbrowser-control-proxy.html
    /// </summary>
    public class AutomatedBrowser : WebBrowser, IOleClientSite, IServiceProvider, IAuthenticate
    {
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
            IntPtr lpBuffer, int lpdwBufferLength);
        private Guid IID_IAuthenticate = new Guid("79eac9d0-baf9-11ce-8c82-00aa004ba90b");
        private const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
        private const int S_OK = unchecked((int)0x00000000);

        /// <summary>
        /// Initializes OLE objects necessary for 
        /// bypassing prompt dialogs
        /// </summary>
        public void InitializeOLE()
        {
            this.Navigate("about:blank");
            while (this.ReadyState != WebBrowserReadyState.Complete) {
                Application.DoEvents();
            }
            object obj = this.ActiveXInstance;
            IOleObject oc = obj as IOleObject;
            oc.SetClientSite(this as IOleClientSite);
        }

        #region IOleClientSite Members

        public void SaveObject() { }

        public void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk) { }

        public void GetContainer(object ppContainer)
        {
            ppContainer = this.Container;
        }

        public void ShowObject() { }

        public void OnShowWindow(bool fShow) { }

        public void RequestNewObjectLayout() { }

        #endregion

        #region IServiceProvider Members

        public int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject)
        {
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
            return INET_E_DEFAULT_ACTION;
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
            IntPtr sUser = Marshal.StringToCoTaskMemAuto(Proxy.Username);
            IntPtr sPassword = Marshal.StringToCoTaskMemAuto(Proxy.Password);

            pszUsername = sUser;
            pszPassword = sPassword;
            return S_OK;
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

    #endregion
}