using System;
using System.Runtime.CompilerServices;
using Firebase.Rest.Auth.Payloads;
using Newtonsoft.Json;
using Questonaut.DependencyServices;
using Questonaut.Model;
using Questonaut.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Questonaut.Controller
{
    public class CurrentUser
    {
        #region instance
        private static CurrentUser _instance;
        private QUser _user;
        #endregion

        #region constructor
        private CurrentUser() { }
        #endregion

        #region properties
        /// <summary>
        /// The singleton instance of this controller.
        /// </summary>
        public static CurrentUser Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrentUser();

                    //save the use to the user controller to access the data in the rest of the app.
                    User userdata = JsonConvert.DeserializeObject<User>(SettingsImp.UserValue);
                    CurrentUser.Instance.User = new QUser() { Name = userdata.email };

                    try
                    {
                        //authenticate the user
                        var result = DependencyService.Get<IFirebaseAuthenticator>().GetCurrentUser();

                    }
                    catch (Exception e)
                    {
                        var test = e.Message;
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// The current loggedin user.
        /// </summary>
        public QUser User
        {
            get => _user;
            set => _user = value;
        }
        #endregion
    }
}
