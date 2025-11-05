namespace FileRenameManager.Core;

public interface IVideoMetadataService
{
    VideoWithDate GetVideoWithDate(FileInfo file);
}