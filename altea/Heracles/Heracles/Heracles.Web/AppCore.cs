namespace Heracles.Web
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;

    using Heracles.Net;


    public static class AppCore
    {
        public static char LocalTicketData
        {
            get
            {
                return 'a';
            }
        }

        /// <summary>
        /// Hours after a local session authentication ticket will expire.
        /// </summary>
        public static int LocalTicketExpiration
        {
            get
            {
                return 8;
            }
        }

        public static char RemoteTicketData
        {
            get
            {
                return 'Z';
            }
        }

        /// <summary>
        /// Hours after a remote session authentication ticket will expire.
        /// </summary>
        public static int RemoteTicketExpiration
        {
            get
            {
                return 48;
            }
        }

        /// <summary>
        /// Months after a persistent authentication ticket will expire.
        /// </summary>
        public static int PersistentTicketExpiration
        {
            get
            {
                return 1;
            }
        }

        #region AppId

        private static Guid _appId = Guid.Empty;
        private static int? _appIdHashCode = null;
        
        public static Func<Guid> GetAppId { private get; set; }
        
        public static Guid AppId
        {
            get
            {
                return _appId == Guid.Empty ? (_appId = AppCore.GetAppId()) : _appId;
            }
        }

        public static int AppIdHashCode
        {
            get
            {
                if (_appId == Guid.Empty)
                {
                    _appId = AppCore.GetAppId();
                }

                if (_appIdHashCode == null)
                {
                    _appIdHashCode = _appId.GetHashCode();
                }

                return _appIdHashCode.Value;
            }
        }

        public static void ClearAppId()
        {
            _appId = Guid.Empty;
            _appIdHashCode = null;
        }

        #endregion

        #region AppName

        private static string _appName = null;
        
        public static Func<string> GetAppName { private get; set; }
        
        public static string AppName
        {
            get
            {
                return _appName ?? (_appName = AppCore.GetAppName());
            }
        }

        public static void ClearAppName()
        {
            _appName = null;
        }

        #endregion

        #region AppSettings

        private static Dictionary<string, string> _appSettings = null;
        
        public static Func<Dictionary<string, string>> GetAppSettings { private get; set; }
        
        public static IReadOnlyDictionary<string, string> AppSettings
        {
            get
            {
                return _appSettings ?? (_appSettings = AppCore.GetAppSettings());
            }
        }

        public static void ClearAppSettings()
        {
            _appSettings = null;
        }

        #endregion

        // Ugly code, but we don't expect having other keys than these. Also, it is faster
        // than a Hashtable, which doesn't have type safety, so it should not be used.
        #region InstanceSettings

        private static bool _instanceSettings = false;
        
        public static Func<bool> GetInstanceSettings { private get; set; }

        public static void ClearInstanceSettings()
        {
            _instanceSettings = false;
        }

        private static bool _setting_IsLocalSet;
        public static bool Setting_IsLocalSet
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_IsLocalSet;
            }
            set
            {
                _setting_IsLocalSet = value;
            }
        }

        private static IPAddress[] _setting_LocalIpAddresses;
        public static IPAddress[] Setting_LocalIpAddresses
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_LocalIpAddresses;
            }
            set
            {
                _setting_LocalIpAddresses = value;
            }
        }

        private static IPNetwork[] _setting_LocalIpNetworks;
        public static IPNetwork[] Setting_LocalIpNetworks
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_LocalIpNetworks;
            }
            set
            {
                _setting_LocalIpNetworks = value;
            }
        }

        private static string[] _setting_LocalHosts;
        public static string[] Setting_LocalHosts
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_LocalHosts;
            }
            set
            {
                _setting_LocalHosts = value;
            }
        }

        private static bool _setting_IsRegisterAllowed;
        public static bool Setting_IsRegisterAllowed
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_IsRegisterAllowed;
            }
            set
            {
                _setting_IsRegisterAllowed = value;
            }
        }

        private static bool _setting_IsRemoteRegisterAllowed;
        public static bool Setting_IsRemoteRegisterAllowed
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_IsRemoteRegisterAllowed;
            }
            set
            {
                _setting_IsRemoteRegisterAllowed = value;
            }
        }

        private static bool _setting_HasInvitations;
        public static bool Setting_HasInvitations
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_HasInvitations;
            }
            set
            {
                _setting_HasInvitations = value;
            }
        }

        private static string _setting_BigLogoName;
        public static string Setting_BigLogoName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_BigLogoName;
            }
            set
            {
                _setting_BigLogoName = value;
            }
        }

        private static string _setting_BigLogo2Name;
        public static string Setting_BigLogo2Name
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_BigLogo2Name;
            }
            set
            {
                _setting_BigLogo2Name = value;
            }
        }

        private static string _setting_BigMiniLogoName;
        public static string Setting_BigMiniLogoName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_BigMiniLogoName;
            }
            set
            {
                _setting_BigMiniLogoName = value;
            }
        }

        private static string _setting_SmallLogoName;
        public static string Setting_SmallLogoName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_SmallLogoName;
            }
            set
            {
                _setting_SmallLogoName = value;
            }
        }

        private static string _setting_SmallLogo2Name;
        public static string Setting_SmallLogo2Name
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_SmallLogo2Name;
            }
            set
            {
                _setting_SmallLogo2Name = value;
            }
        }

        private static string _setting_SmallMiniLogoName;
        public static string Setting_SmallMiniLogoName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_SmallMiniLogoName;
            }
            set
            {
                _setting_SmallMiniLogoName = value;
            }
        }

        private static string _setting_FaviconName;
        public static string Setting_FaviconName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_FaviconName;
            }
            set
            {
                _setting_FaviconName = value;
            }
        }

        private static string _setting_ThemeName;
        public static string Setting_ThemeName
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_ThemeName;
            }
            set
            {
                _setting_ThemeName = value;
            }
        }

        private static string _setting_IsAcademic;
        public static string Setting_IsAcademic
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_IsAcademic;
            }
            set
            {
                _setting_IsAcademic = value;
            }
        }

        private static string _setting_DefaultCulture;
        public static string Setting_DefaultCulture
        {
            get
            {
                _instanceSettings = _instanceSettings || GetInstanceSettings();
                return _setting_DefaultCulture;
            }
            set
            {
                _setting_DefaultCulture = value;
            }
        }

        #endregion

        #region Local

        public static bool IsLocal(IPAddress ipAddress)
        {
#if DEBUG
            return true;
#else
            if (Setting_IsLocalSet)
            {
                // Don't modify this!!
                // Check order: IP Address -> Subnetwork -> Host Addresses
                return
                    !Setting_IsLocalSet ||
                    Setting_LocalIpAddresses.Contains(ipAddress) ||
                    Setting_LocalIpNetworks.Any(network => network != null && network.ContainsAddress(ipAddress)) ||
                    Setting_LocalHosts.SelectMany(GetHostAddresses).Contains(ipAddress);
            }

            return true;
#endif
        }

#if !DEBUG
        private static IEnumerable<IPAddress> GetHostAddresses(string host)
        {
            IEnumerable<IPAddress> addresses;
            AlteaCache.GetOrInsert(
                "__DNS__" + host,
                null,
                () =>
                    {
                        IPAddress[] hostAddresses = string.IsNullOrWhiteSpace(host) ? null : DnsHelper.GetHostAddresses(host);
                        return hostAddresses ?? Enumerable.Empty<IPAddress>();
                    },
                AlteaCache.Scope.Instance,
                AlteaCache.Term.Short,
                out addresses);

            return addresses;
        }
#endif

        #endregion

        #region Cultures

        public static bool IsAcceptedLanguage(string language, out CultureInfo cultureInfo)
        {
            try
            {
                int index = language.IndexOf(";", StringComparison.OrdinalIgnoreCase);
                CultureInfo info = new CultureInfo(index != -1 ? language.Substring(0, index) : language);
                switch (info.TwoLetterISOLanguageName)
                {
                    case "en":
                    case "es":
                        cultureInfo = info;
                        return true;

                    default:
                        cultureInfo = null;
                        return false;
                }
            }
            catch
            {
                cultureInfo = null;
                return false;
            }
        }

        #endregion
    }
}