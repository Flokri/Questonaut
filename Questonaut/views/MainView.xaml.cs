using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Questonaut.Controls;
using Xamarin.Forms;

namespace Questonaut.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        async void Handle_Tapped_Visual(object sender, System.EventArgs e)
        {
            var fr = ((Frame)sender).BackgroundColor;
            ((Frame)sender).BackgroundColor = Color.FromHex("#F3F3F3");
            await Task.Delay(100);
            ((Frame)sender).BackgroundColor = fr;
        }
    }
}
