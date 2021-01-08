using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using Firebase.Database;
using Java.Util.Prefs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public static FirebaseClient NewInstance
        {
            get
            {
                instance = new FirebaseClient(AppConstants.FirebaseDatabaseURL);
                return instance;
            }
        }
    }
}

