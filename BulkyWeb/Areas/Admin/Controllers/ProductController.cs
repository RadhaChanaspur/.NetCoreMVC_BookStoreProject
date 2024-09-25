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

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
           List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties:"category").ToList();
            //each product has Category attribute to map foreign key but by default it is null
            //so in repository we have to explicity include this navigation property called Category
           return View(productsList);
        }

      

        public IActionResult Upsert(int? id) //Update + Insert( Combining both create and Edit forms)
        {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            ProductViewModel productViewModel = new()
            {
                Product = new Product(),
                CategoryList = categoryList
            };

            if (id == null || id == 0)
            {
                //create
                return View(productViewModel);
            }
            else
            {
                //update
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productViewModel);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productvm,IFormFile? file)
        {
            if (productvm == null)
                return NotFound();

            if (ModelState.IsValid)
            {

                string webRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(webRootPath, @"images\product");


                    if(!string.IsNullOrEmpty(productvm.Product.ImageURL))
                    {
                        //image already exists delete it
                        string oldImagePath = Path.Combine(webRootPath, productvm.Product.ImageURL);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productvm.Product.ImageURL = @"images\product\" + filename;
                }


                if(productvm.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productvm.Product);
                    TempData["success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.update(productvm.Product);
                    TempData["success"] = "Product Updated Successfully";
                }

                _unitOfWork.save();
                return RedirectToAction("Index", "Product");
            }
            else
            {
                //if there are any errors, the dropdown wont be loaded with the data on post hence binding the data again and passing it to view
                productvm.CategoryList = _unitOfWork.Category.GetAll().Select(Category => new SelectListItem()
                {
                    Text=Category.Name,
                    Value = Category.Id.ToString()
                });
            }

            return View(productvm);
        }

  


    


        #region APICalls

        //to bind data to products grid we are using ajax calls. In MVC APIs calls are supported by default in controllers, hence we can
        //directly add get method to access this /Admin/Product/GetProducts
        [HttpGet]
        public IActionResult GetProducts() {
            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return Json(new { data = productsList });
        }

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var product = _unitOfWork.Product.Get(u => u.Id == id);
            if (product == null)
                return Json(new { success = false, message = "Error while deleting" });

            var webroot = _webHostEnvironment.WebRootPath;
            var oldpath = Path.Combine(webroot, product.ImageURL);
            if (System.IO.File.Exists(oldpath))
            {
                System.IO.File.Delete(oldpath);
            }

            _unitOfWork.Product.Remove(product);
            _unitOfWork.save();

            List<Product> productsList = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}


