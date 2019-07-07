using System;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Settings;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class MainViewModel : BindableBase
    {

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnLogout { get; set; }
        #endregion

        public MainViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            OnLogout = new DelegateCommand(() => Logout());


            //test the firstore plugin
            var document = CrossCloudFirestore.Current
                                                    .Instance
                                                    .GetCollection("Users");
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
        #endregion
    }
}
