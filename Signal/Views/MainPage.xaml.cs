﻿using Bezysoftware.Navigation.BackButton;
using Signal.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Signal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SignalPage
    {
        public MainPageViewModel ViewModel
        {
            get
            {
                return (MainPageViewModel)DataContext;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            ViewModel.DetailFrame = detailFrame;

            ViewModel.AdaptiveStates = AdaptiveStates;
        }

        private void piv_CC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((PivotItem)piv_CC.SelectedItem).Header.ToString().ToLower() == "chats")
            {
                ArchievedChatsBtn.Visibility = Visibility.Visible;
                RefreshBtn.Visibility = Visibility.Collapsed;
                SelectItmesBtn.Visibility = Visibility.Visible;
            }
            else
            {
                ArchievedChatsBtn.Visibility = Visibility.Collapsed;
                RefreshBtn.Visibility = Visibility.Visible;
                SelectItmesBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}