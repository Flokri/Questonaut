using System;
using System.Linq;
using Com.OneSignal;
using FFImageLoading.Forms.Platform;
using Foundation;
using Microsoft.AppCenter.Crashes;
using PanCardView.iOS;
using Prism;
using Prism.Ioc;
using Questonaut.Helper;
using Questonaut.iOS.DependencyServices;
using UIKit;
using UserNotifications;

namespace Questonaut.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //initialize the shiny framework
            Shiny.iOSShinyHost.Init(new ServicesStartup());

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

            // Watch for notifications while the app is active
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            //initialize the firebase sdk
            Firebase.Core.App.Configure();

            //intialize the ffimageloading framework
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            //intialize the cards view framework
            CardsViewRenderer.Preserve();

            //using the ffloading for the standard Xamarin.Forms.Image
            CachedImageRenderer.InitImageSourceHandler();

            LoadApplication(new App(new iOSInitializer()));

            return base.FinishedLaunching(app, options);
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
                => Shiny.Jobs.JobManager.OnBackgroundFetch(completionHandler);

        public class iOSInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }
        }
    }
}
