using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(UserDataStore))]
namespace AutoMat.Core.Services
{
    public class UserDataStore : IDataStore<FirebaseUser>
    {
        public UserDataStore()
        {
        }

        private FirebaseClient firebase { get; } = FirebaseDatabaseClient.Instance;

        public async Task<bool> AddItemAsync(FirebaseUser item)
        {
            try
            {
                await firebase
                   .Child("users")
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
            var toDeletePerson = (await firebase
                                      .Child("users")
                                      .OnceAsync<FirebaseUser>()).Where(a => a.Object.Id == id).FirstOrDefault();

            try
            {
                await firebase.Child("users").Child(toDeletePerson.Key).DeleteAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<FirebaseUser> GetItemAsync(string id)
        {
            var allPersons = await GetItemsAsync(false);
            await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>();
            return allPersons.Where(a => a.Id == id).FirstOrDefault();
        }

        public async Task<IEnumerable<FirebaseUser>> GetItemsAsync(bool forceReload)
        {
            return (await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>()).Select(item => new FirebaseUser
              {
                  Email = item.Object.Email,
                  Ime = item.Object.Ime,
                  Prezime = item.Object.Prezime
              }).ToList();
        }

        public Task<Dictionary<string, FirebaseUser>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateItemAsync(FirebaseUser item)
        {
            var toUpdateUser = (await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>()).Where(a => a.Object.Id == item.Id).FirstOrDefault();

            try
            {
                await firebase
                      .Child("Persons")
                      .Child(toUpdateUser.Key)
                      .PutAsync(item);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;

        }
    }
}
