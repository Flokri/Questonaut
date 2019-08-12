using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Com.OneSignal;
using Firebase.Rest.Auth.Payloads;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Questonaut.Helpers;
using Questonaut.Model;
using Refit;
using Shiny.Notifications;
using Shiny.Sensors;

namespace Questonaut.Helper
{
    public class CoreDelegateServices
    {
        public CoreDelegateServices(INotificationManager notifications, IPedometer pedometer)
        {
            this.Pedometer = pedometer;
            this.Notifications = notifications;
        }

        public IPedometer Pedometer { get; }
        public INotificationManager Notifications { get; }

        /// <summary>
        /// Send a notification to the user.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message/body of the notification.</param>
        /// <returns>Returns a event containing the notification sending task.</returns>
        public async Task SendNotification(string title, string message)
        {
            try
            {
                OneSignal.Current.IdsAvailable(new Com.OneSignal.Abstractions.IdsAvailableCallback(async (id, arg) =>
                {
                    var post = new HttpPost()
                    {
                        app_id = Secrets.ONESIGNAL_APP_ID,
                        contents = new Contents() { en = message },
                        headings = new Headings() { en = title },
                        ios_badgeType = "Increase",
                        ios_badgeCount = "1",
                        include_player_ids = new List<string>() { id }
                    };

                    var apiCall = RestService.For<IOnesignalNotficafionAPI>(HttpPost.url);
                    var response = apiCall.SendNotification(post, "Basic " + Secrets.ONESIGNAL_AUTHORIZATION_HEADER);
                }));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

        }
    }
}
