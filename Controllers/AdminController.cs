using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Blog_Website.Models;
using System.IO;

namespace MVC_Blog_Website.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        mvcBlogDB db = new mvcBlogDB();
        public ActionResult Index()
        {
            var uyeler = db.Uyes;
            return View(uyeler);
        }
    }
}