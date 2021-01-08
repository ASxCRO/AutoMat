using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(FavoritesDataStore))]
namespace AutoMat.Core.Services
{
    public class FavoritesDataStore : IDataStore<UserFavoriteAd>
    {

        public FavoritesDataStore()
        {

        }
        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;
        public async Task<bool> AddItemAsync(UserFavoriteAd item)
        {
            try
            {
                await firebase
                   .Child("userOglasFavorit")
                   .PostAsync(item);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var toDeleteFavorite = (await firebase
                          .Child("userOglasFavorit")
                          .OnceAsync<UserFavoriteAd>()).Where(a => a.Object.AdId == id).FirstOrDefault();

            try
            {
                await firebase.Child("userOglasFavorit").Child(toDeleteFavorite.Key).DeleteAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Task<UserFavoriteAd> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserFavoriteAd>> GetItemsAsync(bool forceReload)
        {
            return (await firebase
              .Child("userOglasFavorit")
              .OnceAsync<UserFavoriteAd>()).Select(item => new UserFavoriteAd
              {
                  AdId = item.Object.AdId,
                  Username = item.Object.Username
              }).ToList();
        }

        public Task<Dictionary<string, UserFavoriteAd>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(UserFavoriteAd item)
        {
            throw new NotImplementedException();
        }
    }
}
