using FileRenameManager.App.Ch;
using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

public class OrganizePhoneCameraHandler(IFileOrganizer fileOrganizer,IFileSearcher fileSearcher) : IRequestHandler<OrganizePhoneCameraRq, bool>
{
    public async Task<bool> Handle(OrganizePhoneCameraRq request, CancellationToken cancellationToken)
    {
        var files = await fileSearcher.SearchInFolderAsync(request.FolderAddress,request.Hours, request.Recursive, cancellationToken);
        await fileOrganizer.MoveToDateFolderAsync(files, request.FolderAddress, false, cancellationToken);
        return true;

    }


    
}

public class RenameRoamingCameraHandler(IFileOrganizer fileOrganizer, IFileSearcher fileSearcher)
    : IRequestHandler<RenameRoamingCameraRq, bool>
{
    public async Task<bool> Handle(RenameRoamingCameraRq request, CancellationToken cancellationToken)
    {
        var files = await fileSearcher.SearchInFolderAsync(request.FolderAddress,request.Hours, request.Recursive, cancellationToken);
        await fileOrganizer.RenameToDateNameBasedAsync(files, request.FolderAddress, false, cancellationToken);

        return true;
    }
}


public class MovingFixedCameraImagesHandler(IFileOrganizer fileOrganizer, IFileSearcher fileSearcher)
    : IRequestHandler<MovingFixedCameraImagesRq, bool>
{
    public async Task<bool> Handle(MovingFixedCameraImagesRq request, CancellationToken cancellationToken)
    {
        var files = await fileSearcher.SearchForFixedCameraImageAsync(request.FolderAddress, false, cancellationToken);
        await fileOrganizer.MoveCycleFilesAsync(files, request.FolderAddress, false, cancellationToken);

        return true;
    }
}
