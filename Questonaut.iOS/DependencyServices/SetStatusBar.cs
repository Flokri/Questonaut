using System;
using Questonaut.iOS.DependencyServices;
using Questonaut.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SetStatusBar))]
namespace Questonaut.iOS.DependencyServices
{
    public class SetStatusBar : ISetStatusBar
    {
        public void SetColorToBlack()
        {
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.Default;
        }

        public void SetColorToWhite()
        {
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
        }
    }
}
