using AutoMat.Core.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AutoMat.Core.Services.AdvertismentDataStorage))]
namespace AutoMat.Core.Services
{
    public class AdvertismentDataStorage : IDataStore<Advertisement>
    {
        private HttpClient firebaseHttpClient { get; } = FirebaseDatabaseHttpClient.Instance;
        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;

        public AdvertismentDataStorage()
        {

        }

        public async Task<bool> AddItemAsync(Advertisement item)
        {
            try
            {
                await firebase
                  .Child("advertisments")
                  .PostAsync(item, true);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var allAdvertisments = await GetItemsKeyValueAsync();
            var toDeleteAdvertisment = allAdvertisments.Where(a => a.Value.Id == id).FirstOrDefault().Value;

            try
            {
                await firebase.Child("advertisments").Child(toDeleteAdvertisment.Id).DeleteAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<Advertisement> GetItemAsync(string id)
        {
            var allAdvertisments = await GetItemsKeyValueAsync();
            return allAdvertisments.Where(a => a.Value.Id == id).FirstOrDefault().Value;
        }

        public async Task<Dictionary<string,Advertisement>> GetItemsKeyValueAsync()
        {
            HttpResponseMessage response = await firebaseHttpClient.GetAsync("advertisments/.json");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string,Advertisement>>(responseBody);
        }

        public async Task<bool> UpdateItemAsync(Dictionary<string,Advertisement> item)
        {
            var allAdvertisments = await GetItemsKeyValueAsync();
            var toUpdateAdvertisment = allAdvertisments.Where(a => a.Key == item.FirstOrDefault().Key).FirstOrDefault().Value;

            try
            {
                await firebase
                      .Child("advertisments")
                      .Child(item.Keys.FirstOrDefault())
                      .PutAsync(item);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public Task<IEnumerable<Advertisement>> GetItemsAsync(bool forceReload)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Advertisement item)
        {
            throw new NotImplementedException();
        }
    }
}
