using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.Views;
using Newtonsoft.Json;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat
{
    public partial class App : Application
    {
        public IDataStore<Advertisement> DataStoreAdvertisment { get; set; }
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new NavigationPage(new UserPage());
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
           if (uri.Host.EndsWith("automat.hr", StringComparison.OrdinalIgnoreCase))
            {

                if (uri.Segments != null && uri.Segments.Length == 3)
                {
                    var action = uri.Segments[1].Replace("/", "");
                    var id = uri.Segments[2];

                    switch (action)
                    {
                        case "oglas":
                            if (!string.IsNullOrEmpty(id))
                            {
                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    var current = Connectivity.NetworkAccess;
                                    while (current != NetworkAccess.Internet)
                                    {
                                        await Application.Current.MainPage.DisplayAlert("Upozorenje", "Nema internet veze.", "OK");
                                        current = Connectivity.NetworkAccess;
                                    }
                                    DataStoreAdvertisment = DependencyService.Get<IDataStore<Advertisement>>() ?? new AdvertismentDataStorage();
                                    var ad = await DataStoreAdvertisment.GetItemAsync(id);
                                    if (!string.IsNullOrEmpty(Preferences.Get("FirebaseUser", "")))
                                    {
                                        var user = JsonConvert.DeserializeObject<FirebaseUser>(Preferences.Get("FirebaseUser", ""));
                                        App.Current.MainPage = new NavigationPage(new AdvertismentPage(ad));
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.DisplayAlert("Info", "Morate se prvo prijaviti.", "OK");
                                        App.Current.MainPage = new NavigationPage(new UserPage());
                                    }
                                });
                            }
                            break;
                        default:
                            Xamarin.Forms.Device.OpenUri(uri);
                            break;
                    }
                }
            }
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
