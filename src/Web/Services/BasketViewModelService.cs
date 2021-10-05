using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IBasketService _basketService;

        public BasketViewModelService(IHttpContextAccessor httpContextAccesor, IAsyncRepository<Basket> basketRepository, IBasketService basketService)
        {

            _httpContextAccesor = httpContextAccesor;
            _basketRepository = basketRepository;
            _basketService = basketService;
        }

        public async Task<BasketItemAddedViewModel> AddItemToBasketAsync(int productId, int quantity)
        {
            //Get or Create basket id / sepeti getir yoksa oluştur
            var basketId = await GetOrCreateBasketIdAsync();

            //Add item to the basket / sepete ögeyi ekle
            await _basketService.AddItemToBasketAsync(basketId, productId, quantity);

            //Return items count in the basket / sepetteki öge sayısını getir ve döndür
            return new BasketItemAddedViewModel()
            {
                ItemsCount = await _basketService.BasketItemsCountAsync(basketId)
            };
        }

        public async Task<NavbarBasketViewModel> GetNavbarBasketViewModelAsync()
        {
            return new NavbarBasketViewModel()
            {
                ItemsCount = await BasketItemsCountAsync()
            };
        }

        //This method does not create a basket if non exists.
        private async Task<int> BasketItemsCountAsync()
        {
            var basketId = await GetBasketIdAsync();
            return basketId.HasValue ? await _basketService.BasketItemsCountAsync(basketId.Value) : 0;

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

                //If not,create a new basket with the logged in user id antd return its id/loginse ve sepeti yoksa oluştur
                return (await CreateBasketAsync(userId)).Id;

            }

            //Is there a basket cookie? / login degilse sepet cookie var mı varsa Id dondür
            var anonymousUserId = _httpContextAccesor.HttpContext.Request.Cookies[Constants.BASKET_COOKIENAME];

            if (!string.IsNullOrEmpty(anonymousUserId))
            {
                var spec = new BasketSpecification(anonymousUserId);
                Basket basket = await _basketRepository.FirstOrDefaultAsync(spec);
                if (basket != null) return basket.Id;
            }
            //If not,create a new basket with the anonymous user id and return its id / sepet cooki yoksa oluştur
            anonymousUserId = Guid.NewGuid().ToString();
            _httpContextAccesor.HttpContext.Response.Cookies.Append(Constants.BASKET_COOKIENAME, anonymousUserId, new
                CookieOptions()
            { Expires = DateTime.Now.AddMonths(1), IsEssential = true });

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

        public async Task<BasketViewModel> GetBasketViewModelAsync()
        {
            var basketId = await GetBasketIdAsync();
            var vm = new BasketViewModel();
            if (!basketId.HasValue) return vm;
            // sepeti öğeleri ile birlikte döndür
            var spec = new BasketWithItemsAndProductsSpecification(basketId.Value);
            var basket =await _basketRepository.FirstOrDefaultAsync(spec);
            vm.Items = basket.Items.Select(x => new BasketItemVievModel()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.Product.ProductName,
                PictureUri = x.Product.PictureUri,
                UnitPrice = x.Product.Price,
                Quantity = x.Quantity
            }).ToList();
            vm.TotalPrice = vm.Items.Sum(x => x.Quantity * x.UnitPrice);

            return vm;
        }

        public async Task<int?> GetBasketIdAsync()
        {
            string userId = _httpContextAccesor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var anonymousUserId = _httpContextAccesor.HttpContext.Request.Cookies[Constants.BASKET_COOKIENAME];
            var buyerId = userId ?? anonymousUserId;
            if (buyerId != null)
            {
                var spec = new BasketSpecification(buyerId);
                var basket = await _basketRepository.FirstOrDefaultAsync(spec);
                if (basket != null)
                {
                    return basket.Id;
                }
            }

            return null;
        }
    }
}
