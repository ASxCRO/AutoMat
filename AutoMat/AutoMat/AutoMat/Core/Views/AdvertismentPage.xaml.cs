using AutoMat.Core.Models;
using AutoMat.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvertismentPage : ContentPage
    {
        public AdvertismentPage(Advertisement ad)
        {
            InitializeComponent();
         
            BindingContext = AdViewModel = new AdvertismentViewModel { Advertisment = ad, CurrentImageIndex = 0};
        }

        protected async override void OnAppearing()
        {
            foreach (var imageUrl in AdViewModel.Advertisment.PicturesURL)
            {
                AdViewModel.ImageSources.Add(ImageSource.FromUri(new Uri(imageUrl)));
            }
        }

        public AdvertismentViewModel AdViewModel { get; set; }


    }
}