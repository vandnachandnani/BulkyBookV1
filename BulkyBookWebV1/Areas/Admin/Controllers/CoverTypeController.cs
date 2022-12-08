using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWebV1.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> data = _unitOfWork.CoverType.GetAll();
            return View(data);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
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
                CoverType model = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);
                if (model != null)
                {
                    return View(model);
                }

            }
            return View();
        }
        [HttpPost]
        public IActionResult Edit(CoverType category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(category);
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
                CoverType model = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);
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
                CoverType model = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == Id);
                if (model != null)
                {
                    _unitOfWork.CoverType.Delete(model);
                    _unitOfWork.Save();
                    TempData["Success"] = "Record Deleted Successfully";
                    return RedirectToAction("Index");
                }

            }
            return View();
        }

    }
}

