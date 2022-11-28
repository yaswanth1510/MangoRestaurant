using DBAcccessProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace DBAcccessProcessor.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}
