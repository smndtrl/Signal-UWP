using Signal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

namespace Signal.Views
{
    public partial class SignalPage : Page
    {
        public SignalPage()
        {
            this.DataContextChanged += this.OnDataContextChanged;
            SetDefaultAmbientColor();
        }

        private void OnDataContextChanged(object sender, DataContextChangedEventArgs e)
        {

            IAmbientColor ambientColor = e.NewValue as IAmbientColor;
            if (ambientColor != null) {
                ambientColor.AmbientColorChanged += (s, ae) => OnAmbientColorChanged(ae);
            }

        }

        private void OnAmbientColorChanged(EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetDefaultAmbientColor()
        {
            var SignalBlue = (Color)Application.Current.Resources["SignalBlue"];

            var dimmedBlue = SignalBlue;
            dimmedBlue.R -= 20;
            dimmedBlue.G -= 20;
            dimmedBlue.B -= 20;

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = dimmedBlue;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = dimmedBlue;
                    titleBar.ForegroundColor = Colors.White;
                }
            }

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = dimmedBlue;
                    statusBar.ForegroundColor = Colors.White;
                }

            }
        }

    }
}
