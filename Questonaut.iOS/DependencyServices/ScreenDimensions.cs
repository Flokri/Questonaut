using System;
using Questonaut.DependencyServices;
using Questonaut.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ScreenDimensions))]
namespace Questonaut.iOS.DependencyServices
{
    /// <summary>
    /// The native iOS way to get the screen dimensions of the current device.
    /// </summary>
    public class ScreenDimensions : IScreenDimensions
    {
        public double GetScreenHeight()
        {
            return (double)UIScreen.MainScreen.Bounds.Height;
        }

        public double GetScreenWidth()
        {
            return (double)UIScreen.MainScreen.Bounds.Width;
        }
    }
}
