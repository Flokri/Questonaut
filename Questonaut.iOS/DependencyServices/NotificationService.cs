using System;
using System.Diagnostics;

using Xamarin.Forms;

using UIKit;
using UserNotifications;
using Questonaut.iOS.DependencyServices;
using Questonaut.DependencyServices;

[assembly: Dependency(typeof(NotificationService))]
namespace Questonaut.iOS.DependencyServices
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            switch (response.ActionIdentifier)
            {
                default:
                    if (response.IsDefaultAction)
                    {
                        Debug.WriteLine("Default action");
                    }
                    else if (response.IsDismissAction)
                    {
                        Debug.WriteLine("Dismiss Action");
                    }
                    break;
            }

            completionHandler();
        }
    }

    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
        }

        public void Init()
        {
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge, (approved, err) =>
            {
                //Handle approval
            });
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
        }


        public void SendText(string title, string description)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    var allowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                    if (allowed)
                    {
                        Debug.WriteLine($"Creating text nofification: {title}");
                        var content = new UNMutableNotificationContent
                        {
                            Title = title,
                            Body = description,
                        };
                        var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);
                        var request = UNNotificationRequest.FromIdentifier(
                            "MyNotification",
                            content,
                            trigger
                        );
                        UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
                        {
                            if (err != null)
                                Debug.WriteLine(err);
                        });
                    }
                });
            }
            else
                Debug.WriteLine("Not supported");
        }
    }
}
