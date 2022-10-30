using Mango.Web.Models;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddToCartIdAsync<T>(CartDto cartDto, string token = null);
        Task<T> UpdateToCartIdAsync<T>(CartDto cartDto, string token = null);
        Task<T> RemoveFromCartIdAsync<T>(int cartDetailsId, string token = null);
        Task<T> ApplyCoupon<T>(CartDto cartDto, string token = null);
        Task<T> RemoveCoupon<T>(string UserId, string token = null);
    }
}
