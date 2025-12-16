namespace FileRenameManager.Infrastructures.Excels;

public sealed class ColumnSpec(string logicalName, params string[] headerAliases)
{
    public string LogicalName { get; } = logicalName ?? throw new ArgumentNullException(nameof(logicalName));
    public string[] HeaderAliases { get; } = (headerAliases is { Length: > 0 }) ? headerAliases : throw new ArgumentException("Need at least one alias");
}