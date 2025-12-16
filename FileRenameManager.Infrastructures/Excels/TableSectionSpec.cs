using ClosedXML.Excel;

namespace FileRenameManager.Infrastructures.Excels;

public sealed class TableSectionSpec<T>
{
    public string[] StartMarkers { get; init; } = [];

    // optional
    public string[] EndMarkers { get; init; } = [];
    public string[] NextSectionStartMarkers { get; init; } = [];

    // offsets relative to the start marker cell
    public int StartRowOffset { get; init; } = 1;
    public int BaseColOffset { get; init; } = 0;

    // header detection
    public int HeaderSearchMaxRows { get; init; } = 50;
    public int HeaderRowScanMaxCols { get; init; } = 100;

    // If provided, header row is detected when baseCol cell matches any of these.
    // Example: new[]{"Cycle"} if your first header column is "Cycle".
    public string[] HeaderMarkerTexts { get; init; } = [];

    public List<ColumnSpec> Columns { get; init; } = [];

    // Row terminator:
    // default recommendation: return true when the first required column is empty
    public Func<IXLWorksheet, int, Dictionary<string, int>, bool>? Terminator { get; init; }

    // Factory to build T from row
    public required Func<IXLWorksheet, int, Dictionary<string, int>, T> RowFactory { get; init; }

    // Used when HeaderMarkerTexts is not set.
    public bool HasAllRequiredHeaders(Dictionary<string, int> headerMap)
    {
        return Columns.Select(col => col.HeaderAliases.Any(headerMap.ContainsKey)).All(ok => ok);
    }
}