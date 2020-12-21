using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Firebase.Database;
using Firebase.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AutoMat.Core.ViewModels
{
    public class ImageViewModel
    {

        public IDataStore<ImageModel> ImagesDataStore => DependencyService.Get<IDataStore<ImageModel>>() ?? new ImagesDataStorage();
        public List<ImageModel> AdImages { get; set; } = new List<ImageModel>();

        public ImageViewModel()
        {
        }

        public async Task<List<ImageModel>> GetImagesForAd(string advertismentId)
        {
            AdImages = (List<ImageModel>)await ImagesDataStore.GetItemsAsync(false);
            return AdImages;
        }

        private ObservableCollection<ImageModel> imageNodeInfo;

        public ObservableCollection<ImageModel> ImagesNodeInfo
        {
            get { return imageNodeInfo; }
            set { this.imageNodeInfo = value; }
        }

        public void GenerateSource(string advertismentId)
        {
            List<ImageModel> mylist = GetImagesForAd(advertismentId).Result;
            ImagesNodeInfo = new ObservableCollection<ImageModel>(mylist);
        }
    }

}
