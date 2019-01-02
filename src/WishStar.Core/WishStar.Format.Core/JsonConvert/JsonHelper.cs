using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace WishStar.Format.Core.JsonConvert
{
    public class JsonHelper
    {
        /// <summary>
        /// 时间转换器
        /// </summary>
        private static readonly IsoDateTimeConverter TimeConverter;

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObject(object value)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(value, TimeConverter);
            }
            catch (Exception e)
            {
                //log
                return null;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string value)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value, TimeConverter);
            }
            catch (Exception e)
            {
                //log
                return default(T);
            }
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T LoadFile<T>(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);

            return DeserializeObject<T>(json);
        }

        public static T BinaryDeserialize<T>(string value)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(value);
            T result;
            using (MemoryStream ms=new MemoryStream(byteArray))
            {
                object temp = new BinaryFormatter().Deserialize(ms);
                result = (temp != null && temp is T) ? (T)temp : default(T);
            }
            return result;
        }

        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <remarks>对象必须标记为可序列化:Serializable</remarks>
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns> 
        public static byte[] BinarySerializeObject(object value)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, value);
                    return ms.GetBuffer();
                }
            }
            catch (Exception e)
            {
                //log
                return null;
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原         
        /// </summary>
        /// <remarks>对象必须标记为可序列化:Serializable</remarks>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static T BinaryDeserializeObject<T>(byte[] Bytes)
        {
            object obj = null;
            try
            {
                using (MemoryStream ms = new MemoryStream(Bytes))
                {
                    IFormatter formatter = new BinaryFormatter();
                    obj = formatter.Deserialize(ms);
                    if (obj != null)
                    {
                        return (T)obj;
                    }
                }
            }
            catch (Exception e)
            {
                //log
            }
            return default(T);
        }

        static JsonHelper()
        {
            TimeConverter = new IsoDateTimeConverter();
            TimeConverter.DateTimeFormat= "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        }
    }
}
