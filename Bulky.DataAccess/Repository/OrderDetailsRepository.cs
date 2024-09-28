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
    public class OrderDetailsRepository : Repository<OrderDetails> , IOrderDetailsRepository
    {
        private readonly ApplicationDbContext applicationDbContext;
        public OrderDetailsRepository(ApplicationDbContext db):base(db) 
        {
            applicationDbContext = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            applicationDbContext.OrderDetails.Update(orderDetails);
        }
    }
}
