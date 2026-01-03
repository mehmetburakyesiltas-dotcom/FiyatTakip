using Newtonsoft.Json.Linq;

namespace FiyatTakip;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // PİYASAYI TAZELE BUTONUNA TIKLANDIĞINDA ÇALIŞAN KISIM
    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        try
        {
            LblStatus.Text = "Veriler güncelleniyor...";
            HttpClient client = new HttpClient();

            // İnternetten Altın Verilerini Çek
            var altinRes = await client.GetStringAsync("https://api.genelpara.com/embed/altin.json");
            var altinData = JObject.Parse(altinRes);

            // İnternetten Döviz Verilerini Çek
            var dovizRes = await client.GetStringAsync("https://api.genelpara.com/embed/doviz.json");
            var dovizData = JObject.Parse(dovizRes);

            // Gelen verileri sayıya çevir
            double usd = double.Parse(dovizData["USD"]["satis"].ToString().Replace(".", ","));
            double eur = double.Parse(dovizData["EUR"]["satis"].ToString().Replace(".", ","));
            double goldOns = double.Parse(altinData["ONS"]["satis"].ToString().Replace(".", ","));

            // Admin panelinden ayarladığın kar oranını getir
            string kaydedilenKar = Preferences.Get("KarOrani", "8");
            if (!double.TryParse(kaydedilenKar, out double kar)) { kar = 8; }

            // HAS ALTIN HESAPLAMA FORMÜLÜ
            double gram = ((goldOns / 31.1034768) * usd) + kar;
            double ceyrek = gram * 1.63;

            // EKRANA YAZDIRMA (Lbl isimleri XAML ile aynı olmalı)
            LblDolar.Text = usd.ToString("F2") + " $";
            LblEuro.Text = eur.ToString("F2") + " €";
            LblGram.Text = gram.ToString("N2") + " TL";
            LblCeyrek.Text = ceyrek.ToString("N2") + " TL";
            LblYarim.Text = (ceyrek * 2).ToString("N2") + " TL";
            LblTam.Text = (ceyrek * 4).ToString("N2") + " TL";

            LblStatus.Text = kar > 0 ? $"Canlı Veri (+{kar} TL Kar Dahil)" : "Canlı Veri (Piyasa)";
        }
        catch (Exception)
        {
            LblStatus.Text = "İnternet bağlantısını kontrol edin!";
        }
    }

    // ŞİFRE GİRİŞİ (Yönetici Paneli Yazısına Tıklanınca Çalışır)
    private async void OnAdminEntryTapped(object sender, EventArgs e)
    {
        // Kullanıcıdan 3627 şifresini ister
        string res = await DisplayPromptAsync("Yetkili Girişi", "Lütfen şifrenizi giriniz:", "Giriş", "İptal", "Şifre", -1, Keyboard.Numeric);

        if (res == "3627")
        {
            await Navigation.PushAsync(new AdminPage());
        }
        else if (res != null)
        {
            await DisplayAlert("Hata", "Geçersiz şifre!", "Kapat");
        }
    }
}