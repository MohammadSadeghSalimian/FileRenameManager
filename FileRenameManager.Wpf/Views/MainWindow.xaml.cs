using System.Reactive.Disposables;
using FileRenameManager.Wpf.ViewModels;
using Splat;

namespace FileRenameManager.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RBaseWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
           var vv=Splat.Locator.Current.GetService<MainViewModel>();
           if (vv!=null)
           {
               ViewModel = vv;
           }
        }

        protected override void SetupCommands(CompositeDisposable d)
        {
          
        }

        protected override void SetupElements(CompositeDisposable d)
        {
           this.RenameCalenderUc.ViewModel=ViewModel.CalenderRenamerVm;
        }
    }
}