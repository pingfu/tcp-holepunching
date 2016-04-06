using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TcpHolePunching.JsonConverters
{
    /// <summary>
    /// thanks http://stackoverflow.com/questions/18668617/json-net-error-getting-value-from-scopeid-on-system-net-ipaddress
    /// thanks http://stackoverflow.com/questions/18539534/using-json-net-control-serialization-of-final-output
    /// </summary>
    public class IpEndPointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPEndPoint);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JToken.Load(reader).ToString().ToIpEndPoint();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ipEndPoint = value as IPEndPoint;
            if (ipEndPoint != null)
            {
                if (ipEndPoint.Address != null || ipEndPoint.Port != 0)
                {
                    JToken.FromObject(string.Format("{0}:{1}", ipEndPoint.Address, ipEndPoint.Port)).WriteTo(writer);
                    return;
                }
            }
            writer.WriteNull();
        }
    }
}
