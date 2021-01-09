using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Newtonsoft.Json;
using System;
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
        public UpdateProfilePage()
        {
            InitializeComponent();
            BindingContext = FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            UsersDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
        }

        protected override async void OnAppearing()
        {
            if(String.IsNullOrEmpty(FirebaseUser.Username))
            {
                korime.IsEnabled = true;
            }
        }


            private async void UpdateProfile_Clicked(object sender, System.EventArgs e)
        {
            var oldUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));

            if (!oldUser.Equals(FirebaseUser))
            {
                bool success = await UsersDataStore.UpdateItemAsync(FirebaseUser);
                if(success)
                {
                    Preferences.Clear();
                    Preferences.Set("FirebaseUser", JsonConvert.SerializeObject(FirebaseUser));
                    await App.Current.MainPage.DisplayAlert("Informacija", $"Uspješno ste ažurirali profil", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Greška", $"Dogodila se pogreška pri ažuriranju profila", "OK");
                }
            }
        }
    }
}