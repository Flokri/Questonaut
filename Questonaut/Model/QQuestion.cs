using System;
using Plugin.CloudFirestore.Attributes;

namespace Questonaut.Model
{
    public class QQuestion : QBaseQuestion
    {
        /// <summary>
        /// The title of the question
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The body of the question
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The answer to this question.
        /// </summary>
        [Ignored]
        public string Answer { get; set; }
    }
}
