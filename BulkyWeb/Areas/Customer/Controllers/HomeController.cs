using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
           

            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "category");
            return View(products);
        }
        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "category"),
                Quantity = 1,
                ProductId = id
            };
            return View(shoppingCart);

        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart obj)
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            obj.ApplicationUserId = userId;


            ShoppingCart cartfromDB = _unitOfWork.ShoppingCart.Get(u=> u.ProductId == obj.ProductId && u.ApplicationUserId == userId);
            if (cartfromDB != null)
            {
                //exists
                cartfromDB.Quantity += obj.Quantity;
                _unitOfWork.ShoppingCart.update(cartfromDB);
                _unitOfWork.save();
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(obj);
                _unitOfWork.save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }

            
            TempData["success"] = "Card Updated Successfully";

            return RedirectToAction("Index","Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
