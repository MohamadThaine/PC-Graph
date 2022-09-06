using PcInfoApp.PcInfoClasses;
using System;
using System.ComponentModel;
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
        public PcSpecs()
        {
            InitializeComponent();
            GettingStorageTemp = new BackgroundWorker();
            GettingStorageTemp.WorkerSupportsCancellation = true;
            GettingStorageTemp.DoWork += GettingStorageTemp_DoWork;
            GettingStorageTemp.RunWorkerAsync();
            StorageTempTimer = new DispatcherTimer();
            StorageTempTimer.Interval = TimeSpan.FromSeconds(1);
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
            int StorageName = -1;
            this.Dispatcher.Invoke(() =>
            {
                if (StorageNameComboBox.SelectedItem != null)
                    StorageName = StorageNameComboBox.SelectedIndex;
            });
            if (StorageName == -1)
                return;
            double TempValue = Convert.ToDouble(StorageClass.GetStorageTemp(StorageName));
            this.Dispatcher.Invoke(() =>
            {
                StorageTemp.Value = TempValue;
            });
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

        }
    }
}
