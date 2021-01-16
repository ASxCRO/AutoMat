using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.GoogleClient;
using AutoMat.Droid.Services;
using Xamarin.Auth;
using XF.Material.Droid;
using FFImageLoading.Forms.Platform;
using Xamd.ImageCarousel.Forms.Plugin.Droid;

namespace AutoMat.Droid
{
    [Activity(Label = "AutoMat", Theme = "@style/MainTheme", Icon = "@mipmap/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ImageCarouselRenderer.Init();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XF.Material.Droid.Material.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this,savedInstanceState);
            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            CustomTabsConfiguration.CustomTabsClosingMessage = null;

            LoadApplication(new App());



        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
}