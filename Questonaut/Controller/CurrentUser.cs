using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Akavache;
using Com.OneSignal;
using Firebase.Rest.Auth.Payloads;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Navigation;
using Questonaut.DependencyServices;
using Questonaut.Model;
using Questonaut.Settings;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Questonaut.Controller
{
    public class CurrentUser
    {
        #region instance
        private static CurrentUser _instance;
        private QUser _user;
        private bool _deleteUserData;
        private bool _registeredGeofences = false;
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
        /// Updates the user instance (at firebase).
        /// </summary>
        /// <param name="paramName">The name of the parameter that should be updated.</param>
        /// <param name="update">The value that should be updated.</param>
        public async Task<bool> UpdateUser(string paramName, object update)
        {
            return await Update(paramName, update);
        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <returns>Returns a bool representating if the logout was successfull.</returns>
        public bool LogoutUser()
        {
            return Logout();
        }

        public async void AddLocation(string key, Location loc)
        {
            if (CurrentUser.Instance.User.Locations.ContainsKey(key))
            {
                CurrentUser.Instance.User.Locations[key] = new Plugin.CloudFirestore.GeoPoint(loc.Latitude, loc.Longitude);
            }
            else
            {
                CurrentUser.Instance.User.Locations.Add(key, new Plugin.CloudFirestore.GeoPoint(loc.Latitude, loc.Longitude));
                CurrentUser.Instance.UpdateUser("Locations", CurrentUser.Instance.User.Locations);
            }
            await BlobCache.UserAccount.InsertObject("user", CurrentUser.Instance.User);
        }
        #endregion

        #region private methods
        private async Task<bool> Update(string paramName, object update)
        {
            try
            {
                await CrossCloudFirestore.Current
                                         .Instance
                                         .GetCollection(QUser.CollectionPath)
                                         .GetDocument(CurrentUser.Instance.User.Id)
                                         .UpdateDataAsync(paramName, update);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Reset the current singleton instance and delte the in-memory user data.
        /// </summary>
        /// <returns>Returns a bool reprsentating if the logout and resetting was successfull.</returns>
        private bool Logout()
        {
            try
            {
                try
                {
                    BlobCache.UserAccount.InvalidateAll().Wait();
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }

                if (SettingsImp.UserValue != string.Empty)
                {
                    SettingsImp.UserValue = "";
                }

                _instance._user = null;

                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }

        private async void RegisterGeofences()
        {
            var geofences = ShinyHost.Resolve<IGeofenceManager>();

            if (CurrentUser.Instance.User.ActiveStudiesObjects != null && CurrentUser.Instance.User.ActiveStudiesObjects.Count > 0)
            {
                foreach (QStudy study in CurrentUser.Instance.User.ActiveStudiesObjects)
                {
                    var elementsDoc = await CrossCloudFirestore.Current
                           .Instance
                           .GetCollection(QStudy.CollectionPath + "/" + study.Id + "/" + QElement.CollectionPath)
                           .GetDocumentsAsync();

                    IEnumerable<QElement> elements = elementsDoc.ToObjects<QElement>();

                    // this is really only required on iOS, but do it to be safe
                    if (elements != null)
                    {
                        foreach (QElement element in elements)
                        {
                            var contextDoc = await CrossCloudFirestore.Current
                                .Instance
                                .GetCollection(QContext.CollectionPath)
                                .GetDocument(element.LinkToContext)
                                .GetDocumentAsync();
                            QContext context = contextDoc.ToObject<QContext>();

                            if (context.Location != null && !_registeredGeofences)
                            {
                                await geofences.StartMonitoring(new GeofenceRegion(
                                    context.LocationName + "|" + context.LocationAction,
                                new Position(context.Location.Latitude, context.Location.Longitude),
                                Distance.FromMeters(200))
                                {
                                    NotifyOnEntry = true,
                                    NotifyOnExit = true,
                                    SingleUse = false
                                });
                            }
                        }
                    }
                }
                _registeredGeofences = true;
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
                _user.Studies = myModel.First().Studies;
                _user.ActiveStudiesObjects.Clear();

                bool userChanged = false;
                List<string> toDelete = new List<string>();

                foreach (var study in _user.Studies)
                {
                    var studyDoc = await CrossCloudFirestore.Current
                                         .Instance
                                         .GetCollection(QStudy.CollectionPath)
                                         .GetDocument(study.Key)
                                         .GetDocumentAsync();
                    QStudy temp = studyDoc.ToObject<QStudy>();

                    if (!temp.Team.Equals("System") && !(!(temp.EndDate < DateTime.Now) | (temp.EndDate == DateTime.MinValue)))
                    {
                        toDelete.Add(temp.Id);
                        userChanged = true;
                    }
                    else
                    {
                        if (temp.Team.Equals("System") && temp.Title.Equals("Add"))
                        {
                            temp.Command = new DelegateCommand(() =>
                            {
                                (Xamarin.Forms.Application.Current as App).GetNavigationService().NavigateAsync("FindAllStudiesView", null, null, false);
                            });
                        }
                        else
                        {
                            temp.Command = new DelegateCommand(() =>
                            {
                                var navigationParams = new NavigationParameters();
                                navigationParams.Add("study", temp);

                                (Xamarin.Forms.Application.Current as App).GetNavigationService().NavigateAsync("StudyDetailView", navigationParams, null, false);
                            });
                        }

                        var elementsDoc = await CrossCloudFirestore.Current
                        .Instance
                        .GetCollection(QStudy.CollectionPath + "/" + temp.Id + "/" + QElement.CollectionPath)
                        .GetDocumentsAsync();

                        IEnumerable<QElement> elements = elementsDoc.ToObjects<QElement>();

                        if (elements.Count() > 0)
                        {
                            temp.Elements = new List<QElement>(elements.ToList());
                        }

                        _user.ActiveStudiesObjects.Add(temp);
                    }
                }

                await BlobCache.UserAccount.InsertObject("user", CurrentUser.Instance.User);

                if (userChanged)
                {
                    foreach (var delete in toDelete)
                    {
                        if (_user.Studies.Keys.Contains(delete))
                            _user.Studies.Remove(delete);
                    }
                    Update("Studies", _user.Studies);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            finally
            {
                RegisterGeofences();
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

                    //try to get the onesingal id
                    try
                    {
                        OneSignal.Current.IdsAvailable(new Com.OneSignal.Abstractions.IdsAvailableCallback((userId, arg) =>
                        {
                            CurrentUser.Instance.User.OnesignalId = userId;
                        }));
                    }
                    catch (Exception e)
                    {
                        Crashes.TrackError(e);
                    }

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

        /// <summary>
        /// Inidicate if the current in-memory user data should be deleted.
        /// </summary>
        public bool DeleteUserData
        {
            get => _deleteUserData;
            set => _deleteUserData = value;
        }
        #endregion
    }
}
