using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal.Tasks.Library
{
    public sealed class TaskQueue : BlockingCollection<TaskActivity>
    {
    }
}
