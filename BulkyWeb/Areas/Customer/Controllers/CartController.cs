using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private ShoppingCartVM shoppingcartVM;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingcartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product")
            };

            foreach (ShoppingCart cart in shoppingcartVM.ShoppingCartList) {
                cart.Price = getPriceBasedOnQuantity(cart);
                shoppingcartVM.OrderTotal += (cart.Price * cart.Quantity);
            }
            return View(shoppingcartVM);

        }

        public IActionResult Summary() { 
            return View();
        }

        public IActionResult plus(int cartId)
        {
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u=> u.CartId == cartId);
            cartFromDB.Quantity += 1;

            _unitOfWork.ShoppingCart.update(cartFromDB);
            _unitOfWork.save();

            return RedirectToAction(nameof(Index));
           // return RedirectToAction("Index");
        }

        public IActionResult minus(int cartId) { 

            var cartFromDB = _unitOfWork.ShoppingCart.Get(u=> u.CartId == cartId);
            if (cartFromDB.Quantity == 1) { 
                _unitOfWork.ShoppingCart.Remove(cartFromDB);
            }
            else
            {
                cartFromDB.Quantity -= 1;
                _unitOfWork.ShoppingCart.update(cartFromDB);
            }

            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult delete(int cartId) { 
            
            var cartFromDB = _unitOfWork.ShoppingCart.Get(u=>u.CartId == cartId);

            _unitOfWork.ShoppingCart.Remove(cartFromDB);
            _unitOfWork.save();

            return RedirectToAction(nameof(Index));
        }

        private double getPriceBasedOnQuantity(ShoppingCart cart) {
            if (cart != null) {
                if (cart.Quantity <= 50)
                    return cart.Product.Price;
                else
                {
                    if(cart.Quantity <=100)
                        return cart.Product.Price50;
                    else
                        return cart.Product.Price100;
                }
            }
            return 0;
        }
    }
}
