namespace Mango.Services.ShoppingCartAPI.Domain.Entities
{
    public class Cart
    {
        public Cart()
        {
            CartHeader = new CartHeader();
            CartDetails = new HashSet<CartDetails>();
        }
        public virtual CartHeader CartHeader { get; set; }
        public virtual IEnumerable<CartDetails> CartDetails { get; set; }
    }
}
