using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;
using Splat;

namespace FileRenameManager.Wpf.Views;

public abstract class RBaseWindow<T> : Window, IViewFor<T> where T : class
{
    protected virtual void Setup()
    {
        this.WhenActivated(d =>
        {

            SetupElements(d);
            SetupCommands(d);
        });
    }
    protected T? ViewModelP;
    object? IViewFor.ViewModel
    {
        get => ViewModelP;
        set => ViewModel = (T)value;
    }
    public T ViewModel
    {
        get => ViewModelP;
        set
        {
            ViewModelP = value;
            DataContext = ViewModelP;
        }
    }
    protected abstract void SetupCommands(CompositeDisposable d);
    protected abstract void SetupElements(CompositeDisposable d);
    protected void GetViewModel()
    {
        ViewModel = Splat.Locator.Current.GetService<T>();
    }
}