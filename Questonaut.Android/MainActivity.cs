﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Text;
using Prism;
using Prism.Ioc;
using System.Threading.Tasks;
using System.IO;
using Android.Content;
using Android.Graphics;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using PanCardView.Droid;
using FFImageLoading.Forms.Platform;

namespace Questonaut.Droid
{
    [Activity(Label = "Questonaut", Icon = "@mipmap/icon", Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        #region instances
        internal static MainActivity Instance { get; private set; }
        #endregion

        #region properties
        public TaskCompletionSource<Stream> PickImageTaskCompletionSource { get; set; }
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // Name of the MainActivity theme you had there before.
            // Or you can use global::Android.Resource.Style.ThemeHoloLight
            base.SetTheme(Resource.Style.MainTheme);

            //initialize the media plugin
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            //initalize the ffimageloading framework
            CachedImageRenderer.Init(enableFastRenderer: true);

            base.OnCreate(savedInstanceState);
            Instance = this;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //initalize the cards view framework
            CardsViewRenderer.Preserve();

            //using the ffloading for the standard Xamarin.Forms.Image
            CachedImageRenderer.InitImageViewHandler();

            LoadApplication(new App(new AndroidInitializer()));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {

            }
        }
    }
}