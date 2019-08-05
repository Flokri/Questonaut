using System;
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
        public string Location { get; set; }

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
    }
}
