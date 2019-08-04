using System;
using System.Diagnostics;
using Questonaut.DependencyServices;
using Questonaut.iOS.DependencyServices;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace Questonaut.iOS.DependencyServices
{
    class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert);
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
}
