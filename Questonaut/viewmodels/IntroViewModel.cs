using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;

namespace Questonaut.ViewModels
{
    public class IntroViewModel : BindableBase
    {
        private int _currentIndex;
        private int _ImageCount = 0;

        #region commands
        public DelegateCommand OnInteracted { get; set; }
        public DelegateCommand<object> OnPanChanged { get; set; }
        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        public IntroViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            Items = new ObservableCollection<object>
            {
                new { Source = CreateSource(),
                    Ind = _ImageCount++,
                    Color = Color.Transparent,
                    Title ="Start your Journey",
                    Content = "In order to participate in studies, you must allow this app a few permissions. This is necessary to capture the context.",
                    Visible = false,
                    ButtonText = ""},

                new { Source = CreateSource(),
                    Ind = _ImageCount++,
                    Color = Color.Transparent,
                    Title ="Messages",
                    Content = "In order to participate in studies,\nyou must allow this app a few\n permissions. This is necessary to\n  capture the context.",
                    Visible = true,
                    ButtonText = "Allow"},

                new { Source = CreateSource(),
                    Ind = _ImageCount++,
                    Color = Color.Transparent,
                    Title ="GPS Permission",
                    Content = "In order to get your correct position\n you have to allow the App to access your location. ",
                    Visible = true,
                    ButtonText = "Allow"},

                new { Source = CreateSource(),
                    Ind = _ImageCount++,
                    Color = Color.Transparent,
                    Title ="Finished",
                    Content = "In order to participate in studies,\nyou must allow this app a few\npermissions. This is necessary to\ncapture the context.",
                    Visible = true,
                    ButtonText = "Finish",
                    Command = new DelegateCommand(async () => await Task.Run(() => Save()))},
            };
        }

        private async void Save()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            {
                //change to the intro view
                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/MainView", System.UriKind.Absolute));
            });
        }

        public ObservableCollection<object> Items { get; }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if ((CurrentIndex > 1 && value == 0))
                {
                    SetProperty(ref _currentIndex, CurrentIndex);
                }
                else
                {
                    SetProperty(ref _currentIndex, value);
                }
            }
        }

        public bool IsAutoAnimationRunning { get; private set; }
        public bool IsUserInteractionRunning { get; private set; }

        private string CreateSource()
        {
            string source;
            switch (_ImageCount)
            {
                case 0:
                    source = $"https://firebasestorage.googleapis.com/v0/b/questonaut.appspot.com/o/PublicAppImages%2Fqeustonaut_intro.png?alt=media&token=6ba9c1ff-4400-4a8c-a311-094f860bc643";
                    break;
                case 1:
                    source = $"https://firebasestorage.googleapis.com/v0/b/questonaut.appspot.com/o/PublicAppImages%2Fenvelope.png?alt=media&token=45bf620f-c9e3-4e2a-8b16-d836e3faf6b3";
                    break;
                case 2:
                    source = $"https://firebasestorage.googleapis.com/v0/b/questonaut.appspot.com/o/PublicAppImages%2Fway-2.png?alt=media&token=ce3f18e9-bee1-4040-b142-b2e66e3d1ed7";
                    break;
                default:
                    source = "";
                    break;
            }

            return source;
        }
    }
}
