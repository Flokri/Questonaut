using System;
namespace Questonaut.Model
{
    public class QActivity
    {
        /// <summary>
        /// The id of the activity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the activity.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date and time of the occurence of the activity.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The description of the activity.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The status of the activity
        /// </summary>
        public string Status { get; set; }
    }
}
