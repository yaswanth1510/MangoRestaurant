using DBAcccessProcessor.DbContexts;
using DBAcccessProcessor.Messages;
using DBAcccessProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace DBAcccessProcessor.Repository
{
    public class Repository : IRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public Repository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            try
            {
                await using var _db = new ApplicationDbContext(_dbContext);
                _db.OrderHeaders.Add(orderHeader);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
        }

        public async Task UpdateOrderPaymentStatus(int OrderHeaderId, bool paid)
        {
            try
            {
                await using var _db = new ApplicationDbContext(_dbContext);
                var orderHeaderFromDb = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.OrderHeaderId == OrderHeaderId);
                if (orderHeaderFromDb != null)
                {
                    orderHeaderFromDb.PaymentStatus = paid;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            EmailLog emailLog = new()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully."
            };
            try
            {
                await using var _db = new ApplicationDbContext(_dbContext);
                _db.EmailLogs.Add(emailLog);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
