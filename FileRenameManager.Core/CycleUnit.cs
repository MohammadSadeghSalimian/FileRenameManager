namespace FileRenameManager.Core;

public class CycleUnit(double cycle, double driftLevel, string cycleType)
{
    public double Cycle { get; private set; } = cycle;
    public double DriftLevel { get; private set; } = driftLevel;
    public string CycleType { get; private set; } = cycleType;
}