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
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Questonaut.viewmodels
{
    public class FindAllStudiesViewController : BindableBase
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
        public FindAllStudiesViewController(INavigationService navigationService, IPageDialogService pageDialogService)
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
                if (temp.Paricipate && !CurrentUser.Instance.User.ActiveStudies.Contains(temp.Id))
                {
                    CurrentUser.Instance.User.ActiveStudies.Add(temp.Id);
                }
                else if (!temp.Paricipate && CurrentUser.Instance.User.ActiveStudies.Contains(temp.Id))
                {
                    CurrentUser.Instance.User.ActiveStudies.Remove(temp.Id);
                }
            }

            await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection(QUser.CollectionPath)
                         .GetDocument(CurrentUser.Instance.User.Id)
                         .UpdateDataAsync(new { ActiveStudies = CurrentUser.Instance.User.ActiveStudies });

            CurrentUser.Instance.LoadNewStudies();

            Back();
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
                    if (!study.Team.Equals("System"))
                    {
                        if (CurrentUser.Instance.User.ActiveStudies.Contains(study.Id))
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
