using System;
namespace Questonaut.Model
{
    public class QSlider : QQuestion
    {
        /// <summary>
        /// The slider max value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// The slider min value.
        /// </summary>
        public int MinValue { get; set; }
    }
}
