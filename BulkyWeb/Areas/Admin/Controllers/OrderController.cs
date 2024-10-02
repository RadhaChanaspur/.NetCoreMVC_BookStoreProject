using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;


namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult details(int OrderId)
        {
            orderVM = new OrderVM
            {
                Order = _unitOfWork.Order.Get(u => u.Id == OrderId, includeProperties: "applicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(u => u.OrderId == OrderId, includeProperties: "Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin+","+StaticDetails.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            Order orderFromDB = _unitOfWork.Order.Get(u => u.Id == orderVM.Order.Id);

            orderFromDB.Name = orderVM.Order.Name;
            orderFromDB.PhoneNumber = orderVM.Order.PhoneNumber;
            orderFromDB.StreetAddress = orderVM.Order.StreetAddress;
            orderFromDB.City = orderVM.Order.City;
            orderFromDB.PinCode = orderVM.Order.PinCode;
            orderFromDB.State = orderVM.Order.State;

            
            if (!string.IsNullOrEmpty(orderVM.Order.Carrier))
                orderFromDB.Carrier = orderVM.Order.Carrier;

            if (!string.IsNullOrEmpty(orderVM.Order.TrackingNumber))
                orderFromDB.TrackingNumber = orderVM.Order.TrackingNumber;


            _unitOfWork.Order.Update(orderFromDB);
            _unitOfWork.save();

            TempData["success"] = "Order Details Updated successfully";

            return RedirectToAction(nameof(details), new { OrderId= orderVM.Order.Id});
        }
        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.Order.UpdateStatus(orderVM.Order.Id, StaticDetails.StatusInProcess);
            _unitOfWork.save();

            TempData["success"] = "Order Details Updated successfully";

            return RedirectToAction(nameof(details), new { OrderId = orderVM.Order.Id });
        }

        [HttpPost]
        [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
        public IActionResult ShipOrder()
        {
            Order order = _unitOfWork.Order.Get(u => u.Id == orderVM.Order.Id);
            order.TrackingNumber = orderVM.Order.TrackingNumber;
            order.Carrier = orderVM.Order.Carrier;
            order.OrderStatus = StaticDetails.StatusShipped;
            order.ShippingDate = DateTime.Now;

            if(order.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                order.PaymentDue = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.Order.Update(order);
            _unitOfWork.save();

            TempData["success"] = "Order Shipped successfully";

            return RedirectToAction(nameof(details), new { OrderId = orderVM.Order.Id });
        }

        [HttpGet]
        public IActionResult GetOrders(string status)
        {
           

            IEnumerable<Order> orderList;

            if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
             orderList = _unitOfWork.Order.GetAll(includeProperties: "applicationUser").ToList();
            else
            {
                var userIdentity = (ClaimsIdentity)User.Identity;
                var userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orderList = _unitOfWork.Order.GetAll(u => u.ApplicationUserId == userId, includeProperties: "applicationUser").ToList();
            }

            switch (status)
            {
                case "pending":
                    orderList = orderList.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderList = orderList.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
                    break;
                case "completed":
                    orderList = orderList.Where(u => u.OrderStatus == StaticDetails.StatusShipped);
                    break;
                case "approved":
                    orderList = orderList.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
                    break;
                default:
                    break;

            }
            return Json(new { data = orderList });
        }
    }
}
