using System;
using Shiny;
using Android.App;
using Android.Runtime;
using Questonaut.Helper;

namespace Questonaut.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            base.OnCreate();
            Shiny.AndroidShinyHost.Init(this, new ServicesStartup(), services =>
            {
                // register any platform specific stuff you need here
            });
        }
    }
}
