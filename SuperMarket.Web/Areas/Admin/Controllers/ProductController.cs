using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperMarket.DataAccess.Repository.IRepository;
using SuperMarket.Models;
using SuperMarket.Models.ViewModels;
using System.Collections.Generic;

namespace SuperMarket.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return View(products);

        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(s => new SelectListItem
            //{
            //    Text = s.Name,
            //    Value = s.CategoryId.ToString()
            //});

            //ViewBag.categoryList = categoryList;

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.CategoryId.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(p => p.Id == id);
                return View(productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (productVM.Product.Title == productVM.Product.Brand.ToString())
            {
                ModelState.AddModelError("Title", "The Brand Connot exactly match the title.");
            }

            if (productVM.Product.ListPrice < productVM.Product.Price && productVM.Product.ListPrice < productVM.Product.Price12 && productVM.Product.ListPrice < productVM.Product.Price24)
            {
                ModelState.AddModelError("ListPrice", "Please set List Price to a value greater than Price.");
            }

            if (productVM.Product.Price < productVM.Product.Price12 && productVM.Product.Price < productVM.Product.Price24)
            {
                ModelState.AddModelError("Price", "Price set Price to a value greater than Price12+.");
            }

            if (productVM.Product.Price12 < productVM.Product.Price24)
            {
                ModelState.AddModelError("Price12+", "Price set Price12+ to a value greater than Price42+.");
            }

            if (!ModelState.IsValid)
            {
                //ViewBag.categoryList = categoryList;
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.CategoryId.ToString()
                });
                return View(productVM);
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    //delete the old image
                    var oldImagePath =
                        Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }

            if (productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
            }

            _unitOfWork.Save();
            TempData["success"] = "Product Created successfully";
            return RedirectToAction("Index", "Product");
        }
 
        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = products });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath =
                           Path.Combine(_webHostEnvironment.WebRootPath,
                           productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Product Successful" });
        }

        #endregion
    }
}
