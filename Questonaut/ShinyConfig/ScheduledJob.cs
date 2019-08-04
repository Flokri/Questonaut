using System;
using System.Threading;
using System.Threading.Tasks;
using Questonaut.Controller;
using Questonaut.ShinyConfig.Delegates;
using Shiny;
using Shiny.Jobs;
using Shiny.Sensors;
using Xamarin.Essentials;

namespace Questonaut.ShinyConfig
{
    public class ScheduledJob : IJob
    {
        readonly CoreDelegateServices services;
        readonly IPedometer pedometer;
        public ScheduledJob(CoreDelegateServices services, IPedometer pedometer)
        {
            this.services = services;
            this.pedometer = pedometer;
        }

        public async Task<bool> Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            await this.services.SendNotification(
                    "Job Started",
                    $"{jobInfo.Identifier} Started",
                    x => x.UseNotificationsJobStart
                );

            //pedometer stuff
            string value = "";

            IDisposable test = this.pedometer
                .WhenReadingTaken()
                .Subscribe(x => value = x.ToString());

            //geolocation stuff
            string pos = "";
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    pos = ($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }

            //time stuff
            var now = DateTime.Now.ToShortTimeString();

            //battery stuff
            var level = Battery.ChargeLevel; // returns 0.0 to 1.0 or 1.0 when on AC or no battery.

            var state = Battery.State;

            switch (state)
            {
                case BatteryState.Charging:
                    // Currently charging
                    break;
                case BatteryState.Full:
                    // Battery is full
                    break;
                case BatteryState.Discharging:
                case BatteryState.NotCharging:
                    // Currently discharging battery or not being charged
                    break;
                case BatteryState.NotPresent:
                // Battery doesn't exist in device (desktop computer)
                case BatteryState.Unknown:
                    // Unable to detect battery state
                    break;
            }

            await this.services.SendNotification(
                "Job Finished " + CurrentUser.Instance.User.Name,
                $"{jobInfo.Identifier} " + value + " " + pos + " " + now + " " + level.ToString() + " " + state.ToString(),
                x => x.UseNotificationsJobFinish
            );

            // you really shouldn't lie about this on iOS as it is watching :)
            return true;
        }
    }
}
