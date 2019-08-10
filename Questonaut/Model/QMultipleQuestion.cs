using System.Collections.Generic;

namespace Questonaut.Model
{
    /// <summary>
    /// This type represents a multiple choice question.
    /// </summary>
    public class QMultipleQuestion : QQuestion
    {
        /// <summary>
        /// A list of tick questions.
        /// </summary>
        public List<string> Answers { get; set; }
    }
}
