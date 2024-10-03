using Bulky.DataAccess.Data;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public DBInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            applicationDbContext = db;
            _userManager = userManager;
            _rolemanager = roleManager;
        }

        public void Initialize()
        {
            //apply pending migrations
            try
            {
                if (applicationDbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    applicationDbContext.Database.Migrate();
                }
            }
            catch(Exception ex) 
            { 
            }


            //create role if they are not created
            if (!_rolemanager.RoleExistsAsync(StaticDetails.Role_Admin).GetAwaiter().GetResult())
            {
                _rolemanager.CreateAsync(new IdentityRole(StaticDetails.Role_Customer)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(StaticDetails.Role_Employee)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(StaticDetails.Role_Admin)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(StaticDetails.Role_Company)).GetAwaiter().GetResult();

                //only for the first time we will create admin user

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@test.com",
                    Email ="admin@test.com",
                    Name = "Radha C",
                    PhoneNumber = "12334",
                    StreeAddress = "Yaradona",
                    State = "Karnataka",
                    City = "Karatagi",
                    Postalcode = "583229"

                }, "Admin@123").GetAwaiter().GetResult();

                ApplicationUser user = applicationDbContext.applicationUsers.FirstOrDefault(u => u.Email == "admin@test.com");
                _userManager.AddToRoleAsync(user, StaticDetails.Role_Customer).GetAwaiter().GetResult();
            }

            return;
        }
    }
}
