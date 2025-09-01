using MimeKit;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.IO;
using System;
using System.Collections.Generic;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net;
using Microsoft.Data.SqlClient;
using System.Data;
using YourProject.Models;

namespace OptMailWeb.Controllers
{
    [Route("[controller]")]
    public class MailController : Controller
    {

        private readonly IConfiguration _config;
        public MailController(IConfiguration config)
        {
            _config = config;
        }




        [HttpPost("SendSelectedUsersMail")]
        public IActionResult SendSelectedUsersMail([FromBody] List<string> mails)
        {
            try
            {
                var emailList = mails.Where(m => !string.IsNullOrEmpty(m)).ToList();
                if (emailList.Count == 0)
                    return Json(new { success = false, message = "Seçilen kullanıcıların mail adresi bulunamadı!" });

                var smtpSection = _config.GetSection("SmtpSettings");
                using var smtp = new SmtpClient(smtpSection["Host"], int.Parse(smtpSection["Port"]))
                {
                    Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                    EnableSsl = bool.Parse(smtpSection["EnableSsl"])
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpSection["From"], "Optimus Bilgilendirme"),
                    Subject = "Bilgilendirme",
                    Body = "Merhaba, seçilen kullanıcılara gönderilen test mailidir.",
                    IsBodyHtml = true
                };

                foreach (var adres in emailList)
                {
                    mail.To.Add(adres);
                }

                smtp.Send(mail);
                return Json(new { success = true, message = $"{emailList.Count} kişiye mail gönderildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Mail gönderilemedi: " + ex.Message });
            }
        }














        [HttpPost("SaveSelectedUsers")]
        public IActionResult SaveSelectedUsers([FromBody] List<MailKullanici> users)
        {
            try
            {
                using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();

                foreach (var model in users)
                {
                    using var cmd = new SqlCommand(@"INSERT INTO Mail_Kullanici
                (BolumId, KurumId, AraciKurumId, AdSoyad, Telefon, Mail, Adres, Notlar, Aktif)
                VALUES (@BolumId, @KurumId, @AraciKurumId, @AdSoyad, @Telefon, @Mail, @Adres, @Notlar, @Aktif)", con);

                    cmd.Parameters.AddWithValue("@BolumId", model.BolumId);
                    cmd.Parameters.AddWithValue("@KurumId", model.KurumId);
                    cmd.Parameters.AddWithValue("@AraciKurumId", model.AraciKurumId);
                    cmd.Parameters.AddWithValue("@AdSoyad", model.AdSoyad);
                    cmd.Parameters.AddWithValue("@Telefon", model.Telefon ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mail", model.Mail);
                    cmd.Parameters.AddWithValue("@Adres", model.Adres ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Notlar", model.Notlar ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Aktif", model.Aktif);

                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = $"{users.Count} kullanıcı kaydedildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }











        [HttpPost("Kaydet")]
        public IActionResult Kaydet([FromBody] MailKullanici model)
        {
            try
            {
                using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand(@"INSERT INTO Mail_Kullanici
                    (BolumId, KurumId, AraciKurumId, AdSoyad, Telefon, Mail, Adres, Notlar, Aktif)
                    VALUES (@BolumId, @KurumId, @AraciKurumId, @AdSoyad, @Telefon, @Mail, @Adres, @Notlar, @Aktif)", con);

                cmd.Parameters.AddWithValue("@BolumId", model.BolumId);
                cmd.Parameters.AddWithValue("@KurumId", model.KurumId);
                cmd.Parameters.AddWithValue("@AraciKurumId", model.AraciKurumId);
                cmd.Parameters.AddWithValue("@AdSoyad", model.AdSoyad);
                cmd.Parameters.AddWithValue("@Telefon", model.Telefon ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Mail", model.Mail);
                cmd.Parameters.AddWithValue("@Adres", model.Adres ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Notlar", model.Notlar ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Aktif", model.Aktif);

                con.Open();
                cmd.ExecuteNonQuery();

                return Json(new { success = true, message = "Kayıt başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        [HttpGet("Liste")]
        public IActionResult Liste(int? bolumId, int? kurumId, int? araciKurumId)
        {
            var list = new List<MailKullanici>();
            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT * FROM Mail_Kullanici WHERE Aktif = 1", con);

            if (bolumId.HasValue) cmd.CommandText += " AND BolumId = @BolumId";
            if (kurumId.HasValue) cmd.CommandText += " AND KurumId = @KurumId";
            if (araciKurumId.HasValue) cmd.CommandText += " AND AraciKurumId = @AraciKurumId";

            if (bolumId.HasValue) cmd.Parameters.AddWithValue("@BolumId", bolumId.Value);
            if (kurumId.HasValue) cmd.Parameters.AddWithValue("@KurumId", kurumId.Value);
            if (araciKurumId.HasValue) cmd.Parameters.AddWithValue("@AraciKurumId", araciKurumId.Value);

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MailKullanici
                {
                    SYS_NO = (int)reader["SYS_NO"],
                    BolumId = (int)reader["BolumId"],
                    KurumId = (int)reader["KurumId"],
                    AraciKurumId = (int)reader["AraciKurumId"],
                    AdSoyad = reader["AdSoyad"].ToString(),
                    Telefon = reader["Telefon"].ToString(),
                    Mail = reader["Mail"].ToString(),
                    Adres = reader["Adres"].ToString(),
                    Notlar = reader["Notlar"].ToString(),
                    Aktif = (bool)reader["Aktif"],
                    KAYIT_ANI = (DateTime)reader["KAYIT_ANI"],
                    GUNCELLEME_ANI = reader["GUNCELLEME_ANI"] as DateTime?
                });
            }

            return Json(list);
        }




        [HttpGet("GetFilteredUsers")]
        public IActionResult GetFilteredUsers(int? bolumId, int? kurumId, int? araciKurumId)
        {
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SELECT SYS_NO, AdSoyad, Mail, Telefon, Adres, Notlar, Aktif FROM Mail_Kullanici WHERE Aktif = 1", conn);

            if (bolumId.HasValue) cmd.CommandText += " AND BolumId=@BolumId";
            if (kurumId.HasValue) cmd.CommandText += " AND KurumId=@KurumId";
            if (araciKurumId.HasValue) cmd.CommandText += " AND AraciKurumId=@AraciKurumId";

            if (bolumId.HasValue) cmd.Parameters.AddWithValue("@BolumId", bolumId.Value);
            if (kurumId.HasValue) cmd.Parameters.AddWithValue("@KurumId", kurumId.Value);
            if (araciKurumId.HasValue) cmd.Parameters.AddWithValue("@AraciKurumId", araciKurumId.Value);

            conn.Open();
            var reader = cmd.ExecuteReader();
            var list = new List<object>();
            while (reader.Read())
            {
                list.Add(new
                {
                    SYS_NO = reader["SYS_NO"],
                    adSoyad = reader["AdSoyad"].ToString(),
                    mail = reader["Mail"].ToString(),
                    telefon = reader["Telefon"].ToString()
                });
            }
            return Json(list);
        }






        [HttpGet("GetBolumler")]
        public IActionResult GetBolumler()
        {
            var list = new List<dynamic>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GET_BOLUMLER", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new { Id = reader["ID"], Name = reader["Ad"] });
            }
            return Json(list);
        }

        [HttpGet("GetAraciKurumlar")]
        public IActionResult GetAraciKurumlar()
        {
            var list = new List<dynamic>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GET_ARACI_KURUMLAR", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new { Id = reader["KURUM_ID"], Name = reader["KURUM_ADI"] });
            }
            return Json(list);
        }

        [HttpGet("GetBirimler")]
        public IActionResult GetBirimler(int bolumId, int kurumId)
        {
            var list = new List<dynamic>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GET_BIRIMLER", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BOLUM_ID", bolumId);
            cmd.Parameters.AddWithValue("@KURUM_ID", kurumId);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new { Id = reader["ID"], Name = reader["Ad"] });
            }
            return Json(list);
        }


        private static string[] Scopes = { GmailService.Scope.GmailSend };
        private const string ApplicationName = "OPT Mail Sender";

        [HttpPost("SendMail")]
        public IActionResult SendMail([FromBody] MailRequest request)
        {
            if (request.To == null || !request.To.Any())
            {
                return Json(new { success = false, message = "Alıcı seçilmedi!" });
            }

            try
            {
                var smtpConfig = _config.GetSection("SmtpSettings");
                using var client = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"]))
                {
                    Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]),
                    EnableSsl = bool.Parse(smtpConfig["EnableSsl"])
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpConfig["From"], smtpConfig["DisplayName"]),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = true
                };

                foreach (var to in request.To)
                {
                    mail.Bcc.Add(to);
                }

                client.Send(mail);
                return Json(new { success = true, message = "Mail gönderildi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hata: " + ex.Message });
            }
        }
    }
    public class MailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string[] To { get; set; }
    }
}
