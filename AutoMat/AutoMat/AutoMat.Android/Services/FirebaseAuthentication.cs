using System;
using System.Threading.Tasks;
using AutoMat.Core.Services;
using Xamarin.Forms;


[assembly: Dependency(typeof(AutoMat.Droid.Services.FirebaseAuthentication))]
namespace AutoMat.Droid.Services
{
    public class FirebaseAuthentication : IFirebaseAuthentication
    {
        public bool IsSignIn()
        {
            throw new NotImplementedException();
        }

        public Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool SignOut()
        {
            throw new NotImplementedException();
        }
    }
}