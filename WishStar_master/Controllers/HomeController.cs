using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WishStar.Caching.Core;

namespace WishStar_master.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICacheService _cacheService;
        //public HomeController(ICacheService cacheService)
        //{
        //    _cacheService = cacheService;
        //}
        public IActionResult Index()
        {
            return View();
        }
    }
}