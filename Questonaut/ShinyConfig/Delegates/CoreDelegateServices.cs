using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shiny;
using Shiny.Notifications;

namespace Questonaut.ShinyConfig.Delegates
{
    public class CoreDelegateServices
    {
        public CoreDelegateServices(INotificationManager notifications,
                                    IAppSettings appSettings)
        {
            this.Notifications = notifications;
            this.AppSettings = appSettings;
        }

        public INotificationManager Notifications { get; }
        public IAppSettings AppSettings { get; }


        public async Task SendNotification(string title, string message, Expression<Func<IAppSettings, bool>> expression = null)
        {
            var notify = expression == null
                ? true
                : this.AppSettings.ReflectGet(expression);

            if (notify)
                await this.Notifications.Send(title, message);
        }
    }
}
