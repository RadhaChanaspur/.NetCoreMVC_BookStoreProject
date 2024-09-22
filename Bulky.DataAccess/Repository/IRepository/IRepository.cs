using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T can be anything like product, category..
        IEnumerable<T> GetAll(String? includeProperties = null); //to get multiple items
        T Get(Expression<Func<T, bool>> filter, String? includeProperties = null); // to get one item
        void  Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        //update is not added in this interface since we can custom logic for update, hence adding it in specific interface


    }
}
