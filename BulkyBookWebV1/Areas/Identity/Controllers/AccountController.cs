using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Model;
using BulkyBook.Model.ViewModel;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace BulkyBookWebV1.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager,IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Login(LoginVM model,string? ReturnUrl)
        {
            if(ModelState.IsValid)
            {
                
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(ReturnUrl))
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    else
                        return Redirect(ReturnUrl);

                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login attempt");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            RegisterVM registerVM = new()
            {
                RoleList = roleManager.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name,
                }),
                CompanyList=unitOfWork.Company.GetAll().Select(u=>new SelectListItem
                {
                    Text=u.Name,
                    Value=u.Id.ToString()
                })

            };
        
        
        
            return View(registerVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if(ModelState.IsValid)
            {
                var vm = new ApplicationUser { 
                    Email = model.Email,
                    UserName = model.Email,
                    Name=model.Name,
                    StreetAddress=model.StreetAddress,
                    City=model.City,
                    State=model.State,
                    PostalCode=model.PostalCode,
                    PhoneNumber=model.PhoneNumber,
                    CompanyId=model.CompanyId,
                 };
                var result = await userManager.CreateAsync(vm, model.Password);
                
               // var admin = new IdentityRole { Name=SD.Role_Admin};
               //if(!                                                                                                                                           .RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
               // {
               //     var resadmin = roleManager.CreateAsync(admin).GetAwaiter().GetResult();
               //     var indi = new IdentityRole { Name = SD.Role_User_Indi };
               //     var reIndi = roleManager.CreateAsync(indi).GetAwaiter().GetResult();
               //     var emp = new IdentityRole { Name = SD.Role_Employee };
               //     var res = roleManager.CreateAsync(emp).GetAwaiter().GetResult();
               //     var cmp = new IdentityRole { Name = SD.Role_User_Comp };
               //     var rescmp = roleManager.CreateAsync(cmp).GetAwaiter().GetResult();
               // }
               if(string.IsNullOrEmpty( model.RoleId))
                {
                    userManager.AddToRoleAsync(vm, SD.Role_User_Indi).GetAwaiter().GetResult();
                }
                else
                {
                    userManager.AddToRoleAsync(vm, model.RoleId).GetAwaiter().GetResult();
                }
               
                if (result.Succeeded)
                {
                    if (User.IsInRole(SD.Role_Admin))
                    {
                        return RedirectToAction("Home", "Index", new { area = "Customer" });
                        TempData["Success"] = "New User Created Sucessfully!";

                    }
                    else
                    {
                        await emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode("localhost/44564")}'>clicking here</a>.");
                     await signInManager.SignInAsync(vm, isPersistent: false);
                        return RedirectToAction("Index", "Home", new { area = "Customer" });
                    }
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return View("Login");
        }
    }
}
