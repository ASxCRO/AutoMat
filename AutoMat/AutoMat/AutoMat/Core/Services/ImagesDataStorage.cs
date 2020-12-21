using AutoMat.Core.Models;
using Firebase.Database;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AutoMat.Core.Services.ImagesDataStorage))]
namespace AutoMat.Core.Services
{
    public class ImagesDataStorage : IDataStore<ImageModel>
    {
        private FirebaseStorage firebaseStorage { get; } = FirebaseStorageClient.Instance;

        private FirebaseClient firebaseDataBase { get; } = FirebaseDatabaseClient.Instance;

        public Task<bool> AddItemAsync(ImageModel item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ImageModel> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ImageModel>> GetItemsAsync(bool forceReload)
        {
           var adImages =  (await firebaseDataBase
                  .Child("advertismentImages")
                  .OnceAsync<AdvertismentImage>())
                  .Select(item => new AdvertismentImage
                  {
                      Url = item.Object.Url,
                      AdvertismentId = item.Object.AdvertismentId
                  })
                  .Where(a => a.AdvertismentId == "1")
                  .ToList();

            List<ImageModel> imageModels = new List<ImageModel>();
            string source = "";

            foreach (var item in adImages)
            {
                try
                {
                    source = await firebaseStorage
                        .Child("data")
                        .Child("sample.png")
                        .GetDownloadUrlAsync();
                }
                catch (Exception e)
                {
                    throw;
                }

                var image = new ImageModel { Source = source };
                imageModels.Add(image);

            }

            return imageModels;
        }


            public Task<Dictionary<string, ImageModel>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(ImageModel item)
        {
            throw new NotImplementedException();
        }
    }
}
