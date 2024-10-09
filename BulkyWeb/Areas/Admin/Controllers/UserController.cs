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
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        private IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult RoleManagment(string userid)
        {
            RoleManagmentVM roleManagmentVM = new RoleManagmentVM()
            {
                applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userid,includeProperties: "company"),
                CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.ID.ToString()

                }),
                RoleList = _roleManager.Roles.Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.Name
                })
            };



            roleManagmentVM.applicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userid)).
                GetAwaiter().GetResult().FirstOrDefault();
           

            return View(roleManagmentVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id)).
                GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagmentVM.applicationUser.Id);

            if (roleManagmentVM.applicationUser.Role != oldRole) { 
                //a role was updated
                if (roleManagmentVM.applicationUser.Role == StaticDetails.Role_Company) 
                { 
                      applicationUser.CompanyId = roleManagmentVM.applicationUser.CompanyId;
                }
                else
                {
                    applicationUser.CompanyId = null;
                }

                _unitOfWork.ApplicationUser.update(applicationUser);
                _unitOfWork.save();
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.applicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(oldRole == StaticDetails.Role_Company && applicationUser.CompanyId != roleManagmentVM.applicationUser.CompanyId)
                {
                    //company is updated
                    applicationUser.CompanyId = roleManagmentVM.applicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.update(applicationUser);
                    _unitOfWork.save();
                }
            }

           


           
           

            return View(nameof(Index));
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            List<ApplicationUser> UsersList = _unitOfWork.ApplicationUser.GetAll().ToList();


            foreach (var user in UsersList) {

                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.CompanyId == 0) {
                    user.company = new Company()
                    {
                        Name = ""
                    };
                }
                else
                {
                    user.company = _unitOfWork.Company.Get(u=> u.ID == user.CompanyId);
                }
            }

            return Json(new { data = UsersList });
        }

        [HttpPost]
        public IActionResult LockUnLock([FromBody]string userid) {
            var userFromdb = _unitOfWork.ApplicationUser.Get(u=> u.Id == userid);
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
            _unitOfWork.ApplicationUser.update(userFromdb);
            _unitOfWork.save(); 
            return Json(new { success = true, message = "Locking/UnLocking user is Successful" });
        }
    }
}
