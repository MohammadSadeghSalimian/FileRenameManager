namespace FileRenameManager.Core;

public interface IMetadataService
{
    FileWithDate GetMediaWithDate(FileInfo file,double hourOffset);
}