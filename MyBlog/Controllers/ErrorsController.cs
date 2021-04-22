using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class ErrorsController : Controller
    {
        //original
        public IActionResult Index()
        {
            return View();
        }

        //annex
        public IActionResult UploadError()
        {
            return View();
        }

        //annex
        public IActionResult ExtensionsError()
        {
            return View();
        }
    }
}
