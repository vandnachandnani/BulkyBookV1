using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Model.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace BulkyBookWebV1.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public CompanyController (IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
           
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? Id)
        {
            Company model = new Company();



            if (Id == 0)
            {
                return View(model);
            }
            else
            {
                model = _unitofwork.Company.GetFirstOrDefault(u => u.Id == Id);
                return View(model);

            }

        }
        [HttpPost]
        public IActionResult Upsert(Company model)
        {
            if (ModelState.IsValid)
            {

                
                if (model.Id == 0)
                {
                    _unitofwork.Company.Add(model);
                    TempData["Success"] = "Record Created Successfully";
                }
                else
                {
                    _unitofwork.Company.Update(model);
                    TempData["Success"] = "Record Updated Successfully";
                }
                _unitofwork.Save();
               
                return RedirectToAction("Index");
            }

            return View();
        }




        public IActionResult DeleteRecord(int Id)
        {
            Company company = _unitofwork.Company.GetFirstOrDefault(u => u.Id == Id);
            _unitofwork.Company.Delete(company);
            _unitofwork.Save();
            TempData["Success"] = "Record Deleted Successfully";
            return RedirectToAction("Index");

        }

        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList  = _unitofwork.Company.GetAll();
            return Json(new { data = companyList });
        }
        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            Company model = _unitofwork.Company.GetFirstOrDefault(u => u.Id == Id);
            if (model is null)
            {
                return Json(new { Success = false, Message = "Error while deleting" });
            }
           
            _unitofwork.Company.Delete(model);
            _unitofwork.Save();
            return Json(new { Success = true, Message = "Record Deleted Successfully" });
            //TempData["Success"] = "Record Deleted Successfully";
            //return RedirectToAction("Index");

        }
        #endregion
    }

}
