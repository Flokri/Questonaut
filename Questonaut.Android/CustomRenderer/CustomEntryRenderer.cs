using System;
using Android.Content;
using Questonaut.Controls;
using Questonaut.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(CustomEntryRenderer))]
namespace Questonaut.Droid.CustomRenderer
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Background = null;

                var lp = new MarginLayoutParams(Control.LayoutParameters);
                lp.SetMargins(0, 0, 0, 0);
                LayoutParameters = lp;
                Control.LayoutParameters = lp;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);

                this.Control.TextAlignment = Android.Views.TextAlignment.ViewEnd;
            }
        }
    }
}
