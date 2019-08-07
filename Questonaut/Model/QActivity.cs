using System;
using SQLite;

namespace Questonaut.Model
{
    public class QActivity
    {
        /// <summary>
        /// The id of the activity.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// The id of the study
        /// </summary>
        public string ElementId { get; set; }

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

        /// <summary>
        /// The link to the element in firebase.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// The type of the activity.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The answer to the specific question.
        /// </summary>
        public string Answer { get; set; }
    }
}
