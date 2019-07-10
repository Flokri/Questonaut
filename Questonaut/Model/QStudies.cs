using System;
using Plugin.CloudFirestore.Attributes;

namespace Questonaut.Model
{
    /// <summary>
    /// Represents a questonaut study.
    /// </summary>
    public class QStudies
    {
        public static string CollectionPath = "Studies";

        /// <summary>
        /// The uid from the study. This uid comes from firebase auth.
        /// </summary>
        [Id]
        public string Id { get; set; }

        /// <summary>
        /// The name of the study.
        /// </summary>
        public string Name { get; set; }
    }
}
