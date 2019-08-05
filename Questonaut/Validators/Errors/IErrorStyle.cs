using System;
using Xamarin.Forms;

namespace Questonaut.Validators.Errors
{
    public interface IErrorStyle
    {
        void ShowError(View view, string message);
        void RemoveError(View view);
    }
}
