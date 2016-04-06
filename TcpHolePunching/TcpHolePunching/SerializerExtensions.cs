using System.IO;
using MsgPack.Serialization;

namespace TcpHolePunching
{
	public static class SerializerExtensions
    {
        /// <summary>
        /// http://msgpack.org/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T BinaryDeserialize<T>(this byte[] bytes)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var byteStream = new MemoryStream(bytes))
            {
                return serializer.Unpack(byteStream);
            }
        }

        /// <summary>
        /// http://msgpack.org/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisObj"></param>
        /// <returns></returns>
        public static byte[] BinarySerialize<T>(this T thisObj)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var byteStream = new MemoryStream())
            {
                serializer.Pack(byteStream, thisObj);
                return byteStream.ToArray();
            }
        }
	}
}