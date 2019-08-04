using System;
namespace Questonaut.DependencyServices
{
    public interface INotificationService
    {
        /// <summary>
        /// Send a notification with a specific title and a body.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="body">The body of the notification.</param>
        void SendText(string title, string body);

        /// <summary>
        /// The initialization of the notifciation service.
        /// </summary>
        void Init();
    }
}
