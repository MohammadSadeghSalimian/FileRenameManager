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