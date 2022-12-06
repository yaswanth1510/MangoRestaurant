using Mango.Services.ShoppingCartAPI.Domain.Interface;
using Mango.Services.ShoppingCartAPI.Domain.Specifications;
using Mango.Services.ShoppingCartAPI.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Mango.Services.ShoppingCartAPI.Infrastructure.Repository
{
    public class CartRepository<T> : ICartRepository<T> where T :class, new()
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CartRepository<T>> _logger;

        public CartRepository(ApplicationDbContext db, ILogger<CartRepository<T>> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<T> GetById(string id)
        {
            var result = new T();
            try
            {
                result = await _db.Set<T>().FindAsync(id);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message.ToString());
                throw;
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
                throw;
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

        public async Task<bool> Remove(T entity)
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

        public async Task<bool> Clear(List<T> entity)
        {
            try
            {
                _db.Set<T>().RemoveRange(entity);
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
            return _db.Set<T>().AsNoTracking();
        }

        public async Task<T> FirstOrDefaultAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_db.Set<T>().AsQueryable(), spec);
        }
    }
}
