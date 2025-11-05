namespace FileRenameManager.App
{
    public class FileFilter(string description, params string[] extensions)
    {

        public string Description { get; private set; } = description;
        public string[] Extensions { get; private set; } = extensions;




    }
}
