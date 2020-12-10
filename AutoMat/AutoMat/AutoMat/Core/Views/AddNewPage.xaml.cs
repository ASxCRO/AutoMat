using Android.OS;
using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewPage : ContentPage
    {
        public IDataStore<AdditionalEquipment> DataStoreAdditionalEquipment => DependencyService.Get<IDataStore<AdditionalEquipment>>() ?? new AdditionalEquipmentDataStore();

        public IDataStore<CarBrand> DataStoreCarBrands => DependencyService.Get<IDataStore<CarBrand>>() ?? new BrandsDataStore();

        public IDataStore<CarModel> DataStoreCarModels => DependencyService.Get<IDataStore<CarModel>>() ?? new ModelsDataStore();

        public IDataStore<County> DataStoreCounties => DependencyService.Get<IDataStore<County>>() ?? new CountiesDataStore();

        public IDataStore<Town> DataStoreTowns => DependencyService.Get<IDataStore<Town>>() ?? new TownsDataStore();

        private AdditionalEquipment additionalEquipment { get; set; }
        private IEnumerable<County> counties { get; set; }
        private IEnumerable<Town> towns { get; set; }

        private IEnumerable<CarBrand> brands { get; set; }
        private IEnumerable<CarModel> models { get; set; }

        private List<string> productionYearsStrings { get; set; }
        private List<string> countiesStrings { get; set; }
        private List<string> townsStrings { get; set; }
        private List<string> brandsStrings { get; set; }
        private List<string> modelsStrings { get; set; }
        private List<string> registeredUntilStrings { get; set; }

        private  CarBrand selectedCarBrand { get; set; }
        private IEnumerable<CarModel> filteredModels { get; set; }

        private Advertisement Advertisement { get; set; }

        private int AdvertismentEquipmentCount { get; set; }
        private int SecurityEquipmentCount { get; set; }
        private int ACEquipmentCount { get; set; }

        private int AirBagEquipmentCount { get; set; }

        private int StealEquipmentCount { get; set; }

        private int RadioEquipmentCount { get; set; }

        private int ComfortEquipmentCount { get; set; }

        private int PayTypesCount { get; set; }


        private string isEntered { get; set; } = "Unijeli ste";

        private string isNotEntered { get; set; } = "Niste još unijeli";

        public AddNewPage()
        {
            InitializeComponent();
        }


        protected async override void OnAppearing()
        {
            //data storages
            additionalEquipment = await DataStoreAdditionalEquipment.GetItemAsync("0");
            counties = await DataStoreCounties.GetItemsAsync(false);
            towns = await DataStoreTowns.GetItemsAsync(false);
            brands = await DataStoreCarBrands.GetItemsAsync(false);
            models = await DataStoreCarModels.GetItemsAsync(false);

            //string lists
            productionYearsStrings = new List<string>();
            countiesStrings = new List<string>();
            townsStrings = new List<string>();
            brandsStrings = new List<string>();
            modelsStrings = new List<string>();
            registeredUntilStrings = new List<string>();

            selectedCarBrand = new CarBrand();
            Advertisement = new Advertisement();
            SetUpStringLists();
        }

        private void SetUpStringLists()
        {
            foreach (var item in AdvertismentConstants.productionYearsNumber)
            {
                productionYearsStrings.Add(item.ToString());
            }

            foreach (var item in counties)
            {
                countiesStrings.Add(item.Name);
            }


            foreach (var item in towns)
            {
                townsStrings.Add(item.Name);
            }


            foreach (var item in brands)
            {
                brandsStrings.Add(item.Title.ToString());
            }

            for (int i = 0; i <= 12; i++)
            {
                var month = DateTime.Now.Month + i;
                var year = DateTime.Now.Year;
                if (month > 12)
                {
                    month -= 12;
                    year += 1;
                }

                registeredUntilStrings.Add($"{month}/{year}");
            }
        }

        private async void MaterialButton_Clicked(object sender, EventArgs e)
        {
            //brand
            var brand = await MaterialDialog.Instance.SelectChoiceAsync(title: "Marka vozila", choices: brandsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            selectedCarBrand = brands.ElementAt(brand);
            filteredModels = models.Where(m => m.BrandId == selectedCarBrand.Id).ToList();
            Advertisement.Brand = selectedCarBrand.Title;
            Label.Text = Advertisement.Brand;
        }

        private async void MaterialButton_Clicked_1(object sender, EventArgs e)
        {
            modelsStrings.Clear();
            //model se filtrira po brandu
            foreach (var item in filteredModels)
            {
                modelsStrings.Add(item.Title);
            }

            var model = await MaterialDialog.Instance.SelectChoiceAsync(title: "Model vozila", choices: modelsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selectedModel = filteredModels.ElementAt(model);
            Advertisement.Model = selectedModel.Title;
            Label_1.Text = Advertisement.Model;
        }

        private async void MaterialButton_Clicked_2(object sender, EventArgs e)
        {
            var productionYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina proizvodnje", choices: productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selectedProductionYear = productionYearsStrings[productionYear];
            Advertisement.ProductionYear = selectedProductionYear;
            Label_2.Text = Advertisement.ProductionYear;
        }

        private async void MaterialButton_Clicked_3(object sender, EventArgs e)
        {
            //model year max 10 godina ispod production year
            var modelYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina modela", choices: productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = productionYearsStrings[modelYear];
            Advertisement.ModelYear = selected;
            Label_3.Text = Advertisement.ModelYear;
        }

        private async void MaterialButton_Clicked_4(object sender, EventArgs e)
        {
            //nemore bit manji od production year
            var onRoadSince = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vozilo u prometu od", choices: productionYearsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = productionYearsStrings[onRoadSince];
            Advertisement.OnRoadSince = selected;
            Label_4.Text = Advertisement.OnRoadSince;
        }

        private async void MaterialButton_Clicked_6(object sender, EventArgs e)
        {
            var motor = await MaterialDialog.Instance.SelectChoiceAsync(title: "Motor", choices: AdvertismentConstants.motorType, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.motorType.ElementAt(motor);
            Advertisement.MotorType = selected;
            Label_6.Text = Advertisement.MotorType;
        }

        private async void MaterialButton_Clicked_7(object sender, EventArgs e)
        {

            var KM = await MaterialDialog.Instance.InputAsync("Prijeđeno kilometara", "Unesite broj pređenih kilometara", null, "Broj kilometara", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.KM = KM;
            Label_7.Text = Advertisement.KM;
        }

        private async void MaterialButton_Clicked_8(object sender, EventArgs e)
        {

            var fuel = await MaterialDialog.Instance.InputAsync("Potrošnja goriva", "Unesite prosječnu potrošnju goriva", null, "Litre", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.AverageFuel = fuel;
            Label_8.Text = Advertisement.AverageFuel;
        }

        private async void MaterialButton_Clicked_9(object sender, EventArgs e)
        {
            //emisija plinova
            var CO2 = await MaterialDialog.Instance.InputAsync("Max. emisija CO2", "g/km", null, "Litre", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.CO2 = CO2;
            Label_9.Text = Advertisement.CO2;
        }

        private async void MaterialButton_Clicked_10(object sender, EventArgs e)
        {
            //power
            var power = await MaterialDialog.Instance.InputAsync("Snaga motora", "Unesite snagu motora (kW)", null, "kW", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.Power = power;
            Label_10.Text = Advertisement.Power;
        }

        private async void MaterialButton_Clicked_11(object sender, EventArgs e)
        {
            //shift type
            var shiftType = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta mjenjača",
                choices: AdvertismentConstants.shiftTypeStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.shiftTypeStrings.ElementAt(shiftType);
            Advertisement.ShiftType = selected;
            Label_11.Text = Advertisement.ShiftType;

        }

        private async void MaterialButton_Clicked_12(object sender, EventArgs e)
        {
            //shifts
            var shifts = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj brzina",
             choices: AdvertismentConstants.shiftsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.shiftsStrings.ElementAt(shifts);
            Advertisement.Shifts = selected;
            Label_12.Text = Advertisement.Shifts;
        }

        private async void MaterialButton_Clicked_13(object sender, EventArgs e)
        {
            var chassis = await MaterialDialog.Instance.InputAsync("Broj šasije", "Upišite broj šasije sa prometne dozvole ili automobila", null, "Broj šasije", "Dalje", "Odustani", AdvertismentConstants.configText);
            Advertisement.ChassisNumber = chassis;
            Label_13.Text = Advertisement.ChassisNumber;
        }

        private async void MaterialButton_Clicked_14(object sender, EventArgs e)
        {

            var drive = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta pogona", choices: AdvertismentConstants.driveStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.driveStrings.ElementAt(drive);
            Advertisement.Drive = selected;
            Label_14.Text = Advertisement.Drive;
        }

        private async void MaterialButton_Clicked_15(object sender, EventArgs e)
        {
            var volume = await MaterialDialog.Instance.InputAsync(title: "Radni obujam cm³", "Unesite Volumen Motora", null, "Volumen motora", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.MotorVolume = volume;
            Label_15.Text = Advertisement.MotorVolume;
        }

        private async void MaterialButton_Clicked_16(object sender, EventArgs e)
        {


            var doors = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj vrata", choices: AdvertismentConstants.doorsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.doorsStrings.ElementAt(doors);
            Advertisement.Doors = selected;
            Label_16.Text = Advertisement.Doors;
        }

        private async void MaterialButton_Clicked_17(object sender, EventArgs e)
        {
            var body = await MaterialDialog.Instance.SelectChoiceAsync(title: "Oblik karoserije", choices: AdvertismentConstants.bodyStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.bodyStrings.ElementAt(body);
            Advertisement.Body = selected;
            Label_17.Text = Advertisement.Body;
        }

        private async void MaterialButton_Clicked_18(object sender, EventArgs e)
        {
            var suspension = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ovjes",
        choices: AdvertismentConstants.suspensionStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.suspensionStrings.ElementAt(suspension);
            Advertisement.Suspension = selected;
            Label_18.Text = Advertisement.Suspension;

        }

        private async void MaterialButton_Clicked_19(object sender, EventArgs e)
        {
            var color = await MaterialDialog.Instance.SelectChoiceAsync(title: "Boja vozila",
 choices: AdvertismentConstants.colorsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.colorsStrings.ElementAt(color);
            Advertisement.Color = selected;
            Label_19.Text = Advertisement.Color;
        }

        private async void MaterialButton_Clicked_20(object sender, EventArgs e)
        {
            var ecoNorm = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ekološka kategorija vozila",
        choices: AdvertismentConstants.ecoNormStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.ecoNormStrings.ElementAt(ecoNorm);
            Advertisement.EcoNorm = selected;
            Label_20.Text = Advertisement.EcoNorm;
        }

        private async void MaterialButton_Clicked_21(object sender, EventArgs e)
        {
            var owner = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vlasnik",
    choices: AdvertismentConstants.ownerStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.ownerStrings.ElementAt(owner);
            Advertisement.Owner = selected;
            Label_21.Text = Advertisement.Owner;
        }

        private async void MaterialButton_Clicked_22(object sender, EventArgs e)
        {
            var general = await MaterialDialog.Instance.SelectChoicesAsync(title: "Dodatna oprema", choices: additionalEquipment.General, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in general)
            {
                var selected = additionalEquipment.General.ElementAt(equip);
                Advertisement.AdditionalEquipment.Add(selected);
            }
            Label_22.Text = "Odabrano " + Advertisement.AdditionalEquipment.Count().ToString();

        }

        private async void MaterialButton_Clicked_23(object sender, EventArgs e)
        {
            var safety = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost",
 choices: additionalEquipment.Safety, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in safety)
            {
                var selected = additionalEquipment.Safety.ElementAt(equip);
                Advertisement.SafetyEquipment.Add(selected);
            }
            Label_23.Text = "Odabrano " + Advertisement.SafetyEquipment.Count().ToString();

        }

        private async void MaterialButton_Clicked_24(object sender, EventArgs e)
        {
            var ac = await MaterialDialog.Instance.SelectChoicesAsync(title: "Klimatizacija", choices: additionalEquipment.AirConditioner, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in ac)
            {
                var selected = additionalEquipment.AirConditioner.ElementAt(equip);
                Advertisement.ACEquipment.Add(selected);
            }
            Label_24.Text = "Odabrano " + Advertisement.ACEquipment.Count().ToString();

        }

        private async void MaterialButton_Clicked_25(object sender, EventArgs e)
        {

            var airBags = await MaterialDialog.Instance.SelectChoiceAsync(title: "Zračni jastuci", choices: additionalEquipment.AirBags, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = additionalEquipment.AirBags.ElementAt(airBags);
            Advertisement.AirBags = selected;
            Label_25.Text = Advertisement.AirBags;

        }

        private async void MaterialButton_Clicked_26(object sender, EventArgs e)
        {
            var antiSteal = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost protiv krađe", choices: additionalEquipment.AntiSteal, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in antiSteal)
            {
                var selected = additionalEquipment.AntiSteal.ElementAt(equip);
                Advertisement.AntiSteal.Add(selected);
            }
            Label_26.Text = "Odabrano " + Advertisement.AntiSteal.Count().ToString();

        }

        private async void MaterialButton_Clicked_27(object sender, EventArgs e)
        {

            var radio = await MaterialDialog.Instance.SelectChoicesAsync(title: "Auto radio", choices: additionalEquipment.Radio, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in radio)
            {
                var selected = additionalEquipment.Radio.ElementAt(equip);
                Advertisement.RadioEquipment.Add(selected);
            }
            Label_27.Text = "Odabrano " + Advertisement.RadioEquipment.Count().ToString();

        }

        private async void MaterialButton_Clicked_28(object sender, EventArgs e)
        {
            var comfort = await MaterialDialog.Instance.SelectChoicesAsync(title: "Udobnost", choices: additionalEquipment.Comfort, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var equip in comfort)
            {
                var selected = additionalEquipment.Comfort.ElementAt(equip);
                Advertisement.ComfortEquipment.Add(selected);
            }
            Label_28.Text = "Odabrano " + Advertisement.ComfortEquipment.Count().ToString();

        }

        private async void MaterialButton_Clicked_29(object sender, EventArgs e)
        {
            var title = await MaterialDialog.Instance.InputAsync("Unesite naslov oglasa", "Maksimalno 30 znakova", null, "Naslov", "Dalje", "Odustani", AdvertismentConstants.configText);
            Advertisement.Title = title;
            Label_29.Text = Advertisement.Title;
        }

        private async void MaterialButton_Clicked_30(object sender, EventArgs e)
        {

            var description = await MaterialDialog.Instance.InputAsync("Unesite opis oglasa", "Opširniji opis Vašeg vozila pridonosi bržoj i uspješnijoj prodaji.", null, "Opis", "Dalje", "Odustani", AdvertismentConstants.configText);
            Advertisement.Description = description;
            Label_30.Text = Advertisement.Description;
        }

        private async void MaterialButton_Clicked_31(object sender, EventArgs e)
        {
            var price = await MaterialDialog.Instance.InputAsync(title: "Cijena", "Unesite cijenu vozila", null, "EUR", "Dalje", "Odustani", AdvertismentConstants.configNumber);
            Advertisement.Price = price;
            Label_31.Text = Advertisement.Price;
        }

        private async void MaterialButton_Clicked_32(object sender, EventArgs e)
        {
            var payType = await MaterialDialog.Instance.SelectChoicesAsync(title: "Mogućnost plaćanja",
    choices: AdvertismentConstants.payTypeStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            foreach (var type in payType)
            {
                var selected = AdvertismentConstants.payTypeStrings.ElementAt(type);
                Advertisement.PayTypes.Add(selected);
            }
            Label_32.Text = "Odabrano " + Advertisement.PayTypes.Count().ToString();

        }

        private async void MaterialButton_Clicked_33(object sender, EventArgs e)
        {
            var availability = await MaterialDialog.Instance.SelectChoiceAsync(title: "Dostupnost",
choices: AdvertismentConstants.availabilityStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = AdvertismentConstants.availabilityStrings.ElementAt(availability);
            Advertisement.Availability = selected;
            Label_33.Text = Advertisement.Availability;
        }

        private async void MaterialButton_Clicked_34(object sender, EventArgs e)
        {
            //county
            var county = await MaterialDialog.Instance.SelectChoiceAsync(title: "Lokacija vozila - županija", choices: countiesStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = countiesStrings.ElementAt(county);
            Advertisement.County = selected;
            Label_34.Text = Advertisement.County;
        }

        private async void MaterialButton_Clicked_35(object sender, EventArgs e)
        {
            //town
            var town = await MaterialDialog.Instance.SelectChoiceAsync(title: "Lokacija vozila - Grad", choices: townsStrings, "Dalje", "Odustani", AdvertismentConstants.configChoice);
            var selected = townsStrings.ElementAt(town);
            Advertisement.Town = selected;
            Label_35.Text = Advertisement.Town;
        }

        private async void MaterialButton_Clicked_36(object sender, EventArgs e)
        {
            // number
            var phone = await MaterialDialog.Instance.InputAsync("Unesite kontakt broj telefona", "Maksimalno 15 znakova", null, "Naslov", "Dalje", "Odustani", AdvertismentConstants.configPhone);
            Advertisement.PhoneNumber = phone;
            Label_36.Text = Advertisement.PhoneNumber;
        }
    }
}