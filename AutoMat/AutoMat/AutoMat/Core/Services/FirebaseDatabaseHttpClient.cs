using AutoMat.Core.Constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AutoMat.Core.Services
{
    public sealed class FirebaseDatabaseHttpClient
    {
        private static HttpClient instance = null;
        private static readonly object padlock = new object();

        FirebaseDatabaseHttpClient()
        {

        }

        public static HttpClient Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new HttpClient{ BaseAddress = new Uri(AppConstants.FirebaseDatabaseURL)};
                    }
                    return instance;
                }
            }
        }
    }
}
