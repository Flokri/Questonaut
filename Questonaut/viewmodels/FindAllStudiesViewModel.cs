using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Akavache;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Extensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.Model;
using Shiny;
using Shiny.Locations;
using Shiny.Notifications;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Questonaut.viewmodels
{
    public class FindAllStudiesViewModel : BindableBase
    {
        #region instances
        private ObservableCollection<StudiesToParticipate> _studies = new ObservableCollection<StudiesToParticipate>();
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnParticipate { get; set; }
        public DelegateCommand OnBack { get; set; }
        #endregion

        #region constructor
        public FindAllStudiesViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //set the commands
            OnParticipate = new DelegateCommand(() => Participate());
            OnBack = new DelegateCommand(() => Back());

            //set the header for the user
            _ = GetUserDataAsync();
        }
        #endregion

        #region private functions
        /// <summary>
        /// Will be executed if the user want to participate/leave the selected Studies.
        /// </summary>
        private async void Participate()
        {
            foreach (StudiesToParticipate temp in Studies)
            {
                if (temp.Paricipate && !CurrentUser.Instance.User.Studies.Keys.Contains(temp.Id))
                {
                    CurrentUser.Instance.User.Studies.Add(temp.Id, new List<string>());
                    RegisterGeofences(temp);
                }
                else if (!temp.Paricipate && CurrentUser.Instance.User.Studies.Keys.Contains(temp.Id))
                {
                    CurrentUser.Instance.User.Studies.Remove(temp.Id);
                }
            }

            await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection(QUser.CollectionPath)
                         .GetDocument(CurrentUser.Instance.User.Id)
                         .UpdateDataAsync(new { Studies = CurrentUser.Instance.User.Studies });

            CurrentUser.Instance.LoadNewStudies();

            Back();
        }

        /// <summary>
        /// Add geofence to the new study.
        /// </summary>
        private async void RegisterGeofences(QStudy study)
        {
            try
            {
                var geofences = ShinyHost.Resolve<IGeofenceManager>();
                var notifications = ShinyHost.Resolve<INotificationManager>();

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

                        if (context.LocationName != null && CurrentUser.Instance.User.Locations.ContainsKey(context.LocationName) && !CurrentUser.Instance.RegisteredLocations.Contains(context.LocationName + "|" + context.LocationAction))
                        {
                            try
                            {
                                await geofences.StartMonitoring(new GeofenceRegion(
                                    context.LocationName + "|" + context.LocationAction,
                                new Position(CurrentUser.Instance.User.Locations[context.LocationName].Latitude, CurrentUser.Instance.User.Locations[context.LocationName].Longitude),
                                Distance.FromMeters(200))
                                {
                                    NotifyOnEntry = true,
                                    NotifyOnExit = true,
                                    SingleUse = false
                                });

                                CurrentUser.Instance.RegisteredLocations.Add(context.LocationName + "|" + context.LocationAction);
                            }
                            catch (Exception e)
                            {
                                Crashes.TrackError(e);
                            }
                        }
                    }
                }
            }
            catch (Exception e) { Crashes.TrackError(e); }
        }

        /// <summary>
        /// Navigates the user back to the view he comes from.
        /// </summary>
        private async void Back()
        {
            await _navigationService.GoBackAsync();
        }

        /// <summary>
        /// Load all user datas from the firebase db.
        /// </summary>
        /// <returns></returns>
        private async Task GetUserDataAsync()
        {
            try
            {
                var documents = await CrossCloudFirestore.Current
                                     .Instance
                                     .GetCollection(QStudy.CollectionPath)
                                     .GetDocumentsAsync();

                IEnumerable<StudiesToParticipate> myModel = documents.ToObjects<StudiesToParticipate>();

                foreach (var study in myModel)
                {
                    if (!study.Team.Equals("System") && (!(study.EndDate < DateTime.Now) | (study.EndDate == DateTime.MinValue)))
                    {
                        if (CurrentUser.Instance.User.Studies.Keys.Contains(study.Id))
                        {
                            study.Paricipate = true;
                        }
                        Studies.Add(study);
                    }
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// All available studies.
        /// </summary>
        public ObservableCollection<StudiesToParticipate> Studies
        {
            get => _studies;
            set => SetProperty(ref _studies, value);
        }
        #endregion 
    }

    public class StudiesToParticipate : QStudy
    {
        //mark a study to participate
        public bool _participate;

        /// <summary>
        /// Indicator if a user wants to participate on this study.
        /// </summary>
        public bool Paricipate
        {
            get => _participate;
            set => _participate = value;
        }
    }
}
