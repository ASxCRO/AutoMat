using AutoMat.Core.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMat.Core.Services
{
    public class BrandsDataStore : IDataStore<CarBrand>
    {
        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;
        public BrandsDataStore()
        {
        }

        public Task<bool> AddItemAsync(CarBrand item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CarBrand> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CarBrand>> GetItemsAsync(bool forceReload)
        {
            return (await firebase
                      .Child("carBrands")
                      .OnceAsync<CarBrand>()).Select(item => new CarBrand
                      {
                          Id = item.Object.Id,
                          Title = item.Object.Title
                      }).ToList();
        }

        public Task<Dictionary<string, CarBrand>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(CarBrand item)
        {
            throw new NotImplementedException();
        }
    }
}
