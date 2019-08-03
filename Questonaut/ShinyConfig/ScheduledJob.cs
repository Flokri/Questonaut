using System;
using System.Threading;
using System.Threading.Tasks;
using Questonaut.Controller;
using Questonaut.ShinyConfig.Delegates;
using Shiny;
using Shiny.Jobs;

namespace Questonaut.ShinyConfig
{
    public class ScheduledJob : IJob
    {
        readonly CoreDelegateServices services;
        public ScheduledJob(CoreDelegateServices services) => this.services = services;

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await this.services.SendNotification(
                    "Job Started",
                    $"{jobInfo.Identifier} Started",
                    x => x.UseNotificationsJobStart
                );

            var loops = jobInfo.Parameters.Get("Loops", 10);
            for (var i = 0; i < loops; i++)
            {
                if (cancelToken.IsCancellationRequested)
                    break;

                await Task.Delay(1000, cancelToken).ConfigureAwait(false);
            }
            await this.services.SendNotification(
                "Job Finished " + CurrentUser.Instance.User.Name,
                $"{jobInfo.Identifier} Finished",
                x => x.UseNotificationsJobFinish
            );

            // you really shouldn't lie about this on iOS as it is watching :)
            return true;
        }
    }
}
