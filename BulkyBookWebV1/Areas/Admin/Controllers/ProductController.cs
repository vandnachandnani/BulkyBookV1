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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upsert(int? Id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()

                }),
                CoverTypeList = _unitofwork.CoverType.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if (Id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == Id);
                return View(productVM);

            }

        }
        [HttpPost]
        public IActionResult Upsert(ProductVM model, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                var rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(rootPath, @"Images\Product");
                    var extention = Path.GetExtension(file.FileName);
                    if (model.Product.ImageUrl != null)
                    {
                        string oldImage = Path.Combine(rootPath, model.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, fileName + extention), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    };
                    model.Product.ImageUrl = @"\Images\Product\" + fileName + extention;
                }
                if (model.Product.Id == 0)
                {
                    _unitofwork.Product.Add(model.Product);
                }
                else
                {
                    _unitofwork.Product.Update(model.Product);
                }
                _unitofwork.Save();
                TempData["Success"] = "Record Updated Successfully";
                return RedirectToAction("Index");
            }

            return View();
        }




        public IActionResult DeleteRecord(int Id)
        {
            Product product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == Id);
            _unitofwork.Product.Delete(product);
            _unitofwork.Save();
            TempData["Success"] = "Record Deleted Successfully";
            return RedirectToAction("Index");

        }

        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var product = _unitofwork.Product.GetAll("Category,CoverType");
            return Json(new { data = product });
        }
        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            Product product = _unitofwork.Product.GetFirstOrDefault(u => u.Id == Id);
            if (product is null)
            {
                return Json(new { Success = false, Message = "Error while deleting" });
            }
            string oldImage = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }


            _unitofwork.Product.Delete(product);
            _unitofwork.Save();
            return Json(new { Success = true, Message = "Record Deleted Successfully" });
            //TempData["Success"] = "Record Deleted Successfully";
            //return RedirectToAction("Index");

        }
        #endregion
    }

}
