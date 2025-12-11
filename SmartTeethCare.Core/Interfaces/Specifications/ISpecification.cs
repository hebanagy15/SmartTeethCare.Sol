using SmartTeethCare.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartTeethCare.Core.Interfaces.Specifications
{
    public interface ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }       // Where condition p => p.Id == 1

        public List<Expression<Func<T, object>>> Includes { get; set; }   // Include conditions  p => p.Brand , p => p.Category
    }
}
