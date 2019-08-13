using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Microsoft.AppCenter.Crashes;
using Questonaut.DependencyServices;
using Questonaut.iOS.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace Questonaut.iOS.DependencyServices
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public async Task<string> GetCurrentUser()
        {
            return await Auth.DefaultInstance.CurrentUser.GetIdTokenAsync();
        }

        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                return await user.User.GetIdTokenAsync();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return "";
            }
        }
    }
}
