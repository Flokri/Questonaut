using System;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Jobs;

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

            //register the background context checker job
            services.RegisterJob(job);
        }
    }
}
