using Mango.MessageBus;
using MessageBus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PaymentProcessor;

[assembly: FunctionsStartup(typeof(Mango.PaymentProcessor.Startup))]
namespace Mango.PaymentProcessor
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IMessageBus, AzureServiceMessageBus>();
            builder.Services.AddTransient<IProcessPayment, ProcessPayment>();
        }
    }
}
