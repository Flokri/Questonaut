using System;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Questonaut.ShinyConfig
{
    public class AppSettings : ReactiveObject, IAppSettings
    {
        public AppSettings()
        {
            this.WhenAnyValue(
                    x => x.IsChecked,
                    x => x.YourText
                )
                .Skip(1)
                .Subscribe(_ =>
                    this.LastUpdated = DateTime.Now
                );
        }


        [Reactive] public bool IsChecked { get; set; }
        [Reactive] public string YourText { get; set; }
        [Reactive] public DateTime? LastUpdated { get; set; }

        [Reactive] public bool UseNotificationsBle { get; set; } = true;
        [Reactive] public bool UseNotificationsGeofenceEntry { get; set; } = true;
        [Reactive] public bool UseNotificationsGeofenceExit { get; set; } = true;
        [Reactive] public bool UseNotificationsJobStart { get; set; } = true;
        [Reactive] public bool UseNotificationsJobFinish { get; set; } = true;
        [Reactive] public bool UseNotificationsHttpTransfers { get; set; } = true;
        [Reactive] public bool UseNotificationsBeaconRegionEntry { get; set; } = true;
        [Reactive] public bool UseNotificationsBeaconRegionExit { get; set; } = true;
    }
}
