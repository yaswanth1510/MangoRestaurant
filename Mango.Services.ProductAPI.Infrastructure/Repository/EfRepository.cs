using Mango.Services.ProductAPI.Domain.Interface;
using Mango.Services.ProductAPI.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ProductAPI.Infrastructure.Repository
{
    public class EfRepository<T> : IProductRepository<T> where T : class, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EfRepository<T>> _logger;

        public EfRepository(ApplicationDbContext db, ILogger<EfRepository<T>> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IReadOnlyList<T>> GetAll()
        {
            var result = new List<T>();
            try
            {
               result = await _db.Set<T>().ToListAsync();
            }
            catch (Exception e)
            {
               _logger.LogError(e.Message.ToString());
            }

            return result;
        }

        public async Task<T> GetById(int id)
        {
            var result = new T();
            try
            {
               result = await _db.Set<T>().FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
            }

            return result;
        }

        public async Task<(T, bool)> Create(T entity)
        {
            try
            {
                await _db.Set<T>().AddAsync(entity);
                await _db.SaveChangesAsync();
                return (entity, true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
                return (entity, false);
            }
        }
        public async Task<(T, bool)> Update(T entity)
        {
            try
            {
                _db.Entry(entity).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return (entity, true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
                return (entity, false);
            }
        }

        public async Task<bool> Delete(T entity)
        {
            try
            {
                _db.Set<T>().Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
                return false;
            }
        }

        public IQueryable<T> Query()
        {
            return _db.Set<T>();
        }
    }
}
