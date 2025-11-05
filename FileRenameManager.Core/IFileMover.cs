namespace FileRenameManager.Core;

public interface IFileMover
{
    void MoveFile(FileInfo sourceFile, DirectoryInfo targetDirectory);
}