using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using WishStar.Web.Framework.Data;

namespace WishStar.Format.Core.Protobuffer
{
    public class ProtoBufMessage
    {
        public ProtoBufMessage() { }

        public ProtoBufMessage(byte[] content, string contentType)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Content = content;
            this.ContentType = contentType;
        }
        /// <summary>
        /// id
        /// </summary>
        [ProtoMember(1)]
        public string Id { get; set; }

        /// <summary>
        /// 文本压缩内容
        /// </summary>
        [ProtoMember(2)]
        public byte[] Content { get; set; }

        /// <summary>
        /// 文本类型
        /// </summary>
        [ProtoMember(3)]
        public string ContentType { get; set; }

        public object GetObject()
        {
            Type type = Type.GetType(this.ContentType);
            return SerializHelper.Deserialize(this.Content, type);
        }

        public T Get<T>() where T : BaseModel
        {
            return SerializHelper.Deserialize<T>(this.Content);
        }

        public static ProtoBufMessage GetProtoBufMessage<T>(T data) 
            where T : BaseModel
        {
            var content = SerializHelper.Serialize(data);
            var type = typeof(T).Name;
            return new ProtoBufMessage(content, type);
        }
    }
}
