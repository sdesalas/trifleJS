using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using mshtml;

namespace TrifleJS.Native
{
    public class SkipDialogBrowser: IgnoreSSLBrowser
    {
        #region OnShowMessage

        public class ShowMessageEventArgs : EventArgs
        {
            public ShowMessageEventArgs(string text, string caption, uint type, string helpFile, uint helpContext)
            {
                Text = text;
                Caption = caption;
                Type = type;
                HelpFile = helpFile;
                HelpContext = helpContext;
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

        #region EnhancedWebBrowserSite

        protected override System.Windows.Forms.WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new SkipDialogBrowserSite(this);
        }

        protected class SkipDialogBrowserSite : IgnoreSSLErrorBrowserSite, Interface.IDocHostShowUI
        {
            private SkipDialogBrowser host;

            public SkipDialogBrowserSite(SkipDialogBrowser host)
                : base(host)
            {
                this.host = host;
            }

            #region IDocHostShowUI Members

            int Interface.IDocHostShowUI.ShowMessage(IntPtr hwnd, string lpstrText,
                string lpstrCaption, uint dwType,
                string lpstrHelpFile, uint dwHelpContext, ref int lpResult)
            {
                //Initially, lpResult is set 0 //S_OK
                API.Console.warn("ShowMessage: " + lpstrText);
                //Host did not display its UI. MSHTML displays its message box.
                var e = new ShowMessageEventArgs(lpstrText, lpstrCaption, dwType, lpstrHelpFile, dwHelpContext);
                this.host.OnShowMessage(e);

                lpResult = (e.Handled) ? e.Result : 0;

                return HResult.S_OK;
            }

            int Interface.IDocHostShowUI.ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, object pDispatchObjectHit)
            {
                return HResult.E_NOTIMPL;
            }

            #endregion

        }

        #endregion

        #region HResult

        public sealed class HResult
        {
            internal const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
            internal const int E_NOINTERFACE = unchecked((int)0x80004002);
            internal const int RPC_E_RETRY = unchecked((int)0x80010109);
            internal const int S_FALSE = 1;
            internal const int S_OK = unchecked((int)0x00000000);
            internal const int E_NOTIMPL = unchecked((int)0x80004001);
        }

        #endregion
    }
}
