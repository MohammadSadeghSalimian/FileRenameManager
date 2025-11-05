using System.Windows.Controls;
using FileRenameManager.App;
using FileRenameManager.App.Ch;
using MediatR;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Unit = System.Reactive.Unit;

namespace FileRenameManager.Wpf.ViewModels;

public sealed partial class CalenderRenamerVm : BaseVm
{
    private readonly IFfUnit _ffUnit;
    private readonly IMessageUnit _messageUnit;
    private readonly IMediator _mediator;
    public ReactiveCommand<Unit, Unit>? RenameCmd { get; private set; }
    [Reactive] public partial bool RecursiveFolder { get; set; }


    public CalenderRenamerVm(IFfUnit ffUnit, IMessageUnit messageUnit, IMediator mediator)
    {
        _ffUnit = ffUnit;
        _messageUnit = messageUnit;
        _mediator = mediator;
        
        SetupCommands();
    }

    protected override void SetupCommands()
    {
        RenameCmd = ReactiveCommand.CreateFromTask(Rename);
    }

    private async Task Rename()
    {
        var rr = _ffUnit.OpenFolder(out var directory, "Select the image folder");
        if (rr)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(directory);
                await _mediator.Send(new RenamePhoneCameraRq(directory, RecursiveFolder));
            }
            catch (Exception e)
            {
                await _messageUnit.Error(e);
            }
        }
    }

}