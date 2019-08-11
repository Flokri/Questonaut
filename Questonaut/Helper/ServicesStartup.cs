using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Jobs;
using Shiny.Locations;
using Xamarin.Forms;

namespace Questonaut.Helper
{
    public class ServicesStartup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //register services
            services.AddSingleton<CoreDelegateServices>();

            //use notifications
            services.UseNotifications();

            //use the pedometer
            services.UsePedometer();

            //setting up the geofence delegate
            services.UseGeofencing<GeofenceDelegate>();

            //create the background context checker job
            var job = new JobInfo
            {
                Identifier = "CheckContext",
                Type = typeof(CheckContextJob),
                Repeat = true,
                BatteryNotLow = false,
                DeviceCharging = false,
                RequiredInternetAccess = InternetAccess.Any
            };

            job.SetParameter<GeofenceRegion>("region", null);

            //create the a job to upload the activities
            var uploadJob = new JobInfo
            {
                Identifier = "UploadActivities",
                Type = typeof(UploadActivitiesJob),
                Repeat = true,
                BatteryNotLow = false,
                DeviceCharging = false,
                RequiredInternetAccess = InternetAccess.Any
            };

            //register the background context checker and upload job
            services.RegisterJob(job);
            services.RegisterJob(uploadJob);
        }
    }
}
