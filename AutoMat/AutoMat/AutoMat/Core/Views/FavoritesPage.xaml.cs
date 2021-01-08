using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage
    {
        public FavoritesViewModel viewModel{ get; set; }
        public IDataStore<Advertisement> AdverismentDataStore { get; set; }
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public IDataStore<UserFavoriteAd> UserFavoriteAdsDataStore { get; set; }
        public FavoritesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new FavoritesViewModel();
            AdverismentDataStore = DependencyService.Get<IDataStore<Advertisement>>() ?? new AdvertismentDataStorage();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            UserFavoriteAdsDataStore = DependencyService.Get<IDataStore<UserFavoriteAd>>() ?? new FavoritesDataStore();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetCurrentUserFavoriteAds();
        }

        public async Task GetCurrentUserFavoriteAds()
        {
            var currentUserCached = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""),typeof(FirebaseUser));
            if(currentUserCached != null)
            {
                var allUsersFavoriteAds = await UserFavoriteAdsDataStore.GetItemsAsync(false);
                var currentUserFavoriteAds = allUsersFavoriteAds.Where(u => u.Username == currentUserCached.Username).ToList();

                var allAdsDict = await AdverismentDataStore.GetItemsKeyValueAsync();
                viewModel.Items.Clear();

                foreach (var ad in allAdsDict)
                {
                    foreach (var favorite in currentUserFavoriteAds)
                    {
                        if (ad.Value.Id == favorite.AdId)
                        {
                            viewModel.Items.Add(ad.Value);
                        }
                    }
                }

            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Advertisement;
            if (item == null)
                return;

            ItemsListView.SelectedItem = null;
        }

        private async void RemoveAd_Clicked(object sender, EventArgs e)
        {
            string SelectedAdId = ((Button)sender).BindingContext as string;
            var hasConfirmed = await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.ConfirmAsync(message: "Jeste li sigurni?",
                                    confirmingText: "Ukloni", dismissiveText: "Odustani");
            if(hasConfirmed != false && hasConfirmed != null)
            {
                var hasDeleted = await UserFavoriteAdsDataStore.DeleteItemAsync(SelectedAdId);
                if (hasDeleted)
                    GetCurrentUserFavoriteAds();
            }
        }
    }
}