using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI.Dialogs;
using Xamarin.Essentials;
using Firebase.Storage;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewPage : ContentPage
    {
        public AddNewViewModel addNewViewModel { get; set; }
        public IDataStore<Advertisement> DataStoreAdvertisment { get; set; }
        public IDataStore<AdditionalEquipment> DataStoreAdditionalEquipment { get; set; }
        public IDataStore<CarBrand> DataStoreCarBrands { get; set; }
        public IDataStore<CarModel> DataStoreCarModels { get; set; }
        public IDataStore<County> DataStoreCounties { get; set; }
        public IDataStore<Town> DataStoreTowns { get; set; }

        public bool isFirstPost{ get; set; }

        public AddNewPage()
        {

            InitializeComponent();
            CarouselItems.IsVisible = false;
            isFirstPost = true;
            addNewViewModel = new AddNewViewModel();
            InicijalnizirajDataStorage();
        }


        public AddNewPage(Advertisement advertisement)
        {
            using (MaterialDialog.Instance.LoadingDialogAsync(message: "Pričekajte..").Result)
            {
                addNewViewModel = new AddNewViewModel { Advertisement = advertisement };
                isFirstPost = false;
                //data storage initialize
                InicijalnizirajDataStorage();
                foreach (var imageUrl in addNewViewModel.Advertisement.PicturesURL)
                {
                    addNewViewModel.ImageSourcesUri.Add(ImageSource.FromUri(new Uri(imageUrl)));
                }

                BindingContext = addNewViewModel;
                InitializeComponent();
                CarouselItems.IsVisible = true;
                SetUpEditView();
            }
        }

        protected async override void OnAppearing()
        {
                IsBusy = true;
            //brand
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Inicijalniziram.."))
            {
                //data storages get items
                addNewViewModel.additionalEquipment = await DataStoreAdditionalEquipment.GetItemAsync("0");
                addNewViewModel.counties = await DataStoreCounties.GetItemsAsync(false);
                addNewViewModel.towns = await DataStoreTowns.GetItemsAsync(false);
                addNewViewModel.brands = await DataStoreCarBrands.GetItemsAsync(false);
                addNewViewModel.models = await DataStoreCarModels.GetItemsAsync(false);
                addNewViewModel.SetUpStringLists();
            }

            IsBusy = false;
        }


        private async Task InicijalnizirajDataStorage()
        {
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Inicijalniziram.."))
            {
                //data storage initialize
                DataStoreAdvertisment = DependencyService.Get<IDataStore<Advertisement>>() ?? new AdvertismentDataStorage();
                DataStoreAdditionalEquipment = DependencyService.Get<IDataStore<AdditionalEquipment>>() ?? new AdditionalEquipmentDataStore();
                DataStoreCarBrands = DependencyService.Get<IDataStore<CarBrand>>() ?? new BrandsDataStore();
                DataStoreCarModels = DependencyService.Get<IDataStore<CarModel>>() ?? new ModelsDataStore();
                DataStoreCounties = DependencyService.Get<IDataStore<County>>() ?? new CountiesDataStore();
                DataStoreTowns = DependencyService.Get<IDataStore<Town>>() ?? new TownsDataStore();
            }
        }

        private void SetUpEditView()
        {
            Label.Text = addNewViewModel.Advertisement.Brand;
            Label_1.Text = addNewViewModel.Advertisement.Model;
            Label_2.Text = addNewViewModel.Advertisement.ProductionYear;
            Label_3.Text = addNewViewModel.Advertisement.ModelYear;
            Label_4.Text = addNewViewModel.Advertisement.OnRoadSince;
            Label_6.Text = addNewViewModel.Advertisement.MotorType;
            Label_7.Text = addNewViewModel.Advertisement.KM;
            Label_8.Text = addNewViewModel.Advertisement.AverageFuel;
            Label_9.Text = addNewViewModel.Advertisement.CO2;
            Label_10.Text = addNewViewModel.Advertisement.Power;
            Label_11.Text = addNewViewModel.Advertisement.ShiftType;
            Label_12.Text = addNewViewModel.Advertisement.Shifts;


            Label_13.Text = addNewViewModel.Advertisement.ChassisNumber;
            Label_14.Text = addNewViewModel.Advertisement.Drive;
            Label_15.Text = addNewViewModel.Advertisement.MotorVolume;
            Label_16.Text = addNewViewModel.Advertisement.Doors;
            Label_17.Text = addNewViewModel.Advertisement.Body;
            Label_18.Text = addNewViewModel.Advertisement.Suspension;
            Label_19.Text = addNewViewModel.Advertisement.Color;
            Label_20.Text = addNewViewModel.Advertisement.EcoNorm;
            Label_21.Text = addNewViewModel.Advertisement.Owner;
            Label_22.Text = "Odabrano " + addNewViewModel.Advertisement.AdditionalEquipment.Count().ToString();
            Label_23.Text = "Odabrano " + addNewViewModel.Advertisement.SafetyEquipment.Count().ToString();
            Label_24.Text = "Odabrano " + addNewViewModel.Advertisement.ACEquipment.Count().ToString();
            Label_25.Text = addNewViewModel.Advertisement.AirBags;
            Label_26.Text = "Odabrano " + addNewViewModel.Advertisement.AntiSteal.Count().ToString();
            Label_27.Text = "Odabrano " + addNewViewModel.Advertisement.RadioEquipment.Count().ToString();
            Label_28.Text = "Odabrano " + addNewViewModel.Advertisement.ComfortEquipment.Count().ToString();
            Label_29.Text = String.IsNullOrEmpty(addNewViewModel.Advertisement.Title) ? "Neupisano" : "Upisano";
            Label_30.Text = String.IsNullOrEmpty(addNewViewModel.Advertisement.Description) ? "Neupisano" : "Upisano";
            Label_31.Text = addNewViewModel.Advertisement.Price;
            Label_32.Text = "Odabrano " + addNewViewModel.Advertisement.PayTypes.Count().ToString();
            Label_33.Text = addNewViewModel.Advertisement.Availability;
            Label_34.Text = addNewViewModel.Advertisement.County;
            Label_35.Text = addNewViewModel.Advertisement.Town;
            Label_36.Text = addNewViewModel.Advertisement.PhoneNumber;

        }

        async void Button1_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var pickResult = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Odaberite slike oglasa"
                });



                if (pickResult != null)
                {
                    imgSlider.Images = new System.Collections.ObjectModel.ObservableCollection<FileImageSource>();
                    addNewViewModel.ImageSources.Clear();
                    addNewViewModel.imagesByteArrays.Clear();
                    imgSlider.HeightRequest = 0;
                    if (!isFirstPost)
                    {
                        addNewViewModel.ImageSourcesUri.Clear();
                        CarouselItems.IsVisible = false;
                    }
                    foreach (var image in pickResult)
                    {
                        addNewViewModel.ImageSources.Add(image.FullPath);
                        var imageStream = await image.OpenReadAsync();
                        addNewViewModel.imagesByteArrays.Add(imageStream);
                    }
                    imgSlider.Images = addNewViewModel.ImageSources;
                    imgSlider.HeightRequest = 300;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        private async void MaterialButton_Clicked(object sender, EventArgs e)
        {
            int brand;
            addNewViewModel.selectedCarBrand = null;
            addNewViewModel.filteredModels = null;
            addNewViewModel.Advertisement.Brand = null;
            addNewViewModel.Advertisement.Model = null;
            Label_1.Text = null;
            Label.Text = null;

            //brand
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Inicijalniziram.."))
            {
                brand = await MaterialDialog.Instance.SelectChoiceAsync(title: "Marka vozila", choices: addNewViewModel.brandsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            }
            if (brand != -1)
            {
                addNewViewModel.selectedCarBrand = addNewViewModel.brands.ElementAt(brand);
                addNewViewModel.filteredModels = addNewViewModel.models.Where(m => m.BrandId == addNewViewModel.selectedCarBrand.Id).ToList();
                addNewViewModel.Advertisement.Brand = addNewViewModel.selectedCarBrand.Title;
                Label.Text = addNewViewModel.Advertisement.Brand;
            }
        }

        private async void MaterialButton_Clicked_1(object sender, EventArgs e)
        {
            Label_1.Text = null;
            if (addNewViewModel.selectedCarBrand.Id != null)
            {
                addNewViewModel.modelsStrings.Clear();
                //model se filtrira po brandu
                foreach (var item in addNewViewModel.filteredModels)
                {
                    addNewViewModel.modelsStrings.Add(item.Title);
                }

                int model;
                using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Inicijalniziram.."))
                {
                    model = await MaterialDialog.Instance.SelectChoiceAsync(title: "Model vozila", choices: addNewViewModel.modelsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
                }
                if (model != -1)
                {
                    var selectedModel = addNewViewModel.filteredModels.ElementAt(model);
                    addNewViewModel.Advertisement.Model = selectedModel.Title;
                    Label_1.Text = addNewViewModel.Advertisement.Model;
                }
            }
            else
            {
                await MaterialDialog.Instance.SnackbarAsync(message: "Odaberite marku automobila");
            }
        }

        private async void MaterialButton_Clicked_2(object sender, EventArgs e)
        {
            addNewViewModel.Advertisement.ProductionYear = null;
            Label_2.Text = null;
            int productionYear;
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Inicijalniziram.."))
            {
                productionYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina proizvodnje", choices: addNewViewModel.productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            }
            if (productionYear != -1)
            {
                var selectedProductionYear = addNewViewModel.productionYearsStrings[productionYear];
                addNewViewModel.Advertisement.ProductionYear = selectedProductionYear;
                Label_2.Text = addNewViewModel.Advertisement.ProductionYear;
            }
        }

        private async void MaterialButton_Clicked_3(object sender, EventArgs e)
        {
            Label_3.Text = null;
            addNewViewModel.Advertisement.ModelYear = null;
            //model year max 10 godina ispod production year
            var modelYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina modela", choices: addNewViewModel.productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (modelYear != -1)
            {
                var selected = addNewViewModel.productionYearsStrings[modelYear];
                addNewViewModel.Advertisement.ModelYear = selected;
                Label_3.Text = addNewViewModel.Advertisement.ModelYear;
            }
        }

        private async void MaterialButton_Clicked_4(object sender, EventArgs e)
        {
            Label_4.Text = null;
            addNewViewModel.Advertisement.OnRoadSince = null;
            //nemore bit manji od production year
            var onRoadSince = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vozilo u prometu od", choices: addNewViewModel.productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if(onRoadSince != -1)
            {
                var selected = addNewViewModel.productionYearsStrings[onRoadSince];
                addNewViewModel.Advertisement.OnRoadSince = selected;
                Label_4.Text = addNewViewModel.Advertisement.OnRoadSince;
            }
        }

        private async void MaterialButton_Clicked_6(object sender, EventArgs e)
        {
            Label_6.Text = null;
            addNewViewModel.Advertisement.MotorType = null;
            var motor = await MaterialDialog.Instance.SelectChoiceAsync(title: "Motor", choices: AdvertismentConstants.motorType, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (motor != -1)
            {
                var selected = AdvertismentConstants.motorType.ElementAt(motor);
                addNewViewModel.Advertisement.MotorType = selected;
                Label_6.Text = addNewViewModel.Advertisement.MotorType;
            }
        }

        private async void MaterialButton_Clicked_7(object sender, EventArgs e)
        {
            var KM = await MaterialDialog.Instance.InputAsync("Prijeđeno kilometara", "Unesite broj pređenih kilometara", addNewViewModel.Advertisement.KM, "Broj kilometara", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(KM))
            {
                addNewViewModel.Advertisement.KM = KM;
                Label_7.Text = addNewViewModel.Advertisement.KM;
            }
        }

        private async void MaterialButton_Clicked_8(object sender, EventArgs e)
        {
            var fuel = await MaterialDialog.Instance.InputAsync("Potrošnja goriva", "Unesite prosječnu potrošnju goriva", addNewViewModel.Advertisement.AverageFuel, "Litre", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(fuel))
            {
                addNewViewModel.Advertisement.AverageFuel = fuel;
                Label_8.Text = addNewViewModel.Advertisement.AverageFuel;
            }
        }

        private async void MaterialButton_Clicked_9(object sender, EventArgs e)
        {
            //emisija plinova
            var CO2 = await MaterialDialog.Instance.InputAsync("Max. emisija CO2", "g/km", addNewViewModel.Advertisement.CO2, "CO2", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(CO2))
            {
                addNewViewModel.Advertisement.CO2 = CO2;
                Label_9.Text = addNewViewModel.Advertisement.CO2;
            }
        }

        private async void MaterialButton_Clicked_10(object sender, EventArgs e)
        {
            //power
            var power = await MaterialDialog.Instance.InputAsync("Snaga motora", "Unesite snagu motora (kW)", addNewViewModel.Advertisement.Power, "kW", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(power))
            {
                addNewViewModel.Advertisement.Power = power;
                Label_10.Text = addNewViewModel.Advertisement.Power;
            }
        }

        private async void MaterialButton_Clicked_11(object sender, EventArgs e)
        {
            Label_11.Text = null;
            addNewViewModel.Advertisement.ShiftType = null;
            //shift type
            var shiftType = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta mjenjača",
                choices: AdvertismentConstants.shiftTypeStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (shiftType != -1)
            {
                var selected = AdvertismentConstants.shiftTypeStrings.ElementAt(shiftType);
                addNewViewModel.Advertisement.ShiftType = selected;
                Label_11.Text = addNewViewModel.Advertisement.ShiftType;
            }

        }

        private async void MaterialButton_Clicked_12(object sender, EventArgs e)
        {
            Label_12.Text = null;
            addNewViewModel.Advertisement.Shifts = null;
            //shifts
            var shifts = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj brzina",
             choices: AdvertismentConstants.shiftsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (shifts != -1)
            {
                var selected = AdvertismentConstants.shiftsStrings.ElementAt(shifts);
                addNewViewModel.Advertisement.Shifts = selected;
                Label_12.Text = addNewViewModel.Advertisement.Shifts;
            }
        }

        private async void MaterialButton_Clicked_13(object sender, EventArgs e)
        {
            var chassis = await MaterialDialog.Instance.InputAsync("Broj šasije", "Upišite broj šasije sa prometne dozvole ili automobila", addNewViewModel.Advertisement.ChassisNumber, "Broj šasije", "Dalje", "Odustani", AdvertismentConstants.configText);
            if (!String.IsNullOrEmpty(chassis))
            {
                addNewViewModel.Advertisement.ChassisNumber = chassis;
                Label_13.Text = addNewViewModel.Advertisement.ChassisNumber;
            }
        }

        private async void MaterialButton_Clicked_14(object sender, EventArgs e)
        {
            addNewViewModel.Advertisement.Drive = Label_14.Text = null;
            var drive = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta pogona", choices: AdvertismentConstants.driveStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (drive != -1)
            {
                var selected = AdvertismentConstants.driveStrings.ElementAt(drive);
                addNewViewModel.Advertisement.Drive = selected;
                Label_14.Text = addNewViewModel.Advertisement.Drive;
            }
        }

        private async void MaterialButton_Clicked_15(object sender, EventArgs e)
        {
            var volume = await MaterialDialog.Instance.InputAsync(title: "Radni obujam cm³", "Unesite Volumen Motora", addNewViewModel.Advertisement.MotorVolume, "Volumen motora", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(volume))
            {
                addNewViewModel.Advertisement.MotorVolume = volume;
                Label_15.Text = addNewViewModel.Advertisement.MotorVolume;
            }
        }

        private async void MaterialButton_Clicked_16(object sender, EventArgs e)
        {
            Label_16.Text = null;
            addNewViewModel.Advertisement.Doors = null;
            var doors = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj vrata", choices: AdvertismentConstants.doorsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (doors != -1)
            {
                var selected = AdvertismentConstants.doorsStrings.ElementAt(doors);
                addNewViewModel.Advertisement.Doors = selected;
                Label_16.Text = addNewViewModel.Advertisement.Doors;
            }
        }

        private async void MaterialButton_Clicked_17(object sender, EventArgs e)
        {
            Label_17.Text = null;
            addNewViewModel.Advertisement.Body = null;
            var body = await MaterialDialog.Instance.SelectChoiceAsync(title: "Oblik karoserije", choices: AdvertismentConstants.bodyStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (body != -1)
            {
                var selected = AdvertismentConstants.bodyStrings.ElementAt(body);
                addNewViewModel.Advertisement.Body = selected;
                Label_17.Text = addNewViewModel.Advertisement.Body;
            }
        }

        private async void MaterialButton_Clicked_18(object sender, EventArgs e)
        {
            Label_18.Text = null;
            addNewViewModel.Advertisement.Suspension = null;
            var suspension = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ovjes",
            choices: AdvertismentConstants.suspensionStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (suspension != -1)
            {
                var selected = AdvertismentConstants.suspensionStrings.ElementAt(suspension);
                addNewViewModel.Advertisement.Suspension = selected;
                Label_18.Text = addNewViewModel.Advertisement.Suspension;
            }

        }

        private async void MaterialButton_Clicked_19(object sender, EventArgs e)
        {
            Label_19.Text = null;
            addNewViewModel.Advertisement.Color = null;
            var color = await MaterialDialog.Instance.SelectChoiceAsync(title: "Boja vozila",
 choices: AdvertismentConstants.colorsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (color != -1)
            {
                var selected = AdvertismentConstants.colorsStrings.ElementAt(color);
                addNewViewModel.Advertisement.Color = selected;
                Label_19.Text = addNewViewModel.Advertisement.Color;
            }
        }

        private async void MaterialButton_Clicked_20(object sender, EventArgs e)
        {
            Label_20.Text = null;
            addNewViewModel.Advertisement.EcoNorm = null;
            var ecoNorm = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ekološka kategorija vozila",
        choices: AdvertismentConstants.ecoNormStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (ecoNorm != -1)
            {
                var selected = AdvertismentConstants.ecoNormStrings.ElementAt(ecoNorm);
                addNewViewModel.Advertisement.EcoNorm = selected;
                Label_20.Text = addNewViewModel.Advertisement.EcoNorm;
            }
        }

        private async void MaterialButton_Clicked_21(object sender, EventArgs e)
        {
            Label_21.Text = null;
            addNewViewModel.Advertisement.Owner = null;
            var owner = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vlasnik",
    choices: AdvertismentConstants.ownerStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (owner != -1)
            {
                var selected = AdvertismentConstants.ownerStrings.ElementAt(owner);
                addNewViewModel.Advertisement.Owner = selected;
                Label_21.Text = addNewViewModel.Advertisement.Owner;
            }
        }

        private async void MaterialButton_Clicked_22(object sender, EventArgs e)
        {
            Label_22.Text = null;
            addNewViewModel.Advertisement.AdditionalEquipment.Clear();
            var general = await MaterialDialog.Instance.SelectChoicesAsync(title: "Dodatna oprema", choices: addNewViewModel.additionalEquipment.General, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (general.Length != 0)
            {
                foreach (var equip in general)
                {
                    var selected = addNewViewModel.additionalEquipment.General.ElementAt(equip);
                    addNewViewModel.Advertisement.AdditionalEquipment.Add(selected);
                }
                Label_22.Text = "Odabrano " + addNewViewModel.Advertisement.AdditionalEquipment.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_23(object sender, EventArgs e)
        {
            Label_23.Text = null;
            addNewViewModel.Advertisement.SafetyEquipment.Clear();
            var safety = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost",
 choices: addNewViewModel.additionalEquipment.Safety, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (safety.Length != 0)
            {
                foreach (var equip in safety)
                {
                    var selected = addNewViewModel.additionalEquipment.Safety.ElementAt(equip);
                    addNewViewModel.Advertisement.SafetyEquipment.Add(selected);
                }
                Label_23.Text = "Odabrano " + addNewViewModel.Advertisement.SafetyEquipment.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_24(object sender, EventArgs e)
        {
            Label_24.Text = null;
            addNewViewModel.Advertisement.ACEquipment.Clear();
            var ac = await MaterialDialog.Instance.SelectChoicesAsync(title: "Klimatizacija", choices: addNewViewModel.additionalEquipment.AirConditioner, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (ac.Length != 0)
            {
                foreach (var equip in ac)
                {
                    var selected = addNewViewModel.additionalEquipment.AirConditioner.ElementAt(equip);
                    addNewViewModel.Advertisement.ACEquipment.Add(selected);
                }
                Label_24.Text = "Odabrano " + addNewViewModel.Advertisement.ACEquipment.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_25(object sender, EventArgs e)
        {
            Label_25.Text = null;
            addNewViewModel.Advertisement.AirBags = null;
            var airBags = await MaterialDialog.Instance.SelectChoiceAsync(title: "Zračni jastuci", choices: addNewViewModel.additionalEquipment.AirBags, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (airBags != -1)
            {
                var selected = addNewViewModel.additionalEquipment.AirBags.ElementAt(airBags);
                addNewViewModel.Advertisement.AirBags = selected;
                Label_25.Text = addNewViewModel.Advertisement.AirBags;
            }

        }

        private async void MaterialButton_Clicked_26(object sender, EventArgs e)
        {
            Label_26.Text = null;
            addNewViewModel.Advertisement.AntiSteal.Clear();
            var antiSteal = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost protiv krađe", choices: addNewViewModel.additionalEquipment.AntiSteal, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (antiSteal.Length != 0)
            {
                foreach (var equip in antiSteal)
                {
                    var selected = addNewViewModel.additionalEquipment.AntiSteal.ElementAt(equip);
                    addNewViewModel.Advertisement.AntiSteal.Add(selected);
                }
                Label_26.Text = "Odabrano " + addNewViewModel.Advertisement.AntiSteal.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_27(object sender, EventArgs e)
        {
            Label_27.Text = null;
            addNewViewModel.Advertisement.RadioEquipment.Clear();
            Label_27.Text = null;
            var radio = await MaterialDialog.Instance.SelectChoicesAsync(title: "Auto radio", choices: addNewViewModel.additionalEquipment.Radio, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (radio.Length != 0)
            {
                foreach (var equip in radio)
                {
                    var selected = addNewViewModel.additionalEquipment.Radio.ElementAt(equip);
                    addNewViewModel.Advertisement.RadioEquipment.Add(selected);
                }
                Label_27.Text = "Odabrano " + addNewViewModel.Advertisement.RadioEquipment.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_28(object sender, EventArgs e)
        {
            Label_28.Text = null;
            addNewViewModel.Advertisement.ComfortEquipment.Clear();
            var comfort = await MaterialDialog.Instance.SelectChoicesAsync(title: "Udobnost", choices: addNewViewModel.additionalEquipment.Comfort, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (comfort.Length != 0)
            {
                foreach (var equip in comfort)
                {
                    var selected = addNewViewModel.additionalEquipment.Comfort.ElementAt(equip);
                    addNewViewModel.Advertisement.ComfortEquipment.Add(selected);
                }
                Label_28.Text = "Odabrano " + addNewViewModel.Advertisement.ComfortEquipment.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_29(object sender, EventArgs e)
        {
            var title = await MaterialDialog.Instance.InputAsync("Unesite naslov oglasa", "Maksimalno 30 znakova", addNewViewModel.Advertisement.Title, "Naslov", "Dalje", "Odustani", AdvertismentConstants.configText);
            if (!String.IsNullOrEmpty(title))
            {
                addNewViewModel.Advertisement.Title = title;
                Label_29.Text = addNewViewModel.Advertisement.Title;
            }
        }

        private async void MaterialButton_Clicked_30(object sender, EventArgs e)
        {
            Label_30.Text = addNewViewModel.Advertisement.Description = null;
            var description = await MaterialDialog.Instance.InputAsync("Unesite opis oglasa", "Opširniji opis Vašeg vozila pridonosi bržoj i uspješnijoj prodaji.", addNewViewModel.Advertisement.Description, "Opis", "Dalje", "Odustani", AdvertismentConstants.configPlain);
            if (!String.IsNullOrEmpty(description))
            {
                addNewViewModel.Advertisement.Description = description;
                Label_30.Text = addNewViewModel.Advertisement.Description;
            }
        }

        private async void MaterialButton_Clicked_31(object sender, EventArgs e)
        {
            Label_31.Text = addNewViewModel.Advertisement.Price = null;
            var price = await MaterialDialog.Instance.InputAsync(title: "Cijena", "Unesite cijenu vozila", addNewViewModel.Advertisement.Price, "EUR", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            if (!String.IsNullOrEmpty(price))
            {
                addNewViewModel.Advertisement.Price = price;
                Label_31.Text = addNewViewModel.Advertisement.Price;
            }
        }

        private async void MaterialButton_Clicked_32(object sender, EventArgs e)
        {
            addNewViewModel.Advertisement.PayTypes.Clear();
            Label_32.Text = null;

            var payType = await MaterialDialog.Instance.SelectChoicesAsync(title: "Mogućnost plaćanja",
    choices: AdvertismentConstants.payTypeStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (payType.Length != 0)
            {
                foreach (var type in payType)
                {
                    var selected = AdvertismentConstants.payTypeStrings.ElementAt(type);
                    addNewViewModel.Advertisement.PayTypes.Add(selected);
                }
                Label_32.Text = "Odabrano " + addNewViewModel.Advertisement.PayTypes.Count().ToString();
            }

        }

        private async void MaterialButton_Clicked_33(object sender, EventArgs e)
        {
            addNewViewModel.Advertisement.Availability = Label_33.Text = null;
            var availability = await MaterialDialog.Instance.SelectChoiceAsync(title: "Dostupnost",
choices: AdvertismentConstants.availabilityStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (availability != -1)
            {
                var selected = AdvertismentConstants.availabilityStrings.ElementAt(availability);
                addNewViewModel.Advertisement.Availability = selected;
                Label_33.Text = addNewViewModel.Advertisement.Availability;
            }

        }

        private async void MaterialButton_Clicked_34(object sender, EventArgs e)
        {
            addNewViewModel.townsStrings.Clear();
            addNewViewModel.Advertisement.County = Label_34.Text = null;
            addNewViewModel.selectedCounty = new County();
            Label_35.Text = addNewViewModel.Advertisement.Town = null;
           //county
           var county = await MaterialDialog.Instance.SelectChoiceAsync(title: "Lokacija vozila - županija", choices: addNewViewModel.countiesStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            if (county != -1)
            {
                var selected = addNewViewModel.countiesStrings.ElementAt(county);
                addNewViewModel.Advertisement.County = selected;
                addNewViewModel.selectedCounty = addNewViewModel.counties.Where(c => c.Name == selected).FirstOrDefault();
                Label_34.Text = addNewViewModel.Advertisement.County;
            }
        }

        private async void MaterialButton_Clicked_35(object sender, EventArgs e)
        {
            if(addNewViewModel.selectedCounty.Id != null)
            {
                foreach (var item in addNewViewModel.towns.Where(d => d.CountyId == addNewViewModel.selectedCounty.Id).ToList())
                {
                    addNewViewModel.townsStrings.Add(item.Name);
                }
                var town = await MaterialDialog.Instance.SelectChoiceAsync(title: "Lokacija vozila - Grad", choices: addNewViewModel.townsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
                if (town != -1)
                {
                    var selected = addNewViewModel.townsStrings.ElementAt(town);
                    addNewViewModel.Advertisement.Town = selected;
                    Label_35.Text = addNewViewModel.Advertisement.Town;
                }
            }
            else
            {
                await MaterialDialog.Instance.SnackbarAsync(message: "Prvo odaberite županiju");
            }
        }

        private async void MaterialButton_Clicked_36(object sender, EventArgs e)
        {
            // number
            var phone = await MaterialDialog.Instance.InputAsync("Unesite kontakt broj telefona", "Maksimalno 15 znakova", inputText: addNewViewModel.Advertisement.PhoneNumber, "Naslov", "Dalje", "Odustani", AdvertismentConstants.configPhone);
            if (!String.IsNullOrEmpty(phone))
            {
                addNewViewModel.Advertisement.PhoneNumber = phone;
                Label_36.Text = addNewViewModel.Advertisement.PhoneNumber;
            }
        }

        private async void PostAdverisment_Clicked(object sender, EventArgs e)
        {
            IsBusy = true;
            if(
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Title) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Model) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Brand) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.CO2) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.EcoNorm) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.ProductionYear) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.ModelYear) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.KM) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.AverageFuel) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Drive) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Doors) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Body) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Suspension) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Color) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Price) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Availability) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.PhoneNumber) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Town) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.MotorVolume) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.County) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.OnRoadSince) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Availability) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.ChassisNumber) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Shifts) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.ShiftType) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Power) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Type) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.MotorType) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Owner) ||
                 String.IsNullOrEmpty(addNewViewModel.Advertisement.Description) ||
                 (addNewViewModel.ImageSources == null && addNewViewModel.ImageSourcesUri == null ) ||
                 (addNewViewModel.ImageSources.Count() <= 0 && addNewViewModel.ImageSourcesUri.Count() <= 0)
              )
            {
                await MaterialDialog.Instance.SnackbarAsync(message: "Sve informacije osim dodatne opreme su neophodne!");
            }
            else
            {
                var firebaseStorage = new FirebaseStorage("automat-29cec.appspot.com");
                bool isSuccessful = false;
                var user = JsonConvert.DeserializeObject<FirebaseUser>(Preferences.Get("FirebaseUser", ""));

                using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Pričekajte.."))
                {
                    if (isFirstPost)
                    {
                        foreach (var imageStream in addNewViewModel.imagesByteArrays)
                        {
                            var nazivSlike = Guid.NewGuid().ToString();

                            var task = await firebaseStorage
                                .Child("data")
                                .Child(nazivSlike + ".png")
                                .PutAsync(imageStream);

                            var urlSlike = await firebaseStorage
                                .Child("data")
                                .Child(nazivSlike + ".png")
                                .GetDownloadUrlAsync();

                            addNewViewModel.Advertisement.PicturesURL.Add(urlSlike);

                        }

                        addNewViewModel.Advertisement.Date = DateTime.Now;

                        addNewViewModel.Advertisement.Id = Guid.NewGuid().ToString();
                        addNewViewModel.Advertisement.UserId = user.Username;
                        isSuccessful = await DataStoreAdvertisment.AddItemAsync(addNewViewModel.Advertisement);
                    }
                    else
                    {
                        if (addNewViewModel.imagesByteArrays.Count() > 0)
                        {
                            addNewViewModel.Advertisement.PicturesURL.Clear();
                            foreach (var imageStream in addNewViewModel.imagesByteArrays)
                            {
                                var nazivSlike = Guid.NewGuid().ToString();

                                var task = await firebaseStorage
                                    .Child("data")
                                    .Child(nazivSlike + ".png")
                                    .PutAsync(imageStream);

                                var urlSlike = await firebaseStorage
                                    .Child("data")
                                    .Child(nazivSlike + ".png")
                                    .GetDownloadUrlAsync();

                                addNewViewModel.Advertisement.PicturesURL.Add(urlSlike);
                            }

                        }
                        isSuccessful = await DataStoreAdvertisment.UpdateItemAsync(addNewViewModel.Advertisement);

                    }
                }


                if (isSuccessful)
                    await MaterialDialog.Instance.SnackbarAsync(message: "Oglas uspješno objavljen");
                else
                    await MaterialDialog.Instance.SnackbarAsync(message: "Došlo je do pogreške. Provjerite internet vezu.");
                IsBusy = false;
            }
        }
    }
}