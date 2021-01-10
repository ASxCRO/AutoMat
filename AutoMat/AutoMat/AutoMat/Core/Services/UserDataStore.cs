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
            firebase = FirebaseDatabaseClient.Instance;
        }

        private FirebaseClient firebase { get; }

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

        public async Task<bool> DeleteItemAsync(string username)
        {
            var toDeletePerson = (await firebase
                                      .Child("users")
                                      .OnceAsync<FirebaseUser>()).Where(a => a.Object.Username == username).FirstOrDefault();

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
            return (await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>()).Where(a => a.Object.Username == id).FirstOrDefault().Object;
        }

        public async Task<IEnumerable<FirebaseUser>> GetItemsAsync(bool forceReload)
        {
            var allUsers = (await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>()).Select(item => new FirebaseUser
              {
                  Username = item.Object.Username,
                  Email = item.Object.Email,
                  FirstName = item.Object.FirstName,
                  LastName = item.Object.LastName,
                  Year = item.Object.Year,
                  PicturePath = item.Object.PicturePath,
                  PhoneNumber = item.Object.PhoneNumber
              }).ToList();
            return allUsers;
        }

        public Task<Dictionary<string, FirebaseUser>> GetItemsKeyValueAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateItemAsync(FirebaseUser item)
        {
            var toUpdateUser = (await firebase
              .Child("users")
              .OnceAsync<FirebaseUser>()).Where(a => a.Object.Email == item.Email).FirstOrDefault();

            try
            {
                await firebase
                      .Child("users")
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
