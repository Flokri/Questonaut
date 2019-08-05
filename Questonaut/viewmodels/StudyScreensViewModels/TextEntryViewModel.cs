using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Model;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Questonaut.viewmodels.StudyScreensViewModels
{
    public class TextEntryViewModel : BindableBase, INavigatedAware
    {
        #region instances
        private string _answer = "";
        private string _title = "";
        private string _text = "";

        private QQuestion _question;
        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region commands
        public DelegateCommand OnSave { get; set; }
        #endregion

        #region constructor
        public TextEntryViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            //initialize the commands
            OnSave = new DelegateCommand(async () => await Task.Run(() => Save()));
        }
        #endregion

        #region private methods
        private async void Save()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //change to the intro view
                await _navigationService.GoBackAsync();
            });
        }
        #endregion

        #region navigation aware
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            //get the parameter
            Question = parameters["activity"] as QQuestion;
            Title = Question?.Title;
            Text = Question?.Body;
        }
        #endregion

        #region properties
        /// <summary>
        /// The title of this view
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// The text of this view
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        /// <summary>
        /// The answer of this view
        /// </summary>
        public string Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        /// <summary>
        /// The current question.
        /// </summary>
        private QQuestion Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }
        #endregion
    }
}
