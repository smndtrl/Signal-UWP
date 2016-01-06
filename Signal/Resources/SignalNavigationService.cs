using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Signal.Resources
{

    public interface INavigationServiceSignal : INavigationService
    {
        bool CanGoBack { get; }
        bool RemoveBackEntry();
    }

    public class SignalNavigationService : NavigationService, INavigationServiceSignal
    {
        public bool CanGoBack
        {
            get
            {
                var frame = this.GetMainFrame();
                if (frame != null) return frame.CanGoBack;

                return false;
            }
        }

        public bool RemoveBackEntry()
        {
            var frame = this.GetMainFrame();
            if (frame.CanGoBack)
            {
                frame.BackStack.RemoveAt(frame.BackStackDepth - 1);
                return true;
            }

            return false;
        }

        private Frame GetMainFrame()
        {
            return (Frame)Window.Current.Content;
        }
    }
}
