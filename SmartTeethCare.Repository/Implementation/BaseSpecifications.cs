using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Repository.Implementation
{
    public class BaseSpecifications<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }     // null by default when create new instance
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();   // initialize Includes list to avoid null reference exception


        public BaseSpecifications()          // use Include only
        {
            // criteria = null
        }

        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)   // use where and Include
        {
            Criteria = criteriaExpression; // p => p.Id == 1 
        }
    }
}
