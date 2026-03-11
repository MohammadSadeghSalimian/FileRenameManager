using MediatR;

namespace FileRenameManager.App.Ch;

public record AddCycleValueFromParentFolderRq(DirectoryInfo SourceFolder,bool Recursive) : IRequest<AddCycleFromFolderRs>;