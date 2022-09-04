﻿using Microsoft.WindowsAPICodePack.Dialogs;
using PcInfoApp.PcInfoClasses;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PcInfoApp.UserControls
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
        }

        private void ChooseFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog OpenFolder = new CommonOpenFileDialog();
            OpenFolder.IsFolderPicker = true;
            if (OpenFolder.ShowDialog() == CommonFileDialogResult.Ok)
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
                if (!string.IsNullOrWhiteSpace(OpenFolder.FileName))
                {
                    FilesSizeClass.FolderPath = OpenFolder.FileName;
                    FilesSizeClass.StopThread = false;
                    FilesSizeClass.GetFilesBackGroundWorker.DoWork += FilesSizeClass.GetFilesSizeByDir;
                    FilesSizeClass.GetNumberOfFilesBackGroundWorker.DoWork += FilesSizeClass.GetNumberOfFiles;
                    FilesSizeClass.GetFilesBackGroundWorker.RunWorkerAsync();
                    FilesSizeClass.GetNumberOfFilesBackGroundWorker.RunWorkerAsync();
                }
            }
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
    }
}