using System;
using System.Threading.Tasks;

namespace Questonaut.DependencyServices
{
    public interface IFirebaseAuthenticator
    {
        Task<string> LoginWithEmailPassword(string email, string password);
        Task<string> GetCurrentUser();
    }
}
