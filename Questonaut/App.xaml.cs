using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Autofac;
using Questonaut.config.configtypes;
using Prism.Unity;
using Prism;
using Prism.Ioc;

namespace Questonaut
{
    public partial class App : PrismApplication
    {
        private static readonly IContainer _container = config.configreader.Container.Initialize();
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
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

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
