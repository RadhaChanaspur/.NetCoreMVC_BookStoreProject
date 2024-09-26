using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Bulky.Models.ViewModel;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.AspNetCore.Hosting;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;


namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
           List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
           return View(companyList);
        }

      

        public IActionResult Upsert(int? id) //Update + Insert( Combining both create and Edit forms)
        {
            if (id == null || id == 0)
                return View(new Company());

            Company company = _unitOfWork.Company.Get(u => u.ID == id);
            return View(company);
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (company == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (company.ID == 0 || company.ID == null)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company Updated Successfully";

                }
                _unitOfWork.save();
                return RedirectToAction("Index","Company");
            }

            return View(company);
        }

  


    


        #region APICalls

        //to bind data to products grid we are using ajax calls. In MVC APIs calls are supported by default in controllers, hence we can
        //directly add get method to access this /Admin/Product/GetProducts
        [HttpGet]
        public IActionResult GetCompanies() {
            List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var company = _unitOfWork.Company.Get(u => u.ID == id);
            if (company == null)
                return Json(new { success = false, message = "Error while deleting" });


            _unitOfWork.Company.Remove(company);
            _unitOfWork.save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}


