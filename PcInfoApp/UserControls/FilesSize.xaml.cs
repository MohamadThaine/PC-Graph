using Microsoft.WindowsAPICodePack.Dialogs;
using PCGraph.PcInfoClasses;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using File = PCGraph.PcInfoClasses.File;

namespace PCGraph.UserControls
{
    /// <summary>
    /// Interaction logic for FilesSize.xaml
    /// </summary>
    public partial class FilesSize : UserControl
    {
        File SelectedFile;
        bool isButtonsEnabled = false;
        public FilesSize()
        {
            InitializeComponent();
            DriveInfo[] drives = DriveInfo.GetDrives();
            ContextMenu contextMenuForFolders = new ContextMenu();
            MenuItem[] menuItems = new MenuItem[drives.Length + 1];
            for (int i = 0; i < drives.Length; i++)
            {
                if (drives[i].IsReady)
                {
                    menuItems[i] = new MenuItem();
                    menuItems[i].Header = drives[i].Name;
                    menuItems[i].Click += FolderClick;
                    contextMenuForFolders.Items.Add(menuItems[i]);
                }
            }
            menuItems[menuItems.Length - 1] = new MenuItem();
            menuItems[menuItems.Length - 1].Header = "Choose Another Folder";
            menuItems[menuItems.Length - 1].Click += FolderClick;
            contextMenuForFolders.Items.Add(menuItems[menuItems.Length - 1]);
            ChooseFolder.ContextMenu = contextMenuForFolders;
        }

        private void FolderClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string ChoosenPath = "";
            if (menuItem.Header == "Choose Another Folder")
            {
                CommonOpenFileDialog OpenFolder = new CommonOpenFileDialog();
                OpenFolder.IsFolderPicker = true;
                if (OpenFolder.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    ChoosenPath = OpenFolder.FileName;
                }
                else
                    return;
            }
            else
                ChoosenPath = menuItem.Header as string;
            GetFolderFiles(ChoosenPath);
            GetFreeSpace(ChoosenPath[0]);
        }
        private void GetFolderFiles(string FolderPath)
        {
            FilesSizeClass.CurrentProggrass = 0;
            EnableButtons(false);
            isButtonsEnabled = false;
            FilesSizeClass.CurrentProggrass = 0;
            FilesSizeClass.NumberOfFiles = 0;
            ReadingStatus.Visibility = Visibility.Visible;
            if (FilesSizeClass.GetFilesBackGroundWorker.IsBusy || FilesSizeClass.StopThread == true && FilesSizeClass.GetNumberOfFilesBackGroundWorker.IsBusy)
            {
                FilesSizeClass.GetFilesBackGroundWorker.CancelAsync();
                FilesSizeClass.GetNumberOfFilesBackGroundWorker.CancelAsync();
                FilesSizeClass.StopThread = true;
            }
            FilesSizeClass.GetFilesBackGroundWorker = new BackgroundWorker();
            FilesSizeClass.GetNumberOfFilesBackGroundWorker = new BackgroundWorker();
            if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                FilesSizeClass.FolderPath = FolderPath;
                FilesSizeClass.StopThread = false;
                FilesSizeClass.GetFilesBackGroundWorker.DoWork += FilesSizeClass.GetFilesSizeByDir;
                FilesSizeClass.GetNumberOfFilesBackGroundWorker.DoWork += FilesSizeClass.GetNumberOfFiles;
                FilesSizeClass.GetFilesBackGroundWorker.RunWorkerAsync();
                FilesSizeClass.GetNumberOfFilesBackGroundWorker.RunWorkerAsync();
            }
        }
        private void GetFreeSpace(char Drive)
        {
            DriveInfo drive = new DriveInfo(Drive.ToString());
            FilesSizeClass.FreeSpace = "Free Space: " + (drive.AvailableFreeSpace / 1024 / 1024 / 1024) + "GB (OF " + ((drive.TotalSize / 1024 / 1024 / 1024)) + "GB)";
            FilesSizeClass.OnPropertyChanged("FreeSpace");
        }
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile != null)
            {
                MessageBoxResult ButtonClicked = MessageBox.Show("Are you sure want to delete this file: " + SelectedFile.Name + "?", "Are you sure?", MessageBoxButton.YesNo);
                if (ButtonClicked == MessageBoxResult.Yes)
                {
                    System.IO.File.Delete(SelectedFile.FilePath);
                    FilesSizeClass.RemoveDeletedFiles(SelectedFile);

                }
            }
        }
        private void EnableButtons(bool Enable)
        {
            ShowFolder.IsEnabled = Enable;
            DeleteFile.IsEnabled = Enable;
        }
        private void ShowFolder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFile != null)
            {
                string SelectFileInexplorer = "/select , \"" + SelectedFile.FilePath + "\"";
                Process.Start("explorer.exe", SelectFileInexplorer);
            }

        }

        private void FilesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isButtonsEnabled)
            {
                isButtonsEnabled = true;
                EnableButtons(true);
            }
            System.Windows.Controls.ListView SelectedItem = sender as System.Windows.Controls.ListView;
            SelectedFile = SelectedItem.SelectedItem as File;
        }
        private void SortFolders_Click(object sender, RoutedEventArgs e)
        {
            FilesSizeClass.SortFolders();
        }
        private void ChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            var addButton = sender as FrameworkElement;
            if (addButton != null)
                addButton.ContextMenu.IsOpen = true;
        }
    }
}
