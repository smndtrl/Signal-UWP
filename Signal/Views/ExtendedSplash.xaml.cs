/** 
 * Copyright (C) 2015 smndtrl
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using TextSecure.util;
using System.Threading;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Threading.Tasks;
using System.Diagnostics;
using Signal.Views;
using Bezysoftware.Navigation.BackButton;
//using TextSecure.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TextSecure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        internal bool dismissed = false;
        internal Frame rootFrame;
        internal bool isRegistered = false;

        public ExtendedSplash()
        {
            this.InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var highlightColor = (Color)Application.Current.Resources["SystemColorHighlightColor"];
            highlightColor.R = (byte)(highlightColor.R - 20);
            highlightColor.G = (byte)(highlightColor.G - 20);
            highlightColor.B = (byte)(highlightColor.B - 20);
            titleBar.BackgroundColor = highlightColor;
            titleBar.ForegroundColor = Colors.Black;
            titleBar.ButtonBackgroundColor = highlightColor;
            //DirectoryHelper.refreshDirectory();

            rootFrame = new Frame();

            BackButtonManager.RegisterFrame(rootFrame, true, true, true);

        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(200);

            dismissed = false;
            if (TextSecurePreferences.getLocalNumber().Equals(string.Empty))
            {
                Debug.WriteLine("First start, registering");
                rootFrame.Navigate(typeof(RegistrationTypeView));
                Window.Current.Content = rootFrame;
                return;
            }

            rootFrame.Navigate(typeof(View));
            
            //Window.Current.Content = rootFrame;

            //rootFrame.Navigate(typeof(NarrowView));
            Window.Current.Content = rootFrame; // rootFrame;
        }
    }
}
