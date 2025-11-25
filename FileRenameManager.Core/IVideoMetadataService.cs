namespace FileRenameManager.Core;

public interface IVideoMetadataService : IMetadataService
{
    FileWithDate GetMediaWithDate(FileInfo file,double hourOffset);
}