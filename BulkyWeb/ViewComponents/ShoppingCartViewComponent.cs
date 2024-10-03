using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       public async Task<IViewComponentResult> InvokeAsync()
       {
            var userIdentity = (ClaimsIdentity)User.Identity;
            var user = userIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (user != null)
            {
                if(HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null) //if session is null, fetch data from db and set it
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == user.Value).Count());


                return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);

            }
        } 
    }
}
