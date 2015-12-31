namespace Heracles.Net
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    // ReSharper disable once InconsistentNaming
    public class IPNetwork
    {
        private readonly byte[] _netBytes;

        public IPAddress Net { get; private set; }

        public byte[] Mask { get; private set; }

        public bool IsIPv6 { get; private set; }

        public IPNetwork(IPAddress address, int mask)
        {
            bool isIPv6 = address.AddressFamily == AddressFamily.InterNetworkV6;
            byte[] addressBytes = address.GetAddressBytes();
            byte[] maskBytes = GetMaskBytes(mask, isIPv6);

            for (int i = 0, l = isIPv6 ? 16 : 4; i < l; i++)
            {
                addressBytes[i] &= maskBytes[i];
            }

            this._netBytes = addressBytes;

            this.Net = new IPAddress(addressBytes);
            this.Mask = maskBytes;
            this.IsIPv6 = isIPv6;
        }

        private static byte[] GetMaskBytes(int mask, bool isIPv6)
        {
            int numBytes = isIPv6 ? 16 : 4;
            byte[] bytes = new byte[numBytes];

            int maskBytes = mask / 8;
            int maskBits = mask % 8;
            int maskOctet = 0;

            while (maskOctet < maskBytes)
            {
                bytes[maskOctet] = 0xff;
                maskOctet++;
            }

            if (maskBits != 0)
            {
                BitArray bits = new BitArray(8);
                for (int i = 0; i < maskBits; i++)
                {
                    bits.Set(7 - i, true);
                }

                bits.CopyTo(bytes, maskOctet++);
            }

            while (maskOctet < numBytes)
            {
                bytes[maskOctet] = 0;
                maskOctet++;
            }

            return bytes;
        }

        public static bool TryParse(string netString, out IPNetwork network)
        {
            try
            {
                network = IPNetwork.Parse(netString);
                return true;
            }
            catch
            {
                network = default(IPNetwork);
                return false;
            }
        }

        public static IPNetwork Parse(string netString)
        {
            if (string.IsNullOrWhiteSpace(netString))
            {
                throw new ArgumentNullException("netString");
            }

            string[] netSplitted = netString.Split('/');
            if (netSplitted.Length != 2)
            {
               throw new FormatException();
            }

            int mask;
            if (!int.TryParse(netSplitted[1], out mask) || mask < 0)
            {
                throw new FormatException();
            }

            IPAddress address;
            if (!IPAddress.TryParse(netSplitted[0], out address))
            {
                throw new FormatException();
            }

            switch (address.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    if (mask > 32)
                    {
                        throw new FormatException();
                    }

                    break;

                case AddressFamily.InterNetworkV6:
                    if (mask > 128)
                    {
                        throw new FormatException();
                    }

                    break;

                default:
                    throw new FormatException();
            }

            return new IPNetwork(address, mask);
        }

        public bool ContainsAddress(IPAddress address)
        {
            if (address.AddressFamily != this.Net.AddressFamily)
            {
                return false;
            }

            byte[] addressBytes = address.GetAddressBytes();
            return !addressBytes.Where((b, i) => this._netBytes[i] != (b & this.Mask[i])).Any();
        }
    }
}
