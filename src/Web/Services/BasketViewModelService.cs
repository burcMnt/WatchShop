using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IAsyncRepository<Basket> _basketRepository;

        public BasketViewModelService(IHttpContextAccessor httpContextAccesor,IAsyncRepository<Basket> basketRepository)
        {
            _httpContextAccesor = httpContextAccesor;
            _basketRepository = basketRepository;
        }
        public Task<int> BasketItemsCountAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetOrCreateBasketIdAsync()
        {
            string userId = _httpContextAccesor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Is there logged in user ? / kullanıcı Login mi?
            if (!string.IsNullOrEmpty(userId))
            {
                //Does user have a basket?/ loginse sepeti var mı
                var spec = new BasketSpecification(userId);
                Basket basket = await _basketRepository.FirstOrDefaultAsync(spec);
                if (basket != null) return basket.Id;

                //If not,create a new basket/loginse ve sepeti yoksa oluştur
                return (await CreateBasketAsync(userId)).Id;

            }

            //Is there a basket cookie? / login degilse sepet cookie var mı varsa Id dondür
            var anonymousUserId = _httpContextAccesor.HttpContext.Request.Cookies[Constants.BASKET_COOKIENAME];
            if (anonymousUserId != null)
            {
                var spec = new BasketSpecification(anonymousUserId);
                Basket basket = await _basketRepository.FirstOrDefaultAsync(spec);
                return basket.Id; 
            }
            //If not,create a new basket with the anonymous user id and return its id / sepet cooki yoksa oluştur
            anonymousUserId = Guid.NewGuid().ToString();
            _httpContextAccesor.HttpContext.Response.Cookies.Append(Constants.BASKET_COOKIENAME, anonymousUserId, new 
                CookieOptions() { Expires = DateTime.Now.AddMonths(1), IsEssential = true });
            //sepet id döndür
            return (await CreateBasketAsync(anonymousUserId)).Id;
        }

        private async Task<Basket> CreateBasketAsync(string buyerId)
        {
            Basket basket = new Basket()
            {
                BuyerId = buyerId
            };
            return await _basketRepository.AddAsync(basket);
        }
    }
}
