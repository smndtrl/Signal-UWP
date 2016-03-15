using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Signal.Xaml.Interactions
{
    public class ExtendedGoToStateAction : DependencyObject, IAction
    {
        public string StateName
        {
            get { return (string)GetValue(StateNameProperty); }
            set { SetValue(StateNameProperty, value); }
        }

        public static readonly DependencyProperty StateNameProperty =
            DependencyProperty.Register("StateName", typeof(string), typeof(ExtendedGoToStateAction), new PropertyMetadata(string.Empty));

        public bool UseTransitions
        {
            get { return (bool)GetValue(UseTransitionsProperty); }
            set { SetValue(UseTransitionsProperty, value); }
        }

        public static readonly DependencyProperty UseTransitionsProperty =
            DependencyProperty.Register("UseTransitions", typeof(bool), typeof(ExtendedGoToStateAction), new PropertyMetadata(false));

        public object Execute(object sender, object parameter)
        {
            return ExtendedVisualStateManager.GoToElementState((FrameworkElement)sender, this.StateName, this.UseTransitions);
        }
    }
}
