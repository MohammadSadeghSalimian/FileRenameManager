using FileRenameManager.App;
using FileRenameManager.Core;
using FileRenameManager.Infrastructures.Excels;
using FileRenameManager.Infrastructures.MetaData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FileRenameManager.ConsoleApp;

public class ServiceSetupConsoleApp
{
    public IServiceProvider ServiceProvider { get; }

    public ServiceSetupConsoleApp()
    {
        var services = new ServiceCollection();

        // Core services
        AddAppServices(services);

        // Setup MediatR: register handlers from relevant assemblies.
        // Pass marker types to the AddMediatR extension so it scans those assemblies.
        // Adjust marker types if your request/handler implementations live in other assemblies.

        var config = new MediatRServiceConfiguration()
        {
            LicenseKey =
                "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkzNzUwNDAwIiwiaWF0IjoiMTc2MjI5MzU0MSIsImFjY291bnRfaWQiOiIwMTlhNTBlMGZlMTM3OTYxYjg0MjkxOGI2NWUxYmVhMiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazk4ZTJ0NmtleHFianZ0cndxM2NrNXRuIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.ClNtBy4VTEauoSKAJi_oSs6C15OD3P7blJdONT_5NAaSwPcD0kGMsjnOmMdEcBpiKB5IOx0hio03hWsw0O7HNv9APmEfmQyFjAWDBgcnOclgtF4pAQQklmjujhPoxcVgMh8SE272mouUKXCrdxXVRgd5OrjmMWRjZOJnYTEFDDUe5l6opj0YBanOV28Z_aj5UA0SBk5AJ2yJ3wjfnDLZGHEDZCDIV5bTrmwGEinKkPpFocgmalYWI6Rb52zr-XGlRdn-bZtk9xVHpSx-X7DYmrrOguLBhtA_8CYUPKU8QB53OJdHZ_yjr8rPl9NzyHtsL4PonByLgKs_uN42F4vZ_g",
        };
        config.RegisterServicesFromAssemblies(this.GetType().Assembly, typeof(IFfUnit).Assembly);
        services.AddMediatR(config);

        // Add Microsoft.Extensions.Logging so ILogger<T> and logging providers are available.
        // Console and Debug providers are common for WPF development; adjust providers as needed.
        services.AddLogging(builder =>
        {
            builder.AddConsole();
               
            // Optionally set minimum level:
            // builder.SetMinimumLevel(LogLevel.Information);
        });

        // Optionally register WPF stuff here as well:
        // services.AddSingleton<MainWindow>();
        // services.AddSingleton<MainViewModel>();


           
          
        ServiceProvider = services.BuildServiceProvider();
    }

       
    private static void AddAppServices(ServiceCollection services)
    {
        services.AddSingleton<IReporter, AnsiConsoleReporter>(); // Replace with your WPF reporter later
        services.AddSingleton<IImageFileProvider, ImageFileProvider>();
        services.AddSingleton<IVideoFileProvider, VideoFileProvider>();
        services.AddSingleton<IPhotoMetadataService, PhotoMetadataService>();
        services.AddSingleton<INameProvider, DefaultNameProvider>();
        services.AddSingleton<IFileMover, SafeFileMover>();
        services.AddSingleton<IFileOrganizer, FileOrganizer>();
        services.AddSingleton<IMessageUnit, CmMessageUnit>();
        services.AddSingleton<IVideoMetadataService, VideoMetadataService>();
        services.AddSingleton<IFileSearcher, FileSearcher>();
        services.AddSingleton<IExcelInputReader, ExcelInputReader>();


        services.AddSingleton<CmView>();
    }

}