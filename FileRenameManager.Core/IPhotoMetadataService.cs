namespace FileRenameManager.Core;

public interface IPhotoMetadataService
{
    PhotoWithDate GetPhotoWithDate(FileInfo file);
}

