using AutoMat.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.ViewModels
{
    public class FirebaseUserViewModel : BaseViewModel<FirebaseUser>
    {
        public FirebaseUserViewModel()
        {
        }

        public FirebaseUser FirebaseUser { get; set; }
        public string FullName
        { 
            get
            {
                if(!String.IsNullOrEmpty(FirebaseUser.FirstName))
                    return $"{FirebaseUser.FirstName} {FirebaseUser.LastName}";
                else
                    return $"Ime ne postoji";
            }
        }

        public int NumberOfUserAds { get; set; }

    }
}
