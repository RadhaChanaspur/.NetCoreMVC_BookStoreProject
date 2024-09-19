using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Bulky.Models.ViewModel;


namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
           List<Product> productsList = _unitOfWork.Product.GetAll().ToList();
           return View(productsList);
        }

        public IActionResult Create()
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
            return View(productViewModel);
        }
        [HttpPost]
        public IActionResult Create(ProductViewModel productvm)
        {
            if (productvm == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productvm.Product);
                _unitOfWork.save();
                TempData["success"] = "Product Created Successfully";
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

        public IActionResult Edit(int? id) {
            if (id == null || id == 0)
                return NotFound();

            Product product = _unitOfWork.Product.Get(u  => u.Id == id);
            if(product != null)
            {
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Product product) {
            if (product == null)
                return View();


            _unitOfWork.Product.update(product);
            _unitOfWork.save();
            TempData["success"] = "Product Edited Successfully";
            return RedirectToAction("Index", "Product");
        }


        public IActionResult Delete(int? id)
        {
            if(id == null || id==0)
                return NotFound();

            Product product = _unitOfWork.Product.Get(u=> u.Id == id);
            if(product != null)
                return View(product);

            return NotFound();
        }

        [HttpPost]
        public IActionResult Delete(Product obj)
        {
            if(obj == null)
                return NotFound();

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.save();
            TempData["success"] = "Product deleted Successfully";
            return RedirectToAction("Index", "Product");
        }
        
    }
}
