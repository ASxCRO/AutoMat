using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public UsersViewModel userViewModel { get; set; }
        public HomePage()
        {
            InitializeComponent();
            BindingContext = userViewModel = new UsersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (userViewModel.Items.Count == 0)
                userViewModel.LoadItemsCommand.Execute(null);
        }

        private async void PressMeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await userViewModel.DataStore.DeleteItemAsync("0");
            }
            catch (Exception)
            {
                await DisplayAlert("Delete", "Desila se greška", "OK");
                throw;
            }

            await DisplayAlert("Delete", "Mario uspješno obrisan, refresham listu.", "OK");

        }
    }
}