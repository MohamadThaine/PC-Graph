using FastSearchLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
namespace PcInfoApp.PcInfoClasses
{
    public class FilesSizeClass
    {
        static public string FolderPath { get; set; }
        static public ObservableCollection<Folder> Folders { get; set; } = new ObservableCollection<Folder>();
        static public BackgroundWorker GetFilesBackGroundWorker { get; set; } = new BackgroundWorker();
        static public BackgroundWorker GetNumberOfFilesBackGroundWorker { get; set; } = new BackgroundWorker();
        static public bool StopThread { get; set; } = false;
        static public int NumberOfFiles { get; set; } = 1;
        static public int CurrentProggrass { get; set; }
        static public bool EnableSortFolderBT { get; set; }
        static public event PropertyChangedEventHandler StaticPropertyChanged;
        public FilesSizeClass()
        {
        }
        static public void GetFilesSizeByDir(object? sender, DoWorkEventArgs e)
        {
            GetFilesBackGroundWorker.WorkerSupportsCancellation = true;
            EnableSortFolderBT = false;
            OnPropertyChanged("EnableSortFolderBT");
            GetFilesAndFolders();
            EnableSortFolderBT = true;
            OnPropertyChanged("EnableSortFolderBT");
        }
        static public void GetFilesAndFolders()
        {
            if (FolderPath != "" && Directory.Exists(FolderPath))
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    Folders.Clear();
                });
            }
            string FileSizeWithType = "";
            double FileSize = 0;
            string FileSizeType = "";
            string FileName = "";
            foreach (string Folder in Directory.GetDirectories(FolderPath, "*"))
            {
                double FolderSizeD = 0;
                string FolderSizeType = "";
                string FolderSize = "";
                ObservableCollection<File> FolderFiles = new ObservableCollection<File>();
                List<FileInfo> Files = FileSearcher.GetFilesFast(Folder, "*");
                foreach (FileInfo file in Files)
                {
                    FileSize = file.Length / 1024;
                    FolderSizeD += FileSize;
                    FileSizeType = "KB";
                    if (FileSize > 1024)
                    {
                        FileSize /= 1024;
                        FileSizeType = "MB";
                        if (FileSize > 1024)
                        {
                            FileSize /= 1024;
                            FileSizeType = "GB";
                            if (FileSize > 1024)
                            {
                                FileSize /= 1024;
                                FileSizeType = "TB";
                            }
                        }
                    }
                    if (FileSize.ToString().Length > 4)
                    {
                        FileSize = Convert.ToDouble(FileSize.ToString().Substring(0, 4));
                    }
                    FileSizeWithType = FileSize.ToString() + FileSizeType;
                    File file1 = new File(file.Name, FileSizeWithType, file.Length, file.FullName, MimeTypes.MimeTypeMap.GetMimeType(file.Extension));
                    FolderFiles.Add(file1);
                    CurrentProggrass++;
                    OnPropertyChanged("CurrentProggrass");
                }
                string FolderName = Folder.Substring(Folder.LastIndexOf(@"\") + 1).TrimStart();
                FolderSizeType = "KB";
                if (FolderSizeD > 1024)
                {
                    FolderSizeD /= 1024;
                    FolderSizeType = "MB";
                    if (FolderSizeD > 1024)
                    {
                        FolderSizeD /= 1024;
                        FolderSizeType = "GB";
                        if (FolderSizeD > 1024)
                        {
                            FolderSizeD /= 1024;
                            FolderSizeType = "TB";
                        }
                    }
                }
                if (FolderSizeD.ToString().Length > 4)
                {
                    FolderSizeD = Convert.ToDouble(FolderSizeD.ToString().Substring(0, 4));
                }
                FolderSize = FolderSizeD + FolderSizeType;
                Folder folder = new Folder(FolderName, FolderSize, FolderFiles);
                if (FolderFiles.Count > 0)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {

                        {
                            if (FolderFiles[0].FilePath.Contains(FolderPath))
                                Folders.Add(folder);
                            else
                                return;
                        }
                    });
                }
            }
            double OrignaFolderSizeD = 0;
            string OrignaFolderSizeType = "";
            ObservableCollection<File> OrignalFolderFiles = new ObservableCollection<File>();
            foreach (string file in Directory.EnumerateFiles(FolderPath, "*").OrderByDescending(file => FileSize = new FileInfo(file).Length).
                                                                                                             ThenBy(file => FileName = new FileInfo(file).Name))
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!GetFilesBackGroundWorker.CancellationPending || StopThread == false)
                {
                    fileInfo = new FileInfo(file);
                    FileSize = fileInfo.Length / 1024;
                    OrignaFolderSizeD += FileSize;
                    OrignaFolderSizeType = "KB";
                    FileSizeType = "KB";
                    if (FileSize > 1024)
                    {
                        FileSize /= 1024;
                        FileSizeType = "MB";
                        if (FileSize > 1024)
                        {
                            FileSize /= 1024;
                            FileSizeType = "GB";
                            if (FileSize > 1024)
                            {
                                FileSize /= 1024;
                                FileSizeType = "TB";
                            }
                        }
                    }
                    if (FileSize.ToString().Length > 4)
                    {
                        FileSize = Convert.ToDouble(FileSize.ToString().Substring(0, 4));
                    }
                    string FileSizes = FileSize + FileSizeType;
                    OrignalFolderFiles.Add(new File(fileInfo.Name, FileSizes, fileInfo.Length, fileInfo.FullName, MimeTypes.MimeTypeMap.GetMimeType(fileInfo.Extension)));
                    CurrentProggrass++;
                    OnPropertyChanged("CurrentProggrass");
                }
                else
                    return;

            }
            if (OrignaFolderSizeD > 1024)
            {
                OrignaFolderSizeD /= 1024;
                OrignaFolderSizeType = "MB";
                if (OrignaFolderSizeD > 1024)
                {
                    OrignaFolderSizeD /= 1024;
                    OrignaFolderSizeType = "GB";
                    if (OrignaFolderSizeD > 1024)
                    {
                        OrignaFolderSizeD /= 1024;
                        OrignaFolderSizeType = "TB";
                    }
                }
            }
            if (OrignaFolderSizeD.ToString().Length > 4)
            {
                OrignaFolderSizeD = Convert.ToDouble(OrignaFolderSizeD.ToString().Substring(0, 4));
            }
            string OrignalFolderSize = OrignaFolderSizeD + OrignaFolderSizeType;
            if (OrignalFolderFiles.Count != 0)
            {
                string FolderName = new DirectoryInfo(FolderPath).Name;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (OrignalFolderFiles[0].FilePath.Contains(FolderPath))
                        Folders.Add(new Folder(FolderName, OrignalFolderSize, OrignalFolderFiles));
                    else
                        return;
                });
            }
            if (GetFilesBackGroundWorker.IsBusy && StopThread == false)
            {
                GetFilesBackGroundWorker.CancelAsync();
                StopThread = true;
            }
        }
        static public void RemoveDeletedFiles(File file)
        {
            foreach (var folderFiles in Folders)
                if (folderFiles.FolderFiles.Contains(file))
                    folderFiles.FolderFiles.Remove(file);
        }
        static public void SortFolders()
        {
            var Ordered = Folders.OrderByDescending(x => x.FolderSizeInBytes);
            Folders = new ObservableCollection<Folder>(Ordered);
            OnPropertyChanged("Folders");
        }
        static private void OnPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
        static public void GetNumberOfFiles(object? sender, DoWorkEventArgs e)
        {
            GetNumberOfFilesBackGroundWorker.WorkerSupportsCancellation = true;
            NumberOfFiles = 0;
            OnPropertyChanged("NumberOfFiles");
            foreach (string Folder in Directory.GetDirectories(FolderPath, "*"))
            {
                if (GetNumberOfFilesBackGroundWorker.CancellationPending)
                    return;
                NumberOfFiles += Directory.EnumerateFiles(Folder, "*", new EnumerationOptions
                { IgnoreInaccessible = true, RecurseSubdirectories = true }).Count();
                OnPropertyChanged("NumberOfFiles");
            }
            NumberOfFiles += Directory.EnumerateFiles(FolderPath, "*", new EnumerationOptions
            { IgnoreInaccessible = true }).Count();
            OnPropertyChanged("NumberOfFiles");
            if (GetNumberOfFilesBackGroundWorker.IsBusy)
                GetNumberOfFilesBackGroundWorker.CancelAsync();
        }
    }
    public class File
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public long SizeInBytes { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public File(string Name, string Size, long SizeInBytes, string FilePath, string FileType)
        {
            this.Name = Name;
            this.Size = Size;
            this.FilePath = FilePath;
            this.SizeInBytes = SizeInBytes;
            this.FileType = FileType;
        }
    }
    public class Folder
    {
        public string FolderName { get; set; }
        public string FolderSize { get; set; }
        public decimal FolderSizeInBytes { get; set; }
        public ObservableCollection<File> FolderFiles { get; set; } = new ObservableCollection<File>();
        public Folder(string FolderName, string FolderSize, ObservableCollection<File> FolderFiles)
        {
            this.FolderName = FolderName;
            this.FolderSize = FolderSize;
            FolderSizeInBytes = GetFolderSizeInBytes();
            this.FolderFiles = FolderFiles;
        }
        private decimal GetFolderSizeInBytes()
        {
            decimal SizeInBytes = 0;
            if (FolderSize != "0")
            {
                string SizeType = FolderSize.Substring(FolderSize.Length - 2, 2);
                SizeInBytes = decimal.Parse(FolderSize.Substring(0, FolderSize.Length - 2)) * 1024;
                if (SizeType == "MB")
                    SizeInBytes *= 1024;
                else if (SizeType == "GB")
                    SizeInBytes = SizeInBytes * 1024 * 1024;
                else if (SizeType == "TB")
                    SizeInBytes = SizeInBytes * 1024 * 1024 * 1024;
            }
            return SizeInBytes;
        }
    }
}

