using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AutoMat.Core.Services;
using AutoMat.Droid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ShareIntents))]
namespace AutoMat.Droid.Services
{
    public class ShareIntents : IShareable
    {
        public void OpenShareIntent(string textToShare)
        {
            var myIntent = new Intent(Android.Content.Intent.ActionSend);
            myIntent.SetType("text/plain");
            myIntent.PutExtra(Intent.ExtraText, textToShare);
            Forms.Context.StartActivity(Intent.CreateChooser(myIntent, "Odaberite aplikaciju"));
        }
    }
}