﻿using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Firebase.Storage;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Controller;
using Questonaut.DependencyServices;
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

            //set the default user image
            _image = new Image { Source = ImageSource.FromResource("Questonaut.SharedImages.defaultUserImage.png") };

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream file = assembly.GetManifestResourceStream("Questonaut.SharedImages.defaultUserImage.png");
            _cachedImage = GetImageStreamAsBytes(file);
            UserImage = _image.Source;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Save the user to the firestore db and persist the user in the app config.
        /// </summary>
        private async void Save()
        {
            await StoreImage(new MemoryStream(_cachedImage));
        }

        private async Task<string> StoreImage(Stream imageStream)
        {
            var storageImage = await new FirebaseStorage("questonaut.appspot.com")
                                                        .Child("ProfileImages")
                                                        .Child(CurrentUser.Instance.User.Name + ".jpg")
                                                        .PutAsync(imageStream);

            return storageImage;
        }

        /// <summary>
        /// Open the native image picker and let the user choose a profile image.
        /// </summary>
        /// <returns></returns>
        private async System.Threading.Tasks.Task ChooseImageAsync()
        {
            Stream stream = await Xamarin.Forms.DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

            _cachedImage = GetImageStreamAsBytes(stream);

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

        /// <summary>
        /// Get the bytes from the stream.
        /// </summary>
        /// <param name="input"><The stream./param>
        /// <returns>The input stream as byte array.</returns>
        private byte[] GetImageStreamAsBytes(Stream input)
        {
            if (input != null)
            {
                var buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            return null;
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
