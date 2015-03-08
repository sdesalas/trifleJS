using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TrifleJS.Native
{
    public class CacheHelper
    {
        private const int CACHEGROUP_SEARCH_ALL = 0;
        private const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 2;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_NO_MORE_ITEMS = 259;
        private const int ERROR_INSUFFICENT_BUFFER = 122;

        [DllImport("wininet.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetUrlCacheEntryInfoA(string lpszUrlName, IntPtr lpCacheEntryInfo, ref int lpdwCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr FindFirstUrlCacheGroup(int dwFlags, int dwFilter, IntPtr lpSearchCondition, int dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);

        [DllImport("wininet.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteUrlCacheGroup(long GroupId, int dwFlags, IntPtr lpReserved);

        [DllImport("wininet.dll", EntryPoint = "FindFirstUrlCacheEntryA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", EntryPoint = "FindNextUrlCacheEntryA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextUrlCacheEntry(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);

        [DllImport("wininet.dll", EntryPoint = "FindFirstUrlCacheEntryExA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr FindFirstUrlCacheEntryEx([MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern, int dwFlags, WININETCACHEENTRYTYPE dwFilter, long GroupId, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize, IntPtr lpReserved, IntPtr pcbReserved2, IntPtr lpReserved3);

        [DllImport("wininet.dll", EntryPoint = "FindNextUrlCacheEntryExA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FindNextUrlCacheEntryEx(IntPtr hEnumHandle, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize, IntPtr lpReserved, IntPtr pcbReserved2, IntPtr lpReserved3);

        [DllImport("wininet.dll", EntryPoint = "DeleteUrlCacheEntryA", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteUrlCacheEntry(IntPtr lpszUrlName);

        private enum WININETCACHEENTRYTYPE
        {
            None = 0,
            NORMAL_CACHE_ENTRY = 1,
            STICKY_CACHE_ENTRY = 4,
            EDITED_CACHE_ENTRY = 8,
            TRACK_OFFLINE_CACHE_ENTRY = 16,
            TRACK_ONLINE_CACHE_ENTRY = 32,
            SPARSE_CACHE_ENTRY = 65536,
            COOKIE_CACHE_ENTRY = 1048576,
            URLHISTORY_CACHE_ENTRY = 2097152,
            ALL = 3211325,
        }

        /// <summary>
        /// For PInvoke: Contains information about an entry in the Internet cache
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class INTERNET_CACHE_ENTRY_INFOA
        {
            public uint dwStructureSize;
            public IntPtr lpszSourceUrlName;
            public IntPtr lpszLocalFileName;
            public WININETCACHEENTRYTYPE CacheEntryType;
            public uint dwUseCount;
            public uint dwHitRate;
            public uint dwSizeLow;
            public uint dwSizeHigh;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastModifiedTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ExpireTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastSyncTime;
            public IntPtr lpHeaderInfo;
            public uint dwHeaderInfoSize;
            public IntPtr lpszFileExtension;
            public WININETCACHEENTRYINFOUNION _Union;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct WININETCACHEENTRYINFOUNION
        {
            [FieldOffset(0)]
            public uint dwReserved;
            [FieldOffset(0)]
            public uint dwExemptDelta;
        }

        /// <summary>
        /// Delete all permanent WinINET cookies for host; won't clear memory-only session cookies. 
        /// Supports hostnames with an optional leading wildcard, e.g. *example.com. 
        /// NOTE: Will not work on VistaIE Protected Mode cookies.
        /// </summary>
        /// <param name="host">The hostname whose cookies should be cleared</param>
        public static void ClearCookies(string host)
        {
            host = host.Trim();
            if (host.Length < 1)
                return;
            string str1;
            if (host == "*")
            {
                str1 = string.Empty;
                if (Environment.OSVersion.Version.Major > 5)
                {
                    InetCplClearTracks(false, true);
                    return;
                }
            }
            else
                str1 = host.StartsWith("*") ? host.Substring(1).ToLower() : "@" + host.ToLower();
            int num1 = 0;
            IntPtr num2 = IntPtr.Zero;
            IntPtr num3 = IntPtr.Zero;
            if (FindFirstUrlCacheEntry("cookie:", IntPtr.Zero, ref num1) == IntPtr.Zero && 259 == Marshal.GetLastWin32Error())
                return;
            int cb = num1;
            IntPtr num4 = Marshal.AllocHGlobal(cb);
            IntPtr firstUrlCacheEntry = FindFirstUrlCacheEntry("cookie:", num4, ref num1);
        label_8:
            INTERNET_CACHE_ENTRY_INFOA internetCacheEntryInfoa = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(num4, typeof(INTERNET_CACHE_ENTRY_INFOA));
            num1 = cb;
            if (WININETCACHEENTRYTYPE.COOKIE_CACHE_ENTRY == (internetCacheEntryInfoa.CacheEntryType & WININETCACHEENTRYTYPE.COOKIE_CACHE_ENTRY))
            {
                bool flag;
                if (str1.Length == 0)
                {
                    flag = true;
                }
                else
                {
                    string str2 = Marshal.PtrToStringAnsi(internetCacheEntryInfoa.lpszSourceUrlName);
                    int startIndex = str2.IndexOf('/');
                    if (startIndex > 0)
                        str2 = str2.Remove(startIndex);
                    flag = str2.ToLower().EndsWith(str1);
                }
                if (flag)
                    DeleteUrlCacheEntry(internetCacheEntryInfoa.lpszSourceUrlName);
            }
            while (true)
            {
                bool nextUrlCacheEntry = FindNextUrlCacheEntry(firstUrlCacheEntry, num4, ref num1);
                if (nextUrlCacheEntry || 259 != Marshal.GetLastWin32Error())
                {
                    if (!nextUrlCacheEntry && num1 > cb)
                    {
                        cb = num1;
                        num4 = Marshal.ReAllocHGlobal(num4, (IntPtr)cb);
                    }
                    else
                        goto label_8;
                }
                else
                    break;
            }
            Marshal.FreeHGlobal(num4);
        }

        public static void ClearCache(bool includeCookies)
        {
            if (Environment.OSVersion.Version.Major > 5)
            {
                InetCplClearTracks(true, includeCookies);
            }
            else
            {
                if (includeCookies) ClearCookies("*");
                long lpGroupId = 0L;
                int num1 = 0;
                IntPtr num2 = IntPtr.Zero;
                IntPtr num3 = IntPtr.Zero;
                bool flag1 = false;
                IntPtr firstUrlCacheGroup = FindFirstUrlCacheGroup(0, 0, IntPtr.Zero, 0, ref lpGroupId, IntPtr.Zero);
                int lastWin32Error1 = Marshal.GetLastWin32Error();
                if (firstUrlCacheGroup != IntPtr.Zero && 259 != lastWin32Error1 && 2 != lastWin32Error1)
                {
                    bool flag2;
                    int lastWin32Error2;
                    do
                    {
                        flag2 = DeleteUrlCacheGroup(lpGroupId, 2, IntPtr.Zero);
                        lastWin32Error2 = Marshal.GetLastWin32Error();
                        if (!flag2 && 2 == lastWin32Error2)
                        {
                            flag2 = FindNextUrlCacheGroup(firstUrlCacheGroup, ref lpGroupId, IntPtr.Zero);
                            lastWin32Error2 = Marshal.GetLastWin32Error();
                        }
                    }
                    while (flag2 || 259 != lastWin32Error2 && 2 != lastWin32Error2);
                }
                IntPtr firstUrlCacheEntryEx1 = FindFirstUrlCacheEntryEx((string)null, 0, WININETCACHEENTRYTYPE.ALL, 0L, IntPtr.Zero, ref num1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                int lastWin32Error3 = Marshal.GetLastWin32Error();
                if (IntPtr.Zero == firstUrlCacheEntryEx1 && 259 == lastWin32Error3)
                    return;
                int cb = num1;
                IntPtr num4 = Marshal.AllocHGlobal(cb);
                IntPtr firstUrlCacheEntryEx2 = FindFirstUrlCacheEntryEx((string)null, 0, WININETCACHEENTRYTYPE.ALL, 0L, num4, ref num1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                Marshal.GetLastWin32Error();
                bool nextUrlCacheEntryEx;
                do
                {
                    INTERNET_CACHE_ENTRY_INFOA internetCacheEntryInfoa = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(num4, typeof(INTERNET_CACHE_ENTRY_INFOA));
                    num1 = cb;
                    if (WININETCACHEENTRYTYPE.COOKIE_CACHE_ENTRY != (internetCacheEntryInfoa.CacheEntryType & WININETCACHEENTRYTYPE.COOKIE_CACHE_ENTRY))
                        flag1 = DeleteUrlCacheEntry(internetCacheEntryInfoa.lpszSourceUrlName);
                    nextUrlCacheEntryEx = FindNextUrlCacheEntryEx(firstUrlCacheEntryEx2, num4, ref num1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    int lastWin32Error2 = Marshal.GetLastWin32Error();
                    if (nextUrlCacheEntryEx || 259 != lastWin32Error2)
                    {
                        if (!nextUrlCacheEntryEx && num1 > cb)
                        {
                            cb = num1;
                            num4 = Marshal.ReAllocHGlobal(num4, (IntPtr)cb);
                            nextUrlCacheEntryEx = FindNextUrlCacheEntryEx(firstUrlCacheEntryEx2, num4, ref num1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    else
                        break;
                }
                while (nextUrlCacheEntryEx);
                Marshal.FreeHGlobal(num4);
            }
        }

        private static void InetCplClearTracks(bool clearFiles, bool clearCookies)
        {
            int CLEAR_HISTORY =         Convert.ToInt32(0x0001); // Clears history
            int CLEAR_COOKIES =         Convert.ToInt32(0x0002); // Clears cookies
            int CLEAR_CACHE =           Convert.ToInt32(0x0004); // Clears Temporary Internet Files folder
            int CLEAR_CACHE_ALL =       Convert.ToInt32(0x0008); // Clears offline favorites and download history
            int CLEAR_FORM_DATA =       Convert.ToInt32(0x0010); // Clears saved form data for form auto-fill-in
            int CLEAR_PASSWORDS =       Convert.ToInt32(0x0020); // Clears passwords saved for websites
            int CLEAR_PHISHING_FILTER = Convert.ToInt32(0x0040); // Clears phishing filter data
            int CLEAR_RECOVERY_DATA =   Convert.ToInt32(0x0080); // Clears webpage recovery data
            int CLEAR_PRIVACY_ADVISOR = Convert.ToInt32(0x0800); // Clears tracking data
            int CLEAR_SHOW_NO_GUI =     Convert.ToInt32(0x0100); // Do not show a GUI when running the cache clearing
            int CLEAR_USE_NO_THREAD =   Convert.ToInt32(0x0200); // Do not use multithreading for deletion
            int CLEAR_PRIVATE_CACHE =   Convert.ToInt32(0x0400); // Valid only when browser is in private browsing mode
            int CLEAR_DELETE_ALL =      Convert.ToInt32(0x1000); // Deletes data stored by add-ons
            int CLEAR_PRESERVE_FAVORITES = Convert.ToInt32(0x2000); // Preserves cached data for "favorite" websites

            int num = CLEAR_SHOW_NO_GUI
                     | CLEAR_USE_NO_THREAD;

            if (clearCookies)
                num |= CLEAR_COOKIES;
 
            if (clearFiles)
                num |= CLEAR_CACHE // Temporary Files
                        | CLEAR_CACHE_ALL // Offline favourites
                        | CLEAR_DELETE_ALL; // Flash etc
                        
            try
            {
                using (Process.Start("rundll32.exe", "inetcpl.cpl,ClearMyTracksByProcess " + num.ToString()));
            }
            catch
            {
                API.Console.error("Failed to Clear IE History.");
            }
        }
    }
}
