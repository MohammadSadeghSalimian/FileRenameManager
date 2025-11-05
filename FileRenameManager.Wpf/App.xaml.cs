using FileRenameManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using FileRenameManager.Wpf.Services;
using FileRenameManager.Wpf.ViewModels;
using FileRenameManager.Wpf.Views;

namespace FileRenameManager.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        public App()
        {
            var setup = new ServiceSetup();
          
        }
        
    }
}

