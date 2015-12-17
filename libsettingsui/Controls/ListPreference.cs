using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Settings.UI.Xaml.Controls
{
    public sealed class ListPreference : PreferenceBase
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(List<KVStore>), typeof(ListPreference), new PropertyMetadata(new List<KeyValuePair<string, string>>(), OnItemsPropertyChanged));

        private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListPreference;
            if (control != null)
            {
                control.Items = (List<KeyValuePair<string, string>>)e.NewValue;
            }
        }

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(string), typeof(ListPreference), new PropertyMetadata(string.Empty, OnSelectedItemPropertyChanged));

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListPreference;
            if (control != null)
            {
                Debug.WriteLine("OnSelectedValuePropertyChanged");
                control.SelectedValue = (string)e.NewValue;
            }
        }

        public ListPreference()
        {
            this.DefaultStyleKey = typeof(ListPreference);
        }


        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set
            {
                //if (value == null || value == SelectedValue) return;
                Debug.WriteLine($"Setting Selected value to {value}");
                SetValue(SelectedValueProperty, value);
            }
        }


        public List<KeyValuePair<string,string>> Items
        {
            get { return (List<KeyValuePair<string, string>>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        /*private RelayCommand<string> _radioButtonCommand;
        public RelayCommand<string> RadioButtonCommand
        {
            get
            {
                return _radioButtonCommand ?? (_radioButtonCommand = new RelayCommand<string>(
                    (param) =>
                    {
                        Debug.WriteLine($"param {param}");

                    },
                    (param) => true));
            }
        }

        private ICommand _radioButtonCommand;
        public ICommand RadioButtonCommand
        {
            get
            {
                return _radioButtonCommand ?? (_radioButtonCommand = new ButtonCommand());
            }
        }*/

        private class ButtonCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                throw new NotImplementedException();
            }

            public void Execute(object parameter)
            {
                throw new NotImplementedException();
            }
        }


        public class KVStore
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}
