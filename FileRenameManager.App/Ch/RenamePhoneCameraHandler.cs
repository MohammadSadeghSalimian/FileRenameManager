using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

public class RenamePhoneCameraHandler(IPhotoOrganizer photoOrganizer,IVideoOrganizer videoOrganizer) : IRequestHandler<RenamePhoneCameraRq, bool>
{
    public async Task<bool> Handle(RenamePhoneCameraRq request, CancellationToken cancellationToken)
    {
            
        photoOrganizer.OrganizeByDate(request.FolderAddress, request.Recursive);
        await videoOrganizer.OrganizeByDateAsync(request.FolderAddress, request.Recursive, false, cancellationToken);
        return true;

    }
}