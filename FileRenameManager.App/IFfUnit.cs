namespace FileRenameManager.App;

public interface IFfUnit
{
    bool OpenFile(out FileInfo? file, string title, params FileFilter[] filters);
    bool SaveFile(out FileInfo? file, string title, params FileFilter[] filters);
    bool OpenManyFiles(out FileInfo[]? files, string title, params FileFilter[] filters);
    bool OpenFolder(out DirectoryInfo? folder, string title);
    bool OpenManyFolders(out DirectoryInfo[]? folders, string title);
}