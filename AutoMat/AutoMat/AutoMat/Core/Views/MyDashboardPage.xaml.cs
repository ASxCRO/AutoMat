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
        public MyDashboardPage()
        {
            InitializeComponent();
            GetProfileInformationAndRefreshToken();
        }

        public MyDashboardPage(ViewModels.User user)
        {
            InitializeComponent();
            MyUserName.Text = user.Email;
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
                var firebase = new FirebaseClient("https://automat-29cec.firebaseio.com/");
                var users = await firebase
                      .Child("users")
                      .OnceAsync<JObject>();

                string bla = "";
                foreach (var item in users)
                {
                    bla += item.Object.GetValue("Ime");
                }

                App.Current.MainPage.DisplayAlert("Alert", bla, "OK");
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
            App.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}