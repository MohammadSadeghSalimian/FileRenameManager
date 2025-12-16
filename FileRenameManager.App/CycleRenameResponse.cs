using FileRenameManager.Core;

namespace FileRenameManager.App;

public record CycleRenameResponse(IReadOnlyDictionary<double,CycleUnit> cycles,string prefix)
{
    
}