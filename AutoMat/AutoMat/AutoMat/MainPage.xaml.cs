using AutoMat.Core.Services;
using AutoMat.Core.Views;

using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AutoMat
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        async void signupPage_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        async void loginPage_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
