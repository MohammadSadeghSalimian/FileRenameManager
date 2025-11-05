using System.Reactive.Disposables;
using ReactiveUI;

namespace FileRenameManager.Wpf.Views;

public abstract class RBaseUc<T> : ReactiveUserControl<T> where T : class
{
    protected virtual void Setup()
    {
        this.WhenActivated(d =>
        {
            SetupElements(d);
            SetupCommands(d);
        });
        //this.DataContext = this;
    }

    protected abstract void SetupElements(CompositeDisposable d);
    protected abstract void SetupCommands(CompositeDisposable d);
}