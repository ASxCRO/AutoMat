using AutoMat.Core.AuthHelpers;
using AutoMat.Core.Constants;
using AutoMat.Core.Models;
using AutoMat.Core.Services;
using AutoMat.Core.ViewModels;
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
using XF.Material.Forms.UI.Dialogs;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPage : ContentPage
    {
        public Account account { get; set; }
        public AccountStore store { get; set; }
        public UserViewModel user { get; set; }
        public IDataStore<FirebaseUser> UserDataStore { get; set; }

        public UserPage()
        {
            InitializeComponent();
            store = AccountStore.Create();
            UserDataStore = DependencyService.Get<IDataStore<FirebaseUser>>() ?? new UserDataStore();
        }

        protected override void OnAppearing()
        {
            this.IsVisible = false;
            base.OnAppearing();
            registerline.Opacity = 0;

            if (!string.IsNullOrEmpty(Preferences.Get("FirebaseUser", "")))
            {
                var user = JsonConvert.DeserializeObject<FirebaseUser>(Preferences.Get("FirebaseUser", ""));
                App.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
                this.IsVisible = true;
        }

        async void LoginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserLoginEmail.Text, UserLoginPassword.Text);
                var user = await authProvider.GetUserAsync(auth.FirebaseToken);
                if (!user.IsEmailVerified)
                    await App.Current.MainPage.DisplayAlert("Mail", "Potvrdite mail adresu kako bi nastavili koristiti aplikaciju.", "Ok");
                else
                {
                    var users = await  UserDataStore.GetItemsAsync(false);
                    var firebaseUser = users.Where(u => u.Username == user.DisplayName).FirstOrDefault();

                    if(firebaseUser != null)
                    {
                        var serializedUser = JsonConvert.SerializeObject(firebaseUser);
                        Preferences.Set("FirebaseUser", serializedUser);
                        await App.Current.MainPage.DisplayAlert("Successfull", $"Pozdrav {user.DisplayName}, uspješno ste izvršili prijavu!", "OK");
                        await Navigation.PushAsync(new MainPage());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Pogreška", $"Korisnik ne postoji.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Upozorenje", "Neispravni podatci za prijavu.", "OK");
            }
        }

        void OAuthLogin_Clicked(System.Object sender, System.EventArgs e)
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
            FirebaseUser firebaseUser = new FirebaseUser();

            bool isSuccessful = false;
            using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Dohvaćam podatke.."))
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
                        user = JsonConvert.DeserializeObject<UserViewModel>(userJson);

                        var allFbUsers = await UserDataStore.GetItemsAsync(false);
                        var fbUser = allFbUsers.Where(f => f.Email == user.Email).FirstOrDefault();
                        firebaseUser = fbUser;


                        if (firebaseUser != null)
                        {
                            isSuccessful = true;
                        }
                        else
                        {
                            firebaseUser = new FirebaseUser
                            {
                                Email = user.Email,
                                Username = "",
                                Year = DateTime.Now.Year.ToString(),
                                FirstName = "",
                                LastName = "",
                                PicturePath = user.Picture,
                                PhoneNumber = ""
                            };

                            isSuccessful = await UserDataStore.AddItemAsync(firebaseUser);
                        }
                    }
                }
                if (isSuccessful)
                {
                    var serializedUser = JsonConvert.SerializeObject(firebaseUser);
                    Preferences.Set("FirebaseUser", serializedUser);
                    await App.Current.MainPage.DisplayAlert("Uspješna prijava", $"Pozdrav {firebaseUser.Email}, upješno ste se prijavili u aplikaciju!", "OK");
                    if (String.IsNullOrEmpty(firebaseUser.Username) || String.IsNullOrEmpty(firebaseUser.FirstName) || String.IsNullOrEmpty(firebaseUser.LastName) || String.IsNullOrEmpty(firebaseUser.PhoneNumber))
                        App.Current.MainPage = new NavigationPage(new UpdateProfilePage(true));
                    else
                        App.Current.MainPage = new NavigationPage(new MainPage());

                }
                else
                    await App.Current.MainPage.DisplayAlert("Greška", "Pokušajte ponovno ili se obratite korisničkoj podršci", "Ok");
            }
        }
        

        async void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            await App.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
        }

        async void RegistrationButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool isSuccessful = false;
                FirebaseUser firebaseUser = new FirebaseUser();
                User user = new User();

                using (await MaterialDialog.Instance.LoadingDialogAsync(message: "Dohvaćam podatke.."))
                {
                    var users = await UserDataStore.GetItemsAsync(false);
                    var username_alreadyexists = users.Where(u => u.Username == newuser_username.Text).FirstOrDefault();
                    var email_alreadyexists = users.Where(u => u.Email == newuser_email.Text).FirstOrDefault();

                    if (username_alreadyexists != null)
                    {
                        throw new FirebaseAuthException(null, null, null, null, reason: AuthErrorReason.UserNotFound);
                    }

                    if (email_alreadyexists != null)
                    {
                        throw new FirebaseAuthException(null, null, null, null, reason: AuthErrorReason.EmailExists);
                    }


                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg"));
                    var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(newuser_email.Text, newuser_password.Text, newuser_username.Text, true);
                    user = await authProvider.GetUserAsync(auth.FirebaseToken);
                }

                if (!user.IsEmailVerified)
                {
                    var allFbUsers = await UserDataStore.GetItemsAsync(false);
                    var fbUser = allFbUsers.Where(f => f.Email == user.Email).FirstOrDefault();
                    firebaseUser = fbUser;
                    if (firebaseUser != null)
                    {
                        isSuccessful = true;
                    }
                    else
                    {
                        firebaseUser = new FirebaseUser
                        {
                            Email = newuser_email.Text,
                            Username = newuser_username.Text,
                            Year = DateTime.Now.Year.ToString(),
                            FirstName = newuser_firstname.Text,
                            LastName = newuser_lastname.Text,
                            PicturePath = "https://png.pngtree.com/png-vector/20190710/ourmid/pngtree-user-vector-avatar-png-image_1541962.jpg",
                            PhoneNumber = newuser_phonenumber.Text
                        };

                        isSuccessful = await UserDataStore.AddItemAsync(firebaseUser);
                    }
                    if (isSuccessful)
                    {
                        await App.Current.MainPage.DisplayAlert("Uspješna registracija", "Potvrdite mail adresu kako bi nastavili koristiti aplikaciju.", "Ok");
                        App.Current.MainPage = new NavigationPage(new UserPage());
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Greška", "Pokušajte ponovno ili se obratite korisničkoj podršci", "Ok");
                    }
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.EmailExists))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Račun sa ovom mail adresom već postoji. Pokušajte ponovno ili se prijavite sa google-om.", "OK");
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.InvalidEmailAddress))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Email adresa nije valjana. Pokušajte ponovno ili upišite neku drugu email adresu.", "OK");
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.WeakPassword))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Lozinka mora biti dugačka barem 7 znakova, mora se sastojati od kojih barem jedan mora biti znak i barem jedan broj.", "OK");
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.UserNotFound))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Već postoji osoba s tim korisničkim imenom!", "OK");
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.InvalidEmailAddress))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Neispravna mail adresa!", "OK");
                else if (ex.Reason.Equals(Firebase.Auth.AuthErrorReason.WrongPassword))
                    await App.Current.MainPage.DisplayAlert("Upozorenje", "Neispravna lozinka!", "OK");
                else
                    await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }

        }

        private async void gotoLoginTab_Clicked(object sender, EventArgs e)
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

        private async void gotoRegistrationTab_Clicked(object sender, EventArgs e)
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
    }
}