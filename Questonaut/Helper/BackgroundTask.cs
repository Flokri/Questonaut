using System;
using System.Threading;
using System.Threading.Tasks;
using Questonaut.DependencyServices;
using Questonaut.Messages;
using Xamarin.Forms;

namespace Questonaut.Helper
{
    public class BackgroundTask
    {
        public async Task RunCounter(CancellationToken token)
        {
            await Task.Run(async () =>
            {

                for (long i = 0; i < long.MaxValue; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await Task.Delay(60000);
                    var message = new UpdateMessage
                    {
                        Message = i.ToString() + "min"
                    };

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        MessagingCenter.Send<UpdateMessage>(message, "UpdateMessage");
                        Xamarin.Forms.DependencyService.Get<INotificationService>().SendText("Time to answer a question!", "The Questonaut team has a question for you 📝");
                    });
                }
            }, token);
        }
    }
}
