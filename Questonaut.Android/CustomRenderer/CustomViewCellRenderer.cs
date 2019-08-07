using Android.Content;
using Android.Views;
using Questonaut.Controls;
using Questonaut.Droid.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;


[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace Questonaut.Droid.CustomRenderer
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        private Android.Views.View _cellCore;
        private bool _selected = false;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            _cellCore = base.GetCellCore(item, convertView, parent, context);
            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);
            if (args.PropertyName == "IsSelected")
            {
                _selected = !_selected;
                var extendedViewCell = sender as ViewCell;
                if (_selected)
                    _cellCore.SetBackgroundColor(Android.Graphics.Color.ParseColor("#42506B"));
                else
                    _cellCore.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
