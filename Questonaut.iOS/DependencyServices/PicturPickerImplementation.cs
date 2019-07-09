using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using Questonaut.DependencyServices;
using Questonaut.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PicturePickerImplementation))]
namespace Questonaut.iOS.DependencyServices
{
    public class PicturePickerImplementation : IPicturePicker
    {
        #region instances
        TaskCompletionSource<Stream> _taskCompletionSource;
        UIImagePickerController _imagePicker;
        #endregion

        /// <summary>
        /// Get the image from the native image picker.
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetImageStreamAsync()
        {
            //Create and define UIImagePickerController
            _imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };

            //Set the event handlers
            _imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            _imagePicker.Canceled += OnImagePickerCancelled;

            //Present the UIImagePickerController
            // Present UIImagePickerController;
            var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            //get the top view controller
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }


            viewController.PresentModalViewController(_imagePicker, true);



            viewController.PresentViewController(_imagePicker, true, null);

            //Return Task object
            _taskCompletionSource = new TaskCompletionSource<Stream>();
            return _taskCompletionSource.Task;
        }

        void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            UIImage image = args.EditedImage ?? args.OriginalImage;

            if (image != null)
            {
                //Convert UIImage to .Net Stream object
                NSData data = image.AsJPEG(1);
                Stream stream = data.AsStream();

                UnregisterEventHandlers();

                //Set the Stream as completion of the Task
                _taskCompletionSource.SetResult(stream);
            }
            else
            {
                UnregisterEventHandlers();
                _taskCompletionSource.SetResult(null);
            }
        }

        void OnImagePickerCancelled(object sender, EventArgs args)
        {
            UnregisterEventHandlers();
            _taskCompletionSource.SetResult(null);
            _imagePicker.DismissModalViewController(true);
        }

        void UnregisterEventHandlers()
        {
            _imagePicker.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
            _imagePicker.Canceled -= OnImagePickerCancelled;
            _imagePicker.DismissModalViewController(true);
        }
    }
}
