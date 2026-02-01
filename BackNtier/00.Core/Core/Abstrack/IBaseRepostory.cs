using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Abstrack
{
    public interface IBaseRepository<T> where T : BaseEntity, new()
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        List<T> GetAll(Expression<Func<T, bool>>? predicate = null);
        T Get(Expression<Func<T, bool>> predicate);
    }
}
