using System;

using Xamarin.Forms;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Plugin.CurrentActivity;
using Questonaut.Droid.DependencyServices;
using Questonaut.DependencyServices;

[assembly: Dependency(typeof(NotificationService))]
namespace Questonaut.Droid.DependencyServices
{
    public class NotificationService : INotificationService
    {
        static readonly string CHANNEL_ID = "my_channel01";
        public NotificationService()
        {
        }
        public void Init()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, CHANNEL_ID, NotificationImportance.High);
                NotificationManager notifiactionManager = CrossCurrentActivity.Current.Activity.GetSystemService(Context.NotificationService) as NotificationManager;
                notifiactionManager.CreateNotificationChannel(mChannel);
            }
        }

        public void SendText(string title, string body)
        {
            Notification.Builder notificationBuilder = null;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationBuilder = new Notification.Builder(CrossCurrentActivity.Current.Activity, CHANNEL_ID);
            }
            else
            {
                notificationBuilder = new Notification.Builder(CrossCurrentActivity.Current.Activity);
            }

            notificationBuilder.SetSmallIcon(Resource.Drawable.Questonaut)
                    .SetContentTitle(title)
                   .SetContentText(body)
                    .SetAutoCancel(true);

            // When the user clicks the notification, SecondActivity will start up.
            Intent resultIntent = new Intent(CrossCurrentActivity.Current.Activity, typeof(MainActivity));
            resultIntent.PutExtra("ID", 10);

            // Construct a back stack for cross-task navigation:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(CrossCurrentActivity.Current.Activity);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent =
                stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
            notificationBuilder.SetContentIntent(resultPendingIntent);

            Notification notification = notificationBuilder.Build();
            NotificationManager notificationManager =
                CrossCurrentActivity.Current.Activity.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(1, notification);
        }
    }
}
