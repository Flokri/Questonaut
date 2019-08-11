using System;
using System.Threading.Tasks;
using Shiny;
using Shiny.Jobs;
using Shiny.Locations;
using Shiny.Notifications;
using Xamarin.Forms.Internals;

namespace Questonaut.Helper
{
    public class GeofenceDelegate : IGeofenceDelegate
    {
        #region instances
        private readonly CoreDelegateServices dependency;
        #endregion

        public GeofenceDelegate(CoreDelegateServices dependency)
        {
            this.dependency = dependency;
        }

        public async Task OnStatusChanged(GeofenceState newStatus, GeofenceRegion region)
        {
            string[] split = region.Identifier.Split('|');

            if (split.Length == 2)
            {
                if (newStatus == GeofenceState.Entered && split[1].Equals("Enter"))
                {
                    //await this.dependency.SendNotification("WELCOME!", "It is good to have you back " + split[0]);

                    await CheckContext(region);

                }
                else if (newStatus == GeofenceState.Exited && split[1].Equals("Leave"))
                {
                    //await this.dependency.SendNotification("BYE!", "I hope you had a good time at  " + split[0]);

                    await CheckContext(region);
                }
            }
        }

        private async Task<bool> CheckContext(GeofenceRegion region)
        {
            var job = new JobInfo
            {
                Identifier = "GeoContext",
                Type = typeof(CheckContextJob),
                Repeat = false,
                BatteryNotLow = false,
                DeviceCharging = false,
                RequiredInternetAccess = InternetAccess.Any
            };

            // you can pass variables to your job
            job.SetParameter<GeofenceRegion>("region", region);

            await ShinyHost.Resolve<Shiny.Jobs.IJobManager>().Schedule(job);

            var result = await Shiny.ShinyHost.Resolve<Shiny.Jobs.IJobManager>().Run("GeoContext");

            return result.HasNewData;
        }
    }
}
