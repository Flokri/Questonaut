using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using UIKit;
using System;
using System.ComponentModel;
using CoreGraphics;
using Questonaut.Controls;
using Questonaut.iOS.CustomRenderer;

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(CustomViewCellRenderer))]
namespace Questonaut.iOS.CustomRenderer
{
    public class CustomViewCellRenderer : ViewCellRenderer
    {
        UITableViewCell _nativeCell;

        //get access to the associated forms-element and subscribe to property-changed
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            _nativeCell = base.GetCell(item, reusableCell, tv);

            var formsCell = item as CustomViewCell;

            if (formsCell != null)
            {
                formsCell.PropertyChanged -= OnPropertyChanged;
                formsCell.PropertyChanged += OnPropertyChanged;
            }

            //and, update the style 
            SetStyle(formsCell);

            return _nativeCell;
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var formsCell = sender as CustomViewCell;
            if (formsCell == null)
                return;
            //TODO: Trying to find a nicer and more robust way to dispose and unsubscribe :(
            if (_nativeCell == null)
                formsCell.PropertyChanged -= OnPropertyChanged;

            if (e.PropertyName == CustomViewCell.AllowHighlightProperty.PropertyName)
            {
                SetStyle(formsCell);
            }
        }

        private void SetStyle(CustomViewCell formsCell)
        {
            //added this code as sometimes on tap, the separator disappears, if style is updated before tap animation finishes 
            //https://stackoverflow.com/questions/25613117/how-do-you-prevent-uitableviewcellselectionstylenone-from-removing-cell-separato
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (formsCell.AllowHighlight)
                    {
                        _nativeCell.SelectionStyle = UITableViewCellSelectionStyle.Default;
                    }
                    else
                        _nativeCell.SelectionStyle = UITableViewCellSelectionStyle.None;
                });
                return false;
            });
        }

        private UIView GetInnerBounds()
        {
            UIView grid = null;

            //search for a grid
            if (_nativeCell.Subviews.Length > 0)
            {
                grid = _nativeCell.Subviews[0].Subviews.FirstOrDefault(x =>
                {
                    return x.Subviews.FirstOrDefault(s => s.GetType().Equals(typeof(Grid))) != null;
                });

                var test = _nativeCell.Subviews[0].Subviews[0].Subviews[0];
                var type = test.GetType();
                var other = test.Bounds;


            }

            return grid;
        }
    }
}
