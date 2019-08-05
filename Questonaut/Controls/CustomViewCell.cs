using System;
using Xamarin.Forms;

namespace Questonaut.Controls
{
    public class CustomViewCell : ViewCell
    {
        public static readonly BindableProperty AllowHighlightProperty =
            BindableProperty.Create(
                "AllowHighlight", typeof(bool), typeof(CustomViewCell),
                defaultValue: true);

        public bool AllowHighlight
        {
            get { return (bool)GetValue(AllowHighlightProperty); }
            set { SetValue(AllowHighlightProperty, value); }
        }
    }
}
