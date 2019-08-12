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
using Questonaut.Settings;
using Akavache;
using Questonaut.views;
using Questonaut.viewmodels;
using Prism.Navigation;
using Questonaut.views.StudyScreensViews;
using Questonaut.viewmodels.StudyScreensViewModels;
using Questonaut.Model;
using Shiny.Jobs;
using Xamarin.Forms;
using Questonaut.Helper;
using Shiny;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Shiny.Locations;
using Shiny.Notifications;
using Questonaut.Controller;
using Plugin.CloudFirestore;
using System.Collections.Generic;
using Questonaut.Views.StudyScreensViews;
using Questonaut.ViewModels.StudyScreensViewModels;

namespace Questonaut
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();


            //initialize the one signal framework
            OneSignal.Current.StartInit(Secrets.ONESIGNAL_APP_ID)
                             .EndInit();

            //intialize the akavache framework
            Akavache.Registrations.Start("Questonaut");

            //check if user is already logged in
            if (SettingsImp.UserValue != string.Empty && CurrentUser.Instance.User != null && CurrentUser.Instance.User.Email != null && !CurrentUser.Instance.User.Email.Equals(""))
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
            containerRegistry.RegisterForNavigation<FindAllStudiesView, FindAllStudiesViewModel>();
            containerRegistry.RegisterForNavigation<StudyDetailView, StudyDetailViewModel>();
            containerRegistry.RegisterForNavigation<TextEntryView, TextEntryViewModel>();
            containerRegistry.RegisterForNavigation<MultipleChoiceView, MultipleChoiceViewModel>();
            containerRegistry.RegisterForNavigation<SliderEntryView, SliderEntryViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
        }

        protected override void OnStart()
        {
            //register the one signal
            OneSignal.Current.RegisterForPushNotifications();

            //Handle when the onesignal notification is received
            OneSignal.Current.StartInit(Secrets.ONESIGNAL_APP_ID)
            .HandleNotificationReceived(HandleNotificationReceived)
            .InFocusDisplaying(OSInFocusDisplayOption.None)
            .EndInit();

            //Start the appcenter services
            AppCenter.Start(
                String.Format("android={0};" +
                  "uwp={1};" +
                  "ios={2}", Secrets.AppCenter_Android_Secret, "Enter the AppCenter UWP Secret", Secrets.AppCenter_iOS_Secret),
                  typeof(Analytics), typeof(Crashes));

            //Register the geofences from the studies
            //check if a user is logged in
            if (SettingsImp.UserValue != string.Empty)
                RegisterGeofences();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            BlobCache.Shutdown().Wait();
        }

        protected override void OnResume()
        {
            // Handle when your app resume       
        }

        // Called when your app is in focus and a notificaiton is recieved.
        // The name of the method can be anything as long as the signature matches.
        // Method must be static or this object should be marked as DontDestroyOnLoad
        private static async void HandleNotificationReceived(OSNotification notification)
        {
            OSNotificationPayload payload = notification.payload;

            try
            {
                if (payload.additionalData.ContainsKey("action"))
                {
                    payload.additionalData["action"].Equals("checkContext");

                    var results = await Shiny.ShinyHost.Resolve<Shiny.Jobs.IJobManager>().RunAll();
                }
            }
            catch (Exception e) { }
        }

        private async void RegisterGeofences()
        {
            var geofences = ShinyHost.Resolve<IGeofenceManager>();

            foreach (QStudy study in CurrentUser.Instance.User.ActiveStudiesObjects)
            {
                var elementsDoc = await CrossCloudFirestore.Current
                       .Instance
                       .GetCollection(QStudy.CollectionPath + "/" + study.Id + "/" + QElement.CollectionPath)
                       .GetDocumentsAsync();

                IEnumerable<QElement> elements = elementsDoc.ToObjects<QElement>();

                // this is really only required on iOS, but do it to be safe
                if (elements != null)
                {
                    foreach (QElement element in elements)
                    {
                        var contextDoc = await CrossCloudFirestore.Current
                            .Instance
                            .GetCollection(QContext.CollectionPath)
                            .GetDocument(element.LinkToContext)
                            .GetDocumentAsync();
                        QContext context = contextDoc.ToObject<QContext>();

                        if (context.Location != null)
                        {
                            await geofences.StartMonitoring(new GeofenceRegion(
                                context.LocationName + "|" + context.LocationAction,
                            new Position(context.Location.Latitude, context.Location.Longitude),
                            Distance.FromMeters(200))
                            {
                                NotifyOnEntry = true,
                                NotifyOnExit = true,
                                SingleUse = false
                            });
                        }
                    }
                }
            }
        }

        public INavigationService GetNavigationService() => this.NavigationService;
    }
}
