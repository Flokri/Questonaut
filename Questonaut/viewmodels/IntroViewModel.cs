using System;
using System.Collections.ObjectModel;
using System.Drawing;
using Prism.Mvvm;

namespace Questonaut.ViewModels
{
    public class IntroViewModel : BindableBase
    {
        private int _currentIndex;
        private int _ImageCount = 1058;

        public IntroViewModel()
        {
            Items = new ObservableCollection<object>
            {
                new { Source = CreateSource(), Ind = _ImageCount++, Color = Color.Transparent },
                new { Source = CreateSource(), Ind = _ImageCount++, Color = Color.Transparent },
            };
        }

        public ObservableCollection<object> Items { get; }

        private string CreateSource()
        {
            var source = $"https://firebasestorage.googleapis.com/v0/b/questonaut.appspot.com/o/PublicAppImages%2Fqeustonaut_intro.png?alt=media&token=6ba9c1ff-4400-4a8c-a311-094f860bc643";
            return source;
        }
    }
}
