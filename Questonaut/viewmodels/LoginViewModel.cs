using System;
using System.Net.Http;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Payloads;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Converters;
using Questonaut.DependencyServices;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        #region Instances
        private const double USED = 1;
        private const double NOT_USED = 0.49;

        private const double BUTTON_FACTOR_IOS = 0.05;
        private const double BUTTON_FACTOR_ANDROID = 0.045;

        private double _buttonFontSize;

        private string _action = "Login";

        private double _loginButtonOp;
        private double _signupButtonOp;

        private string _password;
        private string _email;

        private bool _register = false;
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;
        #endregion

        #region Commands
        public DelegateCommand OnActionClickedCommand { get; set; }
        public DelegateCommand OnLoginClickedCommand { get; set; }
        public DelegateCommand OnSignupClickedCommand { get; set; }
        #endregion

        #region Constructor
        public LoginViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            OnActionClickedCommand = new DelegateCommand(() => ActionClickedAsync());
            OnLoginClickedCommand = new DelegateCommand(() => OnLogin());
            OnSignupClickedCommand = new DelegateCommand(async () => await Task.Run(() => OnSignup()));

            ButtonFontSize = Xamarin.Forms.Device.GetNamedSize(NamedSize.Large, typeof(Button));

            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    ButtonFontSize = ((double)Xamarin.Forms.DependencyService.Get<IScreenDimensions>().GetScreenHeight()) * BUTTON_FACTOR_IOS;
                    break;
                case Xamarin.Forms.Device.Android:
                    ButtonFontSize = ((double)Xamarin.Forms.DependencyService.Get<IScreenDimensions>().GetScreenHeight()) * BUTTON_FACTOR_ANDROID;
                    break;
                default:
                    ButtonFontSize *= 1.0;
                    break;
            }

            LoginActive();
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Sets the current user action to login.
        /// </summary>
        public void LoginActive()
        {
            Register = false;

            LoginButtonOp = USED;
            SignupButtonOp = NOT_USED;

            Action = "Login";
        }

        /// <summary>
        /// Sets the current user action to signup.
        /// </summary>
        public void SignupActive()
        {
            Register = true;

            SignupButtonOp = USED;
            LoginButtonOp = NOT_USED;

            Action = "Signup";
        }

        private void OnLogin()
        {
            LoginActive();
        }

        private Task OnSignup()
        {
            SignupActive();

            return Task.CompletedTask;
        }

        private async Task ActionClickedAsync()
        {
            switch (Action)
            {
                case "Login":
                    if (await LoginAsync() == false)
                    {
                        _pageDialogservice.DisplayAlertAsync("Error", "Failed to login", "Cancel");
                    }
                    else
                    {
                        //TODO: pass param
                        //change to the dashboard
                    }

                    break;
                case "Signup":
                    if (await SignupAsync() == false)
                    {
                        _pageDialogservice.DisplayAlertAsync("Error", "Failed to signup", "Cancel");
                    }
                    else
                    {
                        //TODO: pass param
                        //change to the dashboard
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Change the context to signup.
        /// </summary>
        public async Task<bool> SignupAsync()
        {
            if (Email != string.Empty && Password != string.Empty)
            {
                //todo: change this service if you don't want to use the firebase service
                //test the firebase signup
                //var token = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().RegsiterWithEmailPassword(Email, Password);

                //try to use the firebase webapi
                var authOptions = new FirebaseAuthOptions();
                authOptions.WebApiKey = "AIzaSyCZw5RPCES9Sh3T0gvE2p81w_vRB47RLww";

                var firebase = new FirebaseAuthService(authOptions);

                var request = new SignUpNewUserRequest()
                {
                    Email = "florian.kriegl@mcp-alfa.com",
                    Password = "Tester123!",
                };

                try
                {
                    var response = await firebase.SignUpNewUser(request);

                    if (response != null)
                    {
                        var verificationRequest = new SendVerificationEmailRequest()
                        {
                            RequestType = "VERIFY_EMAIL",
                            IdToken = response.IdToken,
                        };
                        var sendverification = await firebase.SendVerification(verificationRequest);
                    }
                }
                catch (FirebaseAuthException e)
                {

                }


                return false;
            }
            else
            {
                //if the signup process failed
                return false;
            }
        }

        /// <summary>
        /// Change the current context to login.
        /// </summary>
        public async Task<bool> LoginAsync()
        {
            if (Email != string.Empty && Password != string.Empty)
            {
                //todo: change this service if you don't want to use the firebase service
                //test the firebase auth
                //var token = await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPassword(Email, Password);


                //try to use the firebase web api
                var authOptions = new FirebaseAuthOptions();
                authOptions.WebApiKey = "AIzaSyCZw5RPCES9Sh3T0gvE2p81w_vRB47RLww";
                var firebase = new FirebaseAuthService(authOptions);

                var request = new VerifyPasswordRequest()
                {
                    Email = "floriankriegl@gmx.at",
                    Password = "questonaut"
                };

                try
                {
                    var response = await firebase.VerifyPassword(request);
                }
                catch (FirebaseAuthException e)
                {
                    // App specific error handling.
                }

                //could not find user
                return false;
            }
            else
            {
                //if the login process failed
                return false;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:PowerShare.ViewModels.LoginScreenViewModel"/> is register.
        /// </summary>
        /// <value><c>true</c> if register; otherwise, <c>false</c>.</value>
        public bool Register
        {
            get => _register;
            set { SetProperty(ref _register, value); }
        }

        /// <summary>
        /// Gets or sets the login button opacity.
        /// </summary>
        /// <value>The login button opacity.</value>
        public double LoginButtonOp
        {
            get => _loginButtonOp;
            set { SetProperty(ref _loginButtonOp, value); }
        }

        /// <summary>
        /// Gets or sets the signup button opacity.
        /// </summary>
        /// <value>The signup button opacity.</value>
        public double SignupButtonOp
        {
            get => _signupButtonOp;
            set { SetProperty(ref _signupButtonOp, value); }
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string Action
        {
            get => _action;
            set { SetProperty(ref _action, value); }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        /// <summary>
        /// Gets or sets the size of the button font.
        /// </summary>
        /// <value>The size of the button font.</value>
        public double ButtonFontSize
        {
            get => _buttonFontSize;
            set { SetProperty(ref _buttonFontSize, value); }
        }
        #endregion
    }
}
