using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(TownsDataStore))]
namespace AutoMat.Core.Services
{
    public class TownsDataStore : IDataStore<Town>
    {
        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;
        public TownsDataStore()
        {
        }

        public Task<bool> AddItemAsync(Town item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Town> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Town>> GetItemsAsync(bool forceReload)
        {
            return (await firebase
                      .Child("towns")
                      .OnceAsync<Town>()).Select(item => new Town
                      {
                          Id = item.Object.Id,
                          CountyId = item.Object.CountyId,
                          CountyName = item.Object.CountyName,
                          Name = item.Object.Name,
                          EntityType = item.Object.EntityType
                      }).ToList();
        }

        public Task<Dictionary<string, Town>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Town item)
        {
            throw new NotImplementedException();
        }
    }
}
