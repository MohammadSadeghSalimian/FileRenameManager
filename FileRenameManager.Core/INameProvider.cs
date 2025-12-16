namespace FileRenameManager.Core;

public interface INameProvider
{
    string GetFolderNameMmBased(DateTime dateTaken);


    string GetFileNameBasedOnDate(DateTime dateTaken,string extension);
    string GetNameCycleFile(CycleFileWithDate cycleFile);

    string GetNameWithDriftLevel(CycleFileWithDate cycleFile, double driftLevel,string cycleType);


}