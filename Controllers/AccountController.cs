using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OptMailWeb.Models;
using System.Data;

namespace OptMailWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_SC_LOGIN", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // INPUT PARAMETRELER
                cmd.Parameters.AddWithValue("@KullaniciKodu", model.KullaniciKodu);
                cmd.Parameters.AddWithValue("@PASS", model.PASS);
                cmd.Parameters.AddWithValue("@IP", "0.0.0.0");
                cmd.Parameters.AddWithValue("@MAC_ADRESI", "");
                cmd.Parameters.AddWithValue("@HOSTNAME", Environment.MachineName);
                cmd.Parameters.AddWithValue("@SERVER_ADDR", "localhost");
                cmd.Parameters.AddWithValue("@SERVER_LOCAL_IP", "127.0.0.1");
                cmd.Parameters.AddWithValue("@KULLANIM_YERI", "K");
                cmd.Parameters.AddWithValue("@INTERNAL_PROGRAM", 1);
                cmd.Parameters.AddWithValue("@PORT", DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAIL", "");

                // OUTPUT PARAMETRELER
                var paramKullaniciId = new SqlParameter("@KullaniciId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var paramAciklama = new SqlParameter("@Aciklama", SqlDbType.VarChar, 32) { Direction = ParameterDirection.Output };
                var paramSube = new SqlParameter("@Sube", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var paramSifreDegisimGun = new SqlParameter("@SIFRE_DEGISIMINE_KALAN_GUN_SAYISI", SqlDbType.Int) { Direction = ParameterDirection.Output };
                var paramSubeAciklama = new SqlParameter("@Sube_Aciklama", SqlDbType.VarChar, 64) { Direction = ParameterDirection.Output };
                var paramLastLoginTime = new SqlParameter("@LastLoginTime", SqlDbType.DateTime) { Direction = ParameterDirection.Output };
                var paramInternetLogin = new SqlParameter("@INTERNET_LOGINI", SqlDbType.TinyInt) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(paramKullaniciId);
                cmd.Parameters.Add(paramAciklama);
                cmd.Parameters.Add(paramSube);
                cmd.Parameters.Add(paramSifreDegisimGun);
                cmd.Parameters.Add(paramSubeAciklama);
                cmd.Parameters.Add(paramLastLoginTime);
                cmd.Parameters.Add(paramInternetLogin);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Nullable int olarak al
                int? sifreDegisimGun = paramSifreDegisimGun.Value != DBNull.Value
                        ? (int?)paramSifreDegisimGun.Value
                        : null;

                string aciklama = paramAciklama.Value?.ToString() ?? "";
                int sube = paramSube.Value != DBNull.Value ? (int)paramSube.Value : 0;
                string subeAciklama = paramSubeAciklama.Value?.ToString() ?? "";
                DateTime lastLogin = paramLastLoginTime.Value != DBNull.Value ? (DateTime)paramLastLoginTime.Value : DateTime.MinValue;
                bool internetLogin = paramInternetLogin.Value != DBNull.Value && (byte)paramInternetLogin.Value == 1;

                // Login kontrolü: şifre doğru ise @SIFRE_DEGISIMINE_KALAN_GUN_SAYISI dolu gelir
                if (sifreDegisimGun.HasValue)
                {
                    // Başarılı giriş → session atamaları
                    HttpContext.Session.SetString("KullaniciKodu", model.KullaniciKodu);
                    HttpContext.Session.SetString("Aciklama", aciklama);
                    HttpContext.Session.SetInt32("Sube", sube);
                    HttpContext.Session.SetString("SubeAciklama", subeAciklama);
                    HttpContext.Session.SetInt32("SifreDegisimGun", sifreDegisimGun.Value);
                    HttpContext.Session.SetString("LastLoginTime", lastLogin.ToString("yyyy-MM-dd HH:mm:ss"));
                    HttpContext.Session.SetString("InternetLogin", internetLogin ? "1" : "0");
                    HttpContext.Session.SetInt32("SifreDegisimGun", sifreDegisimGun.HasValue ? sifreDegisimGun.Value : 0);

                    ViewBag.SifreDegisimGun = sifreDegisimGun;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Hatalı şifre
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                    return View(model);
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
