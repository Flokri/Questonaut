using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Akavache;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.Model;
using Xamarin.Forms;

namespace Questonaut.viewmodels
{
    public class FindAllStudiesViewController : BindableBase
    {
        #region instances
        private ObservableCollection<QStudies> _studies = new ObservableCollection<QStudies>();
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnSave { get; set; }
        public DelegateCommand OnBack { get; set; }
        #endregion

        #region constructor
        public FindAllStudiesViewController(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            //initialize the prims stuff
            _navigationService = navigationService;
            _pageDialogservice = pageDialogService;

            //set the commands
            OnSave = new DelegateCommand(() => Save());
            OnBack = new DelegateCommand(() => Back());

            //set the header for the user
            _ = GetUserDataAsync();
        }
        #endregion

        #region private functions
        private void Save()
        {

        }

        private async void Back()
        {
            _navigationService.GoBackAsync();
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
                                     .GetCollection(QStudies.CollectionPath)
                                     .GetDocumentsAsync();

                IEnumerable<QStudies> myModel = documents.ToObjects<QStudies>();

                foreach (var study in myModel)
                {
                    if (!study.Team.Equals("System"))
                        Studies.Add(study);
                }
            }
            catch (Exception e)
            {
                var msg = e.Message;
            }
        }
        #endregion

        #region properties
        /// <summary>
        /// All available studies.
        /// </summary>
        public ObservableCollection<QStudies> Studies
        {
            get => _studies;
            set => SetProperty(ref _studies, value);
        }
        #endregion 
    }
}
