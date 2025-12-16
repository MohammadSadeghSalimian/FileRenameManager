using FileRenameManager.Core;

namespace FileRenameManager.App;

public record ExcelInputModelAddingDriftLevel(string FolderAddress,bool Recursive,IReadOnlyDictionary<double, CycleUnit> Data);