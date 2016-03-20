using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Signal.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Signal.Xaml.Controls
{
    [ContentProperty(Name = "Child")]
    [TemplatePart(Name = RootGridName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartShiftCompensatorName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PartInputPaneSpacerName, Type = typeof(FrameworkElement))]
    public sealed partial class InputAwareContainer : Control
    {
        #region DependencyProperties
        /// <summary>
        /// Identifies the ChildProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(UIElement),
            typeof(InputAwareContainer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the AnimationMode dependency property.
        /// </summary>
        /*public static readonly DependencyProperty AnimationModeProperty =
            DependencyProperty.Register("AnimationMode", typeof(InputAwarePanelAnimationMode),
            typeof(InputAwareContainer), new PropertyMetadata(InputAwarePanelAnimationMode.None, OnAnimationModeChanged));*/
        #endregion
        private const string RootGridName = "RootGrid";
        private const string PartShiftCompensatorName = "part_ShiftCompensator";
        private const string PartInputPaneSpacerName = "part_InputPaneSpacer";

        private FrameworkElement rootGrid;
        private FrameworkElement partShiftCompensator;
        private FrameworkElement partInputPaneSpacer;

        public InputAwareContainer()
        {
            this.DefaultStyleKey = typeof(InputAwareContainer);
            this.Loaded += InputAwarePanel_Loaded;
            this.Unloaded += InputAwarePanel_Unloaded;
        }

        /// <summary>
        /// Gets or sets the child of the InputAwarePanel.
        /// </summary>
        public UIElement Child
        {
            get { return (UIElement)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rootGrid = GetTemplateChild(RootGridName) as FrameworkElement;
            partShiftCompensator = GetTemplateChild(PartShiftCompensatorName) as FrameworkElement;
            partInputPaneSpacer = GetTemplateChild(PartInputPaneSpacerName) as FrameworkElement;
            var inputPane = InputPane.GetForCurrentView();
            UpdatePanelLayout(inputPane.OccludedRect.Height);
        }

        private void UpdatePanelLayout(double occludedHeight)
        {
            partInputPaneSpacer.Height = occludedHeight;
        }

        #region EventHandlers
        private void InputAwarePanel_Loaded(object sender, RoutedEventArgs e)
        {
            var inputPane = InputPane.GetForCurrentView();
            inputPane.Showing += InputPane_Showing;
            inputPane.Hiding += InputPane_Hiding;
        }

        private void InputAwarePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            var inputPane = InputPane.GetForCurrentView();
            inputPane.Showing -= InputPane_Showing;
            inputPane.Hiding -= InputPane_Hiding;
        }

        private void InputPane_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            UpdatePanelLayout(args.OccludedRect.Height);
        }

        private void InputPane_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            args.EnsuredFocusedElementInView = true;
            UpdatePanelLayout(args.OccludedRect.Height);
        }
        #endregion
    }
}
