using System;
using System.Collections.Generic;
using System.Linq;
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
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            OnLogout = new DelegateCommand(() => Logout());
            AddUser = new DelegateCommand(async () => await Task.Run(() => Add()));
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

        private async void Add()
        {
            try
            {
                var documents = await CrossCloudFirestore.Current
                                                         .Instance
                                                         .GetCollection(QUser.CollectionPath)
                                                         .WhereEqualsTo("Name", CurrentUser.Instance.User.Name)
                                                         .GetDocumentsAsync();

                IEnumerable<QUser> myModel = documents.ToObjects<QUser>();
                Username = myModel.First().Name;
                Id = myModel.First().Id;
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }
        #endregion

        #region properties
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
