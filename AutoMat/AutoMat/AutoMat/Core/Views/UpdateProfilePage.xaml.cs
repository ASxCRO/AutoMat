using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateProfilePage : ContentPage
    {
        public FirebaseUser FirebaseUser { get; set; }
        public IDataStore<FirebaseUser> UsersDataStore{ get; set; }

        public bool isFirstLogin{ get; set; }
        public UpdateProfilePage()
        {
            InitializeComponent();
            isFirstLogin = false;
            BindingContext = FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            UsersDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
        }

        public UpdateProfilePage(bool isFirstLogin)
        {
            InitializeComponent();
            this.isFirstLogin = isFirstLogin;
            BindingContext = FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            UsersDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();

        }

        protected override void OnAppearing()
        {
            if(String.IsNullOrEmpty(FirebaseUser.Username))
            {
                korime.IsEnabled = true;
            }
        }

 
        private async void UpdateProfile_Clicked(object sender, System.EventArgs e)
        {
            string errors = "";
            bool hasErrors = false;
            if (String.IsNullOrEmpty(FirebaseUser.FirstName))
                errors += "Ispunite ime\n";
            if (String.IsNullOrEmpty(FirebaseUser.LastName))
                errors += "Ispunite prezime\n";
            if (String.IsNullOrEmpty(FirebaseUser.Username))
                errors += "Ispunite korisničko ime\n";
            if (String.IsNullOrEmpty(FirebaseUser.PhoneNumber))
                errors += "Ispunite broj mobitela\n";

            if (!String.IsNullOrEmpty(errors))
            {
                hasErrors = true;
                await App.Current.MainPage.DisplayAlert("Informacija", errors, "OK");
            }

            if(!hasErrors)
            {

                var oldUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));

                if (!oldUser.Equals(FirebaseUser))
                {
                    bool success = await UsersDataStore.UpdateItemAsync(FirebaseUser);
                    if (success)
                    {
                        Preferences.Clear();
                        Preferences.Set("FirebaseUser", JsonConvert.SerializeObject(FirebaseUser));
                        await App.Current.MainPage.DisplayAlert("Informacija", $"Uspješno ste ažurirali profil", "OK");
                        if(isFirstLogin)
                        {
                            await Navigation.PushAsync(new MainPage());
                        }
                        else
                        {
                            await Navigation.PopAsync();
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Greška", $"Dogodila se pogreška pri ažuriranju profila", "OK");
                    }
                }
            }
        }
    }
}