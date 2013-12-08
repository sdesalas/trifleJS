using System;
using System.Collections.Generic;
using System.Text;

namespace TrifleJS
{
    /// <summary>
    /// Static class used for tracking and modifying proxy information. 
    /// Proxy information used by IE is held in the registry.
    /// </summary>
    public static class Proxy
    {
        /// <summary>
        /// Default settings path for IE
        /// </summary>
        public static string defaultIESettingsPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings";

        /// <summary>
        /// Proxy server information (ie "192.168.0.255:8888")
        /// </summary>
        public static string server = null;

        /// <summary>
        /// Proxy authentication (ie "jsmith:passwd")
        /// </summary>
        public static string auth = null;

        /// <summary>
        /// Type of proxy server (ie [http|socks5|none)
        /// </summary>
        public static string type = "http";

        /// <summary>
        /// Tracks wether the proxy has been set inside the app or not
        /// </summary>
        public static bool isSet = false;

        /// <summary>
        /// Sets the IE Proxy at windows level
        /// </summary>
        public static void Set() {
            Backup.Save();
            if (type == "none")
            {
                Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyEnable", 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
            else { 
                Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyEnable", 1, Microsoft.Win32.RegistryValueKind.DWord);
                Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyServer", GetParsedServerInfo(), Microsoft.Win32.RegistryValueKind.String);
                Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyOverride", "", Microsoft.Win32.RegistryValueKind.String);
            }
            Proxy.isSet = true;
        }

        private static string GetParsedServerInfo()
        {
            switch (type)
            {
                case "http":
                    // Include 'http' & 'https' by simply using the host:port configuration
                    return server;
                case "socks5":
                    // Limit connection to socks only
                    return String.Format("socks={0}", server);
                case "none":
                    // Blank server
                    return "";
                default:
                    Console.Error.WriteLine(String.Format("Incorrect --proxy-type option: \"{0}\"", type));
                    return "";
            }
        }

        /// <summary>
        /// Encapsulates functionality for backing up and restoring existing proxy information
        /// </summary>
        public static class Backup
        {
            /// <summary>
            /// Saves the existing proxy information
            /// </summary>
            public static void Save() {
                MigrateProxy = Utils.TryReadRegistryKey(defaultIESettingsPath, "MigrateProxy");
                ProxyEnable = Utils.TryReadRegistryKey(defaultIESettingsPath, "ProxyEnable");
                ProxyServer = Utils.TryReadRegistryKey(defaultIESettingsPath, "ProxyServer");
                ProxyOverride = Utils.TryReadRegistryKey(defaultIESettingsPath, "ProxyOverride");
            }
            /// <summary>
            /// Restores previous proxy information
            /// </summary>
            public static void Restore() {
                if (Proxy.isSet)
                {
                    Utils.TryWriteRegistryKey(defaultIESettingsPath, "MigrateProxy", MigrateProxy, Microsoft.Win32.RegistryValueKind.DWord);
                    Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyEnable", ProxyEnable, Microsoft.Win32.RegistryValueKind.DWord);
                    Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyServer", ProxyServer, Microsoft.Win32.RegistryValueKind.String);
                    Utils.TryWriteRegistryKey(defaultIESettingsPath, "ProxyOverride", ProxyOverride, Microsoft.Win32.RegistryValueKind.String);
                }
            }
            public static object MigrateProxy;
            public static object ProxyEnable;
            public static object ProxyServer;
            public static object ProxyOverride;
        }

    }
}
