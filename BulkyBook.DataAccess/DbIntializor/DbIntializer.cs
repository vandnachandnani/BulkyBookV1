using BulkyBook.DataAccess.Data;
using BulkyBook.Model;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbIntializor
{
    public class DbIntializer : IDbIntializer
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DbIntializer(ApplicationDbContext applicationDbContext,UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager )
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

       

        public void Intializer()
        {
            try
            {
                if(applicationDbContext.Database.GetPendingMigrations().Count()>0 )
                {
                    applicationDbContext.Database.Migrate(); 
                }
                if(!roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    var adminrole = new IdentityRole { Name = SD.Role_Admin };
                    roleManager.CreateAsync(adminrole).GetAwaiter().GetResult();         
                    var employee = new IdentityRole { Name = SD.Role_Employee };
                    roleManager.CreateAsync(employee).GetAwaiter().GetResult();

                    var compnyRole = new IdentityRole { Name = SD.Role_User_Comp };
                    roleManager.CreateAsync(compnyRole).GetAwaiter().GetResult();
                    var IndiRole = new IdentityRole { Name = SD.Role_User_Indi };
                    roleManager.CreateAsync(IndiRole).GetAwaiter().GetResult();
                }
                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Test@gmail.com",
                    Email = "Test@gmail.comm",
                    Name = "vandana",
                    PhoneNumber = "1112223333",
                    StreetAddress = "test 123 Ave",
                    State = "IL",
                    PostalCode = "23422",
                    City = "Chicago"
                },"Test@123456789").GetAwaiter().GetResult();
                ApplicationUser User = applicationDbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "chandnanivandna@gmail.com");
                userManager.AddToRoleAsync(User, SD.Role_Admin);
            }
            catch(Exception ex)
            {

            }
            
        }
    }
}
