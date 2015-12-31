namespace Heracles.Net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;

    public static class DnsHelper
    {
        // ReSharper disable All
        private static class NativeMethods
        {
            /// <summary>
            /// Provides a DNS query resolution interface
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms682016(v=vs.85).aspx
            /// </summary>
            /// <param name="lpstrName">A pointer to a string that represents the DNS name to query</param>
            /// <param name="wType">A value that represents the Resource Record DNS Record Type that is queried</param>
            /// <param name="Options">A value that contains a bitmap of the DNS Query Options to use in the DNS query</param>
            /// <param name="pExtra">Reserved for future use and must be set to NULL</param>
            /// <param name="ppQueryResultsSet">A pointer to a pointer that points to the list of RRs that comprise the response</param>
            /// <param name="pReserved">Reserved for future use and must be set to NULL</param>
            /// <returns>Success (0), or the DNS-specific error defined in Winerror.h</returns>
            [DllImport("dnsapi", EntryPoint = "DnsQuery_W", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            internal static extern int DnsQuery(
                [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpstrName,
                DnsRecordTypes wType,
                DnsQueryOptions Options,
                ref IP4_ARRAY pExtra,
                ref IntPtr ppQueryResultsSet,
                IntPtr pReserved);

            /// <summary>
            /// Frees memory allocated for DNS records obtained by using the DnsQuery function
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms682021(v=vs.85).aspx
            /// </summary>
            /// <param name="pRecordList">A pointer to the DNS_RECORD structure that contains the list of DNS records to be freed</param>
            /// <param name="FreeType">A specifier of how the record list should be freed</param>
            [DllImport("dnsapi", CharSet = CharSet.Auto, SetLastError = true)]
            internal static extern void DnsRecordListFree(
                IntPtr pRecordList,
                int FreeType);

            /// <summary>
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/cc982162(v=vs.85).aspx
            /// </summary>
            [Flags]
            internal enum DnsQueryOptions
            {
                DNS_QUERY_BYPASS_CACHE = 0x8,
                DNS_QUERY_NO_LOCAL_NAME = 0x20,
                DNS_QUERY_NO_HOSTS_FILE = 0x40,
            }

            /// <summary>
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/cc982162(v=vs.85).aspx
            /// Also see http://www.iana.org/assignments/dns-parameters/dns-parameters.xhtml
            /// </summary>
            internal enum DnsRecordTypes
            {
                DNS_TYPE_A = 1,
                DNS_TYPE_AAAA = 28
            }

            /// <summary>
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms682082(v=vs.85).aspx
            /// These field offsets could be different depending on endianness and bitness
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct DnsRecord
            {
                public IntPtr pNext;
                public string pName;
                public short wType;
                public short wDataLength;
                public int flags;
                public int dwTtl;
                public int dwReserved;
                public IntPtr pNameExchange;
                public short wPreference;
                public short Pad;
            }



            [StructLayout(LayoutKind.Sequential)]
            internal struct IP4_ARRAY
            {
                /// DWORD->unsigned int
                public UInt32 AddrCount;

                /// IP4_ADDRESS[1]
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.U4)]
                public UInt32[] AddrArray;
            }

            internal static IP4_ARRAY DNS_SERVERS = new IP4_ARRAY
            {
                AddrCount = (UInt32)DnsServers.Length,
                AddrArray = DnsServers
            };
        }

        private static readonly UInt32[] DnsServers =
        {
            // TODO: Add custom Azure DNS provider
            BitConverter.ToUInt32(IPAddress.Parse("8.8.8.8").GetAddressBytes(), 0),         /* Google Primary DNS */
            BitConverter.ToUInt32(IPAddress.Parse("8.8.4.4").GetAddressBytes(), 0),         /* Google Secondary DNS */
            BitConverter.ToUInt32(IPAddress.Parse("208.67.222.222").GetAddressBytes(), 0),  /* OpenDNS Primary DNS */
            BitConverter.ToUInt32(IPAddress.Parse("208.67.220.220").GetAddressBytes(), 0)   /* OpenDNS Secondary DNS */
        };

        private static readonly NativeMethods.DnsQueryOptions DnsOptions =
            NativeMethods.DnsQueryOptions.DNS_QUERY_BYPASS_CACHE |
            NativeMethods.DnsQueryOptions.DNS_QUERY_NO_LOCAL_NAME |
            NativeMethods.DnsQueryOptions.DNS_QUERY_NO_HOSTS_FILE;

        public static IPAddress[] GetHostAddresses(string host)
        {
            IPAddress[] addresses;

            try
            {
                addresses = Dns.GetHostAddresses(host);
            }
            catch
            {
                if (Environment.OSVersion.Platform != PlatformID.Win32NT) return null;

                IntPtr resultPtr = IntPtr.Zero;

                int result = NativeMethods.DnsQuery(
                    ref host,
                    NativeMethods.DnsRecordTypes.DNS_TYPE_A,
                    DnsOptions,
                    ref NativeMethods.DNS_SERVERS,
                    ref resultPtr,
                    IntPtr.Zero);

                if (result != 0)
                {
                    return null;
                }

                NativeMethods.DnsRecord dnsRecord;

                HashSet<IPAddress> dnsAddresses = new HashSet<IPAddress>();

                for (IntPtr auxPtr = resultPtr; !auxPtr.Equals(IntPtr.Zero); auxPtr = dnsRecord.pNext)
                {
                    dnsRecord = (NativeMethods.DnsRecord)Marshal.PtrToStructure(auxPtr, typeof(NativeMethods.DnsRecord));

                    if (dnsRecord.wType != 1)
                    {
                        continue;
                    }

                    dnsAddresses.Add(
                        new IPAddress(BitConverter.GetBytes((long)dnsRecord.pNameExchange).Take(4).ToArray()));
                }

                addresses = dnsAddresses.ToArray();

                NativeMethods.DnsRecordListFree(resultPtr, 0);
            }

            return addresses;
        }
    }
}
