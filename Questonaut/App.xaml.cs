using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//AppCenter usings
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Questonaut
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            //Start the appcenter services
            AppCenter.Start("ios={enter iOS App seccret here};" +
                  "uwp={enter UWP App seccret here};" +
                  "android={enter Android App seccret here}",
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
