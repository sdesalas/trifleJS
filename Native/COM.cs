using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using mshtml;

namespace TrifleJS.Native
{
    public class Interface
    {
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
    }

    public sealed class HResult
    {
        internal const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
        internal const int E_NOINTERFACE = unchecked((int)0x80004002);
        internal const int RPC_E_RETRY = unchecked((int)0x80010109);
        internal const int S_FALSE = 1;
        internal const int S_OK = unchecked((int)0x00000000);
        internal const int E_NOTIMPL = unchecked((int)0x80004001);
    }
}
