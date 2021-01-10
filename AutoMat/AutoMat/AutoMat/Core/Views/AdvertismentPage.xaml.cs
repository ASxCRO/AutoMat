using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvertismentPage : ContentPage
    {
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public AdvertismentViewModel AdViewModel { get; set; }

        public Advertisement Advertisement{ get; set; }
        public AdvertismentPage(Advertisement ad)
        {
            InitializeComponent();
            Advertisement = ad;
        }

        protected async override void OnAppearing()
        {
            UserDataStore = new UserDataStore();
            var seller = await UserDataStore.GetItemAsync(Advertisement.UserId);
            BindingContext = AdViewModel = new AdvertismentViewModel { Advertisment = this.Advertisement, CurrentImageIndex = 0, FirebaseUser = seller };
            foreach (var imageUrl in AdViewModel.Advertisment.PicturesURL)
            {
                AdViewModel.ImageSources.Add(ImageSource.FromUri(new Uri(imageUrl)));
            }
        }


        private async void ShowAutomat_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyDashboardPage(AdViewModel.FirebaseUser));
        }
    }
}