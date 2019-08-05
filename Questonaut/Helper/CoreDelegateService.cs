using System;
using System.Threading.Tasks;
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
            await this.Notifications.Send(title, message);
        }
    }
}
