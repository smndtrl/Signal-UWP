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
using Windows.UI.Xaml.Navigation;

namespace Signal.Views
{
    public partial class SignalPage : Page
    {
        public SignalPage()
        {
            this.DataContextChanged += this.OnDataContextChanged;
            SetDefaultAmbientColor();

            this.Loaded += (s,e) => OnLoaded(s, e);
            this.Unloaded += (s, e) => OnUnloaded(s, e);
        }



        protected void OnUnloaded(object s, RoutedEventArgs e)
        {
            var inputPane = InputPane.GetForCurrentView();
            inputPane.Showing -= OnInputPaneShowing;
            inputPane.Hiding -= OnInputPaneHiding;
        }

        protected void OnLoaded(object s, RoutedEventArgs e)
        {
            var inputPane = InputPane.GetForCurrentView();
            inputPane.Showing += OnInputPaneShowing;
            inputPane.Hiding += OnInputPaneHiding;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var navigableViewModel = this.DataContext as INavigableViewModel;
            if (navigableViewModel != null) navigableViewModel.NavigateTo(e);

            //SystemNavigationManager.GetForCurrentView().BackRequested += DetailPage_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var navigableViewModel = this.DataContext as INavigableViewModel;
            if (navigableViewModel != null) navigableViewModel.NavigateFrom(e);

            //SystemNavigationManager.GetForCurrentView().BackRequested -= DetailPage_BackRequested;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //var inputAwarePage = this as IInputAwarePage;
            if (this != null)
            {
                
            }
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

        private void OnInputPaneShowing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            UpdatePanelLayout(args.OccludedRect.Height);
        }

        private void OnInputPaneHiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            UpdatePanelLayout(args.OccludedRect.Height);
        }

        protected virtual void UpdatePanelLayout(double height)
        {
        }
    }
}
