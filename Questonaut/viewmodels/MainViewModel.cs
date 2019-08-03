using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Akavache;
using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.Helper;
using Questonaut.Model;
using Questonaut.Settings;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region instances
        private string _username = "";
        private string _id = "";
        private string _header = "";

        private ObservableCollection<QStudy> _studies = CurrentUser.Instance.User.ActiveStudiesObjects;

        private ObservableCollection<QActivity> _activities = new ObservableCollection<QActivity>();
        public ActivityService _activityService { get; set; } = new ActivityService();
        int _pageNumber = 0;

        //start test code
        // Notify the UI if the app is busy loading data.
        public bool IsBusy { get; set; }

        public ICommand OnLoadMoreCommand { get; set; }
        public ICommand OnItemTappedCommand { get; set; }
        //end testcode

        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnLogout { get; set; }
        public DelegateCommand AddUser { get; set; }
        #endregion

        public MainViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            Akavache.Registrations.Start("Questonaut");

            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //set the commands
            OnLogout = new DelegateCommand(() => Logout());
            AddUser = new DelegateCommand(() => Add());

            //set the header for the user
            _ = GetUserDataAsync();

            //test code
            // Command to load more data from our service
            OnLoadMoreCommand = new DelegateCommand(async () =>
            {
                IsBusy = true;

                try
                {
                    var users = _activityService.GetActivitiesAsync(_pageNumber++, 10);

                    //Add the new data loaded from our service to our existing collection.
                    foreach (var user in users)
                    {
                        Activities.Add(user);
                    }

                }
                catch (Exception ex)
                {
                    //Log any errors that might had occured while calling or using your service.
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                }

            });

            OnLoadMoreCommand.Execute(null);
        }

        #region privateMethods
        /// <summary>
        /// Logout the current user and reset the in-memory user data.
        /// </summary>
        private async void Logout()
        {
            try
            {
                CurrentUser.Instance.LogoutUser();
                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        /// <summary>
        /// Load all user datas from the firebase db.
        /// </summary>
        /// <returns></returns>
        private async Task GetUserDataAsync()
        {
            try
            {
                try
                {
                    QUser inMemory = await BlobCache.UserAccount.GetObject<QUser>("user");
                    CurrentUser.Instance.User = inMemory;
                }
                catch
                {
                    await CurrentUser.Instance.LoadUser();
                }


                Header = "Welcome, " + CurrentUser.Instance.User?.Name;

                await CurrentUser.Instance.LoadNewStudies();

                Studies = CurrentUser.Instance.User.ActiveStudiesObjects;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        private void Add()
        {
            //tod: change after creating the create view
            //change to the create a user view
            _navigationService.NavigateAsync("StudyDetailView");
        }
        #endregion

        #region properties
        /// <summary>
        /// The header is the welcome statement for the user.
        /// </summary>
        public string Header
        {
            get => _header;
            set => SetProperty(ref _header, value);
        }
        /// <summary>
        /// The username of the current user.
        /// </summary>
        public string Username
        {
            get => _username;
            set { SetProperty(ref _username, value); }
        }

        /// <summary>
        /// The id of the current user.
        /// </summary>
        public string Id
        {
            get => _id;
            set { SetProperty(ref _id, value); }
        }

        /// <summary>
        /// A obserable collection of all studies the current user is particpating at.
        /// </summary>
        public ObservableCollection<QStudy> Studies
        {
            get => _studies;
            set => SetProperty(ref _studies, value);
        }

        /// <summary>
        /// A list of the recent occured activities.
        /// </summary>
        public ObservableCollection<QActivity> Activities
        {
            get => _activities;
            set => SetProperty(ref _activities, value);
        }
        #endregion
    }
}
