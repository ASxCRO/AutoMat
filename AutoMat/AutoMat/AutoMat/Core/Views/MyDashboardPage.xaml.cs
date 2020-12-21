using AutoMat.Core.ViewModels;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDashboardPage : ContentPage
    {
        public int AdCount { get; set; } = 20;
        public string IsActive { get; set; } = "Da";

        public int RegistrationYear { get; set; } = 2020;

        public MyDashboardPage()
        {
            InitializeComponent();
            GetProfileInformationAndRefreshToken();
        }

        public MyDashboardPage(ViewModels.User user)
        {
            InitializeComponent();
        }

        async private void GetProfileInformationAndRefreshToken()
        {
            var authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
            try
            {
                string authLink = Preferences.Get("MyFirebaseRefreshToken", "");
                var savedfirebaseauth = JsonConvert.DeserializeObject<FirebaseAuthLink>(authLink);
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", $"Token expired {ex.Message}", "OK");
            }
        }

        void Logout_Clicked(System.Object sender, System.EventArgs e)
        {
            Preferences.Remove("MyFirebaseRefreshToken");
            Preferences.Remove("UserToken");
            Preferences.Remove("CurrentUser");
            App.Current.MainPage = new NavigationPage(new UserPage());
        }
    }
}