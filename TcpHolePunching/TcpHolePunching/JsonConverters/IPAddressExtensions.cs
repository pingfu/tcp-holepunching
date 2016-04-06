using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpHolePunching.JsonConverters
{
    public static class IpAddressExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool IsMulticast(this IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily != AddressFamily.InterNetwork) return ipAddress.IsIPv6Multicast;
            var firstByte = ipAddress.GetAddressBytes()[0];
            return (firstByte >= 224 && firstByte <= 239);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static IPAddress ToIpAddress(this uint ipAddress)
        {
            var ip = String.Empty;
            for (var n = 1; n < 5; n++)
            {
                var octet = (uint)(Math.Truncate(ipAddress / Math.Pow(256, 4 - n)));
                ipAddress = ipAddress - (uint)(octet * Math.Pow(256, 4 - n));
                if (octet > 255) return IPAddress.Parse("xxxxxx");
                ip += (n == 1) ? octet.ToString(CultureInfo.InvariantCulture) : "." + octet;
            }
            return IPAddress.Parse(ip);
        }

        /// <summary>
        /// Converts a string representation of a hostname or ip address into an IPAddress object
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static IPAddress ToIpAddress(this string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname)) return null;

            try
            {
                IPAddress parsedIpAddress;
                if (!IPAddress.TryParse(hostname, out parsedIpAddress))
                {
                    parsedIpAddress = Dns.GetHostAddresses(hostname)[0];
                }

                return parsedIpAddress;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static int ToCidrSubnetMask(this uint subnetMask)
        {
            // todo warning- endian specific converter, not suitable for use without checking endian byte order of subnetMask
            // var decimalAddress = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);

            switch (subnetMask)
            {
                // little endian
                case 0: return 0;
                case 2147483648: return 1; // 128.0.0.0
                case 3221225472: return 2;
                case 3758096384: return 3;
                case 4026531840: return 4;
                case 4160749568: return 5;
                case 4227858432: return 6;
                case 4261412864: return 7;
                case 4278190080: return 8; // 255.0.0.0
                case 4286578688: return 9;
                case 4290772992: return 10;
                case 4292870144: return 11;
                case 4293918720: return 12;
                case 4294443008: return 13;
                case 4294705152: return 14;
                case 4294836224: return 15;
                case 4294901760: return 16; // 255.255.0.0
                case 4294934528: return 17;
                case 4294950912: return 18;
                case 4294959104: return 19;
                case 4294963200: return 20;
                case 4294965248: return 21;
                case 4294966272: return 22;
                case 4294966784: return 23;
                case 4294967040: return 24; // 255.255.255.0
                case 4294967168: return 25;
                case 4294967232: return 26;
                case 4294967264: return 27;
                case 4294967280: return 28;
                case 4294967288: return 29;
                case 4294967292: return 30;
                case 4294967294: return 31;
                case 4294967295: return 32; // 255.255.255.255

                // big endian
                //case 0: return 0;
                //case 128: return 1;
                //case 192: return 2;
                //case 224: return 3;
                //case 240: return 4;
                //case 248: return 5;
                //case 252: return 6;
                //case 254: return 7;
                //case 255: return 8; // 255.0.0.0
                //case 33023: return 9;
                //case 49407: return 10;
                //case 57599: return 11;
                //case 61695: return 12;
                //case 63743: return 13;
                //case 64767: return 14;
                //case 65279: return 15;
                //case 65535: return 16; // 255.255.0.0
                //case 8454143: return 17;
                //case 12648447: return 18;
                //case 14745599: return 19;
                //case 15794175: return 20;
                //case 16318463: return 21;
                //case 16580607: return 22;
                //case 16711679: return 23;
                //case 16777215: return 24;  // 255.255.255.0
                //case 2164260863: return 25;
                //case 3238002687: return 26;
                //case 3774873599: return 27;
                //case 4043309055: return 28;
                //case 4177526783: return 29;
                //case 4244635647: return 30;
                //case 4278190079: return 31;
                //case 4294967295: return 32;
            }

            throw new Exception("invalid subnet");
        }

        /// <summary>
        /// Converts a string representation of a hostname or ip address and port combination into an IPEndPoint object
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <returns></returns>
        public static IPEndPoint ToIpEndPoint(this string ipEndPoint)
        {
            if (string.IsNullOrWhiteSpace(ipEndPoint)) return null;
            if (!ipEndPoint.Contains(":")) return null;

            try
            {
                var components = ipEndPoint.Split(':');

                IPAddress ipAddress;
                if (!IPAddress.TryParse(components[0], out ipAddress))
                {
                    ipAddress = Dns.GetHostAddresses(components[0])[0];
                }

                var port = Convert.ToInt32(components[1]);

                return new IPEndPoint(ipAddress, port);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// todo: does not handle Ipv6
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static uint ToDecimal(this IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var octets = ipAddress.ToString().Split('.');

                return UInt32.Parse(octets[3]) +
                    UInt32.Parse(octets[2]) * 256 +
                    UInt32.Parse(octets[1]) * 65536 +
                    UInt32.Parse(octets[0]) * 16777216;
            }
            throw new InvalidDataException("No support for IPv6");
        }
    }
}