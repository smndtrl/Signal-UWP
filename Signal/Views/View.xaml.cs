

using Bezysoftware.Navigation.BackButton;
using Signal.ViewModel;
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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TextSecure.database;
using TextSecure.database.loaders;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Signal.Views
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class View : Page, IBackAwareObject
    {

        public MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)DataContext;
            }
        }

        public bool AllowBackNavigation()
        {
            return false;
        }

        public View()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            

            UpdateForVisualState(AdaptiveStates.CurrentState);

            // Don't play a content transition for first item load.
            // Sometimes, this content will be animated as part of the page transition.
            //DisableContentTransitions();

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void idchange(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("changed");
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            Debug.WriteLine("state changed");
            UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == DefaultState /*&& _lastSelectedItem != null*/)
            {
                ViewModel.NarrowStateCommand.Execute(null);
                // Resize down to the detail item. Don't play a transition.
                //Frame.Navigate(typeof(ConversationView), _lastSelectedItem.number, new SuppressNavigationTransitionInfo());
            }

            
            EntranceNavigationTransitionInfo.SetIsTargetElement(masterFrame, isNarrow);
            if (detailFrame != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(detailFrame, !isNarrow);
            }
        }

        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            /*var clickedItem = (TestContact)e.ClickedItem;
            _lastSelectedItem = clickedItem;

            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // Use "drill in" transition for navigating from master list to detail view
                Frame.Navigate(typeof(ConversationView), "asdf", new DrillInNavigationTransitionInfo());
            }
            else
            {
                detailFrame = new Frame();
                Debug.WriteLine($"change detail frame");
                detailFrame.Navigate(typeof(ConversationView));
                // Play a refresh animation when the user switches detail items.
                //EnableContentTransitions();
            }*/
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            //MasterListView.SelectedItem = _lastSelectedItem;
           
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            /*Debug.WriteLine("Button clicked");
            if (identities != null)
            {
                identities.CollectionChanged -= idchange;
            }
            identities = new DirectoryLoader<TextSecureDirectory.Directory>();
            identities.CollectionChanged += idchange;
            identitiesCVS.Source = identities;*/
        }

        private void NewConversation_Click(object sender, RoutedEventArgs e)
        {



            //conversationListFrame = new Frame();
            //Debug.WriteLine($"change detail frame");
            //conversationListFrame.Navigate(typeof(NewConversationView));
            /*Debug.WriteLine("Button clicked");
            if (identities != null)
            {
                identities.CollectionChanged -= idchange;
            }
            identities = new DirectoryLoader<TextSecureDirectory.Directory>();
            identities.CollectionChanged += idchange;
            identitiesCVS.Source = identities;*/
        }

        private void detailFrame_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /*private void EnableContentTransitions()
        {
            DetailContentPresenter.ContentTransitions.Clear();
            DetailContentPresenter.ContentTransitions.Add(new EntranceThemeTransition());
        }

        private void DisableContentTransitions()
        {
            if (DetailContentPresenter != null)
            {
                DetailContentPresenter.ContentTransitions.Clear();
            }
        }*/

    }


}
