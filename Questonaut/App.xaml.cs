using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//AppCenter usings
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Autofac;
using Questonaut.config.configreader;
using Questonaut.config.configtypes;

namespace Questonaut
{
    public partial class App : Application
    {
        private static readonly IContainer _container = Container.Initialize();
        private readonly string _appSecret;

        public App()
        {
            InitializeComponent();

            //reading the config
            using (var scope = _container.BeginLifetimeScope())
            {
                var appCenterService = scope.Resolve<IAppCenterConfig>();
                _appSecret = appCenterService.GetAppSecret();
            }

            MainPage = new MainPage();
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
