using FileRenameManager.App;
using FileRenameManager.App.Ch;
using Humanizer;
using MediatR;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public class CmView(IMessageUnit messageUnit, IMediator mediator)
{


    public async Task Start()
    {
        Console.Clear();
        AnsiConsole.Clear();
        bool keepRunning = true;
        while (keepRunning)
        {

            var request = await StartOptionChoices();
            switch (request)
            {
                case "Phone Camera":
                    await RunPhoneCameraRenaming();
                    break;
                case "Roaming Camera":
                    await RunRoamingCameraRenaming();
                    break;
                case "Fixed Camera":
                    await RunFixedCamera();
                    break;
                case "DIC cameras":
                    break;
                case "Adding Drift Level to Name":
                    await RunAddingDriftLevel();
                    break;
                case "Exit":
                    keepRunning = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
    private async Task<string> StartOptionChoices()
    {
        var pp = new SelectionPrompt<string>()
        {
            Title = "`Select renaming method",
            Converter = x => x
        };
        pp.AddChoices("Phone Camera", "Fixed Camera", "Roaming Camera", "DIC cameras", "Adding Drift Level to Name", "Exit");


        var res = await AnsiConsole.PromptAsync(pp);
        return res;


    }

    private async Task RunPhoneCameraRenaming()
    {
        try
        {
            AnsiConsole.WriteLine("This option can rename the images taken from a camera. It reads the date from each image's metadata and try to make folders based on date of each images and move the images inside each folder. It should be noted that the images name wont be renamed!");
            var recursive = await AnsiConsole.PromptAsync(new SelectionPrompt<bool>()
            {
                Title = "Search other folders inside the main folder?",
                Converter = x => x ? "Yes" : "No"
            }.AddChoices(false, true));
            var dd = await AnsiConsole.PromptAsync(new TextPrompt<string>("Please enter the folder path:")
            {
                AllowEmpty = true,

            }.Validate(x => Directory.Exists(x)
                ? ValidationResult.Success()
                : ValidationResult.Error("The folder doesn't exist")));
            var directoryInfo = new DirectoryInfo(dd);
            var hours = await AnsiConsole.PromptAsync(new TextPrompt<double>("Please enter the hours to add:")
            {
                AllowEmpty = true,
            });

            await messageUnit.InfoAsync("Renaming started...");
            var res = await mediator.Send(new OrganizePhoneCameraRq(directoryInfo, recursive, hours));
            if (res)
            {
                await messageUnit.InfoAsync("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.ErrorAsync(e);
        }
    }

    private async Task RunRoamingCameraRenaming()
    {
        try
        {
            AnsiConsole.WriteLine("This option rename the images based on the date time and make folders based on the date of images and move all to the folders");
            var recursive = await AnsiConsole.PromptAsync(new SelectionPrompt<bool>()
            {
                Title = "Search other folders inside the main folder?",
                Converter = x => x ? "Yes" : "No"
            }.AddChoices(false, true));
            var dd = await AnsiConsole.PromptAsync(new TextPrompt<string>("Please enter the folder path:")
            {
                AllowEmpty = true,

            }.Validate(x => Directory.Exists(x)
                ? ValidationResult.Success()
                : ValidationResult.Error("The folder doesn't exist")));
            var directoryInfo = new DirectoryInfo(dd);
            var hours = await AnsiConsole.PromptAsync(new TextPrompt<double>("Please enter the hours to add:")
            {
                AllowEmpty = true,
            });


            await messageUnit.InfoAsync("Renaming started...");
            var res = await mediator.Send(new RenameRoamingCameraRq(directoryInfo, recursive, hours));
            if (res)
            {
                await messageUnit.InfoAsync("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.ErrorAsync(e);
        }
    }

    private async Task RunFixedCamera()
    {
        try
        {
            AnsiConsole.WriteLine("Reformat and naming the images taken by the fixed cameras!");
            var dd = await AnsiConsole.PromptAsync(new TextPrompt<string>("Please enter the folder path:")
            {
                AllowEmpty = true,

            }.Validate(x => Directory.Exists(x)
                ? ValidationResult.Success()
                : ValidationResult.Error("The folder doesn't exist")));
            var directoryInfo = new DirectoryInfo(dd);


            await messageUnit.InfoAsync("Renaming started...");
            var res = await mediator.Send(new MovingFixedCameraImagesRq(directoryInfo));
            if (res)
            {
                await messageUnit.InfoAsync("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.ErrorAsync(e);
        }
    }

    private async Task RunAddingDriftLevel()
    {
        try
        {
            AnsiConsole.WriteLine("This option allows adding a drift level to the images name which has cy in their names.");
           
           
            var excelFileAddress = await AnsiConsole.PromptAsync(new TextPrompt<string>("Please enter the excel input file:")
            {
                AllowEmpty = false,
            });
            excelFileAddress = excelFileAddress.Replace("\"", "").Trim(); // Added Trim() to clean up any surrounding whitespace
            await messageUnit.InfoAsync("Adding drift level started...");
            var response = await mediator.Send(new AddDriftLevelRq( new FileInfo(excelFileAddress)));
            if (response.IsSuccess)
            {
                await messageUnit.InfoAsync("Finished");
            }
            else
            {
                await messageUnit.ErrorAsync(response.Message);
            }
        }
        catch (Exception e)
        {
            await messageUnit.ErrorAsync(e);
        }
    }

}