using System;
using Prism.Mvvm;
using Questonaut.DependencyServices;
using Xamarin.Forms;

namespace Questonaut.ViewModels
{
    public class CreateUserViewModel : BindableBase
    {
        #region instance
        private double _imageSection;
        #endregion


        public CreateUserViewModel()
        {
            ImageSectionSize = DependencyService.Get<IScreenDimensions>().GetScreenHeight() * 0.15;
        }

        #region properties
        /// <summary>
        /// Set the image section height.
        /// </summary>
        public double ImageSectionSize
        {
            get => _imageSection;
            set => SetProperty(ref _imageSection, value);
        }
        #endregion 
    }
}

