using System;
using Xamarin.Forms;

namespace Questonaut.Validators
{
    public class PasswordValidator : BindableObject, IValidator
    {
        #region Instances
        #endregion

        #region Constructor
        public PasswordValidator()
        {
        }
        #endregion

        #region PublicMethods
        public bool Check(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (PasswordValidation == null)
                    return false;

                return PasswordValidation.Equals(value);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region PrivateMethods
        private void ReCheck()
        {
            Check(PasswordValidation);
        }
        #endregion

        #region Properties
        public string Message { get; set; } = "The passwords does not match";

        public string PasswordValidation
        {
            get => (string)GetValue(PasswordValidationProperty);
            set => SetValue(PasswordValidationProperty, value);
        }
        #endregion

        #region BindableProperties
        public static readonly BindableProperty PasswordValidationProperty =
                               BindableProperty.Create(propertyName: nameof(PasswordValidation),
                                                       returnType: typeof(string),
                                                       declaringType: typeof(PasswordValidator),
                                                       defaultValue: "");
        #endregion

        #region EventHandler
        protected override void OnPropertyChanged(string propertyName)
        {
            /*base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(PasswordValidation):
                    ReCheck();
                    break;
            }*/
        }
        #endregion
    }
}
