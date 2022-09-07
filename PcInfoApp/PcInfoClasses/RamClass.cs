using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace PcInfoApp.PcInfoClasses
{
    public class RamClass : INotifyPropertyChanged
    {
        public double RamSize { get; set; }
        public long RamLoad { get; set; }
        public int MaxRamLoad { get; set; }
        public int RamUsage { get; set; }
        public string RamSpeed { get; set; }
        public List<string> HighestAppUsing { get; set; }
        public List<string> RamUsageFromTheApp { get; set; }
        private BackgroundWorker GetChangingInfo;
        public RamClass()
        {
            GetRamInfo();
            RamUsageFromTheApp = new List<string>(2);
            HighestAppUsing = new List<string>(2);
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.WorkerSupportsCancellation = true;
            GetChangingInfo.DoWork += GetChangingInfo_DoWork;
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
            GetChangingInfo.WorkerSupportsCancellation = true;
            GetChangingInfo.DoWork += GetChangingInfo_DoWork;
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
                        RamSpeed = ram.Speed + "MHZ";
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
            Process[] process = Process.GetProcesses().OrderByDescending(proc => proc.PagedMemorySize64).Distinct().ToArray();
            long maxMemory = 0;
            string memorytype = "KB";
            decimal MemoryUsed = 0;
            for(int i = 0; i < 2; i++)
            {
                this.HighestAppUsing.Add(process[i].ProcessName);
                MemoryUsed = process[i].PagedMemorySize64 / 1024;
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
                this.RamUsageFromTheApp.Add(MemoryUsed + memorytype);
            }
            GetChangingInfo.CancelAsync();

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
