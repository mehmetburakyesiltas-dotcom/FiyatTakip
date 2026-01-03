namespace FiyatTakip;

public partial class AdminPage : ContentPage
{
    public AdminPage()
    {
        InitializeComponent();
        EntKar.Text = Preferences.Get("KarOrani", "0");
    }
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        Preferences.Set("KarOrani", EntKar.Text);
        await DisplayAlert("Sistem", "Kâr marjý güncellendi!", "Tamam");
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}