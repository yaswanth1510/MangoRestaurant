using DBAcccessProcessor.Messages;
using DBAcccessProcessor.Models;

namespace DBAcccessProcessor.Repository
{
    public interface IRepository
    {
        Task<bool> AddOrder(OrderHeader orderHeader);
        Task UpdateOrderPaymentStatus(int OrderHeaderId, bool paid);
        Task SendAndLogEmail(UpdatePaymentResultMessage message);
    }
}
