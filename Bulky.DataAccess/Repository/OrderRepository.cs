using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderRepository :  Repository<Order> , IOrderRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public void Update(Order order)
        {
            _applicationDbContext.Orders.Update(order);
        }

  
        void IOrderRepository.UpdatePaymentId(int id, string sessionId, string PaymentReferenceId)
        {
            Order order = _applicationDbContext.Orders.FirstOrDefault(u=> u.Id == id);  
            if(order != null)
            {
                if(!string.IsNullOrEmpty(sessionId))
                    order.SessionID = sessionId;

                if (!string.IsNullOrEmpty(PaymentReferenceId))
                {
                    order.PaymentReferenceId = PaymentReferenceId;
                    order.PaymentDate = DateTime.Now;
                }
            }
        }

        void IOrderRepository.UpdateStatus(int id, string OrderStatus, string Paymentstatus = null)
        {
            Order order = _applicationDbContext.Orders.FirstOrDefault(u => u.Id == id);
            if (order != null)
            {
                order.OrderStatus = OrderStatus;
                if(!string.IsNullOrEmpty(Paymentstatus)) 
                    order.PaymentStatus = Paymentstatus;
            }

        }
    }
}
