using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWebV1.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> data = _unitOfWork.Category.GetAll();
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["Success"] = "Record Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            if (Id > 0)
            {
                Category model = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == Id);
                if (model != null)
                {
                    return View(model);
                }

            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["Success"] = "Record Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            if (Id > 0)
            {
                Category model = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == Id);
                if (model != null)
                {
                    return View(model);
                }

            }
            return View();
        }
        [HttpPost]
        public IActionResult DeleteRecord(int Id)
        {
            if (Id > 0)
            {
                Category model = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == Id);
                if (model != null)
                {
                    _unitOfWork.Category.Delete(model);
                    _unitOfWork.Save();
                    TempData["Success"] = "Record Deleted Successfully";
                    return RedirectToAction("Index");
                }

            }
            return View();
        }

    }
}
