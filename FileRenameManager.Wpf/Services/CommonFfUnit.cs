using FileRenameManager.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileRenameManager.Wpf.Services
{
    public class CommonFfUnit : IFfUnit
    {
        public static string ExtensionString(FileFilter filter)
        {
            var aa = filter.Extensions.Select(x => x.ToLower().Replace("*", "").Replace(".", "")).Select(x => $"{x}");

            return string.Join(";", aa);
        }

        public static string ExtensionString2(FileFilter filter)
        {
            var aa = filter.Extensions.Select(x => x.ToLower().Replace("*", "").Replace(".", "")).Select(x => $"*.{x}");

            return string.Join(";", aa);
        }

        public bool OpenFile(out FileInfo? file, string title, params FileFilter[] filters)
        {

            var cOp = new CommonOpenFileDialog()

            {
                Title = title,
                AddToMostRecentlyUsedList = true,

                IsFolderPicker = false,
            };

            foreach (var item in filters)
            {
                cOp.Filters.Add(new CommonFileDialogFilter(item.Description, ExtensionString2(item)));
            }

            var result = cOp.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                file = null;
                return false;
            }

            file = new FileInfo(cOp.FileName);
            return true;
        }

        public bool SaveFile(out FileInfo? file, string title, params FileFilter[] filters)
        {

            var sv = new CommonSaveFileDialog()
            {
                Title = title,
                AddToMostRecentlyUsedList = true,

            };
            foreach (var item in filters)
            {
                sv.Filters.Add(new CommonFileDialogFilter(item.Description, ExtensionString(item)));
            }

            var result = sv.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                file = null;
                return false;
            }

            file = new FileInfo(sv.FileName);
            return true;
        }

        public bool OpenManyFiles(out FileInfo[]? files, string title, params FileFilter[] filters)
        {

            var op = new CommonOpenFileDialog()
            {
                Title = title,
                AddToMostRecentlyUsedList = true,

                Multiselect = true,
            };
            foreach (var item in filters)
            {
                op.Filters.Add(new CommonFileDialogFilter(item.Description, ExtensionString2(item)));
            }

            var result = op.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                files = null;
                return false;
            }

            files = op.FileNames.Select(x => new FileInfo(x)).ToArray();
            return true;
        }

        public bool OpenFolder(out DirectoryInfo? folder, string title)
        {
            var cOp = new CommonOpenFileDialog()
            {
                Title = title,
                AddToMostRecentlyUsedList = true,
                IsFolderPicker = true,
            };
            var result = cOp.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                folder = null;
                return false;
            }

            folder = new DirectoryInfo(cOp.FileName);
            return true;
        }

        public bool OpenManyFolders(out DirectoryInfo[]? folders, string title)
        {
            var cOp = new CommonOpenFileDialog()
            {
                Title = title,
                AddToMostRecentlyUsedList = true,
                IsFolderPicker = true,
                Multiselect = true,
            };
            var result = cOp.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
            {
                folders = null;
                return false;
            }

            folders = cOp.FileNames.Select(x => new DirectoryInfo(x)).ToArray();
            return true;
        }
    }
}
