using System;
using FFImageLoading.Forms.Platform;
using Foundation;
using PanCardView.iOS;
using Prism;
using Prism.Ioc;
using Shiny;
using Shiny.Jobs;
using UIKit;
using Questonaut.ShinyConfig.SetUp;

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
            // this needs to be loaded before EVERYTHING
            iOSShinyHost.Init(new ShinyStartupClass());

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();

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
            => JobManager.OnBackgroundFetch(completionHandler);

        public class iOSInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }
        }
    }
}
