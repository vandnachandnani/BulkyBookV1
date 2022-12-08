using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Utility;
using BulkyBookWebV1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWebV1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unifogwork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unifogwork= unitOfWork;
        }

        public IActionResult Index()
         {
            IEnumerable<Product> model = _unifogwork.Product.GetAll("Category,CoverType");
            return View(model);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unifogwork.Product.GetFirstOrDefault(u => u.Id == ProductId, includeProperty: "Category,CoverType"),
                ProductId= ProductId,
                Count = 1
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart model)
        {
             var test = (ClaimsIdentity)User.Identity;
            
            model.ApplicationUserId= test.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCart cartFromDb=_unifogwork.ShoppingCart.GetFirstOrDefault(u=>u.ProductId==model.ProductId && u.ApplicationUserId==model.ApplicationUserId);
            if (cartFromDb == null)
            {
                _unifogwork.ShoppingCart.Add(model);
                _unifogwork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unifogwork.ShoppingCart.GetList(x=>x.ApplicationUserId==test.FindFirst(ClaimTypes.NameIdentifier).Value).ToList().Count);
            }
            else
            {
                _unifogwork.ShoppingCart.Increment(cartFromDb, model.Count);
                _unifogwork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, _unifogwork.ShoppingCart.GetList(x => x.ApplicationUserId == test.FindFirst(ClaimTypes.NameIdentifier).Value).ToList().Count);
            }
           
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}