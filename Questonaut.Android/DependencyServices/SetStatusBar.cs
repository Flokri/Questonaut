using System;
using Questonaut.DependencyServices;
using Questonaut.Droid.DependencyServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(SetStatusBar))]
namespace Questonaut.Droid.DependencyServices
{
    public class SetStatusBar : ISetStatusBar
    {
        public void SetColorToBlack()
        {
            //just a placeholder 
        }

        public void SetColorToWhite()
        {
            //just a placeholder 
        }
    }
}
