using AutoMapper;
using Mango.Services.ShoppingCartAPI.DbContexts;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = couponCode;
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId);
                if (cartHeaderFromDb != null)
                {
                    _db.CartDetails.RemoveRange(_db.CartDetails.Where(x => x.CartHeaderId == cartHeaderFromDb.CartHeaderId));
                    _db.CartHeaders.Remove(cartHeaderFromDb);
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CartDto> CreateUpdateCart(CartDto CartDto)
        {
            try
            {
                Cart cart = _mapper.Map<Cart>(CartDto);
                var prodInDb = await _db.Products.FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId);
                if (prodInDb == null)
                {
                    _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                    await _db.SaveChangesAsync();
                }

                var CartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);
                if (CartHeaderFromDb == null)
                {
                    _db.CartHeaders.Add(cart.CartHeader);
                    await _db.SaveChangesAsync();
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var CartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId && u.CartHeaderId == CartHeaderFromDb.CartHeaderId);
                    if (CartDetailsFromDb == null)
                    {
                        cart.CartDetails.FirstOrDefault().CartHeaderId = CartHeaderFromDb.CartHeaderId;
                        cart.CartDetails.FirstOrDefault().Product = null;
                        _db.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        cart.CartDetails.FirstOrDefault().Product = null;
                        cart.CartDetails.FirstOrDefault().Count += CartDetailsFromDb.Count;
                        _db.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }
                }
                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {
                return _mapper.Map<CartDto>(new CartDto());
            }
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            try
            {
                Cart cart = new()
                {
                    CartHeader = await _db.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
                };
                cart.CartDetails = _db.CartDetails.Where(x => x.CartHeaderId == cart.CartHeader.CartHeaderId).Include(u => u.Product);
                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {
                return _mapper.Map<CartDto>(new CartDto());
            }
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId);
            cartFromDb.CouponCode = "";
            _db.CartHeaders.Update(cartFromDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCart(int CartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstOrDefaultAsync(x => x.CartDetailsId == CartDetailsId);
                int totalCountOfCartItems = _db.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItems == 1)
                {
                    var cardHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cardHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
