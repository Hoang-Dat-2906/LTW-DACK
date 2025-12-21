using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DACK.Models;


namespace DACK.Controllers
{
    public class ProductsController : Controller
    {
        private ShopThoiTrangEntities1 db = new ShopThoiTrangEntities1();       
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Product.ToList();

            return View(products);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            var product = db.Product.Find(id);
            if (product == null) return HttpNotFound();

            // Lấy list đánh giá
            var reviews = (from r in db.ProductReview
                           join oi in db.OrderItem on r.OrderItemId equals oi.OrderItemId
                           join pv in db.ProductVariant on oi.VariantId equals pv.VariantId // <--- Thêm dòng này để Join bảng Variant
                           join o in db.Order on oi.OrderId equals o.OrderId

                           where pv.ProductId == id // <--- Sửa điều kiện lọc tại đây (dùng pv thay vì oi.ProductVariant)
                           select new
                           {
                               FullName = o.CustomerName,
                               Rating = r.Rating,
                               Comment = r.Comment,
                               CreatedAt = r.CreatedAt,
                               SizeName = oi.SizeName,
                               ColorName = oi.ColorName
                           }).OrderByDescending(x => x.CreatedAt).ToList();

            ViewBag.Reviews = reviews;

            return View(product);
        }



        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductId,SKU,ProductName,Slug,Description,BasePrice,IsActive,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductId,SKU,ProductName,Slug,Description,BasePrice,IsActive,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }


        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
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
