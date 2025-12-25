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
        private ShopThoiTrangEntities1 db = new ShopThoiTrangEntities1 ();

        public ActionResult Index(string searchString, string categoryGroup, string sortOrder)
        {
            // 1. Khởi tạo Query an toàn (tránh lỗi .Value bằng cách so sánh trực tiếp)
            var products = db.Product.Include(p => p.ProductImage).Where(p => p.IsActive == true);

            // 2. Lọc theo nhóm CategoryGroup
            if (categoryGroup == "ao")
            {
                var aoIds = new List<int?> { 1, 2, 3 };
                products = products.Where(p => aoIds.Contains(p.CategoryId));
            }
            else if (categoryGroup == "quan")
            {
                var quanIds = new List<int?> { 6,7 };
                products = products.Where(p => quanIds.Contains(p.CategoryId));
            }
            else if (categoryGroup == "phu-kien")
            {
                var phuKienIds = new List<int?> { 4, 5, 8, 9, 10 };
                products = products.Where(p => phuKienIds.Contains(p.CategoryId));
            }

            // 3. Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }

            // 4. Sắp xếp (Sort)
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.BasePrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.BasePrice);
                    break;
                default:
                    products = products.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            // 5. Gán lại dữ liệu cho ViewBag
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategoryGroup = categoryGroup;

            return View(products.ToList());
        }
        public ActionResult Details(int? id)
        {
            var product = db.Product.Find(id);
            if (product == null)
                return HttpNotFound();

            var relatedProducts = db.Product
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id && p.IsActive == true)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;

            var reviews = (
                from r in db.ProductReview
                join oi in db.OrderItem on r.OrderItemId equals oi.OrderItemId
                join pv in db.ProductVariant on oi.VariantId equals pv.VariantId
                join o in db.Order on oi.OrderId equals o.OrderId
                where pv.ProductId == id
                orderby r.CreatedAt descending
                select new ReviewViewModel
                {
                    FullName = o.CustomerName,  
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    SizeName = oi.SizeName,
                    ColorName = oi.ColorName
                }
            ).ToList();

            ViewBag.Reviews = reviews;

            return View(product);
        }

        public ActionResult Ao()
        {
            var listAo = db.Product.Include(p => p.ProductImage)
                                   .Where(p => (p.CategoryId == 1 || p.CategoryId == 2 || p.CategoryId == 3)
                                                && p.IsActive == true)
                                   .ToList();
            return View(listAo);
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
