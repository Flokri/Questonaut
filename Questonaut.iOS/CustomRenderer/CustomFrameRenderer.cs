using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using UIKit;
using System;
using System.ComponentModel;
using CoreGraphics;
using Questonaut.Controls;
using Questonaut.iOS.CustomRenderer;
using Foundation;

[assembly: ExportRenderer(typeof(TouchEffectFrame), typeof(CustomFrameRenderer))]
namespace Questonaut.iOS.CustomRenderer
{
    public class CustomFrameRenderer : FrameRenderer
    {

        UIColor _savedColor;

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && e.OldElement == null)
            {
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            _savedColor = this.BackgroundColor;
            this.BackgroundColor = UIColor.LightGray;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            this.BackgroundColor = _savedColor;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            this.BackgroundColor = _savedColor;
        }
    }
}
