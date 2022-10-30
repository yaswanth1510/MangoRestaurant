namespace Mango.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<T> GetCoupon<T>(string CouponCode, string token = null);
    }
}
