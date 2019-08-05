using System;
using Questonaut.DependencyServices;
using Questonaut.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(ScreenDimensions))]
namespace Questonaut.Droid.DependencyServices
{
    /// <summary>
    /// The nactive Android way to get the screen dimensions of the current device.
    /// </summary>
    public class ScreenDimensions : IScreenDimensions
    {
        public double GetScreenHeight()
        {
            return ((double)Android.App.Application.Context.Resources.DisplayMetrics.HeightPixels / (double)Android.App.Application.Context.Resources.DisplayMetrics.Density);
        }

        public double GetScreenWidth()
        {
            return ((double)Android.App.Application.Context.Resources.DisplayMetrics.WidthPixels / (double)Android.App.Application.Context.Resources.DisplayMetrics.Density);
        }
    }
}
