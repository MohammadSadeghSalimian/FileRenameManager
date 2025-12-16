using MediatR;

namespace FileRenameManager.App.Ch;

public record RenameRoamingCameraRq(DirectoryInfo FolderAddress, bool Recursive,double Hours) : IRequest<bool>;