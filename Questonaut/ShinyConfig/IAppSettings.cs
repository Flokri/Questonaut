using System;
namespace Questonaut.ShinyConfig
{
    public interface IAppSettings
    {
        bool UseNotificationsBle { get; set; }
        bool UseNotificationsGeofenceEntry { get; set; }
        bool UseNotificationsGeofenceExit { get; set; }
        bool UseNotificationsJobStart { get; set; }
        bool UseNotificationsJobFinish { get; set; }
        bool UseNotificationsHttpTransfers { get; set; }
        bool UseNotificationsBeaconRegionEntry { get; set; }
        bool UseNotificationsBeaconRegionExit { get; set; }
    }
}
