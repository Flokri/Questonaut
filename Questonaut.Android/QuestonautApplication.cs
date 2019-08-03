using System;
using Shiny;
using Android.App;
using Android.Runtime;
using Questonaut.ShinyConfig.SetUp;

namespace Questonaut.Droid
{
    [Application]
    public class QuestonautApplication : Application
    {
        public QuestonautApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }


        public override void OnCreate()
        {
            base.OnCreate();
            AndroidShinyHost.Init(this, new ShinyStartupClass(), services =>
            {
                // register any platform specific stuff you need here
            });
        }
    }
}
