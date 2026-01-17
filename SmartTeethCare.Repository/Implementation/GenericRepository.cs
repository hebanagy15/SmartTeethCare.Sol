using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Repositories;
using SmartTeethCare.Repository.Data;


namespace SmartTeethCare.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)         // Ask CLR for creating instance of ApplicationDbContext and pass it here
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public Task<T?> GetByIdAsync(int id)
        {
            return _dbContext.Set<T>().FindAsync(id).AsTask();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }



    }
}
