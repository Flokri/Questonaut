using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akavache;
using Firebase.Rest.Auth.Payloads;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Navigation;
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

        #region public methods
        /// <summary>
        /// Load all studies for the current user.
        /// </summary>
        /// <returns>Returns a task as this is a async method.</returns>
        public async Task LoadNewStudies()
        {
            await LoadStudies();
        }

        /// <summary>
        /// Load all user data for the current user.
        /// </summary>
        /// <returns>Returns a task as this is a async method.</returns>
        public async Task LoadUser()
        {
            await LoadUserData();
        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <returns>Returns a bool representating if the logout was successfull.</returns>
        public bool LogoutUser()
        {
            return Logout();
        }
        #endregion

        #region private methods
        /// <summary>
        /// Reset the current singleton instance and delte the in-memory user data.
        /// </summary>
        /// <returns>Returns a bool reprsentating if the logout and resetting was successfull.</returns>
        private bool Logout()
        {
            try
            {
                BlobCache.UserAccount.InvalidateAll().Subscribe();

                if (SettingsImp.UserValue != string.Empty)
                {
                    SettingsImp.UserValue = "";
                }

                _instance = null;

                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        /// <summary>
        /// Load all studies the current user is participating at.
        /// </summary>
        /// <returns>Returns a task as this is a async method.</returns>
        private async Task LoadStudies()
        {
            try
            {
                var documents = await CrossCloudFirestore.Current
                                        .Instance
                                        .GetCollection(QUser.CollectionPath)
                                        .WhereEqualsTo("Email", this._user.Email)
                                        .GetDocumentsAsync();

                IEnumerable<QUser> myModel = documents.ToObjects<QUser>();
                _user.ActiveStudies = myModel.First().ActiveStudies;
                _user.ActiveStudiesObjects.Clear();

                foreach (var study in _user.ActiveStudies)
                {
                    var studyDoc = await CrossCloudFirestore.Current
                                         .Instance
                                         .GetCollection(QStudy.CollectionPath)
                                         .GetDocument(study)
                                         .GetDocumentAsync();
                    QStudy temp = studyDoc.ToObject<QStudy>();

                    if (temp.Team.Equals("System") && temp.Title.Equals("Add"))
                    {
                        temp.Command = new DelegateCommand(() =>
                        {
                            (Xamarin.Forms.Application.Current as App).GetNavigationService().NavigateAsync("FindAllStudiesView");
                        });
                    }
                    else
                    {
                        temp.Command = new DelegateCommand(() =>
                        {
                            //implement the navigation to the detail view
                        });
                    }

                    _user.ActiveStudiesObjects.Add(temp);
                }

                await BlobCache.UserAccount.InsertObject("user", CurrentUser.Instance.User);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        /// <summary>
        /// Load all user relevant infos.
        /// </summary>
        /// <returns>Returns a task as this is a async method.</returns>
        private async Task LoadUserData()
        {
            try
            {
                var documents = await CrossCloudFirestore.Current
                                        .Instance
                                        .GetCollection(QUser.CollectionPath)
                                        .WhereEqualsTo("Email", this._user.Email)
                                        .GetDocumentsAsync();

                IEnumerable<QUser> myModel = documents.ToObjects<QUser>();
                _user = myModel.First();
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
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
                    CurrentUser.Instance.User = new QUser() { Email = userdata.email };

                    try
                    {
                        //authenticate the user
                        var result = DependencyService.Get<IFirebaseAuthenticator>().GetCurrentUser();

                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
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
