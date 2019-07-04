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
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            var user = await FirebaseAuth.Instance
                .SignInWithEmailAndPasswordAsync(email, password);
            var token = await user.User.GetIdTokenAsync(false);
            return token.Token;
        }

        public async Task<string> RegsiterWithEmailPassword(string email, string password)
        {
            var user = await FirebaseAuth.Instance
                .CreateUserWithEmailAndPasswordAsync(email, password);
            var token = await user.User.GetTokenAsync(false);
            return token.Token;
        }
    }
}
