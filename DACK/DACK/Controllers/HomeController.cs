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
        [HttpPost]
        public ActionResult SendContact(string FullName, string Email, string MessageText)
        {
            try
            {
                var contact = new ContactMessages();
                contact.FullName = FullName;
                contact.Email = Email;
                contact.MessageText = MessageText;
                contact.CreatedAt = DateTime.Now;
                contact.IsRead = false;

                db.ContactMessages.Add(contact);
                db.SaveChanges(); 

                TempData["Message"] = "Gửi thành công!";
                return RedirectToAction("Contact");
            }
            catch (Exception ex)
            {
                return Content("Lỗi: " + ex.Message);
            }
        }
    }
}