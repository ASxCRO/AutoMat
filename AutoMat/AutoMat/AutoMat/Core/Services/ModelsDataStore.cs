using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(ModelsDataStore))]
namespace AutoMat.Core.Services
{
    class ModelsDataStore : IDataStore<CarModel>
    {
        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;
        public ModelsDataStore()
        {
        }

        public Task<bool> AddItemAsync(CarModel item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CarModel> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CarModel>> GetItemsAsync(bool forceReload)
        {
            return (await firebase
                      .Child("carModels")
                      .OnceAsync<CarModel>()).Select(item => new CarModel
                      {
                          Id = item.Object.Id,
                          Title = item.Object.Title,
                          BrandId = item.Object.BrandId
                      }).ToList();
        }

        public Task<Dictionary<string, CarModel>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(CarModel item)
        {
            throw new NotImplementedException();
        }
    }
}
