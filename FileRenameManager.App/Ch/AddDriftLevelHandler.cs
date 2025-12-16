using FileRenameManager.Core;
using MediatR;

namespace FileRenameManager.App.Ch;

public class AddDriftLevelHandler(IFileOrganizer fileOrganizer,IFileSearcher fileSearcher,IExcelInputReader excelInputReader) : IRequestHandler<AddDriftLevelRq, AddDriftLevelRs>
{
    public async Task<AddDriftLevelRs> Handle(AddDriftLevelRq request, CancellationToken cancellationToken)
    {
        if (!request.ExcelFileAddress.Exists)
        {
            return new AddDriftLevelRs(false)
            {
                Message = "Excel file does not exist."
            };
        }

        var excelModel = await excelInputReader.ReadFileAsync(request.ExcelFileAddress, cancellationToken);
        var files = await fileSearcher.SearchForFilesWithCycleNumberAsync(new DirectoryInfo(excelModel.FolderAddress), excelModel.Recursive, cancellationToken);
        await fileOrganizer.AddDriftLevelToCycleFilesAsync(files, excelModel.Data,false, cancellationToken);


        bool isSuccess = true; // Replace with actual logic
        var response = new AddDriftLevelRs(isSuccess);
            
        return await Task.FromResult(response);
    }
}