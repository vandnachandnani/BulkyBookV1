using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWebV1.ViewComponents
{
    public class ShoppingCartViewComponent:ViewComponent
    {
        private readonly IUnitOfWork unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork )
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult>  InvokeAsync()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var Id = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (Id != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    int count = unitOfWork.ShoppingCart.GetList(x => x.ApplicationUserId == Id.Value).ToList().Count;
                    HttpContext.Session.SetInt32(SD.SessionCart, count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
           
        }
    }
}
