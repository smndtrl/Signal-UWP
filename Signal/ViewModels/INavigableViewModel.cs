using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace Signal.ViewModels
{
    public interface INavigableViewModel
    {
        void NavigateTo(NavigationEventArgs e);
        void NavigateFrom(NavigationEventArgs e);
    }
}
