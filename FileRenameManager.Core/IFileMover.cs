namespace FileRenameManager.Core;

public interface IFileMover
{
    void MoveFileToFolder(FileInfo sourceFile, DirectoryInfo targetDirectory);
    void MoveFile(FileInfo sourceFile,FileInfo destFile);
    void RenameFile(FileInfo sourceFile, string newFileName);
}