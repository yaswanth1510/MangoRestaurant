 using System;
 using DBAcccessProcessor.DbContexts;
 using DBAcccessProcessor.Repository;
 using DBAcccessProcessor.Services;
 using Mango.MessageBus;
 using MessageBus;
 using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

 [assembly: FunctionsStartup(typeof(Mango.OrderProcessor.Startup))]
namespace Mango.OrderProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IOrderProcessorCheckout, OrderProcessorCheckout>();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Data Source=yaswanth-micro.database.windows.net;Initial Catalog=MangoOrderAPI;Persist Security Info=False;User ID=yaswamicroservices;Password=Kamalesh@05;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"));
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionBuilder.UseSqlServer("Data Source=yaswanth-micro.database.windows.net;Initial Catalog=MangoOrderAPI;Persist Security Info=False;User ID=yaswamicroservices;Password=Kamalesh@05;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
            builder.Services.AddSingleton(new Repository(optionBuilder.Options));
            builder.Services.AddSingleton<IMessageBus, AzureServiceMessageBus>();
        }
    }
}
