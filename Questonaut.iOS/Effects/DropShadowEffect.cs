using System;
using System.ComponentModel;
using System.Linq;
using CoreGraphics;
using Questonaut.Effects;
using Questonaut.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Questonaut")]
[assembly: ExportEffect(typeof(DropShadowEffect), "DropShadowEffect")]
namespace Questonaut.iOS.Effects
{
    public class DropShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var effect = (ViewShadowEffect)Element.Effects.FirstOrDefault(e => e is ViewShadowEffect);

                if (effect != null)
                {
                    Container.Layer.BorderWidth = 0.5f;
                    Container.Layer.BorderColor = Color.FromHex("E8E8E8").ToCGColor();

                    Container.Layer.CornerRadius = effect.Radius;
                    Container.Layer.ShadowColor = effect.Color.ToCGColor();
                    Container.Layer.ShadowOffset = new CGSize(effect.DistanceX, effect.DistanceY);
                    Container.Layer.ShadowOpacity = 0.5f;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: {0}", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
