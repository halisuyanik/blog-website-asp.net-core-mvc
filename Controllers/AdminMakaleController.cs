using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Blog_Website.Models;
using System.Web.Helpers;
using System.IO;

namespace MVC_Blog_Website.Controllers
{
    public class AdminMakaleController : Controller
    {

        mvcBlogDB db = new mvcBlogDB();
        // GET: AdminMakale
        public ActionResult Index()
        {
            var makales = db.Makales.ToList();
            return View(makales);       
        }

        // GET: AdminMakale/Details/5
        public ActionResult Details(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            return View(makale);
        }

        // GET: AdminMakale/Create
        public ActionResult Create()
        {
            ViewBag.kategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View();
        }

        // POST: AdminMakale/Create
        [HttpPost]
        public ActionResult Create(Makale makale, string etiketler, HttpPostedFileBase gorsel)
        {
            if (ModelState.IsValid)
            {
                if (gorsel != null)
                {
                    WebImage img = new WebImage(gorsel.InputStream);
                    FileInfo fotoinfo = new FileInfo(gorsel.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makale.Gorsel = "/Uploads/MakaleFoto/" + newfoto;
                }
                else
                {
                    //Response.Write(@"< script language = 'javascript' > alert('Görsel alanı boş bırakılamaz.');</ script > ");
                    ViewBag.HataMesaji = "Görsel alanı boş bırakılamaz.";
                }
                if (etiketler != null)
                {
                    string[] etiketdizi = etiketler.Split(',');
                    foreach (var i in etiketdizi)
                    {
                        var yenietiket = new Etiket { EtiketAdi = i };
                        db.Etikets.Add(yenietiket);
                        var uyeid = Session["uyeid"];
                        makale.UyeId = Convert.ToInt32(uyeid);
                        makale.Okunma = 0;
                        makale.Tarih = DateTime.Now;
                        db.Makales.Add(makale);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.HataMesaji = "Görsel alanı boş bırakılamaz.";
                }
            }
            ViewBag.KategoriListesi = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi");
            return View(makale);
        }

        // GET: AdminMakale/Edit/5
        public ActionResult Edit(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
          
            ViewBag.kategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
            return View(makale);
        }

        // POST: AdminMakale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, HttpPostedFileBase Gorsel, Makale makale)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (Gorsel != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(makales.Gorsel)))
                    {
                        System.IO.File.Delete(Server.MapPath(makales.Gorsel));
                    }
                    WebImage img = new WebImage(Gorsel.InputStream);
                    FileInfo fotoinfo = new FileInfo(Gorsel.FileName);
                    string newfoto = Guid.NewGuid().ToString() + fotoinfo.Extension;   
                    img.Save("~/Uploads/MakaleFoto/" + newfoto);
                    makales.Gorsel = "/Uploads/MakaleFoto/" + newfoto;
                }
                makales.Baslik = makale.Baslik;
                makales.Icerik = makale.Icerik;
                makales.KategoriId = makale.KategoriId;
                ViewBag.kategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
                db.SaveChanges();

                return RedirectToAction("Index");



            }

            catch
            {
                ViewBag.kategoriId = new SelectList(db.Kategoris, "KategoriId", "KategoriAdi", makale.KategoriId);
                return View(makale);
            }
        }

        // GET: AdminMakale/Delete/5
        public ActionResult Delete(int id)
        {
            var makale = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
            if (makale == null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }

        // POST: AdminMakale/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var makales = db.Makales.Where(m => m.MakaleId == id).SingleOrDefault();
                if (makales == null)
                {
                    return HttpNotFound();
                }
                if (System.IO.File.Exists(Server.MapPath(makales.Gorsel)))
                {
                    System.IO.File.Delete(Server.MapPath(makales.Gorsel));
                }
                foreach (var i in makales.Yorums.ToList())
                {
                    db.Yorums.Remove(i);
                }
                foreach (var i in makales.Etikets.ToList())
                {
                    db.Etikets.Remove(i);
                }
                db.Makales.Remove(makales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
