using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace FileRenameManager.App.Ch
{
    public record OrganizePhoneCameraRq(DirectoryInfo FolderAddress,bool Recursive,double Hours) : IRequest<bool>;

    public record RenameRoamingCameraRq(DirectoryInfo FolderAddress, bool Recursive,double Hours) : IRequest<bool>;

    public record MovingFixedCameraImagesRq(DirectoryInfo FolderAddress) : IRequest<bool>;
}

