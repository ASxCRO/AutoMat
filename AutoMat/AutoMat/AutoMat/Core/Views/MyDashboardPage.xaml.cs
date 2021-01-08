using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDashboardPage : ContentPage
    {
        public int AdCount { get; set; }
        public string IsActive { get; set; } = "Da";
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public FirebaseUser FirebaseUser { get; set; }
        public FirebaseUserViewModel FirebaseUserViewModel { get; set; }
        public MyDashboardPage()
        {
            InitializeComponent();
            FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            InitViewModel();
            FirebaseUserViewModel = new FirebaseUserViewModel { FirebaseUser = this.FirebaseUser };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = FirebaseUserViewModel;
        }


        private void InitViewModel()
        {
            if (String.IsNullOrEmpty(FirebaseUser.Username))
                FirebaseUser.Username = "Kor. ime ne postoji";

        }

        void Logout_Clicked(System.Object sender, System.EventArgs e)
        {
            Preferences.Remove("FirebaseUser");
            App.Current.MainPage = new NavigationPage(new UserPage());
        }

        private async void CallNumber_Clicked(object sender, EventArgs e)
        {
            await PlacePhoneCall(FirebaseUser.PhoneNumber);
        }

        public async Task PlacePhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                await App.Current.MainPage.DisplayAlert("Upozorenje", $"Mobilni broj ne postoji!", "OK");

            }
            catch (FeatureNotSupportedException ex)
            {
                await App.Current.MainPage.DisplayAlert("Upozorenje", $"Radnja ne-kompaktibilna sa ovim uređajem!", "OK");

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Greška", $"Dogodila se pogreška: {ex.Message}", "OK");

            }
        }

        private async void SendEmail_Clicked(object sender, EventArgs e)
        {
            await SendEmail("Upit sa Automata", $"\nPoslano iz aplikacije automat,\n{FirebaseUser.FirstName}", new List<string> { FirebaseUser.Email});
        }

        public async Task SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                await App.Current.MainPage.DisplayAlert("Upozorenje", $"Radnja ne-kompaktibilna sa ovim uređajem!", "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Greška", $"Dogodila se pogreška: {ex.Message}", "OK");
            }
        }
    }
}