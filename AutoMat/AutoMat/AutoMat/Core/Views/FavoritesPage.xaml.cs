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
using XF.Material.Forms.UI.Dialogs;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavoritesPage : ContentPage
    {
        public string typeOfAdsPage { get; set; }
        public FavoritesViewModel viewModel{ get; set; }
        public IDataStore<Advertisement> AdverismentDataStore { get; set; }
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public IDataStore<UserFavoriteAd> UserFavoriteAdsDataStore { get; set; }
        public FirebaseUser FirebaseUser{ get; set; }

        public FavoritesPage()
        {
            InitializeComponent();
            typeOfAdsPage = "favorites";
            Title = "Spremljeni oglasi";
            BindingContext = viewModel = new FavoritesViewModel();
            AdverismentDataStore = DependencyService.Get<IDataStore<Advertisement>>() ?? new AdvertismentDataStorage();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            UserFavoriteAdsDataStore = DependencyService.Get<IDataStore<UserFavoriteAd>>() ?? new FavoritesDataStore();
        }

        public FavoritesPage(string typeOfFavorites, FirebaseUser user)
        {
            InitializeComponent();
            if (user != null)
            {
                FirebaseUser = user;
                Title = "Oglasi oglašivača";
            }
            else
            {
                FirebaseUser = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
                Title = "Moji oglasi";
            }

            typeOfAdsPage = typeOfFavorites;
            BindingContext = viewModel = new FavoritesViewModel();
            AdverismentDataStore = DependencyService.Get<IDataStore<Advertisement>>() ?? new AdvertismentDataStorage();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            UserFavoriteAdsDataStore = DependencyService.Get<IDataStore<UserFavoriteAd>>() ?? new FavoritesDataStore();

        }

        protected override async void OnAppearing()
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Učitavanje.."))
            {
                base.OnAppearing();
                switch (typeOfAdsPage)
                {
                    case "favorites":
                        await GetCurrentUserFavoriteAds();
                        break;
                    case "profile":
                        await GetCurrentUserAds(FirebaseUser);
                        break;
                    default:
                        break;
                }
            }
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

        public async Task GetCurrentUserAds(FirebaseUser user)
        {
            var currentUserCached = user;
            if (currentUserCached != null)
            {
                var allAds = await AdverismentDataStore.GetItemsKeyValueAsync();
                var currentUserAds = allAds.Where(u => u.Value.UserId == currentUserCached.Username).ToList();

                viewModel.Items.Clear();

                foreach (var ad in currentUserAds)
                {
                    viewModel.Items.Add(ad.Value);
                }
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Advertisement;
            if (item == null)
                return;

            await Navigation.PushAsync(new AdvertismentPage(item));

            ItemsListView.SelectedItem = null;
        }
        private async void DeleteAd_Clicked(object sender, EventArgs e)
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Učitavanje.."))
            {
                switch (typeOfAdsPage)
                {
                    case "favorites":
                        string SelectedAdId = ((Button)sender).BindingContext as string;
                        var hasConfirmed = await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.ConfirmAsync(message: "Jeste li sigurni?",
                                                confirmingText: "Ukloni", dismissiveText: "Odustani");
                        if (hasConfirmed != false && hasConfirmed != null)
                        {
                            var hasDeleted = await UserFavoriteAdsDataStore.DeleteItemAsync(SelectedAdId);
                            if (hasDeleted)
                                await GetCurrentUserFavoriteAds();
                        }
                        break;
                    case "profile":
                        string SelectedAd = ((Button)sender).BindingContext as string;
                        var hasConfirm = await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.ConfirmAsync(message: "Jeste li sigurni?",
                                                confirmingText: "Obriši", dismissiveText: "Odustani");
                        if (hasConfirm != false && hasConfirm != null)
                        {
                            var hasDeleted = await AdverismentDataStore.DeleteItemAsync(SelectedAd);
                            if (hasDeleted)
                            {
                                await Task.WhenAny
                                    (
                                        GetCurrentUserAds(FirebaseUser),
                                        Navigation.PopAsync()
                                    );
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private async void UpdateButton_Clicked(object sender, EventArgs e)
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Učitavanje.."))
            {
                switch (typeOfAdsPage)
                {
                    case "favorites":
                        await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.AlertAsync(message: "Ne možete urediti oglas koji nije vaš");
                        break;
                    case "profile":
                        string SelectedAd = ((Button)sender).BindingContext as string;
                        var ad = (await AdverismentDataStore.GetItemsKeyValueAsync()).Where(a => a.Value.Id == SelectedAd).FirstOrDefault().Value;
                        await Navigation.PushAsync(new AddNewPage(ad));
                        break;
                    default:
                        break;
                }
            }
        }

        private void ShareButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}