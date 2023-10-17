using Microsoft.AspNetCore.Mvc;
using SuperMarket.DataAccess.Data;
using SuperMarket.DataAccess.Repository.IRepository;
using SuperMarket.Models;

namespace SuperMarket.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categorys = _unitOfWork.Category.GetAll();


            return View(categorys);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder Connot exactly match the Name.");
            }
            if (!ModelState.IsValid)
            { return View(); }
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Created successfully";
            return RedirectToAction("Index", "Category");
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(c => c.CategoryId == id);
            //Category? category1 = _db.Categories.Find(id);
            //Category? category2 = _db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder Connot exactly match the Name.");
            }
            if (!ModelState.IsValid)
            { return View(); }

            _unitOfWork.Category.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Updated successfully";
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (!ModelState.IsValid)
            { return View(); }

            Category? obj = _unitOfWork.Category.Get(c => c.CategoryId == id);
            if (obj == null) { return NotFound(); }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted successfully";
            return RedirectToAction("Index", "Category");
        }
    }
}
