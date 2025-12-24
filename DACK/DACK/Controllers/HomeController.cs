using DACK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DACK.Controllers
{
    public class HomeController : Controller
    {
        private ShopThoiTrangEntities1 db = new ShopThoiTrangEntities1();
        public ActionResult Index()
        {
            // Thêm .Include(p => p.ProductImage) để lấy danh sách hình ảnh
            var products = db.Product.Include(p => p.ProductImage).ToList();
            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}