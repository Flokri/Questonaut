using System;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;
using Questonaut.ShinyConfig.Delegates;
using Shiny;
using Shiny.Jobs;

namespace Questonaut.ShinyConfig.SetUp
{
    public class ShinyStartupClass : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            //Use notifications
            services.UseNotifications();

            //Use the build in pedometer sensor
            services.UsePedometer();

            //Core Delegate
            services.AddSingleton<CoreDelegateServices>();

            //configuration
            services.RegisterSettings<IAppSettings, AppSettings>("AppSettings");

            //creating a quick scheduled Job
            var job = new JobInfo
            {
                Identifier = "Test",
                Type = typeof(ScheduledJob),
                Repeat = true,
                BatteryNotLow = false,
                DeviceCharging = false,
                RequiredInternetAccess = (InternetAccess)Enum.Parse(typeof(InternetAccess), InternetAccess.None.ToString())
            };

            job.SetParameter("Loops", 10);

            services.RegisterJob(job);
        }
    }
}
