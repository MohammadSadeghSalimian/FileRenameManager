namespace FileRenameManager.Core;



public sealed partial class DefaultNameProvider : INameProvider
{
    public string GetFolderNameMmBased(DateTime dateTaken)
    {
        // e.g. 2023-07-12 -> "23-07 July-12"
        return $"{dateTaken:yy-MM} {dateTaken:MMMM}-{dateTaken:dd}";
    }

    public string GetFileNameBasedOnDate(DateTime dateTaken, string extension)
    {
        //2024-07-21-09-55-43
        return $"{dateTaken:yyyy-MM-dd-HH-mm-ss}{extension}";

    }

    public string GetNameCycleFile(CycleFileWithDate cycleFile)
    {
        
        //  input: Left_12619 _2025-11-03-09-54-23_Cy-0.50
        // return Side_2023-12-01-10-47-51_Cy-4.50 (00375)
        return $"{cycleFile.Prefix}_{cycleFile.DateTaken:yyyy-MM-dd-HH-mm-ss}_Cy-{cycleFile.CycleNumber:0.00} ({cycleFile.Id}){cycleFile.Extension}";
    }

    
}