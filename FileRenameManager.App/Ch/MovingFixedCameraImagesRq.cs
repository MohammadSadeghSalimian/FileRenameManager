using MediatR;

namespace FileRenameManager.App.Ch;

public record MovingFixedCameraImagesRq(DirectoryInfo FolderAddress) : IRequest<bool>;