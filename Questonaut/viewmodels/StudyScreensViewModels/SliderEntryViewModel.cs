using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Helper;
using Questonaut.Model;
using Xamarin.Forms;

namespace Questonaut.ViewModels.StudyScreensViewModels
{
    public class SliderEntryViewModel : BindableBase, INavigatedAware
    {
        #region instances
        private int _answer = 0;
        private string _title = "";
        private string _text = "";
        private int _maxValue = 1;
        private int _minValue = 0;

        private int _activityId;

        private QSlider _slider;
        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region commands
        public DelegateCommand OnSave { get; set; }
        #endregion

        #region constructor
        public SliderEntryViewModel(INavigationService navigationService, IPageDialogService dialogService)
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
            string answer = JsonConvert.SerializeObject(Answer);

            new ActivityDB().SetActivityAsAnswered(answer, _activityId);
            MessagingCenter.Send<string>("Questonaut", "refreshDB");

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
            Question = parameters["question"] as QSlider;
            Title = Question?.Title;
            Text = Question?.Body;
            MaxValue = Question.MaxValue;
            MinValue = Question.MinValue;

            //get the activity id
            _activityId = (parameters["activity"] as QActivity).ID;
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
        public int Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        /// <summary>
        /// The current question.
        /// </summary>
        private QSlider Question
        {
            get => _slider;
            set => SetProperty(ref _slider, value);
        }

        /// <summary>
        /// The max value of the slider
        /// </summary>
        public int MaxValue
        {
            get => _maxValue;
            set => SetProperty(ref _maxValue, value);
        }

        /// <summary>
        /// The min value of the slider
        /// </summary>
        public int MinValue
        {
            get => _minValue;
            set => SetProperty(ref _minValue, value);
        }
        #endregion
    }
}
