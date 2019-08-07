using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Questonaut.Controls
{
    public class ExtendedListView : ListView
    {
        #region constructor
        //There is currently a bug in XF 4 so the recycle Element does not work as expected. If the bug is fixed use the commented code as this boosts the performance.
        //public ExtendedListView() : this(ListViewCachingStrategy.RecycleElement) { }

        //public ExtendedListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        //{
        //    this.ItemAppearing += OnItemAppearing;
        //    this.ItemTapped += OnItemTapped;
        //}

        public ExtendedListView()
        {
            this.ItemAppearing += OnItemAppearing;
            this.ItemTapped += OnItemTapped;
        }
        #endregion

        #region bindable properties
        // ItemAppearing Property and field to allow binding of a custom command in our control
        // directly from our xaml code
        public static readonly BindableProperty ItemApperingCommandProperty =
            BindableProperty.Create(nameof(ItemAppearingCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        // LoadMoreCommandProperty Property and field, which will be the one we will call
        // If we need to Load more data as our control needs it.
        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        // TappedCommandProperty Property and field, which will be the one we will call
        // If the user taps an item on our listview.
        public static readonly BindableProperty TappedCommandProperty =
            BindableProperty.Create(nameof(TappedCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));
        #endregion

        #region properties
        public ICommand ItemAppearingCommand
        {
            get => (ICommand)GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public ICommand LoadMoreCommand
        {
            get => (ICommand)GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public ICommand TappedCommand
        {
            get { return (ICommand)GetValue(TappedCommandProperty); }
            set { SetValue(TappedCommandProperty, value); }
        }
        #endregion

        #region event handler
        /// <summary>
        /// If the new item that is appearing on the screen is the last one in the 
        /// collection, we execute the LoadMoreCommand so our ViewModel knows we
        /// want to load more data for our user from the data service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ItemAppearingCommand != null)
            {
                ItemAppearingCommand?.Execute(e.Item);
            }
            if (LoadMoreCommand != null)
            {
                //If its the last item execute command and load more data.
                if (e.Item == ItemsSource.Cast<object>().Last())
                {
                    LoadMoreCommand?.Execute(null);
                }
            }
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (TappedCommand != null)
            {
                TappedCommand?.Execute(e.Item);
            }
        }
        #endregion
    }
}
