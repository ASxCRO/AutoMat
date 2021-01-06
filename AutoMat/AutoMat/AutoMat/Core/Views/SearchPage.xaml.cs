using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public IDataStore<UserFavoriteAd> UserFavoriteAdsDataStore { get; set; }
        public IDataStore<FirebaseUser> UserDataStore { get; set; }
        public IDataStore<CarBrand> DataStoreCarBrands { get; set; }
        public IDataStore<CarModel> DataStoreCarModels { get; set; }
        public IDataStore<County> DataStoreCounties { get; set; }

        AddNewViewModel addNewViewModel { get; set; }

        AdvertismentsViewModel viewModel;
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new AdvertismentsViewModel();
            addNewViewModel = new AddNewViewModel();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            IsBusy = true;
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            ItemsListView.EndRefresh();

            UserFavoriteAdsDataStore = DependencyService.Get<IDataStore<UserFavoriteAd>>() ?? new FavoritesDataStore();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
            DataStoreCarBrands = DependencyService.Get<IDataStore<CarBrand>>() ?? new BrandsDataStore();
            DataStoreCounties = DependencyService.Get<IDataStore<County>>() ?? new CountiesDataStore();

            addNewViewModel.counties = await DataStoreCounties.GetItemsAsync(false);
            addNewViewModel.brands = await DataStoreCarBrands.GetItemsAsync(false);

            addNewViewModel.SetUpStringListsForFilter();
            IsBusy = false;
        }

            async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Advertisement;
            if (item == null)
                return;

            await Navigation.PushAsync(new AdvertismentPage(item));

            ItemsListView.SelectedItem = null;
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


        public string filter_selectedBrand { get; set; }
        public Dictionary<string, string> filter_selectedYear { get; set; } = new Dictionary<string, string>();
        public string filter_selectedCounty { get; set; }

        public Dictionary<string,string> filter_selectedPrice { get; set; } = new Dictionary<string, string>();
        public Dictionary<string,string> filter_selectedKm { get; set; } = new Dictionary<string, string>();

        private async void Filter_Clicked(object sender, EventArgs e)
        {
            var prikazFiltera_brand = "";
            var prikazFiltera_year = "";

            if (filter_selectedBrand != null)
            {
                prikazFiltera_brand = $"\nMarka - {filter_selectedBrand}";
                prikazFiltera_year = $"\nGod. {filter_selectedYear["yearFrom"]}-{filter_selectedYear["yearTo"]}";

            }
            var filterby = await MaterialDialog.Instance.SelectChoiceAsync(title: $"Trenutno filtrirano po:{prikazFiltera_brand}{prikazFiltera_year} \nFiltriraj oglase po: ", choices: AdvertismentConstants.filterAdBy, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (filterby != -1)
            {
                var selected = AdvertismentConstants.filterAdBy.ElementAt(filterby);
                switch (selected)
                {
                    case "Brandu":
                           int brand = await MaterialDialog.Instance.SelectChoiceAsync(title: "Marka vozila", choices: addNewViewModel.brandsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
                        if (brand != -1)
                        {
                            filter_selectedBrand = addNewViewModel.brandsStrings.ElementAt(brand);
                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => o.Brand == filter_selectedBrand).ToList());
                        }
                        break;
                    case "Godini proizvodnje":
                        int yearFrom = await MaterialDialog.Instance.SelectChoiceAsync(title: "Od godine", choices: addNewViewModel.productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
                        int yearTo = await MaterialDialog.Instance.SelectChoiceAsync(title: "Do godine", choices: addNewViewModel.productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
                        if (yearFrom != -1 && yearTo != -1)
                        {
                            filter_selectedYear.Add("yearFrom", addNewViewModel.productionYearsStrings.ElementAt(yearFrom));
                            filter_selectedYear.Add("yearTo", addNewViewModel.productionYearsStrings.ElementAt(yearTo));

                            var yf = Convert.ToInt32(filter_selectedYear["yearFrom"]);
                            var yt = Convert.ToInt32(filter_selectedYear["yearTo"]);

                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => Convert.ToInt32(o.ProductionYear) >= yf && Convert.ToInt32(o.ProductionYear) <= yt).ToList());
                        }
                        break;
                    case "Županiji":


                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.OrderBy(o => o.Date).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    case "Cijeni":


                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.OrderBy(o => o.Date).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    case "Kilometrima":


                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.OrderBy(o => o.Date).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //ResetFilterModel();
            }
        }

        private async void Sort_Clicked(object sender, EventArgs e)
        {
            var sortby = await MaterialDialog.Instance.SelectChoiceAsync(title: "Sortiraj oglase po: ", choices: AdvertismentConstants.sortAdBy, "Sortiraj", "Odustani", AdvertismentConstants.configChoice);
            if (sortby != -1)
            {
                var selected = AdvertismentConstants.sortAdBy.ElementAt(sortby);
                switch (selected)
                {
                    case "Od najskupljeg":
                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.ToList().OrderByDescending(o => o.Price).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    case "Od najjeftinijeg":
                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.OrderBy(o => o.Price).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    case "Od najnovijeg":
                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.ToList().OrderByDescending(o => o.Date).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    case "Od najstarijeg":
                        viewModel.Items = new ObservableCollection<Advertisement>(viewModel.Items.OrderBy(o => o.Date).ToList());
                        ItemsListView.ItemsSource = viewModel.Items;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}