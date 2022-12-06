namespace Mango.Services.ShoppingCartAPI.Domain.Models.Dto
{
    public class CartDto
    {
        public virtual CartHeaderDto CartHeader { get; set; }
        public virtual IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
