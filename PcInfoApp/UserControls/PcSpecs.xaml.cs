using PcInfoApp.PcInfoClasses;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
namespace PcInfoApp.UserControls
{
    /// <summary>
    /// Interaction logic for PcSpecs.xaml
    /// </summary>
    public partial class PcSpecs : UserControl
    {
        BackgroundWorker GettingStorageTemp;
        DispatcherTimer StorageTempTimer;
        bool IsFirstTimeReadingStorageTemp = true;
        public PcSpecs()
        {
            InitializeComponent();
            GettingStorageTemp = new BackgroundWorker();
            GettingStorageTemp.WorkerSupportsCancellation = true;
            GettingStorageTemp.DoWork += GettingStorageTemp_DoWork;
            GettingStorageTemp.RunWorkerAsync();
            StorageTempTimer = new DispatcherTimer();
            StorageTempTimer.Interval = TimeSpan.FromSeconds(120);
            StorageTempTimer.Tick += StorageTempTimer_Tick;
            StorageTempTimer.Start();
        }

        private void StorageTempTimer_Tick(object? sender, EventArgs e)
        {
            GettingStorageTemp = new BackgroundWorker();
            GettingStorageTemp.WorkerSupportsCancellation = true;
            GettingStorageTemp.DoWork += GettingStorageTemp_DoWork;
            GettingStorageTemp.RunWorkerAsync();
        }

        private void GettingStorageTemp_DoWork(object? sender, DoWorkEventArgs e)
        {
            bool IsTheAppInFocus = false;
            this.Dispatcher.Invoke(() =>
            {
                IsTheAppInFocus = Application.Current.Windows[0].IsActive;
            });
            if (IsTheAppInFocus || IsFirstTimeReadingStorageTemp)
            {
                int StorageIndex = -1;
                this.Dispatcher.Invoke(() =>
                {
                    if (StorageNameComboBox.SelectedItem != null)
                        StorageIndex = StorageNameComboBox.SelectedIndex;
                });
                if (StorageIndex == -1)
                    StorageIndex = 0;
                double TempValue = Convert.ToDouble(StorageClass.GetStorageTemp(StorageIndex));
                this.Dispatcher.Invoke(() =>
                {
                    StorageTemp.Value = TempValue;
                });
                IsFirstTimeReadingStorageTemp = false;
            }
            GettingStorageTemp.CancelAsync();
        }

        private void StorageNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                SpaceProggrass.Value = Convert.ToDouble(StorageClass.StorageSpace[StorageNameComboBox.SelectedIndex] - StorageClass.StorageFreeSpace[StorageNameComboBox.SelectedIndex]);
                SpaceProggrass.Maximum = Convert.ToDouble(StorageClass.StorageSpace[StorageNameComboBox.SelectedIndex]);
            }
            catch (ArgumentOutOfRangeException ex)
            {

            }
            if (!GettingStorageTemp.IsBusy)
                GettingStorageTemp.RunWorkerAsync();
        }
    }
}
