using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Firebase.Auth;
using Firebase.Auth.Payloads;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.Configuration.Types;
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

        private FirebaseAuthService _firebase;
        #endregion

        #region DependencyInjection
        INavigationService _navigationService;
        IPageDialogService _pageDialogservice;

        private static readonly IContainer _container = Configuration.Reader.Container.Initialize();
        #endregion

        #region Commands
        public DelegateCommand OnActionClickedCommand { get; set; }
        public DelegateCommand OnLoginClickedCommand { get; set; }
        public DelegateCommand OnSignupClickedCommand { get; set; }
        public DelegateCommand OnTappedForgotPassword { get; set; }
        #endregion

        #region Constructor
        public LoginViewModel(INavigationService navigationService, IPageDialogService dialogService)
        {
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            OnActionClickedCommand = new DelegateCommand(() => ActionClickedAsync());
            OnLoginClickedCommand = new DelegateCommand(() => OnLogin());
            OnSignupClickedCommand = new DelegateCommand(async () => await Task.Run(() => OnSignup()));
            OnTappedForgotPassword = new DelegateCommand(() => OnForgotPassword());

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

            //reading the config
            using (var scope = _container.BeginLifetimeScope())
            {
                _firebase = new FirebaseAuthService(new FirebaseAuthOptions() { WebApiKey = scope.Resolve<IFirebaseAuthConfig>().GetWebAPI() });
            }
            //initialize the firebase rest api


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

        private void OnForgotPassword()
        {

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
                    if (await LoginAsync() == true)
                    {
                        //TODO: pass param
                        //change to the dashboard
                    }

                    break;
                case "Signup":
                    if (await SignupAsync() == true)
                    {
                        //TODO: pass param
                        //change to the dashboard
                    }
                    break;
                default:
                    break;
            }
        }

        private bool CheckIfVerified(GetUserDataResponse user)
        {
            if (user != null && user.users.Count > 0)
            {
                return user.users[0].emailVerified;
            }

            return false;
        }

        private string GetRigthErrorMessage(FirebaseAuthException e)
        {
            if (e.Message.Contains("EMAIL_NOT_FOUND") || e.Message.Contains("INVALID_PASSWORD"))
            {
                return "Email or/and Password incorrect.";
            }
            else if (e.Message.Contains("EMAIL_EXISTS"))
            {
                return "There is already a user with this email address.";
            }
            else if (e.Message.Contains("WEAK_PASSWORD"))
            {
                return "The password should be at least 6 characters long.";
            }

            return e.Message;
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
                try
                {
                    var response = await _firebase.SignUpNewUser(new SignUpNewUserRequest()
                    {
                        Email = this.Email,
                        Password = this.Password,
                    });

                    if (response != null)
                    {
                        var sendverification = await _firebase.SendVerification(new SendVerificationEmailRequest()
                        {
                            RequestType = "VERIFY_EMAIL",
                            IdToken = response.IdToken,
                        });

                        if (sendverification != null)
                        {
                            _pageDialogservice.DisplayAlertAsync("Verify", "Your account was created successfully. Please confirm the account with the mail we send to you.", "Cancel");
                            return true;
                        }
                    }
                }
                catch (FirebaseAuthException e)
                {
                    _pageDialogservice.DisplayAlertAsync("Error", GetRigthErrorMessage(e), "Cancel");
                    return false;
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
                var request = new VerifyPasswordRequest()
                {
                    Email = this.Email,
                    Password = this.Password,
                };

                try
                {
                    var response = await _firebase.VerifyPassword(request);
                    if (response != null)
                    {
                        var userData = await _firebase.GetUserData(new GetUserDataRequest() { IdToken = response.IdToken });
                        if (CheckIfVerified(userData))
                            return true;
                        else
                        {
                            if (await _pageDialogservice.DisplayAlertAsync("Not Verified", "To use this account please verfiy your email address.", "Resend Email", "Cancel"))
                            {
                                var sendverification = await _firebase.SendVerification(
                                    new SendVerificationEmailRequest()
                                    {
                                        RequestType = "VERIFY_EMAIL",
                                        IdToken = response.IdToken,
                                    });
                            }
                        }
                    }
                }
                catch (FirebaseAuthException e)
                {
                    _pageDialogservice.DisplayAlertAsync("Error", GetRigthErrorMessage(e), "Cancel");
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
