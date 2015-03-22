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

        public enum ResultId
        {
            Ok = 1,
            Cancel = 2,
            Abort = 3,
            Retry = 4,
            Ignore = 5,
            Yes = 6,
            No = 7,
            TryAgain = 10,
            Continue = 11
        }

        public class ShowMessageEventArgs : EventArgs
        {
            public ShowMessageEventArgs(string text, string caption, uint type, string helpFile, uint helpContext)
            {
                Text = text;
                Caption = caption;
                Type = type;
                HelpFile = helpFile;
                HelpContext = helpContext;
                Handled = true; // Default to Handled
                Result = ResultId.Ok; // Default to OK (IDOK = 1) @see https://msdn.microsoft.com/en-us/library/ms645505(v=vs.85).aspx
            }

            public bool Handled { get; set; }
            public ResultId Result { get; set; }
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

                lpResult = (e.Handled) ? (int)e.Result : 0;

                return HResult.S_OK;
            }

            int Interface.IDocHostShowUI.ShowHelp(IntPtr hwnd, string pszHelpFile, uint uCommand, uint dwData, tagPOINT ptMouse, object pDispatchObjectHit)
            {
                return HResult.E_NOTIMPL;
            }

            #endregion

        }

        #endregion

    }
}
