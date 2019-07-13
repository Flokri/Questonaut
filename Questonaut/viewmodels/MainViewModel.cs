using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.Model;
using Questonaut.Settings;

namespace Questonaut.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region instances
        private string _username = "";
        private string _id = "";
        private string _header = "";
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
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //set the commands
            OnLogout = new DelegateCommand(() => Logout());
            AddUser = new DelegateCommand(() => Add());

            _ = GetUserDataAsync();
        }

        #region privateMethods
        private void Logout()
        {
            if (SettingsImp.UserValue != string.Empty)
            {
                SettingsImp.UserValue = "";
                _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
            }
        }

        private async Task GetUserDataAsync()
        {
            try
            {
                var documents = await CrossCloudFirestore.Current
                                                         .Instance
                                                         .GetCollection(QUser.CollectionPath)
                                                         .WhereEqualsTo("Email", CurrentUser.Instance.User.Email)
                                                         .GetDocumentsAsync();

                IEnumerable<QUser> myModel = documents.ToObjects<QUser>();

                if (myModel.Count() > 0)
                {
                    CurrentUser.Instance.User = myModel.First();
                    //set the header for the user
                    Header = "Welcome, " + CurrentUser.Instance.User.Name;
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
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
        #endregion
    }
}
