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
		Account account;
		AccountStore store;
		ViewModels.User user = null;

		public object AppConstant { get; private set; }

        public LoginPage()
        {
            InitializeComponent();
			store = AccountStore.Create();
		}

		protected async override void OnAppearing()
        {
			this.IsVisible = false;

			base.OnAppearing();
			if (!string.IsNullOrEmpty(Preferences.Get("UserToken", "")))
			{
				var request = new OAuth2Request("GET", new Uri(AppConstants.UserInfoUrl), null, Account.Deserialize(Preferences.Get("UserToken", "")));
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					string userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<ViewModels.User>(userJson);
				}
				if (user != null)
				{
					App.Current.MainPage = new NavigationPage(new MyDashboardPage(user));
				}
				else
				{
					this.IsVisible = true;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(Preferences.Get("MyFirebaseRefreshToken", "")))
				{
					App.Current.MainPage = new NavigationPage(new MyDashboardPage());
				}
				else
				{
					this.IsVisible = true;
				}
			}
		}

		async void loginbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
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
					await App.Current.MainPage.DisplayAlert("Successfull", $"Hi {user.DisplayName}, you have successfully logged into the App!", "OK");
					await Navigation.PushModalAsync(new MyDashboardPage());
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", $"Invalid useremail or password: {ex.Message}", "OK");
            }
        }

        void loginbuttonOAuth_Clicked(System.Object sender, System.EventArgs e)
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

			if (e.IsAuthenticated)
			{
				var request = new OAuth2Request("GET", new Uri(AppConstants.UserInfoUrl), null, e.Account);
				var response = await request.GetResponseAsync();
				if (response != null)
				{
					string userJson = await response.GetResponseTextAsync();
					user = JsonConvert.DeserializeObject<ViewModels.User>(userJson);
					Preferences.Set("UserToken", e.Account.Serialize());
				}
				if (user != null)
				{
					await App.Current.MainPage.DisplayAlert("Successfull", $"Hi {user.Email}, you have successfully logged into the App!", "OK");

					App.Current.MainPage = new NavigationPage(new MyDashboardPage(user));
				}
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