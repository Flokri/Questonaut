using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class MultipleChoiceViewModel : BindableBase, INavigatedAware
    {
        #region instances
        private QMultipleQuestion instance;
        private string _title = "";
        private string _text = "";

        private int _activityId;

        private List<string> _answers;
        private ObservableCollection<TickAnswers> _ticks;
        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region commands
        public DelegateCommand OnSave { get; set; }
        #endregion

        #region constructor
        public MultipleChoiceViewModel(INavigationService navigationService, IPageDialogService dialogService)
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
            if (Ticks.Where(x => x.Checked).Count() > 0)
            {
                Question.Timestamp = DateTime.Now;
                Question.Answer = JsonConvert.SerializeObject(Ticks);
                string answer = JsonConvert.SerializeObject(Question);

                new ActivityDB().SetActivityAsAnswered(answer, _activityId);
                MessagingCenter.Send<string>("Questonaut", "refreshDB");

                Device.BeginInvokeOnMainThread(async () =>
                {
                    //change to the intro view
                    await _navigationService.GoBackAsync();
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await _pageDialogservice.DisplayAlertAsync("Something is missing", "Please provide all informations to submit this question", "Cancel");
                });
            }
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
            instance = parameters["question"] as QMultipleQuestion;
            Title = instance?.Title;
            Text = instance?.Body;
            Answers = instance?.Answers;

            Ticks = new ObservableCollection<TickAnswers>();
            foreach (string tmp in Answers)
            {
                Ticks.Add(new TickAnswers() { Answer = tmp, Checked = false });
            }

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
        /// The current question.
        /// </summary>
        private List<string> Answers
        {
            get => _answers;
            set => SetProperty(ref _answers, value);
        }

        /// <summary>
        /// The current Answers
        /// </summary>
        public ObservableCollection<TickAnswers> Ticks
        {
            get => _ticks;
            set => SetProperty(ref _ticks, value);
        }

        /// <summary>
        /// The question instance of the current multiple choice question.
        /// </summary>
        private QMultipleQuestion Question
        {
            get => instance;
            set => SetProperty(ref instance, value);
        }
        #endregion
    }

    public class TickAnswers
    {
        public string Answer { get; set; }
        public bool Checked { get; set; }
    }
}
