using Microsoft.AspNetCore.Http;
using System;

namespace WishStar.Common
{
    public class CookieHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieHelper(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public string GetCookie(string strName)
        {
            if (_httpContextAccessor.HttpContext.Request.Cookies != null && _httpContextAccessor.HttpContext.Request.Cookies[strName] != null)
                return _httpContextAccessor.HttpContext.Request.Cookies[strName].ToString();

            return "";
        }

        /// <summary>
        /// 写入cookie并设置域
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        public void SetCookieAndDomain(string key, string value, DateTime expires)
        {

            if (expires == DateTime.MinValue)
            {
                expires = DateTime.Now.AddMonths(1);
            }
            CookieOptions options = new CookieOptions()
            {
                Expires = expires,
                Path="/"
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
            
        }

        /// <summary>
        /// 如果满足判定要求，截取特定的域名地址部分字符串
        /// </summary>
        /// <param name="cookie"></param>
        //private string GetCutDomainPath()
        //{
        //    var _requestPath = _httpContextAccessor.HttpContext.Request.Host;
        //    if (!_requestPath.Equals("localhost") && HttpContext.Current.Request.Url.HostNameType == UriHostNameType.Dns)
        //    {
        //        var firstDot = _requestPath.IndexOf(".");
        //        cookie.Domain = _requestPath.Substring(firstDot + 1, _requestPath.Length - firstDot - 1);
        //    }
        //}
    }
}
