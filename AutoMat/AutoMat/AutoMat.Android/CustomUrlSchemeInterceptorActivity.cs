using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AutoMat.Core.AuthHelpers;
using AutoMat.Core.Constants;
using System;

namespace AutoMat.Droid
{
	[Activity(Label = "CustomUrlSchemeInterceptorActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
	[IntentFilter(
	new[] { Intent.ActionView },
	Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
	DataSchemes = new[] { "com.googleusercontent.apps.915728342891-8mfpa7hmqhtfeugs61d6e9t1jhi4pt6m" },
	DataPath = "/oauth2redirect")]
	public class CustomUrlSchemeInterceptorActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Convert Android.Net.Url to Uri
			var uri = new Uri(Intent.Data.ToString());

			// Load redirectUrl page
			AuthenticationState.Authenticator.OnPageLoading(uri);

			//SetResult(Result.FirstUser);
			Finish();

		}


	}
}