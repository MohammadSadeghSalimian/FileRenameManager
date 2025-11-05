using ReactiveUI;

namespace FileRenameManager.Wpf.ViewModels;

public abstract class BaseVm : ReactiveObject
{
    protected abstract void SetupCommands();
}