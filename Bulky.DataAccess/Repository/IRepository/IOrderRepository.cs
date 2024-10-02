using Bulky.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
    {
        public void Update(Order order);
        public void UpdateStatus(int id, string OrderStatus, string Paymentstatus = null);
        public void UpdatePaymentId(int id, string sessionId, string PaymentStatus );
    }
}
