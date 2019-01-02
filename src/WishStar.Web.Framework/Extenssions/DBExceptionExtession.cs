using System;
using System.Collections.Generic;
using System.Text;

namespace WishStar.Web.Framework.Extenssions
{
    public static class DbEntityValidationExceptionExtession 
    {
        /// <summary>
        /// 记录ef实体校验异常信息
        /// </summary>
        /// <param name="dbEx"></param>
        //public static void WriteLog(this DbEntityValidationException dbEx)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine(dbEx.ToString());
        //    if (dbEx.EntityValidationErrors != null && dbEx.EntityValidationErrors.Any())
        //    {
        //        foreach (var validationError in dbEx.EntityValidationErrors)
        //        {
        //            if (!validationError.IsValid && validationError.ValidationErrors != null)
        //                foreach (var error in validationError.ValidationErrors)
        //                {
        //                    sb.AppendLine(error.ErrorMessage);
        //                }
        //        }
        //    }

        //   //LOG
        //}
    }
}
