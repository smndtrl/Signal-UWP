using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Signal.ViewModels
{
    interface IBackAwareViewModel
    {
        void BackRequested(BackRequestedEventArgs args);

    }
}
