using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Blog_Website.Models;
using PagedList;
using PagedList.Mvc;

namespace MVC_Blog_Website.Controllers
{
    public class HomeController : Controller
    {
        mvcBlogDB db = new mvcBlogDB();
        // GET: Home
        public ActionResult Index(int Page=1)
        {
            var makaledb = db.Makales.OrderByDescending(m => m.Tarih).ToPagedList(Page,5);
            return View(makaledb);
        }
        public ActionResult Hakkinda()
        {
            return View();
        }
        public ActionResult Iletisim()
        {
            return View();
        }
        public ActionResult KategoriPartial()
        {
            return View(db.Kategoris.ToList());
        }
        public ActionResult MakaleDetay(int id)
        {
            var makale = db.Makales.Where(m=>m.MakaleId==id).SingleOrDefault();
            if (makale==null)
            {
                return HttpNotFound();
            }
            return View(makale);
        }
        public ActionResult sliderPartial()
        {
            var makale = db.Makales;
            return View(makale);
        }
        public ActionResult YorumSil(int id)
        {
            var uyeid = Session["uyeid"];
            var yorum = db.Yorums.Where(u=>u.YorumId==id).SingleOrDefault();
            var makale = db.Makales.Where(m=>m.MakaleId==yorum.MakaleId).SingleOrDefault();
            if (yorum.UyeId==Convert.ToInt32(uyeid))
            {
                db.Yorums.Remove(yorum);
                db.SaveChanges();
                return RedirectToAction("MakaleDetay","Home", new {id=makale.MakaleId });
            }
            else
            {
                return HttpNotFound();
            }
        }
        public JsonResult YorumYap(string yorum,int MakaleId)
        {

            var uyeid = Session["uyeid"];
            if (yorum!=null)
            {
                db.Yorums.Add(new Yorum { UyeId=Convert.ToInt32(uyeid),MakaleId=MakaleId,Icerik=yorum,Tarih=DateTime.Now});
                db.SaveChanges();
            }
            return Json(false,JsonRequestBehavior.AllowGet);
        }
        public ActionResult OkunmaSayısınıArttır(int MakaleId)
        {
            var makale = db.Makales.Where(m=>m.MakaleId==MakaleId).SingleOrDefault();
            makale.Okunma += 1;
            db.SaveChanges();
            return View();
        }
        public ActionResult Kategori(int id)
        {

            //var kategori = db.Kategoris.Where(k=>k.KategoriId==id).SingleOrDefault();
            var makaleler = db.Makales.Where(m => m.KategoriId == id).ToList();
            if (makaleler==null)
            {
                return HttpNotFound();
            }
            return View(makaleler.OrderByDescending(m=>m.Tarih));
        }
        public ActionResult BlogAra(string Ara)
        {
 
             
            return View(aranan.OrderByDescending(o=>o.Tarih));
        }
    }
}