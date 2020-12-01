using AutoMat.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Auth;
using AutoMat.Core.AuthHelpers;
using AutoMat.Core.Constants;
using System.Diagnostics;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public string WebAPIkey = "AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg";
		Account account;
		AccountStore store;

        public object AppConstant { get; private set; }

        public LoginPage()
        {
            InitializeComponent();
			store = AccountStore.Create();

		}

		async void loginbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserLoginEmail.Text, UserLoginPassword.Text);
                var user = await authProvider.GetUserAsync(auth.FirebaseToken);
                if (!user.IsEmailVerified)
                {
                    await App.Current.MainPage.DisplayAlert("Mail", "Potvrdite mail adresu kako bi nastavili koristiti aplikaciju.", "Ok");
                }
                else
                {
                    var content = await auth.GetFreshAuthAsync();
                    var serializedcontnet = JsonConvert.SerializeObject(content);
                    Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);
                   // await Navigation.PushAsync(new MyDashboardPage(user));
                    await App.Current.MainPage.DisplayAlert("Successfull", $"Hi {user.DisplayName}, you have successfully logged into the App!", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Invalid useremail or password", "OK");
            }
        }

        async void loginbuttonOAuth_Clicked(System.Object sender, System.EventArgs e)
        {
			string clientId = null;
			string redirectUri = null;

			switch (Device.RuntimePlatform)
			{
				case Device.Android:
					clientId = AppConstants.AndroidClientId;
					redirectUri =AppConstants.AndroidRedirectUrl;
					break;
			}

			account = store.FindAccountsForService(AppConstants.AppName).FirstOrDefault();

			var authenticator = new OAuth2Authenticator(
				clientId,
				null,
				AppConstants.Scope,
				new Uri(AppConstants.AuthorizeUrl),
				new Uri(redirectUri),
				new Uri(AppConstants.AccessTokenUrl),
				null,
				true);

			authenticator.Completed += OnAuthCompleted;
			authenticator.Error += OnAuthError;

			AuthenticationState.Authenticator = authenticator;

			var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			presenter.Login(authenticator);
		}

		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			AuthHelpers.User user = null;
			if (e.IsAuthenticated)
			{
				// If the user is authenticated, request their basic user data from Google
				// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
				var request = new OAuth2Request("GET", new Uri(AppConstants.UserInfoUrl), null, e.Account);
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					// Deserialize the data and store it in the account store
					// The users email address will be used to identify data in SimpleDB
					string userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<AuthHelpers.User>(userJson);
				}

				if (user != null)
				{
					App.Current.MainPage = new NavigationPage(new MyDashboardPage(user));
				}

				//await store.SaveAsync(account = e.Account, AppConstant.AppConstants.AppName);
				//await DisplayAlert("Email address", user.Email, "OK");
			}
		}


		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			var authenticator = sender as OAuth2Authenticator;
			if (authenticator != null)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			Debug.WriteLine("Authentication error: " + e.Message);
		}

	}
}