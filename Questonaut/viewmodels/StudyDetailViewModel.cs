using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.DependencyServices;
using Questonaut.Model;

namespace Questonaut.viewmodels
{
    public class StudyDetailViewModel : BindableBase, INavigatedAware
    {
        #region instances
        private QStudy _study;
        private List<QPermission> _permissions;
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnBack { get; set; }
        #endregion

        #region constructor
        public StudyDetailViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //initialize the back command
            OnBack = new DelegateCommand(() => Back());

            Xamarin.Forms.DependencyService.Get<ISetStatusBar>().SetColorToBlack();

            //test code
            Permissions = new List<QPermission>() {
                new QPermission { Name = "Location", Description = "This is used send you notifications bases on your location." },
                new QPermission { Name = "Steps", Description = "This is used send you notifications bases on your step count." },
            };
        }
        #endregion

        #region public methods

        #endregion

        #region private methods
        /// <summary>
        /// Navigates the user back to the view he comes from.
        /// </summary>
        private async void Back()
        {
            Xamarin.Forms.DependencyService.Get<ISetStatusBar>().SetColorToWhite();
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
            //get a single typed parameter
            Study = parameters.GetValue<QStudy>("study");
        }
        #endregion

        #region properties
        /// <summary>
        /// The current study object.
        /// </summary>
        public QStudy Study
        {
            get => _study;
            set => SetProperty(ref _study, value);
        }

        public List<QPermission> Permissions
        {
            get => _permissions;
            set => SetProperty(ref _permissions, value);
        }
        #endregion
    }
}
