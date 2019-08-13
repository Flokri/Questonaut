using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using dotMorten.Xamarin.Forms;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.DependencyServices;
using Questonaut.Model;
using Xamarin.Essentials;

namespace Questonaut.ViewModels
{
    public class SettingsViewModel : BindableBase, INavigatedAware
    {
        #region instances
        private ObservableCollection<string> _suggestedLocations;
        private string _homeAddress = "";
        private string _workAddress = "";
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnBack { get; set; }
        public DelegateCommand OnEnterHomeAddress { get; set; }
        public DelegateCommand OnWorkAddress { get; set; }
        public DelegateCommand OnLogout { get; set; }
        #endregion

        #region constructor
        public SettingsViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //initialize the back command
            OnBack = new DelegateCommand(() => Back());
            OnEnterHomeAddress = new DelegateCommand(() => EnterHomeAddress());
            OnWorkAddress = new DelegateCommand(() => EnterWorkAddress());
            OnLogout = new DelegateCommand(() => Logout());

            _suggestedLocations = new ObservableCollection<string>();

            FillPreData();
        }
        #endregion

        #region public methods

        #endregion

        #region private methods
        private async void FillPreData()
        {
            try
            {
                foreach (KeyValuePair<string, GeoPoint> point in CurrentUser.Instance.User.Locations)
                {
                    Placemark mark = await ResolveLocation(new Location(point.Value.Latitude, point.Value.Longitude));
                    switch (point.Key)
                    {
                        case "HOME":
                            HomeAddress = mark.FeatureName + ", " + mark.Locality;
                            break;

                        case "WORK":
                            WorkAddress = mark.FeatureName + ", " + mark.Locality;
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (NullReferenceException e)
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
            //var test = new ActivityDB();
            //test.AddActivity(new QActivity() { StudyId = "nCEviVVAF6CqEQEdM5Hn", Name = "Question", Date = DateTime.Now, Description = "Answered a question based on a step context.", Status = "open", Link = "wuBomdj98G4GGdX7Zt5s" });
            //end test code

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

        private async void EnterHomeAddress()
        {
            Location loc = await GetAddress(HomeAddress);
            if (loc != null)
            {
                CurrentUser.Instance.AddLocation("HOME", loc);
            }
        }

        private async void EnterWorkAddress()
        {
            Location loc = await GetAddress(WorkAddress);
            if (loc != null)
            {
                CurrentUser.Instance.AddLocation("WORK", loc);
            }
        }

        private async Task<Location> GetAddress(string address)
        {
            try
            {
                var locations = await Geocoding.GetLocationsAsync(HomeAddress);

                var location = locations?.FirstOrDefault();
                if (location != null)
                {
                    Placemark placemark = await ResolveLocation(location);
                    if (placemark != null)
                    {
                        var geocodeAddress =
                                    $"CountryCode: {placemark.CountryCode}\n" +
                                    $"CountryName: {placemark.CountryName}\n" +
                                    $"Street:      {placemark.FeatureName}\n" +
                                    $"City:        {placemark.Locality}\n" +
                                    $"PostalCode:  {placemark.PostalCode}\n";

                        var answer = await _pageDialogservice.DisplayAlertAsync("Correct?", "Is this the correct address:\n" + geocodeAddress, "Yes", "No");
                        if (answer.Equals("No"))
                        {
                            return null;
                        }

                        return location;

                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }

        private async Task<Placemark> ResolveLocation(Location loc)
        {
            Console.WriteLine($"Latitude: {loc.Latitude}, Longitude: {loc.Longitude}, Altitude: {loc.Altitude}");
            var placemarks = await Geocoding.GetPlacemarksAsync(loc.Latitude, loc.Longitude);

            var placemark = placemarks?.FirstOrDefault();
            if (placemark != null)
            {
                return placemark;
            }

            return null;
        }

        /// <summary>
        /// Navigates the user back to the view he comes from.
        /// </summary>
        private async void Back()
        {
            await _navigationService.GoBackAsync();
        }
        #endregion

        #region navigation aware
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// The suggested locations as string
        /// </summary>
        public ObservableCollection<string> SuggestedLocations
        {
            get => _suggestedLocations;
            set => SetProperty(ref _suggestedLocations, value);
        }

        /// <summary>
        /// The home location entered text
        /// </summary>
        public string HomeAddress
        {
            get => _homeAddress;
            set => SetProperty(ref _homeAddress, value);
        }

        /// <summary>
        /// The work location entered text
        /// </summary>
        public string WorkAddress
        {
            get => _workAddress;
            set => SetProperty(ref _workAddress, value);
        }
        #endregion
    }
}