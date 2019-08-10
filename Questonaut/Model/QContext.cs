using System;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Xamarin.Essentials;

namespace Questonaut.Model
{
    public class QContext
    {
        public static string CollectionPath = "Context";

        /// <summary>
        /// The id of the context.
        /// </summary>
        [Id]
        public string ID { get; set; }

        /// <summary>
        /// The location of the user-
        /// </summary>
        public GeoPoint Location { get; set; }

        /// <summary>
        /// The batter state.
        /// </summary>
        public string Battery { get; set; }

        /// <summary>
        /// The start time of the event.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The end time of the event.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// This is the name of the location geo point.
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// The action based on the location.
        /// Enter -> The user entered the specified location.
        /// Leave -> The user leaves the specified location.
        /// </summary>
        public string LocationAction { get; set; }
    }
}
