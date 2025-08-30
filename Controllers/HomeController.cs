using Microsoft.AspNetCore.Mvc;

namespace OptMailWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Login kontrolü
            var kullaniciKodu = HttpContext.Session.GetString("KullaniciKodu");
            if (string.IsNullOrEmpty(kullaniciKodu))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.KullaniciKodu = kullaniciKodu;
            ViewBag.Aciklama = HttpContext.Session.GetString("Aciklama");
            ViewBag.SifreDegisimGun = HttpContext.Session.GetInt32("SifreDegisimGun");

            return View();
        }
    }
}
