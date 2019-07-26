using System;
using Questonaut.Controls;
using Questonaut.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace Questonaut.Droid.CustomRenderer
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        Android.Views.View _nativeCell;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            _nativeCell = base.GetCellCore(item, convertView, parent, context);

            SetStyle();

            return _nativeCell;
        }

        // this one is simpler as the base class has a nice override-able method for our purpose - so we don't need to subscribe 
        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == CustomViewCell.AllowHighlightProperty.PropertyName)
            {
                SetStyle();
            }
        }

        private void SetStyle()
        {
            var formsCell = Cell as CustomViewCell;
            if (formsCell == null)
                return;

            _nativeCell.Clickable = !formsCell.AllowHighlight;
        }
    }
}
