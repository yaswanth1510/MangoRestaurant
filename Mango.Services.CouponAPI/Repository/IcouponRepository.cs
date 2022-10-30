using Mango.Services.CouponAPI.Models.Dtos;

namespace Mango.Services.CouponAPI.Repository
{
    public interface IcouponRepository
    {
        Task<CouponDto> GetCouponByCode(string couponCode);
    }
}
