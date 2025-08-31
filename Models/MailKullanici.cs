namespace YourProject.Models
{
    public class MailKullanici
    {
        public int SYS_NO { get; set; }
        public int BolumId { get; set; }
        public int KurumId { get; set; }
        public int AraciKurumId { get; set; }
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string Mail { get; set; }
        public string Adres { get; set; }
        public string Notlar { get; set; }
        public bool Aktif { get; set; }
        public DateTime KAYIT_ANI { get; set; }
        public DateTime? GUNCELLEME_ANI { get; set; }
    }
}
