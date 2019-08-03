using System;
using System.Threading;
using System.Threading.Tasks;
using Questonaut.Helper;
using Questonaut.Messages;
using UIKit;
using Xamarin.Forms;

namespace Questonaut.iOS.Services
{
    public class iOSBackgroundService
    {
        nint _taskId;
        CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();

            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("BackgroundTask", OnExpiration);

            try
            {
                //INVOKE THE SHARED CODE
                var counter = new BackgroundTask();
                await counter.RunCounter(_cts.Token);

            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    var message = new CancelledMessage();
                    Device.BeginInvokeOnMainThread(
                        () => MessagingCenter.Send(message, "CancelledMessage")
                    );
                }
            }

            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            _cts.Cancel();
        }
    }
}
