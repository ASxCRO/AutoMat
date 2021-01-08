using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using Firebase.Auth;
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
            var currentUserCached = (FirebaseUser)JsonConvert.DeserializeObject(Preferences.Get("FirebaseUser", ""), typeof(FirebaseUser));
            var allUsers = await UserDataStore.GetItemsAsync(false);
            var currentUserFirebase = allUsers.Where(u => u.Email == currentUserCached.Email).FirstOrDefault();
            if(currentUserFirebase != null)
            {
                UserFavoriteAd favoriteAd = new UserFavoriteAd
                {
                    Username = currentUserFirebase.Username,
                    AdId = SelectedAdId
                };
                var allUserFavorites = await UserFavoriteAdsDataStore.GetItemsAsync(false);
                var alreadyExists = allUserFavorites.Where(f => f.AdId == favoriteAd.AdId && f.Username == favoriteAd.Username).ToList();
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
            var prikazFilter_county = "";
            var prikazFilter_price = "";
            var prikazFilter_km = "";


            if (filter_selectedBrand != null)
            {
                prikazFiltera_brand = $"\nMarka - {filter_selectedBrand}";
            }

            if(filter_selectedYear.ContainsKey("yearFrom") && filter_selectedYear.ContainsKey("yearTo"))
            {
                prikazFiltera_year = $"\nGod. {filter_selectedYear["yearFrom"]}-{filter_selectedYear["yearTo"]}";
            }

            if (filter_selectedCounty != null)
            {
                prikazFilter_county = $"\nŽupanija - {filter_selectedCounty}";
            }

            if (filter_selectedPrice.ContainsKey("priceFrom") && filter_selectedPrice.ContainsKey("priceTo"))
            {
                prikazFilter_price = $"\nCijena {filter_selectedPrice["priceFrom"]}€ - {filter_selectedPrice["priceTo"]}€";
            }

            if (filter_selectedKm.ContainsKey("kmFrom") && filter_selectedKm.ContainsKey("kmTo"))
            {
                prikazFilter_km = $"\nPrijeđeno {filter_selectedKm["kmFrom"]}km - {filter_selectedKm["kmTo"]}km";
            }

            var filterby = await MaterialDialog.Instance.SelectChoiceAsync(title: $"Trenutno filtrirano po:{prikazFiltera_brand}{prikazFiltera_year}{prikazFilter_county}{prikazFilter_price}{prikazFilter_km} \nFiltriraj oglase po: ", choices: AdvertismentConstants.filterAdBy, "Dalje", "Resetiraj", AdvertismentConstants.configChoice);
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
                            filter_selectedPrice.Clear();
                            filter_selectedYear.Add("yearFrom", addNewViewModel.productionYearsStrings.ElementAt(yearFrom));
                            filter_selectedYear.Add("yearTo", addNewViewModel.productionYearsStrings.ElementAt(yearTo));

                            var yf = Convert.ToInt32(filter_selectedYear["yearFrom"]);
                            var yt = Convert.ToInt32(filter_selectedYear["yearTo"]);

                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => Convert.ToInt32(o.ProductionYear) >= yf && Convert.ToInt32(o.ProductionYear) <= yt).ToList());
                        }
                        break;
                    case "Županiji":
                        int county = await MaterialDialog.Instance.SelectChoiceAsync(title: "Županija", choices: addNewViewModel.countiesStrings, "Dalje", "Odustani",         AdvertismentConstants.configChoice);
                        if (county != -1)
                        {
                            filter_selectedCounty = addNewViewModel.countiesStrings.ElementAt(county);
                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => o.County == filter_selectedCounty).ToList());
                        }
                        break;
                    case "Cijeni":
                        var priceFrom = await MaterialDialog.Instance.InputAsync("Cijena od", "Unesite cijenu od koje filtrirate", null, "Cijena od", "Dalje", "Odustani", AdvertismentConstants.configNumber);
                        var priceTo = await MaterialDialog.Instance.InputAsync("Cijena do", "Unesite cijenu do koje filtrirate", null, "Cijena do", "Filtriraj", "Odustani", AdvertismentConstants.configNumber);
                        if (!String.IsNullOrEmpty(priceFrom) && !String.IsNullOrEmpty(priceTo))
                        {
                            filter_selectedPrice.Clear();
                            filter_selectedPrice.Add("priceFrom", priceFrom);
                            filter_selectedPrice.Add("priceTo", priceTo);

                            var pf = Convert.ToInt32(filter_selectedPrice["priceFrom"]);
                            var pt = Convert.ToInt32(filter_selectedPrice["priceTo"]);

                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => Convert.ToInt32(o.Price) >= pf && Convert.ToInt32(o.Price) <= pt).ToList());
                        }
                        break;
                    case "Kilometrima":
                        var kmFrom = await MaterialDialog.Instance.InputAsync("KM od", "Unesite KM od kojih filtrirate", null, "KM od", "Dalje", "Odustani", AdvertismentConstants.configNumber);
                        var kmTo = await MaterialDialog.Instance.InputAsync("KM do", "Unesite KM do kojih filtrirate", null, "KM do", "Filtriraj", "Odustani", AdvertismentConstants.configNumber);
                        if (!String.IsNullOrEmpty(kmFrom) && !String.IsNullOrEmpty(kmTo))
                        {
                            filter_selectedKm.Clear();
                            filter_selectedKm.Add("kmFrom", kmFrom);
                            filter_selectedKm.Add("kmTo", kmTo);

                            var pf = Convert.ToInt32(filter_selectedKm["kmFrom"]);
                            var pt = Convert.ToInt32(filter_selectedKm["kmTo"]);

                            ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList().Where(o => Convert.ToInt32(o.KM) >= pf && Convert.ToInt32(o.KM) <= pt).ToList());
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                prikazFiltera_brand = "";
                prikazFiltera_year = "";
                prikazFilter_county = "";
                prikazFilter_price = "";
                prikazFilter_km = "";
                filter_selectedBrand = "";
                filter_selectedYear = new Dictionary<string, string>();
                filter_selectedCounty = "";
                filter_selectedPrice = new Dictionary<string, string>();
                filter_selectedKm = new Dictionary<string, string>();

                ItemsListView.ItemsSource = new ObservableCollection<Advertisement>(viewModel.Items.ToList());
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