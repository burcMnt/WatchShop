﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Interfaces;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;

        public BasketController(IBasketViewModelService basketViewModelService)
        {
            _basketViewModelService = basketViewModelService;
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(int productId,int quantity=1)
        {
            return Json(await _basketViewModelService.AddItemToBasket(productId,quantity));
        }
    }
}