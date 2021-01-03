using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public IDataStore<UserFavoriteAd> UserFavoriteAdsDataStore { get; set; }
        public IDataStore<FirebaseUser> UserDataStore { get; set; }

        AdvertismentsViewModel viewModel;
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new AdvertismentsViewModel();
            UserFavoriteAdsDataStore = DependencyService.Get<IDataStore<UserFavoriteAd>>() ?? new FavoritesDataStore();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Advertisement;
            if (item == null)
                return;

            await Navigation.PushAsync(new AdvertismentPage(item));

            //var ads = await advertismentDataStorage.GetItemsKeyValueAsync();
            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }

        private async void SaveFavorite_Clicked(object sender, EventArgs e)
        {
            string SelectedAdId = ((Button)sender).BindingContext as string;
            var currentUserCached = (Firebase.Auth.User)JsonConvert.DeserializeObject(Preferences.Get("CurrentUser", ""), typeof(Firebase.Auth.User));
            var allUsers = await UserDataStore.GetItemsAsync(false);
            var currentUserFirebase = allUsers.Where(u => u.Email == currentUserCached.Email).FirstOrDefault();
            if(currentUserFirebase != null)
            {
                UserFavoriteAd favoriteAd = new UserFavoriteAd
                {
                    UserId = currentUserFirebase.Id,
                    OglasId = SelectedAdId
                };
                var allUserFavorites = await UserFavoriteAdsDataStore.GetItemsAsync(false);
                var alreadyExists = allUserFavorites.Where(f => f.OglasId == favoriteAd.OglasId && f.UserId == favoriteAd.UserId).ToList();
                if (alreadyExists.Count() > 0)
                {
                    await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.AlertAsync(message: "Oglas je već bio spremljen!");
                }
                else
                {
                    await UserFavoriteAdsDataStore.AddItemAsync(favoriteAd);
                    await XF.Material.Forms.UI.Dialogs.MaterialDialog.Instance.AlertAsync(message: "Uspješno ste spremili oglas!");
                }
            }
        }
    }
}