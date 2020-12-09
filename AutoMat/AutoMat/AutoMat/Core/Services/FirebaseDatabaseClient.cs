using AutoMat.Core.Constants;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Services
{
    public sealed class FirebaseDatabaseClient
    {
        private static FirebaseClient instance = null;
        private static readonly object padlock = new object();

        FirebaseDatabaseClient()
        {

        }

        public static FirebaseClient Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new FirebaseClient(AppConstants.FirebaseDatabaseURL);
                    }
                    return instance;
                }
            }
        }
    }
}

