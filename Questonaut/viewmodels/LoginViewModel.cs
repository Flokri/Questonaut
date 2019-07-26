using System;
using System.Threading.Tasks;
using Firebase.Rest.Auth;
using Firebase.Rest.Auth.Payloads;
using Newtonsoft.Json;
using Plugin.CloudFirestore;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Questonaut.DependencyServices;
using Questonaut.Helpers;
using Questonaut.Model;
using Questonaut.Settings;
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
            //set the prism stuff
            _navigationService = navigationService;
            _pageDialogservice = dialogService;

            //set all the delegates command using by the ui
            OnActionClickedCommand = new DelegateCommand(() => ActionClickedAsync());
            OnLoginClickedCommand = new DelegateCommand(() => OnLogin());
            OnSignupClickedCommand = new DelegateCommand(async () => await Task.Run(() => OnSignup()));
            OnTappedForgotPassword = new DelegateCommand(() => OnForgotPassword());

            //change the button size depending on the actual screen size
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

            //intialize the firebase rest api
            _firebase = new FirebaseAuthService(new FirebaseAuthOptions() { WebApiKey = Secrets.Firebase_Auth_Secret });

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

        /// <summary>
        /// Get called when the user tappes forgot password.
        /// </summary>
        private void OnForgotPassword()
        {
            //todo: add feature 
        }

        /// <summary>
        /// Get called when the user wants to signup a new user account
        /// </summary>
        /// <returns></returns>
        private Task OnSignup()
        {
            SignupActive();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get called when the user clicks the action (login or signup) button.
        /// </summary>
        /// <returns></returns>
        private async Task ActionClickedAsync()
        {
            switch (Action)
            {
                case "Login":
                    if (await LoginAsync() == true)
                    {
                        //check if the user has a fully configured account
                        if (await CeckAccountStatusAsync())
                        {
                            //check if this is the first login
                            if (SettingsImp.ShowIntro.Equals("true"))
                            {
                                //change to the create a user view
                                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/IntroView", System.UriKind.Absolute));
                            }
                            else
                            {
                                //change to the main view
                                await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/MainView", System.UriKind.Absolute));
                            }
                        }
                        else
                        {
                            await ChangeToCreationAsync();
                        }
                    }

                    break;
                case "Signup":
                    if (await SignupAsync() == true)
                    {
                        await ChangeToCreationAsync();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Navigate to the creat a user view.
        /// </summary>
        private async Task ChangeToCreationAsync()
        {
            SettingsImp.ShowIntro = "true";

            //change to the create a user view
            await _navigationService.NavigateAsync(new System.Uri("https://www.Questonaut/CreateUserView", System.UriKind.Absolute));
        }

        /// <summary>
        /// Check if a user account is just created or fully configured.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CeckAccountStatusAsync()
        {
            try
            {
                var query = await CrossCloudFirestore.Current
                                     .Instance
                                     .GetCollection(QUser.CollectionPath)
                                     .WhereEqualsTo("Email", this.Email)
                                     .GetDocumentsAsync();

                if (query.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the user has verified his/her email address.
        /// </summary>
        /// <param name="user">The user form the firebase response.</param>
        /// <returns>Returns the verify status of the user.</returns>
        private bool CheckIfVerified(GetUserDataResponse user)
        {
            if (user != null && user.users.Count > 0)
            {
                return user.users[0].emailVerified;
            }

            return false;
        }

        /// <summary>
        /// Converts the firebase api auth call to a full text error message.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>The full text error message.</returns>
        private string GetRigthErrorMessage(FirebaseAuthException e)
        {
            if (e.Message.Contains("EMAIL_NOT_FOUND") || e.Message.Contains("INVALID_PASSWORD") || e.Message.Contains("INVALID_EMAIL"))
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
                            await _pageDialogservice.DisplayAlertAsync("Verify", "Your account was created successfully. Please confirm the account with the mail we send to you.", "Ok");
                            return true;
                        }
                    }
                }
                catch (FirebaseAuthException e)
                {
                    await _pageDialogservice.DisplayAlertAsync("Error", GetRigthErrorMessage(e), "Cancel");
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

                        if (userData != null)
                        {
                            //save a flag to the application data to know that a user is logged in
                            SettingsImp.UserValue = JsonConvert.SerializeObject(userData.users[0]);

                            if (CheckIfVerified(userData))
                            {
                                await Xamarin.Forms.DependencyService.Get<IFirebaseAuthenticator>().LoginWithEmailPassword(this.Email, this.Password);
                                return true;
                            }
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
                }
                catch (FirebaseAuthException e)
                {
                    await _pageDialogservice.DisplayAlertAsync("Error", GetRigthErrorMessage(e), "Cancel");
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
