using AutoMat.Core.Models;
using AutoMat.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class FavoritesViewModel : BaseViewModel<Advertisement>
    {
        public ObservableCollection<Advertisement> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public object Debug { get; internal set; }

        public FavoritesViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Advertisement>();
        }


    }
}
