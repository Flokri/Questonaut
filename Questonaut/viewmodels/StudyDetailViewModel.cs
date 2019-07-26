using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.DependencyServices;

namespace Questonaut.viewmodels
{
    public class StudyDetailViewModel : BindableBase
    {
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

        #region properties

        #endregion
    }
}
