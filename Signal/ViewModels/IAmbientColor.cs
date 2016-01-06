using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Signal.ViewModels
{
    interface IAmbientColor
    {

        Color AmbientColor { get; set; }
        event EventHandler AmbientColorChanged;
    }
}
