using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace WishStar.Format.Core.Protobuffer
{
    public static class SerializHelper
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static byte[] Serialize(object value)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static T Deserialize<T>(byte[] data)
        {
            if (data == null)
            {
                return default(T);
            }
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        internal static object Deserialize(byte[] data, Type type)
        {
            if (data == null)
                return null;
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize(type, stream);
            }
        }
    }
}
