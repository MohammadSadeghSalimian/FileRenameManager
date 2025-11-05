using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileRenameManager.Wpf.ViewModels;

namespace FileRenameManager.Wpf.Views
{
    /// <summary>
    /// Interaction logic for RenameCalenderUc.xaml
    /// </summary>
    public sealed partial class RenameCalenderUc : RBaseUc<CalenderRenamerVm>
    {
        public RenameCalenderUc()
        {
            InitializeComponent();
            Setup();
        }

        protected override void SetupElements(CompositeDisposable d)
        {
            this.Bind(ViewModel, x => x.RecursiveFolder, v => RecursiveCheckBox.IsChecked).DisposeWith(d);
        }

        protected override void SetupCommands(CompositeDisposable d)
        {
            this.BindCommand(ViewModel, x => x.RenameCmd, v => CalRenameBtn).DisposeWith(d);
        }
    }
}
