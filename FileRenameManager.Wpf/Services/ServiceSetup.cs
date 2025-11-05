using FileRenameManager.App;
using FileRenameManager.Core;
using FileRenameManager.Infrastructures;
using FileRenameManager.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;

namespace FileRenameManager.Wpf.Services
{
    /// <summary>
    /// Configures the DI container and exposes IServiceProvider.
    /// Create one instance of this at startup (e.g. in App.xaml.cs).
    /// </summary>
    public sealed class ServiceSetup
    {
        public IServiceProvider ServiceProvider { get; }
        private readonly ServiceCollection _services = [];

        public ServiceSetup()
        {
            _services = new ServiceCollection();
                
            // Core services
            AddAppServices(_services);

            // Setup MediatR: register handlers from relevant assemblies.
            // Pass marker types to the AddMediatR extension so it scans those assemblies.
            // Adjust marker types if your request/handler implementations live in other assemblies.

            var config = new MediatRServiceConfiguration()
            {
                LicenseKey =
                    "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkzNzUwNDAwIiwiaWF0IjoiMTc2MjI5MzU0MSIsImFjY291bnRfaWQiOiIwMTlhNTBlMGZlMTM3OTYxYjg0MjkxOGI2NWUxYmVhMiIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazk4ZTJ0NmtleHFianZ0cndxM2NrNXRuIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.ClNtBy4VTEauoSKAJi_oSs6C15OD3P7blJdONT_5NAaSwPcD0kGMsjnOmMdEcBpiKB5IOx0hio03hWsw0O7HNv9APmEfmQyFjAWDBgcnOclgtF4pAQQklmjujhPoxcVgMh8SE272mouUKXCrdxXVRgd5OrjmMWRjZOJnYTEFDDUe5l6opj0YBanOV28Z_aj5UA0SBk5AJ2yJ3wjfnDLZGHEDZCDIV5bTrmwGEinKkPpFocgmalYWI6Rb52zr-XGlRdn-bZtk9xVHpSx-X7DYmrrOguLBhtA_8CYUPKU8QB53OJdHZ_yjr8rPl9NzyHtsL4PonByLgKs_uN42F4vZ_g",
            };
            config.RegisterServicesFromAssemblies(this.GetType().Assembly, typeof(IFfUnit).Assembly);
            _services.AddMediatR(config);

            // Add Microsoft.Extensions.Logging so ILogger<T> and logging providers are available.
            // Console and Debug providers are common for WPF development; adjust providers as needed.
            _services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                // Optionally set minimum level:
                // builder.SetMinimumLevel(LogLevel.Information);
            });

            // Optionally register WPF stuff here as well:
            // services.AddSingleton<MainWindow>();
            // services.AddSingleton<MainViewModel>();


            AddViewModels(_services);
            SetLocator();
            ServiceProvider = _services.BuildServiceProvider();
        }

        private static void AddViewModels(ServiceCollection service)
        {
            service.AddSingleton<MainViewModel>();
            service.AddSingleton<CalenderRenamerVm>();
        }
        private static void AddAppServices(ServiceCollection services)
        {
            services.AddSingleton<IReporter, DebugReporter>(); // Replace with your WPF reporter later
            services.AddSingleton<IImageFileProvider, ImageFileProvider>();
            services.AddSingleton<IPhotoMetadataService, PhotoMetadataService>();
            services.AddSingleton<IFolderNameProvider, DefaultFolderNameProvider>();
            services.AddSingleton<IFileMover, SafeFileMover>();
            services.AddSingleton<IPhotoOrganizer, PhotoOrganizer>();
            services.AddSingleton<IFfUnit, CommonFfUnit>();
            services.AddSingleton<IMessageUnit, MessageUnit>();
        }

        private void SetLocator()
        {

            _services.UseMicrosoftDependencyResolver();
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
        }

    }
}