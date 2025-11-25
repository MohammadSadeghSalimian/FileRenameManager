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
                case RequestedMethod.PhoneCamera:
                    await RunPhoneCameraRenaming();
                    break;
                case RequestedMethod.RoamingCamera:
                    await RunRoamingCameraRenaming();
                    break;
                case RequestedMethod.FixedCamera:
                    await RunFixedCamera();
                    break;
                case RequestedMethod.DicCamera:
                    break;
                case RequestedMethod.Exit:
                    keepRunning = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
    private async Task<RequestedMethod> StartOptionChoices()
    {
        var pp = new SelectionPrompt<RequestedMethod>()
        {
            Title = "`How are your images are taken?",
            Converter = x => x.ToString().Humanize()
        };
        pp.AddChoices([RequestedMethod.PhoneCamera, RequestedMethod.FixedCamera, RequestedMethod.RoamingCamera, RequestedMethod.DicCamera, RequestedMethod.Exit]);

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

            await messageUnit.Info("Renaming started...");
            var res = await mediator.Send(new OrganizePhoneCameraRq(directoryInfo, recursive, hours));
            if (res)
            {
                await messageUnit.Info("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.Error(e);
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


            await messageUnit.Info("Renaming started...");
            var res = await mediator.Send(new RenameRoamingCameraRq(directoryInfo, recursive, hours));
            if (res)
            {
                await messageUnit.Info("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.Error(e);
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


            await messageUnit.Info("Renaming started...");
            var res = await mediator.Send(new MovingFixedCameraImagesRq(directoryInfo));
            if (res)
            {
                await messageUnit.Info("Finished");
            }
        }
        catch (Exception e)
        {
            await messageUnit.Error(e);
        }
    }

}