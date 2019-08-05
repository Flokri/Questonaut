using System;
using Xamarin.Forms;
using Questonaut.Validators.Errors;
using Questonaut.Validators;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Questonaut.Behaviors
{
    public class ValidationBehavior : Behavior<View>
    {
        #region Instances
        private IErrorStyle _style;
        private View _view;
        #endregion

        #region Constructor
        public ValidationBehavior()
        {
            _style = new BasicErrorStyle();
            Validators = new ObservableCollection<IValidator>();
        }
        #endregion

        #region PublicMethods
        public bool Validate()
        {
            bool isValid = true;
            string errorMessage = "";

            foreach (IValidator validator in Validators)
            {
                bool result = validator.Check(_view.GetType()
                                       .GetProperty(PropertyName)
                                       .GetValue(_view) as String);

                isValid = isValid && result;

                if (!result)
                {
                    errorMessage = validator.Message;
                    break;
                }
            }

            if (!isValid)
            {
                _style.ShowError(_view, errorMessage);
                return false;
            }
            else
            {
                _style.RemoveError(_view);
                return true;
            }
        }
        #endregion

        #region ProtectedMethods
        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);

            _view = bindable as View;
            _view.PropertyChanged += OnPropertyChanged;
            _view.Unfocused += OnUnFocused;
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            base.OnDetachingFrom(bindable);

            _view.PropertyChanged -= OnPropertyChanged;
            _view.Unfocused -= OnUnFocused;
        }
        #endregion

        #region Events
        void OnUnFocused(object sender, FocusEventArgs e)
        {
            Validate();
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                Validate();
            }
        }
        #endregion

        #region Properties
        public string PropertyName { get; set; }
        public ObservableCollection<IValidator> Validators { get; set; }
        #endregion
    }
}
