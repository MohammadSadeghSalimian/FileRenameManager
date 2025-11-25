namespace FileRenameManager.Core;

public interface IMetadataService
{
    FileWithDate GetMediaWithDate(FileInfo file,double hourOffset);
}
public interface IPhotoMetadataService : IMetadataService
{
    FileWithDate GetMediaWithDate(FileInfo file,double hourOffset);
}

