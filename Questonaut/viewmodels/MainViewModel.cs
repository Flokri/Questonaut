using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Akavache;
using Firebase.Rest.Auth.Payloads;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
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

        private List<QStudies> _studies;
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
        }

        #region privateMethods
        private async void Logout()
        {
            try
            {
                await BlobCache.UserAccount.InvalidateAll();

                if (SettingsImp.UserValue != string.Empty)
                {
                    SettingsImp.UserValue = "";
                    _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
                }
            }
            catch { }
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
                catch { }


                Header = "Welcome, " + CurrentUser.Instance.User?.Name;

                Studies = CurrentUser.Instance.User.ActiveStudiesObjects;

                foreach (QStudies item in Studies)
                {
                    if (item.Team.Equals("System") && item.Title.Equals("Add"))
                    {
                        item.Command = new DelegateCommand(() => AddStudy());
                    }
                    else
                    {
                        item.Command = new DelegateCommand(() => GoToDetailView());
                    }
                }

                var documents = await CrossCloudFirestore.Current
                                     .Instance
                                     .GetCollection(QUser.CollectionPath)
                                     .WhereEqualsTo("Email", CurrentUser.Instance.User.Email)
                                     .GetDocumentsAsync();

                IEnumerable<QUser> myModel = documents.ToObjects<QUser>();

                if (myModel.Count() > 0)
                {
                    CurrentUser.Instance.User = myModel.First();

                    if (Header.Equals("Welcome, "))
                    {
                        //set the header for the user
                        Header = "Welcome, " + CurrentUser.Instance.User.Name;
                    }
                }

                foreach (var study in CurrentUser.Instance.User.ActiveStudies)
                {
                    var studyDoc = await CrossCloudFirestore.Current
                                         .Instance
                                         .GetCollection(study.Parent.Id)
                                         .GetDocument(study.Id)
                                         .GetDocumentAsync();
                    QStudies temp = studyDoc.ToObject<QStudies>();

                    if (temp.Team.Equals("System") && temp.Title.Equals("Add"))
                    {
                        temp.Command = new DelegateCommand(() => AddStudy());
                    }
                    else
                    {
                        temp.Command = new DelegateCommand(() => GoToDetailView());
                    }

                    CurrentUser.Instance.User.ActiveStudiesObjects.Add(temp);
                }

                await BlobCache.UserAccount.InsertObject("user", CurrentUser.Instance.User);

                Studies = CurrentUser.Instance.User.ActiveStudiesObjects;
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }

        private void AddStudy()
        {

            _navigationService.NavigateAsync("FindAllStudiesView");
        }

        private void GoToDetailView()
        {

        }

        private void Add()
        {
            //tod: change after creating the create view
            //change to the create a user view
            _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/IntroView", System.UriKind.Absolute));

            //try
            //{
            //var documents = await CrossCloudFirestore.Current
            //                                         .Instance
            //                                         .GetCollection(QUser.CollectionPath)
            //                                         .WhereEqualsTo("Name", CurrentUser.Instance.User.Name)
            //                                         .GetDocumentsAsync();

            //IEnumerable<QUser> myModel = documents.ToObjects<QUser>();
            //Username = myModel.First().Name;
            //Id = myModel.First().Id;
            //}
            //catch (Exception e)
            //{
            //    var msg = e.Message;
            //}
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
        public string Username
        {
            get => _username;
            set { SetProperty(ref _username, value); }
        }
        public string Id
        {
            get => _id;
            set { SetProperty(ref _id, value); }
        }

        public List<QStudies> Studies
        {
            get => _studies;
            set => SetProperty(ref _studies, value);
        }
        #endregion
    }
}
