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
                productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                return View(productViewModel);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductViewModel productvm,List<IFormFile> files)
        {
            if (productvm == null)
                return NotFound();

            if (productvm.Product.Id == 0)
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

            if (ModelState.IsValid)
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                foreach (IFormFile file in files)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = @"images\product\product-" + productvm.Product.Id;
                    string finalpath = Path.Combine(webRootPath, productpath);


                    if(!Directory.Exists(finalpath))
                        Directory.CreateDirectory(finalpath);

                    using (var fileStream = new FileStream(Path.Combine(finalpath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new ProductImage()
                    {
                        ImageURL = @"\" + productpath + @"\" + filename,
                        ProductID = productvm.Product.Id
                    };

                    if(productvm.Product.ProductImages == null)
                        productvm.Product.ProductImages = new List<ProductImage>();

                    productvm.Product.ProductImages.Add(productImage);

                    
                }

                _unitOfWork.Product.update(productvm.Product); //when saving product, this inturn saves product image table as well
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
       

        public IActionResult DeleteImage(int imageId)
        {
            ProductImage productImage = _unitOfWork.ProductImage.Get(u => u.id == imageId);
            if (productImage != null)
            {
                if (!string.IsNullOrEmpty(productImage.ImageURL))
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    var imagePath = Path.Combine(webRootPath, productImage.ImageURL.Trim('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(productImage);
                _unitOfWork.save();

                TempData["Success"] = "Deleted Successfully";
            }

            

            return RedirectToAction(nameof(Upsert), new { id = productImage.ProductID});
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
            var product = _unitOfWork.Product.Get(u => u.Id == id,includeProperties: "ProductImages");
            if (product == null)
                return Json(new { success = false, message = "Error while deleting" });

            var webroot = _webHostEnvironment.WebRootPath;
            List<ProductImage> productImages = product.ProductImages;
            foreach(var image in productImages)
            {
                var oldpath = Path.Combine(webroot, image.ImageURL.Trim('\\'));
                if (System.IO.File.Exists(oldpath))
                {
                    System.IO.File.Delete(oldpath);
                }
            }

            //delete folder 
            string productpath = @"images\product\product-" + id;
            string finalfolderpath = Path.Combine(webroot, productpath.Trim('\\'));


            if (Directory.Exists(finalfolderpath))
                Directory.Delete(finalfolderpath);



            _unitOfWork.Product.Remove(product);
            _unitOfWork.save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}


