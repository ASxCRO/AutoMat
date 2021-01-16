using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyDashboardPage : ContentPage
    {
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public FirebaseUser FirebaseUser { get; set; }
        public FirebaseUserViewModel FirebaseUserViewModel { get; set; }
        public MyDashboardPage()
        {
            InitializeComponent();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            InitViewModel();
            FirebaseUserViewModel = new FirebaseUserViewModel { FirebaseUser = this.FirebaseUser };
            BindingContext = FirebaseUserViewModel;
        }

        public MyDashboardPage(FirebaseUser user)
        {
            InitializeComponent();
            
            FirebaseUser = user;
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            InitViewModel();
            FirebaseUserViewModel = new FirebaseUserViewModel { FirebaseUser = this.FirebaseUser };
            BindingContext = FirebaseUserViewModel;
            FabButton.IsVisible = false;
            BottomButton.Text = "Pogledaj sve oglase";
            BottomButton.Clicked -= Logout_Clicked;
            BottomButton.Clicked += ViewAds_Clicked;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

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

        private async void Fab_Clicked(object sender, EventArgs e)
        {
            
            if (updateButton.Opacity == 1)
            {
                await Task.WhenAll(
                 updateButton.FadeTo(0, 1100),
                 viewAdsButton.FadeTo(0, 800),
                 profilePictureButton.FadeTo(0, 500)
                );
            }
            else
            {
                await Task.WhenAll(
                 updateButton.FadeTo(1, 500),
                 viewAdsButton.FadeTo(1, 800),
                 profilePictureButton.FadeTo(1, 1100)
                );
            }

        }
        private async void UpdateProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateProfilePage());
        }

        private async void ViewAds_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FavoritesPage("profile", this.FirebaseUser));
        }

        private async void NewProfilePicture_Clicked(object sender, EventArgs e)
        {
            try
            {
                var pickResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Odaberite sliku profila"
                });

                if (pickResult != null)
                {
                    bool success = false;
                    using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Pričekajte.."))
                    {
                        var imageStream = await pickResult.OpenReadAsync();
                        var firebaseStorage = new FirebaseStorage("automat-29cec.appspot.com");

                        var nazivSlike = Guid.NewGuid().ToString();

                        var task = await firebaseStorage
                            .Child("data")
                            .Child(nazivSlike + ".png")
                            .PutAsync(imageStream);

                        var urlSlike = await firebaseStorage
                            .Child("data")
                            .Child(nazivSlike + ".png")
                            .GetDownloadUrlAsync();

                        FirebaseUserViewModel.FirebaseUser.PicturePath = urlSlike;
                        success = await UserDataStore.UpdateItemAsync(FirebaseUserViewModel.FirebaseUser);
                    };

                    if (success)
                    {
                        await App.Current.MainPage.DisplayAlert("Informacija", $"Uspješno ste promjenili profilnu sliku", "OK");
                        Preferences.Clear();
                        Preferences.Set("FirebaseUser", JsonConvert.SerializeObject(FirebaseUserViewModel.FirebaseUser));
                        profilePicture.Source = FirebaseUserViewModel.FirebaseUser.PicturePath;
                    }
                    else
                        await App.Current.MainPage.DisplayAlert("Greška", $"Dogodila se pogreška pri postavljanju nove slike profila", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }
    }
}