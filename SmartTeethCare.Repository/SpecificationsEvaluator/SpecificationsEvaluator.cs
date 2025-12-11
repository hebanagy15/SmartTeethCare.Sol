using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Specifications;


namespace SmartTeethCare.Repository.SpecificationsEvaluator
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;  // _dbcontext.Set<Product>()
            // Apply criteria (where conditions)
            if (spec.Criteria != null)  // P => P.Id == 1
            {
                query = query.Where(spec.Criteria);
            }
            // query = _dbcontext.Set<Product>().Where(P => P.Id == 1)


            // Apply includes (eager loading)
            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));     // for each include expression apply it to the current query
            // query = _dbcontext.Set<Product>().Where(P => P.Id == 1).Include(P => P.Brand).Include(P => P.Category)
            return query;

        }
    }

}
