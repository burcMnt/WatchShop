using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        public async Task<IActionResult> AddItem(int productId,int quantity=1)
        {
            //sepeti getir yoksa oluştur
            //sepete ögeyi ekle
            //sepetteki öge sayısını getir ve döndür

            return View();
        }
    }
}
