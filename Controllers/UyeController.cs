using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MVC_Blog_Website.Models;

namespace MVC_Blog_Website.Controllers
{
    public class UyeController : Controller
    {
        mvcBlogDB db = new mvcBlogDB();

        // GET: Uye
        public ActionResult Index(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(Uye uye)
        {
            var login = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi && u.Sifre == uye.Sifre).SingleOrDefault();
            if (login != null)
            {
                TempData["girissonuc"] = "";
                Session["uyeid"] = login.UyeId;
                Session["kullaniciadi"] = login.KullaniciAdi;
                Session["yetkiid"] = login.YetkiId;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["girissonuc"] = "Kullanıcı adı veya şifre yanlış.";
                return RedirectToAction("Login", "Uye");
            }
            
        }



        public ActionResult Logout()
        {
            Session["yetkiid"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }



        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Uye uye, HttpPostedFileBase gorsel)
        {
            var kadikontrol = db.Uyes.Where(u => u.KullaniciAdi == uye.KullaniciAdi).SingleOrDefault();
            //bool dene = kadikontrol.KullaniciAdi != uye.KullaniciAdi;
            if (ModelState.IsValid)
            {
                if (kadikontrol == null)
                {
                    if (uye.Sifre != null && uye.Sifre.Length >= 6)
                    {
                        TempData["kayıtsonuc"] = "";
                        WebImage img = new WebImage(gorsel.InputStream);
                        FileInfo fotoinfo = new FileInfo(gorsel.FileName);
                        string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                        img.Resize(150, 150);
                        img.Save("~/Uploads/UyeFoto/" + newfoto);
                        uye.Foto = "/Uploads/UyeFoto/" + newfoto;
                        uye.YetkiId = 2;       
                        db.Uyes.Add(uye);
                        db.SaveChanges();
                        Session["yetkiid"] = uye.YetkiId;
                        Session["kullaniciadi"] = uye.KullaniciAdi;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["kayıtsonuc"] = "Şifre uzunluğu en az 6 karakter içermelidir.";
                        return RedirectToAction("Create", "Uye");
                    }
                }
                else
                {
                    TempData["kayıtsonuc"] = "Bu kullanıcı adı daha önce alınmış.";
                    return RedirectToAction("Create", "Uye");
                }
            }
            return View();
        }
        public ActionResult Edit(int id)
        {
            var uye = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
            if (Convert.ToInt32(Session["uyeid"]) != uye.UyeId)
            {
                return HttpNotFound();
            }
            return View(uye);
        }
        [HttpPost]
        public ActionResult Edit(Uye uye, int id, HttpPostedFileBase Gorsel)
        {
            if (ModelState.IsValid)
            {
                var uyes = db.Uyes.Where(u => u.UyeId == id).SingleOrDefault();
                if (Gorsel != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(uyes.Foto)))
                    {
                        System.IO.File.Delete(Server.MapPath(uyes.Foto));
                    }
                    WebImage img = new WebImage(Gorsel.InputStream);
                    FileInfo fotoinfo = new FileInfo(Gorsel.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Resize(150, 150);
                    img.Save("~/Uploads/UyeFoto/" + newfoto);
                    uyes.Foto = "/Uploads/UyeFoto/" + newfoto;
                }
                uyes.AdSoyad = uye.AdSoyad;
                uyes.Sifre = uye.Sifre;
                uyes.Email = uye.Email;
                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { id = uyes.UyeId });
            }
            return View();
        }
    }
}