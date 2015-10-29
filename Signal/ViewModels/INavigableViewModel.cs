using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModels
{
    public interface INavigableViewModel
    {
        void Activate(object parameter);
        void Deactivate(object parameter);
    }
}
