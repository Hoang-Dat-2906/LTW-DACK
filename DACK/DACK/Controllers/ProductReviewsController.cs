using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DACK.Models;

namespace DACK.Controllers
{
    public class ProductReviewsController : Controller
    {
        private ShopThoiTrangEntities1 db = new ShopThoiTrangEntities1();

        // GET: ProductReviews
        public ActionResult Index()
        {
            var productReview = db.ProductReview.Include(p => p.OrderItem);
            return View(productReview.ToList());
        }

        // GET: ProductReviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReview.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        public ActionResult Create(int productId)
        {
            var user = Session["user"] as AppUser;
            if (user == null) return RedirectToAction("Login", "AppUsers");
            var purchasedItem = (from oi in db.OrderItem
                                 join o in db.Order on oi.OrderId equals o.OrderId
                                 join pv in db.ProductVariant on oi.VariantId equals pv.VariantId
                                 where pv.ProductId == productId && o.UserId == user.UserId
                                 select oi).FirstOrDefault();

          
            if (purchasedItem == null)
            {
                TempData["ReviewMessage"] = "Bạn chưa mua sản phẩm này nên không thể đánh giá";
                TempData["MessageType"] = "danger"; // Màu đỏ cho Alert
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            var model = new ProductReview
            {
                OrderItemId = purchasedItem.OrderItemId,
               
                ReviewCode = "REV-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Rating = 5
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductReview review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                db.ProductReview.Add(review);
                db.SaveChanges();

                // Tìm ProductId để quay về trang chi tiết
                var productId = (from oi in db.OrderItem
                                 join pv in db.ProductVariant on oi.VariantId equals pv.VariantId
                                 where oi.OrderItemId == review.OrderItemId
                                 select pv.ProductId).FirstOrDefault();

                return RedirectToAction("Details", "Products", new { id = productId });
            }
            return View(review);
        }

        // GET: ProductReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReview.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderItemId = new SelectList(db.OrderItem, "OrderItemId", "ProductName", productReview.OrderItemId);
            return View(productReview);
        }

        // POST: ProductReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ReviewId,ReviewCode,OrderItemId,Rating,Comment,CreatedAt")] ProductReview productReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OrderItemId = new SelectList(db.OrderItem, "OrderItemId", "ProductName", productReview.OrderItemId);
            return View(productReview);
        }

        // GET: ProductReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReview.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        // POST: ProductReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductReview productReview = db.ProductReview.Find(id);
            db.ProductReview.Remove(productReview);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
