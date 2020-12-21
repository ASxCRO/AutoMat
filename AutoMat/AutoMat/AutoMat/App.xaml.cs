using AutoMat.Core.Services;
using AutoMat.Core.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new NavigationPage(new UserPage());
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
