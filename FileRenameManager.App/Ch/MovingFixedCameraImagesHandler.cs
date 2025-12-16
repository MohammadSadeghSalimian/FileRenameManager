using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

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