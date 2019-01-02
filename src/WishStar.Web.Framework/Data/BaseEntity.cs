using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WishStar.Web.Framework.Data
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// 获取唯一键
        /// </summary>
        /// <returns></returns>
        public abstract object GetKey();

        /// <summary>
        /// 是否新建
        /// </summary>
        [NotMapped]
        public bool IsNew { get; set; }
    }
}
