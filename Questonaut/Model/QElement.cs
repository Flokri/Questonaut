using System;
using Plugin.CloudFirestore.Attributes;

namespace Questonaut.Model
{
    public class QElement
    {
        public static string CollectionPath = "Elements";

        /// <summary>
        /// The Id of the entry.
        /// </summary>
        [Id]
        public string ID { get; set; }

        /// <summary>
        /// The title of the question object
        /// </summary>
        public string LinkToUserElement { get; set; }

        /// <summary>
        /// The text of the question object
        /// </summary>
        public string LinkToContext { get; set; }

        /// <summary>
        /// This is the description of the element the activity points at.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Specifies how often the element can occure on one day.
        /// </summary>
        public int RepeatPerDay { get; set; }
    }
}
