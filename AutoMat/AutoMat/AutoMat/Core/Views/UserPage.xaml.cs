using AutoMat.Core.AuthHelpers;
using AutoMat.Core.Constants;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {

        Account account;
        AccountStore store;
        ViewModels.User user = null;

        public UserPage()
        {
            InitializeComponent();
            store = AccountStore.Create();
            registerline.Opacity = 0;
        }

       



        private async void loginButton_Clicked(object sender, EventArgs e)
        {
            registrationPageTab.IsVisible = false;
            loginPageTab.IsVisible = true;
            lines.ForceLayout();
            loginPageTab.ForceLayout();
            await Task.WhenAny<bool>
            (
                loginline.FadeTo(1, 1000),
                registerline.FadeTo(0, 1000)
            );
            
        }


        private async void registrationButton_Clicked(object sender, EventArgs e)
        {

            loginPageTab.IsVisible = false;
            registrationPageTab.IsVisible = true;
            lines.ForceLayout();
            registrationPageTab.ForceLayout();
            await Task.WhenAny<bool>
            (
                loginline.FadeTo(0, 1000),
                registerline.FadeTo(1, 1000)
            );
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
                    App.Current.MainPage = new NavigationPage(new MainPage(user));
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
                    App.Current.MainPage = new NavigationPage(new MainPage());
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
                    var serializedUser = JsonConvert.SerializeObject(user);
                    Preferences.Set("CurrentUser", serializedUser);
                    await App.Current.MainPage.DisplayAlert("Successfull", $"Pozdrav {user.DisplayName}, uspješno ste izvršili prijavu!", "OK");
                    await Navigation.PushModalAsync(new MainPage());
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", $"Invalid useremail or password: {ex.Message}", "OK");
            }
        }

        async private void GetProfileInformationAndRefreshToken()
        {
            var authProvider = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
            try
            {
                string authLink = Preferences.Get("MyFirebaseRefreshToken", "");
                var savedfirebaseauth = JsonConvert.DeserializeObject<FirebaseAuthLink>(authLink);
                var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                var firebase = new FirebaseClient("https://automat-29cec.firebaseio.com/");
                var users = await firebase
                      .Child("users")
                      .OnceAsync<JObject>();

                string bla = "";
                foreach (var item in users)
                {
                    bla += item.Object.GetValue("Ime");
                }

                App.Current.MainPage.DisplayAlert("Alert", bla, "OK");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", $"Token expired {ex.Message}", "OK");
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
                    redirectUri = AppConstants.AndroidRedirectUrl;
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

                    App.Current.MainPage = new NavigationPage(new UserPage());
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

        async void signupbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(UserNewEmail.Text, UserNewPassword.Text, UserName.Text, true);
                var user = await authProvider.GetUserAsync(auth.FirebaseToken);
                if (!user.IsEmailVerified)
                {
                    await App.Current.MainPage.DisplayAlert("Mail", "Potvrdite mail adresu kako bi nastavili koristiti aplikaciju.", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Bravo", "Uspješno ste se registrirali, prijavite se kako bi nastavili koristiti aplikaciju.", "Ok");
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.EmailExists))
                {
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Račun sa ovom mail adresom već postoji. Pokušajte ponovno", "OK");
                }
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.InvalidEmailAddress))
                {
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Email adresa nije valjana. Pokušajte ponovno ili upišite neku drugu email adresu.", "OK");
                }
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.WeakPassword))
                {
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Lozinka mora biti dugačka barem 7 znakova, mora se sastojati od kojih barem jedan mora biti znak i barem jedan broj.", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                }
            }

        }
    }
}