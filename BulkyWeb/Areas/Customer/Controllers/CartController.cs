using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty] //when we add this, values on the form will be binded to the model passed on to the view, since this 
        //property is updated with UI values we can use this directly in summaryPost method
        public ShoppingCartVM shoppingcartVM { get; set; }

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
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                Order = new Order()
            };

            foreach (ShoppingCart cart in shoppingcartVM.ShoppingCartList) {
                cart.Price = getPriceBasedOnQuantity(cart);
                shoppingcartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
            return View(shoppingcartVM);

        }

        public IActionResult Summary() {

            var userIdentity = (ClaimsIdentity)User.Identity;
            var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingcartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                Order = new Order()
            };

            shoppingcartVM.Order.applicationUser = _unitOfWork.ApplicationUser.Get(u=> u.Id == userId);

            shoppingcartVM.Order.Name = shoppingcartVM.Order.applicationUser.Name;
            shoppingcartVM.Order.StreetAddress = shoppingcartVM.Order.applicationUser.StreeAddress;
            shoppingcartVM.Order.City = shoppingcartVM.Order.applicationUser.City;
            shoppingcartVM.Order.PhoneNumber = shoppingcartVM.Order.applicationUser.PhoneNumber;
            shoppingcartVM.Order.State = shoppingcartVM.Order.applicationUser.State;
            shoppingcartVM.Order.PinCode = shoppingcartVM.Order.applicationUser.Postalcode;



            foreach (ShoppingCart cart in shoppingcartVM.ShoppingCartList)
            {
                cart.Price = getPriceBasedOnQuantity(cart);
                shoppingcartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
            return View(shoppingcartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
		{
			var userIdentity = (ClaimsIdentity)User.Identity;
			var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingcartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");
				

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            shoppingcartVM.Order.OrderDate = DateTime.Now;
            shoppingcartVM.Order.ApplicationUserId = userId;

            //we are not updating address related fields, since they are already present in the view it will be automatically binded to the model because of BindProperty

            foreach (ShoppingCart cart in shoppingcartVM.ShoppingCartList)
            {
                cart.Price = getPriceBasedOnQuantity(cart);
                shoppingcartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }

            var companyID = applicationUser.CompanyId;
            if(companyID == null || companyID == 0)
            {
                //regular customer account, we have to capture payment details at the time of order placement
                shoppingcartVM.Order.OrderStatus = StaticDetails.StatusPending;
                shoppingcartVM.Order.PaymentStatus = StaticDetails.PaymentStatusPending;
            }
            else
            {
				shoppingcartVM.Order.OrderStatus = StaticDetails.StatusApproved;
				shoppingcartVM.Order.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
			}

            _unitOfWork.Order.Add(shoppingcartVM.Order);
            _unitOfWork.save();

            foreach (var cart in shoppingcartVM.ShoppingCartList) { 
                OrderDetails orderDetails = new OrderDetails();
                orderDetails.OrderId = shoppingcartVM.Order.Id;
                orderDetails.ProductId = cart.ProductId;
                orderDetails.Count = cart.Quantity;
                orderDetails.Price = cart.Price;    

                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.save();

            }

            //implement payment logic
            if(companyID ==0 || companyID == null)
            {
                //regular customer account
            }
           
			return RedirectToAction(nameof(OrderConfirmation), new {shoppingcartVM.Order.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
           Order order = _unitOfWork.Order.Get(u=> u.Id == id);
           List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u=> u.ApplicationUserId == order.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.save();

            HttpContext.Session.Clear();

            return View(id);
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

                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDB.ApplicationUserId).Count()-1);
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

            var userIdentity = (ClaimsIdentity)User.Identity;
            var userid = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userid).Count());

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
