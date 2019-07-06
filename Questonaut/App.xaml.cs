using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Autofac;
using Prism.Unity;
using Questonaut.Configuration.Types;
using Prism;
using Prism.Ioc;
using Questonaut.Views;
using Questonaut.ViewModels;

namespace Questonaut
{
    public partial class App : PrismApplication
    {
        private static readonly IContainer _container = Configuration.Reader.Container.Initialize();
        private string _appSecret;

        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            //reading the config
            using (var scope = _container.BeginLifetimeScope())
            {
                var appCenterService = scope.Resolve<IAppCenterConfig>();
                _appSecret = appCenterService.GetAppSecret();
            }

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
            AppCenter.Start(_appSecret,
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
