using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace PcInfoApp.PcInfoClasses
{
    public class RamClass : INotifyPropertyChanged
    {
        public double RamSize { get; set; }
        public long RamLoad { get; set; }
        public int MaxRamLoad { get; set; }
        public int RamUsage { get; set; }
        public string HighestAppUsing { get; set; }
        public string RamUsageFromTheApp { get; set; }
        private BackgroundWorker GetChangingInfo;
        public RamClass()
        {
            GetRamInfo();
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.DoWork += GetChangingInfo_DoWork; ; ;
            GetChangingInfo.RunWorkerAsync();
            DispatcherTimer RamInfoTimers = new DispatcherTimer();
            RamInfoTimers.Interval = TimeSpan.FromSeconds(1);
            RamInfoTimers.Tick += RamInfoTimer_Tick1;
            RamInfoTimers.Start();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RamInfoTimer_Tick1(object? sender, EventArgs e)
        {
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.DoWork += GetChangingInfo_DoWork; ; ;
            GetChangingInfo.RunWorkerAsync();
            OnPropertyChanged("RamLoad");
            OnPropertyChanged("MaxRamLoad");
            OnPropertyChanged("RamUsage");
            OnPropertyChanged("HighestAppUsing");
            OnPropertyChanged("RamUsageFromTheApp");
        }
        private void GetRamInfo()
        {
            Computer pc = new Computer
            {
                IsMotherboardEnabled = true,
            };
            pc.Open();
            foreach (LibreHardwareMonitor.Hardware.Motherboard.Motherboard motherboard in pc.Hardware)
            {
                motherboard.Update();
                motherboard.GetReport();
                foreach (MemoryDevice ram in motherboard.SMBios.MemoryDevices)
                {
                    if (ram.ManufacturerName != "")
                    {
                        RamSize += ram.Size / 1024;
                    }
                }
            }

        }
        private void GetChangingInfo_DoWork(object? sender, DoWorkEventArgs e)
        {
            this.RamLoad = (GC.GetGCMemoryInfo().MemoryLoadBytes) / 1024 / 1024 / 1024;
            if (Convert.ToInt32((this.RamLoad / RamSize) * 100) > this.MaxRamLoad)
                this.MaxRamLoad = Convert.ToInt32((this.RamLoad / RamSize) * 100);
            this.RamUsage = Convert.ToInt32((this.RamLoad / RamSize) * 100);
            Process[] process = Process.GetProcesses();
            long maxMemory = 0;
            string memorytype = "KB";
            foreach (Process processItem in process)
            {
                long memory = processItem.PagedMemorySize64;
                if (memory > maxMemory)
                {
                    maxMemory = memory;
                    this.HighestAppUsing = processItem.ProcessName;
                }
            }
            decimal MemoryUsed = maxMemory / 1024;
            if (MemoryUsed > 1024)
            {
                MemoryUsed /= 1024;
                memorytype = "MB";
                if (MemoryUsed > 1024)
                {
                    MemoryUsed /= 1024;
                    memorytype = "GB";
                }
            }
            if (MemoryUsed.ToString().Length > 4)
            {
                MemoryUsed = Convert.ToDecimal(MemoryUsed.ToString().Substring(0, 4));
            }
            this.RamUsageFromTheApp = MemoryUsed + memorytype;

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
