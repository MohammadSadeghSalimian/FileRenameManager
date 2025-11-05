using FileRenameManager.App;
using FileRenameManager.App.Ch;
using Humanizer;
using MediatR;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp;

public class CmView(IMessageUnit messageUnit,IMediator mediator)
{


    public async Task Start()
    {
        Console.Clear();
        AnsiConsole.Clear();
        var request = await StartOptionChoices();
        switch (request)
        {
            case RequestedMethod.PhoneCamera:
                await RunPhoneCameraRenaming();
                break;
            case RequestedMethod.RoamingCamera:
                break;
            case RequestedMethod.StationaryCamera:
                break;
            case RequestedMethod.DicCamera:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public async Task<RequestedMethod> StartOptionChoices()
    {
        var pp = new SelectionPrompt<RequestedMethod>()
        {
            Title = "`How are your images are taken?",
            Converter = x => x.ToString().Humanize()
        };
        pp.AddChoices([RequestedMethod.PhoneCamera, RequestedMethod.StationaryCamera, RequestedMethod.RoamingCamera, RequestedMethod.DicCamera]);

        var res = await AnsiConsole.PromptAsync(pp);
        return res;

       
    }

    public async Task RunPhoneCameraRenaming()
    {
        try
        {
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
            await messageUnit.Info("Renaming started...");
            var res= await mediator.Send(new RenamePhoneCameraRq(directoryInfo, recursive));
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