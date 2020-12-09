using AutoMat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class AdvertismentsViewModel : BaseViewModel<Advertisement>
    {
        public ObservableCollection<Advertisement> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public AdvertismentsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Advertisement>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsKeyValueAsync();
                foreach (var item in items)
                {
                    Items.Add(item.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}