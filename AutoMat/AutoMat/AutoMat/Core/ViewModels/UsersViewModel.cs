using AutoMat.Core.Models;
using AutoMat.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class UsersViewModel : BaseViewModel<FirebaseUser>
    {
        public ObservableCollection<FirebaseUser> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public UsersViewModel()
        {
            Title = "Users";
            Items = new ObservableCollection<FirebaseUser>();
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
                var items = await DataStore.GetItemsAsync(true);
                 foreach (var item in items)
                {
                    Items.Add(item);
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
