using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Akavache;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Plugin.LocalNotifications;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.DependencyServices;
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
        private bool _isRefreshing = false;

        private QActivity _selectedItem;

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
        public DelegateCommand OnRefresh { get; set; }
        public DelegateCommand OnItemTapped { get; set; }
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
            OnRefresh = new DelegateCommand(async () =>
            {
                IsRefreshing = true;
                Activities.Clear();
                foreach (var tmp in _activityService.GetActivitiesAsync(0, 1000))
                    Activities.Add(tmp);
                IsRefreshing = false;
            });
            OnItemTapped = new DelegateCommand(() => ItemTapped());

            //set the header for the user
            _ = GetUserDataAsync();

            Activities = new ObservableCollection<QActivity>(_activityService.GetActivitiesAsync(0, 100));

            MessagingCenter.Subscribe<string>("Questonaut", "refreshDB", (arg) =>
            {
                Activities = new ObservableCollection<QActivity>(_activityService.GetActivitiesAsync(0, 100));
            });
        }

        #region privateMethods
        /// <summary>
        /// Get called when the user tapps a activity item.
        /// </summary>
        private async void ItemTapped()
        {
            if (SelectedItem != null)
            {
                if (SelectedItem.Status.Equals("open"))
                {
                    LoadElementAndNavigateAsync();
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await _pageDialogservice.DisplayAlertAsync("Already Answered", "You've already answered this question ✔", "Ok");
                    });
                }
            }
        }

        /// <summary>
        /// Load the user element from firebase.
        /// </summary>
        private async void LoadElementAndNavigateAsync()
        {
            var questionDoc = await CrossCloudFirestore.Current
                                                      .Instance
                                                      .GetCollection(QBaseQuestion.CollectionPath)
                                                      .GetDocument(SelectedItem.Link)
                                                      .GetDocumentAsync();
            QBaseQuestion baseQuestion = questionDoc.ToObject<QBaseQuestion>();

            try
            {
                if (baseQuestion != null && baseQuestion.Type != null)
                {

                    switch (baseQuestion.Type)
                    {
                        case "TextEntry":
                            QQuestion question = questionDoc.ToObject<QQuestion>();

                            var navigationParamsQuestion = new NavigationParameters();
                            navigationParamsQuestion.Add("question", question);
                            navigationParamsQuestion.Add("activity", SelectedItem);

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await _navigationService.NavigateAsync("TextEntryView", navigationParamsQuestion, null, false);
                            });
                            break;
                        case "Slider":
                            break;
                        case "MultipleChoice":
                            QMultipleQuestion multipleChoice = questionDoc.ToObject<QMultipleQuestion>();

                            var navigationParamsMultipleChoice = new NavigationParameters();
                            navigationParamsMultipleChoice.Add("question", multipleChoice);
                            navigationParamsMultipleChoice.Add("activity", SelectedItem);

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await _navigationService.NavigateAsync("MultipleChoiceView", navigationParamsMultipleChoice, null, false);
                            });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        /// <summary>
        /// Logout the current user and reset the in-memory user data.
        /// </summary>
        private async void Logout()
        {
            //test code
            var test = new ActivityDB();
            test.AddActivity(new QActivity() { Name = "Question", Date = DateTime.Now, Description = "Answered a question based on a step context.", Status = "open", Link = "58ZIoxSLNUbncyXB2xxb" });
            //end test code

            //try
            //{
            //    CurrentUser.Instance.LogoutUser();
            //    await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
            //}
            //catch (Exception e)
            //{
            //    Crashes.TrackError(e);
            //}
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

        /// <summary>
        /// Sets the refreshing status of the listview.
        /// </summary>
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        /// <summary>
        /// The selected item from the list view.
        /// </summary>
        public QActivity SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        #endregion
    }
}
