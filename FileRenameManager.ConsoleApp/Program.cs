using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace FileRenameManager.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ss = new ServiceSetupConsoleApp();
            var cm = ss.ServiceProvider.GetService<CmView>();
            if (cm == null)
            {
                AnsiConsole.WriteLine("CmView service is not available.");
                return;
            }
            await cm.Start();


        }


    }
}
