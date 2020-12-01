using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AutoMat.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public string WebAPIkey = "AIzaSyAg4riVkvSMtWwKZ6_UssK28-2K6xOndrg";

        public RegistrationPage()
        {
            InitializeComponent();
        }

        async void signupbutton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
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