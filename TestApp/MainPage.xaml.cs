using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private bool _switch1 = true;
        public bool Switch1
        {
            get
            {
                return _switch1;
            }
            set
            {
                _switch1 = value;
                Debug.WriteLine($"Switch1 is now {_switch1}");
            }
        }

        private bool _switch2 = false;
        public bool Switch2
        {
            get
            {
                return _switch2;
            }
            set
            {
                _switch2 = value;
                Debug.WriteLine($"Switch2 is now {_switch2}");
            }
        }

        public ICommand ButtonCommand1 = new TestCommand();


        public class TestCommand : ICommand
        {
            private bool execute = true;

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                //throw new NotImplementedException();
                return execute;
            }

            public void Execute(object parameter)
            {
                Debug.WriteLine("Button execute");
                execute = false;
                CanExecuteChanged(this, EventArgs.Empty);
                //throw new NotImplementedException();
            }
        }

    }
}
