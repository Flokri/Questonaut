using System;
using System.Text.RegularExpressions;

namespace Questonaut.Validators
{
    public class FormatValidator : IValidator
    {
        #region Instances
        #endregion

        #region Constructor
        public FormatValidator()
        {
        }
        #endregion

        #region PublicMethods
        public bool Check(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Regex format = new Regex(Format);

                return format.IsMatch(value);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Properties
        public string Message { get; set; } = "Invalid format";
        public string Format { get; set; }
        #endregion
    }
}
