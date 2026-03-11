namespace FileRenameManager.Core;

public class CycleAndDlFile(FileInfo file, DateTime date, double cycleNumber, double driftLevel)
{
    public FileInfo File { get; } = file ?? throw new ArgumentNullException(nameof(file));
    public DateTime DateTaken { get; } = date;
    public double CycleNumber { get; } = cycleNumber;
    public double DriftLevel { get; } = driftLevel;
    public string Extension => file.Extension;

}