using System.Threading;

namespace FileRenameManager.Core;

public interface IFileOrganizer
{
    

    Task MoveToDateFolderAsync(IReadOnlyList<FileWithDate> photos,
        DirectoryInfo destination,
        bool dryRun = false,
        CancellationToken cancellationToken = default);


    Task RenameToDateNameBasedAsync(IReadOnlyList<FileWithDate> files,
        bool dryRun = false, CancellationToken cancellationToken = default);

    Task MoveCycleFilesAsync(IReadOnlyList<CycleFileWithDate> files, DirectoryInfo destination,
        bool dryRun = false, CancellationToken cancellationToken = default);


    Task AddDriftLevelToCycleFilesAsync(IReadOnlyList<CycleFileWithDate> files,IReadOnlyDictionary<double,CycleUnit> dictionary, bool dryRun = false, CancellationToken cancellationToken = default);
}