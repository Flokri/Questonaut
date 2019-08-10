using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.DependencyServices;
using Questonaut.Settings;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class IntroViewModel : BindableBase
    {
        #region instances
        private int _currentIndex;
        private int _ImageCount = 0;
        private double _variableHeigth;
        private List<object> _items;
        #endregion

        #region commands
        public DelegateCommand OnInteracted { get; set; }
        public DelegateCommand<object> OnPanChanged { get; set; }
        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region constructor
        public IntroViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            VariableHeigth = Xamarin.Forms.DependencyService.Get<IScreenDimensions>().GetScreenHeight() * 0.3;

            CreateItems();
        }
        #endregion

        #region private functions
        /// <summary>
        /// This function creates the carousel items.
        /// </summary>
        private void CreateItems()
        {
            Items = new List<object>
            {
               new { Source = "resource://Questonaut.SharedImages.qeustonaut_intro.png",
                    Ind = _ImageCount++,
                    Color = Xamarin.Forms.Color.Transparent,
                    Title ="Start your Journey",
                    Content = "In order to participate in studies, you must allow this app a few permissions. This is necessary to capture the context.",
                    ImageSize = VariableHeigth,
                    Visible = false,
                    ButtonText = ""},

                new { Source = "resource://Questonaut.SharedImages.health.png",
                    Ind = _ImageCount++,
                    Color = Xamarin.Forms.Color.Transparent,
                    Title ="Sensors",
                    Content = "To messure the context for the studies you want to participate this app requires to use the sensors of your phone to check if you are currently in the rigth context to answer the survey. To do so please grant the permission to use your sensor values.",
                    ImageSize = VariableHeigth,
                    Visible = true,
                    Command = new DelegateCommand(() => GrantSensorPermission()),
                    ButtonText = "Allow"},

                new { Source = "resource://Questonaut.SharedImages.location.png",
                    Ind = _ImageCount++,
                    Color = Xamarin.Forms.Color.Transparent,
                    Title ="GPS Permission",
                    Content = "In order to get your correct position you have to allow the App to access your location.",
                    ImageSize = VariableHeigth,
                    Visible = true,
                    Command = new DelegateCommand(() => GrantLocationPermission()),
                    ButtonText = "Allow"},

                new { Source = "resource://Questonaut.SharedImages.survey.png",
                    Ind = _ImageCount++,
                    Color = Xamarin.Forms.Color.Transparent,
                    Title ="Finished",
                    Content = "In order to participate in studies, you must allow this app a few permissions. This is necessary to capture the context.",
                    ImageSize = VariableHeigth,
                    Visible = true,
                    ButtonText = "Finish",
                    Command = new DelegateCommand(() => Save())},
            };
        }

        private async void Save()
        {
            //Set the show intro view to false
            SettingsImp.ShowIntro = "false";

            Device.BeginInvokeOnMainThread(async () =>
            {
                //navigate to the intro view
                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/MainView", System.UriKind.Absolute));
            });
        }

        /// <summary>
        /// Get the location permissions for the app.
        /// </summary>
        private async void GrantLocationPermission()
        {
            //check the permission
            var locationStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<LocationPermission>();

            if (locationStatus != PermissionStatus.Granted)
            {
                //if permissiore missig get them here
                locationStatus = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }
        }

        /// <summary>
        /// Get the notification permissions for the app.
        /// </summary>
        private async void GrantSensorPermission()
        {
            //check the permission
            var sensorStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<SensorsPermission>();

            if (sensorStatus != PermissionStatus.Granted)
            {
                //if pssions are missig get them here
                sensorStatus = await CrossPermissions.Current.RequestPermissionAsync<SensorsPermission>();
            }
        }
        #endregion

        #region properies
        /// <summary>
        /// The items property holds all the items which should displayed in the carousel view.
        /// </summary>
        public List<object> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        /// <summary>
        /// The current index shows which page is currently visible.
        /// </summary>
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                SetProperty(ref _currentIndex, value);
            }
        }

        /// <summary>
        /// Returns a screen dependent heigth for displaying the page image.
        /// </summary>
        public double VariableHeigth
        {
            get => _variableHeigth;
            set => SetProperty(ref _variableHeigth, value);
        }
        #endregion
    }
}
