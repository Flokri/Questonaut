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

namespace Questonaut
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            //navigate to the root view controller of this app
            NavigationService.NavigateAsync(new System.Uri("https://www.Questonaut/LoginView", System.UriKind.Absolute));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //todo: register all views with there corresponding viewmodels
            containerRegistry.RegisterForNavigation<CustomNavigationPage>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
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
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
