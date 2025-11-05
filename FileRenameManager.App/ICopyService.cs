namespace FileRenameManager.App;

public interface ICopyService
{
    Task<string> GetCopiedTextAsync();
    string GetCopiedText();
}