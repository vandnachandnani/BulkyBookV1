using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Model.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWebV1.Areas.Customer.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork  unitOfWork, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
            //this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var loginUser = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM model = new ShoppingCartVM();
            model.ListCart = unitOfWork.ShoppingCart.GetList(u=>u.ApplicationUserId== loginUser, "Product");
            model.OrderHeader = new OrderHeader();
            
            foreach (var cart in model.ListCart)
            {
                cart.Price = GetPriceBasedOnQualtity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                model.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(model);
        }
        public IActionResult Summary()
        {
            var claims = (ClaimsIdentity)User.Identity;
            var loginUser = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM model = new ShoppingCartVM();
            model.ListCart = unitOfWork.ShoppingCart.GetList(u => u.ApplicationUserId == loginUser, "Product");
            model.OrderHeader = new OrderHeader();
            model.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == loginUser);
            model.OrderHeader.Name = model.OrderHeader.ApplicationUser.Name;
            model.OrderHeader.PhoneNumber = model.OrderHeader.ApplicationUser.PhoneNumber;
            model.OrderHeader.StreetAddress = model.OrderHeader.ApplicationUser.StreetAddress;
            model.OrderHeader.PhoneNumber = model.OrderHeader.ApplicationUser.PhoneNumber;
            model.OrderHeader.City = model.OrderHeader.ApplicationUser.City;
            model.OrderHeader.State = model.OrderHeader.ApplicationUser.State;
            model.OrderHeader.PostalCode = model.OrderHeader.ApplicationUser.PostalCode;
            foreach (var cart in model.ListCart)
            {
                cart.Price = GetPriceBasedOnQualtity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                model.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(model); 
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = unitOfWork.ShoppingCart.GetList(u => u.ApplicationUserId == claim.Value, "Product");


            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQualtity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            ApplicationUser applicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                unitOfWork.OrderDetail.Add(orderDetail);
                unitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //stripe settings 
                var domain = "https://localhost:44366/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                };

                foreach (var item in ShoppingCartVM.ListCart)
                {

                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            },

                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);

                }

                var service = new SessionService();
                Session session = service.Create(options);
                unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
            }
            return View(ShoppingCartVM);
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId, session.PaymentIntentId);
                    unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    unitOfWork.Save();
                }
            }
           // emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book", "<p>New Order Created</p>");
            List<ShoppingCart> shoppingCarts = unitOfWork.ShoppingCart.GetList(u => u.ApplicationUserId ==
            orderHeader.ApplicationUserId).ToList();
            unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            HttpContext.Session.Clear();
            unitOfWork.Save();
            return View(id);
        }
        public IActionResult Plus(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            unitOfWork.ShoppingCart.Increment(cart, 1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 0)
            {
                unitOfWork.ShoppingCart.Delete(cart);
                HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.GetList(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count-1);
            }
            else
            {
                unitOfWork.ShoppingCart.Decrement(cart, 1);
            }
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
            public IActionResult Remove(int cartId)
            {
                var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
                unitOfWork.ShoppingCart.Delete(cart);
                unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart, unitOfWork.ShoppingCart.GetList(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count);
            return RedirectToAction(nameof(Index));
            }
        
        private Double GetPriceBasedOnQualtity(double Qualtity,double Price,double Price50,double Price100)
        {
            if(Qualtity<=50)
            {
                return Price;
            }
            else
            {
                if (Qualtity <= 100)
                {
                    return Price50;
                }
                else
                    return Price100;
            }
        }
                                                                                                                                                                                                                                                                                                    
    }
    
}
