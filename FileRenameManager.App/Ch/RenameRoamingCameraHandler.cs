using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

public class RenameRoamingCameraHandler(IFileOrganizer fileOrganizer, IFileSearcher fileSearcher)
    : IRequestHandler<RenameRoamingCameraRq, bool>
{
    public async Task<bool> Handle(RenameRoamingCameraRq request, CancellationToken cancellationToken)
    {
        var files = await fileSearcher.SearchInFolderAsync(request.FolderAddress,request.Hours, request.Recursive, cancellationToken);
        await fileOrganizer.RenameToDateNameBasedAsync(files, false, cancellationToken);

        return true;
    }
}