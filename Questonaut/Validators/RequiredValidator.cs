using System;
namespace Questonaut.Validators
{
    public class RequiredValidator : IValidator
    {
        #region Instances
        #endregion

        #region Constructor
        public RequiredValidator()
        {
        }
        #endregion

        #region PublicMethods
        public bool Check(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        #endregion

        #region Properties
        public string Message { get; set; } = "This field is required";
        #endregion
    }
}
