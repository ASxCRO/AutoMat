using AutoMat.Core.Models;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AutoMat.Core.Services.AdditionalEquipmentDataStore))]
namespace AutoMat.Core.Services
{
    public class AdditionalEquipmentDataStore : IDataStore<AdditionalEquipment>
    {
        private HttpClient firebase { get; } = FirebaseDatabaseHttpClient.Instance;

        public AdditionalEquipmentDataStore()
        {
        }
        public Task<bool> AddItemAsync(AdditionalEquipment item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<AdditionalEquipment> GetItemAsync(string id)
        {
            HttpResponseMessage response = await firebase.GetAsync("oglasiDodatnaOprema/.json");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AdditionalEquipment>(responseBody);
        }

        public async Task<IEnumerable<AdditionalEquipment>> GetItemsAsync(bool forceReload)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(AdditionalEquipment item)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, AdditionalEquipment>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }
    }
}
