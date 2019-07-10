using System.Threading.Tasks;
using Firebase.Auth;
using Questonaut.DependencyServices;
using Questonaut.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthenticator))]
namespace Questonaut.Droid.DependencyServices
{
    public class FirebaseAuthenticator : IFirebaseAuthenticator
    {
        public Task<string> GetCurrentUser()
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            var user = await FirebaseAuth.Instance
                .SignInWithEmailAndPasswordAsync(email, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }
    }
}
