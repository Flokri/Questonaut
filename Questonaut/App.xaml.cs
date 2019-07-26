using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism.Unity;
using Prism;
using Prism.Ioc;
using Questonaut.Views;
using Questonaut.ViewModels;
using System;
using Questonaut.Helpers;
using Xamarin.Forms;
using Questonaut.Settings;
using Questonaut.Controller;
using Newtonsoft.Json;
using Firebase.Rest.Auth.Payloads;
using Plugin.CloudFirestore;
using Questonaut.DependencyServices;
using Akavache;
using Questonaut.views;
using Questonaut.viewmodels;
using Prism.Navigation;

namespace Questonaut
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            //intialize the akavache framework
            Akavache.Registrations.Start("Questonaut");

            //check if user is already logged in
            if (SettingsImp.UserValue != string.Empty)
            {
                //if there is a user logged in go to the mainscreen
                NavigationService.NavigateAsync(new System.Uri("https://www.Questonaut/MainView", System.UriKind.Absolute));
            }
            else
            {
                //if there is no user go to the login screen
                NavigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //todo: register all views with there corresponding viewmodels
            containerRegistry.RegisterForNavigation<CustomNavigationPage>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<CreateUserView, CreateUserViewModel>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<IntroView, IntroViewModel>();
            containerRegistry.RegisterForNavigation<FindAllStudiesView, FindAllStudiesViewController>();
        }

        protected override void OnStart()
        {
            //Start the appcenter services
            AppCenter.Start(
                String.Format("android={0};" +
                  "uwp={1};" +
                  "ios={2}", Secrets.AppCenter_Android_Secret, "Enter the AppCenter UWP Secret", Secrets.AppCenter_iOS_Secret),
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            BlobCache.Shutdown().Wait();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public INavigationService GetNavigationService() => this.NavigationService;
    }
}
