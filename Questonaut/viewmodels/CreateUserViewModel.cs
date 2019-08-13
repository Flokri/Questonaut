using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Firebase.Storage;
using Microsoft.AppCenter.Crashes;
using Plugin.CloudFirestore;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.DependencyServices;
using Questonaut.Helper;
using Questonaut.Model;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class CreateUserViewModel : BindableBase
    {
        #region instance
        //instances for styling
        private double _imageSection;
        private DateTime _maxDate;
        private DateTime _minDate;

        //user instances
        private string _name;
        private DateTime _birthday;

        //gender instances
        private bool _male;
        private bool _female;

        //user image
        private ImageSource _userImage;
        private Image _image;
        private byte[] _cachedImage;

        #endregion

        #region dependency injection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region commands
        public DelegateCommand OnSaveClicked { get; set; }
        public DelegateCommand OnImageChooseTapped { get; set; }
        public DelegateCommand OnCancel { get; set; }
        #endregion

        #region constructor
        public CreateUserViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            //setting the display size dependet size of the imageview
            ImageSectionSize = Xamarin.Forms.DependencyService.Get<IScreenDimensions>().GetScreenHeight() * 0.15;

            //set the min and max date
            MinDate = DateTime.Today.AddYears(-99);
            MaxDate = DateTime.Today.AddYears(-13);

            //set the min possible age
            Birthday = MaxDate;

            //initialize the commands
            OnSaveClicked = new DelegateCommand(async () => await Task.Run(() => Save()));
            OnImageChooseTapped = new DelegateCommand(async () => await ChooseImageAsync());
            OnCancel = new DelegateCommand(() => Cancel());

            //set the default user image
            _image = new Image { Source = ImageSource.FromResource("Questonaut.SharedImages.defaultUserImage.png") };

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream file = assembly.GetManifestResourceStream("Questonaut.SharedImages.defaultUserImage.png");
            _cachedImage = GetImageStreamAsBytes.Convert(file);
            UserImage = _image.Source;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Cancels the creation step and returns to the login screen
        /// </summary>
        private void Cancel()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                //change to the intro view
                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
            });
        }

        /// <summary>
        /// Save the user to the firestore db and persist the user in the app config.
        /// </summary>
        private async void Save()
        {
            //check the user enterd all neccassary properties
            if (Name != string.Empty && (Male | Female))
            {
                try
                {
                    var storedImage = await StoreImage(new MemoryStream(_cachedImage));

                    CurrentUser.Instance.User.Name = this.Name;
                    CurrentUser.Instance.User.Birthday = this.Birthday.ToShortDateString();
                    CurrentUser.Instance.User.Gender = this.Male ? "Male" : "Female";
                    //CurrentUser.Instance.User.Image = storedImage;
                    CurrentUser.Instance.User.Locations = new Dictionary<string, GeoPoint>();

                    await CrossCloudFirestore.Current
                         .Instance
                         .GetCollection(QUser.CollectionPath)
                         .AddDocumentAsync(CurrentUser.Instance.User);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //change to the intro view
                        await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/IntroView", System.UriKind.Absolute));
                    });

                }
                catch (Exception e)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await _pageDialogservice.DisplayAlertAsync("Error", "Something went wrong... Please retry it later", "Ok");
                    });

                    Crashes.TrackError(e);
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await _pageDialogservice.DisplayAlertAsync("Error", "Please fill in all the informations.", "Ok");
                });
            }
        }

        /// <summary>
        /// Save the selected image to the firestore cloud.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <returns></returns>
        private async Task<string> StoreImage(Stream imageStream)
        {
            try
            {
                //set the authentication token 
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = async () => await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().GetCurrentUser()
                };

                var storageImage = await new FirebaseStorage("questonaut.appspot.com", options)
                                                            .Child("UserImages")
                                                            .Child(CurrentUser.Instance.User.Email + ".jpg")
                                                            .PutAsync(imageStream);

                //set the image localy
                CurrentUser.Instance.User.LocalImage = GetImageStreamAsBytes.Convert(imageStream);

                return storageImage;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Open the native image picker and let the user choose a profile image.
        /// </summary>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ChooseImageAsync()
        {
            //check the permissions
            var action = await _pageDialogservice.DisplayActionSheetAsync("Choose Image Source", "Cancel", null, "Camera", "Album");
            MediaFile file = null;

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                //if permissions are missig get them here
                cameraStatus = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                storageStatus = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                switch (action)
                {
                    case "Camera":
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            Directory = "Sample",
                            Name = "UserImage.jpg",
                            CompressionQuality = 50,
                            PhotoSize = PhotoSize.Medium,
                            SaveToAlbum = true
                        });
                        break;

                    case "Album":
                        file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            CompressionQuality = 50,
                            PhotoSize = PhotoSize.Medium
                        });
                        break;
                    default:
                        break;
                }

                ProcessingImage(file);
            }
        }

        /// <summary>
        /// Get the image and convert it to a byte[] and set it to the rigth properties.
        /// </summary>
        private void ProcessingImage(MediaFile file)
        {
            if (file == null)
            {
                return;
            }

            var stream = file.GetStream();

            _cachedImage = GetImageStreamAsBytes.Convert(stream);

            if (_cachedImage != null)
            {
                Stream reRead = new MemoryStream(_cachedImage);

                if (stream != null)
                {
                    _image = new Image
                    {
                        Source = ImageSource.FromStream(() => reRead),
                        BackgroundColor = Color.Gray,
                    };

                    UserImage = _image.Source;
                }
            }
        }
        #endregion

        #region public methods

        #endregion

        #region properties
        /// <summary>
        /// Set the image section height.
        /// </summary>
        public double ImageSectionSize
        {
            get => _imageSection;
            set => SetProperty(ref _imageSection, value);
        }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// The birthday of the user
        /// </summary>
        public DateTime Birthday
        {
            get => _birthday;
            set => SetProperty(ref _birthday, value);
        }

        /// <summary>
        /// The male gender proptery of the user.
        /// </summary>
        public bool Male
        {
            get => _male;
            set
            {
                SetProperty(ref _male, value);

                if (_male)
                {
                    Female = false;
                }
            }
        }

        /// <summary>
        /// The female property of the user.
        /// </summary>
        public bool Female
        {
            get => _female;
            set
            {
                SetProperty(ref _female, value);

                if (_female)
                {
                    Male = false;
                }
            }
        }

        /// <summary>
        /// The max date of the date picker.
        /// </summary>
        public DateTime MaxDate
        {
            get => _maxDate;
            set => SetProperty(ref _maxDate, value);
        }

        /// <summary>
        /// The min date of the date picker.
        /// </summary>
        public DateTime MinDate
        {
            get => _minDate;
            set => SetProperty(ref _minDate, value);
        }

        /// <summary>
        /// The profile image of the user.
        /// </summary>
        public ImageSource UserImage
        {
            get => _userImage;
            set => SetProperty(ref _userImage, value);
        }
        #endregion 
    }
}

