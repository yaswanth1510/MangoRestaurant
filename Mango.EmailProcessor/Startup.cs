using DBAcccessProcessor.DbContexts;
using DBAcccessProcessor.Repository;
using DBAcccessProcessor.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Mango.EmailProcessor.Startup))]
namespace Mango.EmailProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddTransient<IOrderPaymentUpdateReceived, OrderPaymentUpdateReceived>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Data Source=yaswanth-micro.database.windows.net;Initial Catalog=MangoEmailAPI;Persist Security Info=False;User ID=yaswamicroservices;Password=Kamalesh@05;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"));
            builder.Services.AddTransient<IRepository, Repository>();
        }
    }
}
