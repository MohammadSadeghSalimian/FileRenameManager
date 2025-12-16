namespace FileRenameManager.App;

public interface IExcelInputReader
{
    Task<ExcelInputModelAddingDriftLevel>  ReadFileAsync(FileInfo file,CancellationToken ct);
}