using FileRenameManager.App;

namespace FileRenameManager.Infrastructures.MetaData;

public class CopyService:ICopyService
{

    public async Task<string> GetCopiedTextAsync()
    {
        var aa= await TextCopy.ClipboardService.GetTextAsync();
        return aa ?? string.Empty;
    }

    public string GetCopiedText()
    {
        var aa =  TextCopy.ClipboardService.GetText();
        return aa ?? string.Empty;
    }

}