using System;
namespace Questonaut.Configuration.Types
{
    public interface IFirebaseAuthConfig
    {
        /// <summary>
        /// Returns the WebAPI firebase key.
        /// </summary>
        /// <returns>WebAPI as string</returns>
        string GetWebAPI();
    }

    public class FirebaseAuthConfig : IFirebaseAuthConfig
    {
        private readonly FirebaseAuthSettings _firebaseAuthSettings;

        public FirebaseAuthConfig(FirebaseAuthSettings firebaseAuthSettings)
        {
            _firebaseAuthSettings = firebaseAuthSettings;
        }

        /// <summary>
        /// Returns the WebAPI firebase key.
        /// </summary>
        /// <returns>WebAPI as string</returns>
        public string GetWebAPI()
        {
            return _firebaseAuthSettings.WebAPI;
        }
    }
}
