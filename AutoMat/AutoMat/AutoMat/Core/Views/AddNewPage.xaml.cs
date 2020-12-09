using Android.OS;
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
        public IDataStore<AdditionalEquipment> DataStore => DependencyService.Get<IDataStore<AdditionalEquipment>>() ?? new AdditionalEquipmentDataStore();

        public AdditionalEquipment AdditionalEquipment { get; set; }
        public AddNewPage()
        {
            InitializeComponent();

        }

        protected async override void OnAppearing()
        {
            AdditionalEquipment = await DataStore.GetItemAsync("0");


            IEnumerable<int> productionYearsNumber = Enumerable.Range(1930, DateTime.Now.Year - 1930 + 1).Reverse().ToList();
            List<string> productionYearsStrings = new List<string>();

            foreach (var item in productionYearsNumber)
            {
                productionYearsStrings.Add(item.ToString());
            }

            var motorType = new List<string> { "Benzin", "Diesel", "Hibrid", "Električni" , "Benzin + Plin" };
            var driveStrings = new List<string> { "Prednji", "Zadnji", "4x4" };
            var doorsStrings = new List<string> { "2", "3", "4", "5", "6" };
            var bodyStrings = new List<string> { "Limuzina", "Karavan", "Monovolumen", "Coupe", "Kabriolet" , "SUV", "Terensko vozilo" , "Pick up", "Kombibus", "Hatchback" };
            var availabilityStrings = new List<string>() { "Dostupno odmah", "U dolasku", "Moguća narudžba" };
            var suspensionStrings = new List<string>() { "Tvrdi", "Sportski", "Obični" };
            var colorsStrings = new List<string> { "bež", "bijela", "crna", "crvena", "ljubičasta", "narančasta", "plava", "siva", "smeđa", "srebrna", "zlatna", "žuta", "zelena" };
            var ecoNormStrings = new List<string> { "Euro 6 i bolji", "Euro 5", "Euro 4", "Euro 3", "Euro 2", "Euro 1" };
            var ownerStrings = new List<string>() { "Prvi", "Drugi", "Treći", "Četvrti i više" };
            var payTypeStrings = new List<string>() { "gotovina", "kredit", "leasing", "preuzimanje leasinga", " staro za novo", "obročno bankovnim karticama", "zamjena" };
            var shiftsStrings = new List<string>() { "4", "5", "6", "7 i više" };
            var shiftTypeStrings = new List<string>() { "automatski", "ručni" };

            //var counties = await DataStore.GetItemsAsync();
            //var towns = await DataStore.GetItemsAsync();
            //var brands = await DataStore.GetItemsAsync();
            //var models = await DataStore.GetItemsAsync();


            var registeredUntil = new List<string>();
            for (int i = 0; i <= 11; i++)
            {
                var month = DateTime.Now.Month + i;
                var year = DateTime.Now.Year;
                if (month > 12)
                {
                    month -= 12;
                    year += 1;
                }

                registeredUntil.Add($"{month}/{year}");
            }


            var configText = new MaterialInputDialogConfiguration()
            {
                InputType = MaterialTextFieldInputType.Text,
                CornerRadius = 8,
                InputMaxLength = 30,
                BackgroundColor = Color.FromHex("#2c3e50"),
                InputTextColor = Color.White,
                InputPlaceholderColor = Color.White.MultiplyAlpha(0.6),
                TintColor = Color.White,
                TitleTextColor = Color.White,
                MessageTextColor = Color.FromHex("#DEFFFFFF")
            };


            var configNumber = new MaterialInputDialogConfiguration()
            {
                InputType = MaterialTextFieldInputType.Numeric,
                CornerRadius = 8,
                InputMaxLength = 30,
                BackgroundColor = Color.FromHex("#2c3e50"),
                InputTextColor = Color.White,
                InputPlaceholderColor = Color.White.MultiplyAlpha(0.6),
                TintColor = Color.White,
                TitleTextColor = Color.White,
                MessageTextColor = Color.FromHex("#DEFFFFFF")
            };

            var configConfirmation = new MaterialConfirmationDialogConfiguration()
            {
                CornerRadius = 8,
                BackgroundColor = Color.FromHex("#2c3e50"),
                ControlSelectedColor = Color.White,
                ControlUnselectedColor = Color.White.MultiplyAlpha(0.6),
                TintColor = Color.White,
                TitleTextColor = Color.White,
                TextColor = Color.FromHex("#DEFFFFFF")
            };


            //brand

            //model

            var productionYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina proizvodnje",
                                     choices: productionYearsStrings, "Dalje", "Odustani", configConfirmation);

            var modelYear = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina modela",
                                     choices: productionYearsStrings, "Dalje", "Odustani", configConfirmation);

            var onRoadSince = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vozilo u prometu od",
                         choices: productionYearsStrings, "Dalje", "Odustani", configConfirmation);

            var firstRegistrationInCroatia = await MaterialDialog.Instance.SelectChoiceAsync(title: "Godina prve registracije u RH",
             choices: productionYearsStrings, "Dalje", "Odustani", configConfirmation);

            var motor = await MaterialDialog.Instance.SelectChoiceAsync(title: "Motor",
                         choices: motorType, "Dalje", "Odustani", configConfirmation);

            var KM = await MaterialDialog.Instance.InputAsync("Prijeđeno kilometara", "Unesite broj pređenih kilometara", null, "Broj kilometara", "Dalje", "Odustani", configNumber);

            var fuel = await MaterialDialog.Instance.InputAsync("Potrošnja godina", "Unesite prosječnu potrošnju godina", null, "Litre", "Dalje", "Odustani", configNumber);

            var CO2 = await MaterialDialog.Instance.InputAsync("Max. emisija CO2", "g/km", null, "Litre", "Dalje", "Odustani", configNumber);

            //power
            var power = await MaterialDialog.Instance.InputAsync("Snaga motora", "Unesite snagu motora (kW)", null, "kW", "Dalje", "Odustani", configNumber);

            //shift type
            var shiftType = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta mjenjača",
                choices: shiftTypeStrings, "Dalje", "Odustani", configConfirmation);

            //shifts
            var shifts = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj brzina",
             choices: shiftsStrings, "Dalje", "Odustani", configConfirmation);


            var chassis = await MaterialDialog.Instance.InputAsync("Broj šasije", "Upišite broj šasije sa prometne dozvole ili automobila", null, "Broj šasije", "Dalje", "Odustani", configText);

            var drive = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vrsta pogona",
                         choices: driveStrings, "Dalje", "Odustani", configConfirmation);

            var volume = await MaterialDialog.Instance.InputAsync(title: "Radni obujam cm³",
                          "Unesite Volumen Motora", null, "Volumen motora", "Dalje", "Odustani", configNumber);


            var doors = await MaterialDialog.Instance.SelectChoiceAsync(title: "Broj vrata",
                         choices: doorsStrings, "Dalje", "Odustani", configConfirmation);


            var body = await MaterialDialog.Instance.SelectChoiceAsync(title: "Oblik karoserije",
                         choices: bodyStrings, "Dalje", "Odustani", configConfirmation);


            var availability = await MaterialDialog.Instance.SelectChoiceAsync(title: "Dostupnost",
             choices: availabilityStrings, "Dalje", "Odustani", configConfirmation);

            var suspension = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ovjes",
                choices: suspensionStrings, "Dalje", "Odustani", configConfirmation);

            var color = await MaterialDialog.Instance.SelectChoiceAsync(title: "Boja vozila",
             choices: colorsStrings, "Dalje", "Odustani", configConfirmation);

            var ecoNorm = await MaterialDialog.Instance.SelectChoiceAsync(title: "Ekološka kategorija vozila",
                    choices: ecoNormStrings, "Dalje", "Odustani", configConfirmation);

            var owner = await MaterialDialog.Instance.SelectChoiceAsync(title: "Vlasnik",
                choices: ownerStrings, "Dalje", "Odustani", configConfirmation);

            var general = await MaterialDialog.Instance.SelectChoicesAsync(title: "Dodatna oprema",
                                     choices: AdditionalEquipment.General, "Dalje", "Odustani", configConfirmation);

            var safety = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost",
             choices: AdditionalEquipment.Safety, "Dalje", "Odustani", configConfirmation);

            var ac = await MaterialDialog.Instance.SelectChoicesAsync(title: "Klimatizacija",
                                                             choices: AdditionalEquipment.AirBags,"Dalje","Odustani", configConfirmation);

            var airBags = await MaterialDialog.Instance.SelectChoiceAsync(title: "Zračni jastuci",
                                                 choices: AdditionalEquipment.AirBags, "Dalje", "Odustani", configConfirmation);

            var antiSteal = await MaterialDialog.Instance.SelectChoicesAsync(title: "Sigurnost protiv krađe",
                                                 choices: AdditionalEquipment.AntiSteal, "Dalje", "Odustani", configConfirmation);

            var radio = await MaterialDialog.Instance.SelectChoicesAsync(title: "Auto radio",
                                     choices: AdditionalEquipment.Radio, "Dalje", "Odustani", configConfirmation);

            var comfort = await MaterialDialog.Instance.SelectChoicesAsync(title: "Udobnost",
                         choices: AdditionalEquipment.Comfort, "Dalje", "Odustani", configConfirmation);


            var title = await MaterialDialog.Instance.InputAsync("Unesite naslov oglasa", "Maksimalno 30 znakova", null, "Naslov", "Dalje", "Odustani", configText);

            var description = await MaterialDialog.Instance.InputAsync("Unesite opis oglasa", "Opširniji opis Vašeg vozila pridonosi bržoj i uspješnijoj prodaji.", null, "Opis", "Dalje", "Odustani", configText);

            var price = await MaterialDialog.Instance.InputAsync(title: "Cijena",
                          "Unesite cijenu vozila", null, "EUR", "Dalje", "Odustani", configNumber);

            var payType = await MaterialDialog.Instance.SelectChoicesAsync(title: "Mogućnost plaćanja",
                choices: payTypeStrings, "Dalje", "Odustani", configConfirmation);

            //county

            //town

            // number

        }
    }
}