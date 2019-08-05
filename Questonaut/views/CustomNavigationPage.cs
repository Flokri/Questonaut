using System;
using Xamarin.Forms;

namespace Questonaut.Views
{
    public class CustomNavigationPage : Xamarin.Forms.NavigationPage
    {
        public CustomNavigationPage(Page page) : base(page)
        {
            this.BackgroundColor = Color.Transparent;
            this.BarTextColor = Color.FromHex("#42506B");

            this.MinimumHeightRequest = 0;
        }
    }
}
