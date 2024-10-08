using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        private IUnitOfWork _unitOfWork;
        public UserController(ApplicationDbContext applicationDbContext,IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _context = applicationDbContext;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult RoleManagment(string userid)
        {
            RoleManagmentVM roleManagmentVM = new RoleManagmentVM()
            {
                applicationUser = _context.applicationUsers.FirstOrDefault(u => u.Id == userid),
                CompanyList = _context.Companies.Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.ID.ToString()

                }),
                RoleList = _context.Roles.Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Name
                })
            };

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userid).RoleId;
            roleManagmentVM.applicationUser.Role = _context.Roles.FirstOrDefault(u => u.Id == roleId).Name;
           

            return View(roleManagmentVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var oldRoleID = _context.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.applicationUser.Id).RoleId;
            var oldRole = _context.Roles.FirstOrDefault(u => u.Id == oldRoleID).Name;

            if (roleManagmentVM.applicationUser.Role != oldRole) { 
                //a role was updated
                ApplicationUser applicationUser = _context.applicationUsers.FirstOrDefault(u=> u.Id == roleManagmentVM.applicationUser.Id);
                if (roleManagmentVM.applicationUser.Role == StaticDetails.Role_Company) 
                { 
                      applicationUser.CompanyId = roleManagmentVM.applicationUser.CompanyId;
                }
                else
                {
                    applicationUser.CompanyId = null;
                }

                _context.SaveChanges();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.applicationUser.Role).GetAwaiter().GetResult();   
            }

           


           
           

            return View(nameof(Index));
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            List<ApplicationUser> UsersList = _context.applicationUsers.ToList();

            var UserRoles = _context.UserRoles.ToList(); //gets data from UserRole Mapping table
            var Roles = _context.Roles.ToList();     // gets data from Role tables

            foreach (var user in UsersList) {

                var roleId = UserRoles.Where(u=> u.UserId == user.Id).FirstOrDefault().RoleId; 
                user.Role = Roles.Where(u=> u.Id == roleId).FirstOrDefault().Name;

                if (user.CompanyId == 0) {
                    user.company = new Company()
                    {
                        Name = ""
                    };
                }
                else
                {
                    user.company = _context.Companies.FirstOrDefault(u=> u.ID == user.CompanyId);
                }
            }

            return Json(new { data = UsersList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string userid) {
            var userFromdb = _context.applicationUsers.Where(u=> u.Id == userid).FirstOrDefault();
            if (userFromdb == null) {
                return Json(new { success = false, message = "Error while Locking/UnLocking" });
            }
            else
            {
                if(userFromdb.LockoutEnd!=null && userFromdb.LockoutEnd > DateTime.Now)
                    userFromdb.LockoutEnd = DateTime.Now; //unlocking the user
                else
                    userFromdb.LockoutEnd= DateTime.Now.AddDays(30); //lock user for next 30 days

            }
            _context.SaveChanges();
            return Json(new { success = true, message = "Locking/UnLocking user is Successful" });
        }
    }
}
