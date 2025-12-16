using System.Globalization;
using ClosedXML.Excel;

namespace FileRenameManager.Infrastructures.Excels;

public static class CellParsers
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    /// <summary>
    /// Fast "best guess" conversion:
    /// - if cell is numeric -> double (or int if integral and fits)
    /// - if boolean -> bool
    /// - if date -> DateTime
    /// - else -> trimmed string
    /// </summary>
    public static object? Auto(IXLCell cell)
    {
        if (cell.IsEmpty()) return null;

        // ClosedXML value inspection
        var v = cell.Value;
        if (v.IsNumber)
        {
            double d = v.GetNumber();
            // If it's integral, you can return long/int (optional behavior)
            if (Math.Abs(d - Math.Round(d)) < 1e-12 && d <= int.MaxValue && d >= int.MinValue)
                return (int)Math.Round(d);
            return d;
        }
        if (v.IsBoolean) return v.GetBoolean();
        if (v.IsDateTime) return v.GetDateTime();

        // fallback to string
        var s = cell.GetString().Trim();
        if (s.Length == 0) return null;

        // Try fast numeric parse from string (covers cases where Excel stored as text)
        if (int.TryParse(s, NumberStyles.Integer, Inv, out int i)) return i;
        if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, Inv, out double dd)) return dd;

        return s;
    }

    public static int Int(IXLCell cell, int defaultValue = 0)
    {
        if (cell.IsEmpty()) return defaultValue;
        var v = cell.Value;
        if (v.IsNumber) return (int)Math.Round(v.GetNumber());
        var s = cell.GetString().Trim();
        return int.TryParse(s, NumberStyles.Integer, Inv, out var i) ? i : defaultValue;
    }

    public static double Double(IXLCell cell, double defaultValue = 0.0)
    {
        if (cell.IsEmpty()) return defaultValue;
        var v = cell.Value;
        if (v.IsNumber) return v.GetNumber();
        var s = cell.GetString().Trim();
        return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, Inv, out var d) ? d : defaultValue;
    }

    public static string String(IXLCell cell, string defaultValue = "")
    {
        if (cell.IsEmpty()) return defaultValue;
        var s = cell.GetString();
        return string.IsNullOrWhiteSpace(s) ? defaultValue : s.Trim();
    }

    public static TEnum Enum<TEnum>(IXLCell cell, TEnum defaultValue) where TEnum : struct
    {
        var s = String(cell, "");
        if (s.Length == 0) return defaultValue;
        return System.Enum.TryParse<TEnum>(s, ignoreCase: true, out var e) ? e : defaultValue;
    }
}