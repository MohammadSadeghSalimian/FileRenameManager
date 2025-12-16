using ClosedXML.Excel;

namespace FileRenameManager.Infrastructures.Excels;

/// <summary>
/// Reads semi-structured Excel sheets: sections delimited by markers (anchors),
/// with mixed key/value and table-like blocks, without relying on fixed indices.
/// </summary>
public sealed class SemiStructuredExcelReader
{
    private readonly IXLWorksheet _ws;
    private readonly UsedArea _area;
    private readonly MarkerIndex _markers;

    public SemiStructuredExcelReader(IXLWorksheet worksheet)
    {
        _ws = worksheet ?? throw new ArgumentNullException(nameof(worksheet));
        _area = UsedArea.FromWorksheet(_ws);
        _markers = new MarkerIndex(_ws, _area);
    }

    /// <summary>
    /// Finds the first cell matching any of the marker texts (case-insensitive).
    /// Uses a prebuilt index for speed.
    /// </summary>
    private IXLCell FindMarker(params string[] markerTexts)
        => _markers.FindFirst(markerTexts);

    /// <summary>
    /// Reads a Key/Value section located under a start marker.
    /// By default: key column = startMarker column, value column = keyCol + 1.
    /// Ends when:
    ///   - endMarker is found in the key column (optional)
    ///   - or key cell is blank
    ///   - or next section's start marker is encountered (optional list)
    /// </summary>
    public Dictionary<string, object?> ReadKeyValueSection(KeyValueSectionSpec spec)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));

        var startCell = FindMarker(spec.StartMarkers);
        var startRow = startCell.Address.RowNumber + spec.StartRowOffset;
        var keyCol = startCell.Address.ColumnNumber + spec.KeyColOffset;
        var valCol = startCell.Address.ColumnNumber + spec.ValueColOffset;

        // determine stop row: by end markers / next markers / blank keys.
        var stopRowExclusive = ResolveStopRowExclusive(
            startRow,
            keyCol,
            spec.EndMarkers,
            spec.NextSectionStartMarkers);

        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        for (var r = startRow; r < stopRowExclusive; r++)
        {
            var keyCell = _ws.Cell(r, keyCol);
            if (keyCell.IsEmpty())
                break;

            var key = keyCell.GetString().Trim();
            if (key.Length == 0)
                break;

            // End markers in the key column (if provided)
            if (spec.EndMarkers.Length > 0 && ContainsIgnoreCase(spec.EndMarkers, key))
                break;

            // Next start marker encountered (if provided)
            if (spec.NextSectionStartMarkers.Length > 0 && ContainsIgnoreCase(spec.NextSectionStartMarkers, key))
                break;

            var valueCell = _ws.Cell(r, valCol);

            // Apply per-key parsers if provided; otherwise auto
            object? value;
            if (spec.PerKeyParsers.TryGetValue(key, out var parser))
                value = parser(valueCell);
            else
                value = CellParsers.Auto(valueCell);

            result[key] = value;
        }

        return result;
    }

    /// <summary>
    /// Reads a table-like block that begins under a start marker (e.g. "Data"),
    /// with a header row containing column names (can vary per scenario).
    /// Rows are read until an end marker (optional) or until the "terminator" condition is met.
    /// </summary>
    public List<T> ReadTableSection<T>(TableSectionSpec<T> spec)
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));

        var startCell = FindMarker(spec.StartMarkers);
        var baseRow = startCell.Address.RowNumber + spec.StartRowOffset;
        var baseCol = startCell.Address.ColumnNumber + spec.BaseColOffset;

        // Find header row:
        // - either by a specific header marker (e.g. first header "Cycle")
        // - or by scanning the next N rows for required headers.
        var headerRow = FindHeaderRow(spec, baseRow, baseCol);

        // Build header->column map for that header row.
        var colMap = BuildHeaderMap(headerRow, spec.HeaderRowScanMaxCols);

        // Resolve column indices from aliases
        var resolvedCols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var colSpec in spec.Columns)
        {
            var col = ResolveColumn(colMap, colSpec);
            resolvedCols[colSpec.LogicalName] = col;
        }

        var firstDataRow = headerRow + 1;

        var stopRowExclusive = ResolveStopRowExclusive(
            firstDataRow,
            baseCol,
            spec.EndMarkers,
            spec.NextSectionStartMarkers);

        var list = new List<T>(capacity: Math.Max(8, stopRowExclusive - firstDataRow));

        for (var r = firstDataRow; r < stopRowExclusive; r++)
        {
            // Terminator rule (default: first required column empty)
            if (spec.Terminator != null && spec.Terminator(_ws, r, resolvedCols))
                break;

            // Build row using the row factory
            var item = spec.RowFactory(_ws, r, resolvedCols);
            list.Add(item);
        }

        return list;
    }

    // -------------------- Internals --------------------

    private int FindHeaderRow<T>(TableSectionSpec<T> spec, int baseRow, int baseCol)
    {
        var maxRow = Math.Min(_area.LastRow, baseRow + spec.HeaderSearchMaxRows);

        for (var r = baseRow; r <= maxRow; r++)
        {
            // if header marker provided, check that exact column
            if (spec.HeaderMarkerTexts.Length > 0)
            {
                var cell = _ws.Cell(r, baseCol);
                var s = cell.GetString().Trim();
                if (s.Length > 0 && ContainsIgnoreCase(spec.HeaderMarkerTexts, s))
                    return r;
            }
            else
            {
                // Otherwise: check if required headers exist in this row.
                // We build a quick map of that row headers and validate required ones.
                var map = BuildHeaderMap(r, spec.HeaderRowScanMaxCols);
                if (spec.HasAllRequiredHeaders(map))
                    return r;
            }
        }

        throw new InvalidOperationException($"Header row not found under start markers: {string.Join(", ", spec.StartMarkers)}");
    }

    private Dictionary<string, int> BuildHeaderMap(int headerRow, int maxColsToScan)
    {
        var firstCol = _area.FirstCol;
        var lastCol = Math.Min(_area.LastCol, firstCol + maxColsToScan - 1);

        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (var c = firstCol; c <= lastCol; c++)
        {
            var cell = _ws.Cell(headerRow, c);
            if (cell.IsEmpty()) continue;

            var name = cell.GetString().Trim();
            if (name.Length == 0) continue;

            // first occurrence wins
            if (!map.ContainsKey(name))
                map[name] = c;
        }

        return map;
    }

    private static int ResolveColumn(Dictionary<string, int> headerMap, ColumnSpec colSpec)
    {
        // Try each alias until found
        foreach (var alias in colSpec.HeaderAliases)
        {
            if (headerMap.TryGetValue(alias, out var col))
                return col;
        }

        throw new InvalidOperationException(
            $"Could not resolve column '{colSpec.LogicalName}'. Tried aliases: {string.Join(", ", colSpec.HeaderAliases)}");
    }

    private int ResolveStopRowExclusive(
        int startRow,
        int markerCol,
        string[] endMarkers,
        string[] nextStartMarkers)
    {
        // Default stop: end of used area + 1
        var stop = _area.LastRow + 1;

        // If end markers exist, find earliest occurrence below startRow in markerCol
        if (endMarkers.Length > 0)
        {
            var endRow = _markers.FindFirstRowInColumnBelow(markerCol, startRow, endMarkers);
            if (endRow > 0) stop = Math.Min(stop, endRow);
        }

        // If next section start markers provided, also stop at earliest of those
        if (nextStartMarkers.Length > 0)
        {
            var nextRow = _markers.FindFirstRowInColumnBelow(markerCol, startRow, nextStartMarkers);
            if (nextRow > 0) stop = Math.Min(stop, nextRow);
        }

        return stop;
    }

    private static bool ContainsIgnoreCase(string[] items, string value)
    {
        for (var i = 0; i < items.Length; i++)
            if (string.Equals(items[i], value, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    // -------------------- Specs --------------------

    public sealed class KeyValueSectionSpec
    {
        public string[] StartMarkers { get; init; } = [];

        // optional: if the key column contains this marker, stop reading
        public string[] EndMarkers { get; init; } = [];

        // optional: you can use the next section start marker as the end (works when sections are back-to-back)
        public string[] NextSectionStartMarkers { get; init; } = [];

        // offsets relative to the start marker cell
        public int StartRowOffset { get; init; } = 1;
        public int KeyColOffset { get; init; } = 0;
        public int ValueColOffset { get; init; } = 1;

        // per-key custom parsing (fast and explicit when you know types)
        public Dictionary<string, Func<IXLCell, object?>> PerKeyParsers { get; init; }
            = new(StringComparer.OrdinalIgnoreCase);
    }

    

    // -------------------- Marker index (performance) --------------------

    private sealed class MarkerIndex
    {
        private readonly Dictionary<string, List<(int Row, int Col)>> _index;

        private readonly IXLWorksheet _ws;
        private readonly UsedArea _area;

        public MarkerIndex(IXLWorksheet ws, UsedArea area)
        {
            _ws = ws;
            _area = area;
            _index = new Dictionary<string, List<(int, int)>>(StringComparer.OrdinalIgnoreCase);

            // One pass over used area only; store positions of non-empty text cells
            for (var r = _area.FirstRow; r <= _area.LastRow; r++)
            {
                for (var c = _area.FirstCol; c <= _area.LastCol; c++)
                {
                    var cell = ws.Cell(r, c);
                    if (cell.IsEmpty()) continue;

                    // Using GetString() is OK; we only do this once per sheet and only in used region.
                    var s = cell.GetString().Trim();
                    if (s.Length == 0) continue;

                    if (!_index.TryGetValue(s, out var list))
                    {
                        list = new List<(int, int)>(1);
                        _index[s] = list;
                    }
                    list.Add((r, c));
                }
            }
        }

        public IXLCell FindFirst(string[] markerTexts)
        {
            if (markerTexts is null || markerTexts.Length == 0)
                throw new ArgumentException("At least one marker text is required.");

            (int Row, int Col) best = (int.MaxValue, int.MaxValue);
            var found = false;

            foreach (var t in markerTexts)
            {
                if (!_index.TryGetValue(t, out var positions)) continue;
                // choose smallest row then col
                foreach (var pos in positions.Where(pos => pos.Row < best.Row || (pos.Row == best.Row && pos.Col < best.Col)))
                {
                    best = pos;
                    found = true;
                }
            }

            if (!found)
                throw new InvalidOperationException($"Marker not found. Tried: {string.Join(", ", markerTexts)}");

            return _ws.Cell(best.Row, best.Col);
        }

        public int FindFirstRowInColumnBelow(int col, int startRow, string[] markerTexts)
        {
            var bestRow = int.MaxValue;

            foreach (var t in markerTexts)
            {
                if (!_index.TryGetValue(t, out var positions)) continue;
                foreach (var pos in positions.Where(pos => pos.Col == col && pos.Row >= startRow && pos.Row < bestRow))
                {
                    bestRow = pos.Row;
                }
            }

            return bestRow == int.MaxValue ? -1 : bestRow;
        }
    }

    private readonly struct UsedArea
    {
        public int FirstRow { get; }
        public int LastRow { get; }
        public int FirstCol { get; }
        public int LastCol { get; }

        private UsedArea(int fr, int lr, int fc, int lc)
        {
            FirstRow = fr; LastRow = lr; FirstCol = fc; LastCol = lc;
        }

        public static UsedArea FromWorksheet(IXLWorksheet ws)
        {
            var ru = ws.RangeUsed();
            if (ru == null)
                return new UsedArea(1, 1, 1, 1);

            return new UsedArea(
                ru.RangeAddress.FirstAddress.RowNumber,
                ru.RangeAddress.LastAddress.RowNumber,
                ru.RangeAddress.FirstAddress.ColumnNumber,
                ru.RangeAddress.LastAddress.ColumnNumber);
        }
    }
}