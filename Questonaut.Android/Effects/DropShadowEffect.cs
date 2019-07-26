using System;
using System.Linq;
using Questonaut.Droid.Effects;
using Questonaut.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Questonaut")]
[assembly: ExportEffect(typeof(DropShadowEffect), "DropShadowEffect")]
namespace Questonaut.Droid.Effects
{
    public class DropShadowEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                var control = Control ?? Container as Android.Views.View;

                var effect = (ViewShadowEffect)Element.Effects.FirstOrDefault(e => e is ViewShadowEffect);

                if (effect != null)
                {
                    float radius = effect.Radius;

                    control.Elevation = radius;
                    control.TranslationZ = (effect.DistanceX + effect.DistanceY) / 2;
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
