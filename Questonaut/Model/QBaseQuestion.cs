using System;
using Plugin.CloudFirestore.Attributes;

namespace Questonaut.Model
{
    public class QBaseQuestion
    {
        public static string CollectionPath = "Questions";

        /// <summary>
        /// The firebase id.
        /// </summary>
        [Id]
        public string ID { get; set; }

        /// <summary>
        /// The type of the user element.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The Link to the element in which the question is located.
        /// </summary>
        [Ignored]
        public string ElementId { get; set; }
    }
}
