using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileRenameManager.Core;
using ReactiveUI;

namespace FileRenameManager.Wpf.ViewModels
{
    public class MainViewModel(CalenderRenamerVm calenderRenamerVm) : ReactiveObject
    {
        public CalenderRenamerVm CalenderRenamerVm { get; } = calenderRenamerVm;
    }
}
