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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) 
        {
            _context = applicationDbContext;
        }

        public void Update(ProductImage productImage)
        {
            _context.ProductImages.Update(productImage);
        }
    }
}
