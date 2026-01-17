
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Repositories;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Repository.Data;
using System.Collections;

namespace SmartTeethCare.Repository.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private Hashtable _repositories ;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {   
           
            var type = typeof(TEntity).Name;
            if(!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(type, repositoryInstance );
            }
            return _repositories[type] as IGenericRepository<TEntity>;
        }
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
