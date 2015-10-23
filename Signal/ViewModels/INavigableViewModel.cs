using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.ViewModel
{
    public interface INavigableViewModel
    {
        void Activate(object parameter);
        void Deactivate(object parameter);
    }
}
