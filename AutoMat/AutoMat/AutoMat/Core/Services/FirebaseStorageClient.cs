using AutoMat.Core.Constants;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Services
{
    public sealed class FirebaseStorageClient
    {
        private static FirebaseStorage instance = null;
        private static readonly object padlock = new object();

        FirebaseStorageClient()
        {

        }

        public static FirebaseStorage Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new FirebaseStorage(AppConstants.FirebaseStorageURL);
                    }
                    return instance;
                }
            }
        }
    }
}
