using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FileRenameManager.App;
using FileRenameManager.Core;

namespace FileRenameManager.Infrastructures.Excels
{
    public class ExcelInputReader : IExcelInputReader
    {
       
        public async Task<ExcelInputModelAddingDriftLevel>  ReadFileAsync(FileInfo file,CancellationToken ct)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("File does not exist.");
            }

            var workBook = new XLWorkbook(file.FullName);
            try
            {
                var sheet = workBook.Worksheet(1);
                var excelReader = new SemiStructuredExcelReader(sheet);
                var keyValueConfig = new SemiStructuredExcelReader.KeyValueSectionSpec()
                {
                    StartMarkers = ["Inputs"],
                    NextSectionStartMarkers = ["Data"],
                    PerKeyParsers =
                    {
                        ["Folder Address"]=c=>c.GetString(),
                        ["Recursive"]=c=>c.GetBoolean()
                    }
                };
               
                var config = new TableSectionSpec<CycleUnit>()
                {
                    StartMarkers = ["Data"],
                    HeaderMarkerTexts = ["Cycle"],
                    Columns =
                    [
                        new ColumnSpec("Cycle", "Cycle"), new ColumnSpec("Drift", "Drift Level"),
                        new ColumnSpec("Type", "Cycle Type")
                    ],
                    RowFactory = (ws, row, cols) =>
                        new CycleUnit(
                            ws.Cell(row, cols["Cycle"]).GetDouble(),
                            ws.Cell(row, cols["Drift"]).GetDouble(),
                            ws.Cell(row, cols["Type"]).GetString()
                        ),
                    Terminator = (ws, row, cols) => ws.Cell(row, cols["Cycle"]).IsEmpty()
                };
                var aas = await Task.Run(() =>
                {
                    var inputs = excelReader.ReadKeyValueSection(keyValueConfig);
                    if (inputs==null)
                    {
                        throw new Exception("Inputs configuration is missing.");
                    }
                    var data= excelReader.ReadTableSection(config);

                    inputs.TryGetValue("Folder Address", out var fo);
                    var folderAddress = fo as string ?? throw new Exception("Folder Address is missing in Inputs.");

                    if (!inputs.TryGetValue("Recursive", out var recursiveValue) || recursiveValue is null)
                    {
                        throw new Exception("Recursive configuration is missing.");
                    }

                    bool recursive;
                    if (recursiveValue is bool b)
                    {
                        recursive = b;
                    }
                    else if (recursiveValue is string s && bool.TryParse(s, out var parsed))
                    {
                        recursive = parsed;
                    }
                    else
                    {
                        throw new Exception("Recursive configuration must be a boolean or parsable string.");
                    }

                    var dataDictionary = data.ToDictionary(x => x.Cycle, x => x);
                    return new ExcelInputModelAddingDriftLevel(folderAddress, recursive, dataDictionary);
                }, ct);

                return aas; // or any further processing needed
            }
            finally
            {
                workBook.Dispose();
            }
        }
        
    }



}
