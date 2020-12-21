using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace AutoMat.Core.Constants
{
    public class AdvertismentConstants
    {
        public static List<string> motorType = new List<string> { "Benzin", "Diesel", "Hibrid", "Električni", "Benzin + Plin" };
        public static List<string> driveStrings = new List<string> { "Prednji", "Zadnji", "4x4" };
        public static List<string> doorsStrings = new List<string> { "2", "3", "4", "5", "6" };
        public static List<string> bodyStrings = new List<string> { "Limuzina", "Karavan", "Monovolumen", "Coupe", "Kabriolet", "SUV", "Terensko vozilo", "Pick up", "Kombibus", "Hatchback" };
        public static List<string> availabilityStrings = new List<string>() { "Dostupno odmah", "U dolasku", "Moguća narudžba" };
        public static List<string> suspensionStrings = new List<string>() { "Tvrdi", "Sportski", "Obični" };
        public static List<string> colorsStrings = new List<string> { "bež", "bijela", "crna", "crvena", "ljubičasta", "narančasta", "plava", "siva", "smeđa", "srebrna", "zlatna", "žuta", "zelena" };
        public static List<string> ecoNormStrings = new List<string> { "Euro 6 i bolji", "Euro 5", "Euro 4", "Euro 3", "Euro 2", "Euro 1" };
        public static List<string> ownerStrings = new List<string>() { "Prvi", "Drugi", "Treći", "Četvrti i više" };
        public static List<string> payTypeStrings = new List<string>() { "gotovina", "kredit", "leasing", "preuzimanje leasinga", " staro za novo", "obročno bankovnim karticama", "zamjena" };
        public static List<string> shiftsStrings = new List<string>() { "4", "5", "6", "7 i više" };
        public static List<string> shiftTypeStrings = new List<string>() { "automatski", "ručni" };
        public static IEnumerable<int> productionYearsNumber = Enumerable.Range(1970, DateTime.Now.Year - 1970 + 1).Reverse().ToList();


        public static MaterialInputDialogConfiguration configText = new MaterialInputDialogConfiguration()
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

        public static MaterialInputDialogConfiguration configPhone = new MaterialInputDialogConfiguration()
        {
            InputType = MaterialTextFieldInputType.Telephone,
            CornerRadius = 8,
            InputMaxLength = 15,
            BackgroundColor = Color.FromHex("#2c3e50"),
            InputTextColor = Color.White,
            InputPlaceholderColor = Color.White.MultiplyAlpha(0.6),
            TintColor = Color.White,
            TitleTextColor = Color.White,
            MessageTextColor = Color.FromHex("#DEFFFFFF")
        };


        public static MaterialInputDialogConfiguration configNumber = new MaterialInputDialogConfiguration()
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

        public static MaterialConfirmationDialogConfiguration configChoice = new MaterialConfirmationDialogConfiguration()
        {
            CornerRadius = 8,
            BackgroundColor = Color.FromHex("#2c3e50"),
            ControlSelectedColor = Color.White,
            ControlUnselectedColor = Color.White.MultiplyAlpha(0.6),
            TintColor = Color.White,
            TitleTextColor = Color.White,
            TextColor = Color.FromHex("#DEFFFFFF")
        };
    }
}
