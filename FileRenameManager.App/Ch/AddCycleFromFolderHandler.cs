using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

public class AddCycleFromFolderHandler(IFileOrganizer fileOrganizer, IFileSearcher fileSearcher) : IRequestHandler<AddCycleValueFromParentFolderRq, AddCycleFromFolderRs>
{
    public async Task<AddCycleFromFolderRs> Handle(AddCycleValueFromParentFolderRq request, CancellationToken cancellationToken)
    {
        var files = await fileSearcher.SearchFilesWithCycleNumberInParentFolder(request.SourceFolder, request.Recursive,
            cancellationToken);
         await fileOrganizer.RenameFileForCycleAndDriftAsync(files, false, cancellationToken);
        bool isSuccess = true; // Replace with actual logic
        var response = new AddCycleFromFolderRs(isSuccess);
            
        return await Task.FromResult(response);
    }
}