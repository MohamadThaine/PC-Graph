using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;

namespace PCGraph.PcInfoClasses
{
    public class RamClass : INotifyPropertyChanged
    {
        public double RamSize { get; set; }
        public long RamLoad { get; set; }
        public int MaxRamLoad { get; set; }
        public int RamUsage { get; set; }
        public string RamSpeed { get; set; }
        public ObservableCollection<string> HighestAppUsing { get; set; }
        public ObservableCollection<string> RamUsageFromTheApp { get; set; }
        private BackgroundWorker GetChangingInfo;
        public event PropertyChangedEventHandler PropertyChanged;
        public RamClass()
        {
            GetRamInfo();
            RamUsageFromTheApp = new ObservableCollection<string>();
            HighestAppUsing = new ObservableCollection<string>();
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.WorkerSupportsCancellation = true;
            GetChangingInfo.DoWork += GetChangingInfo_DoWork;
            GetChangingInfo.RunWorkerAsync();
            DispatcherTimer RamInfoTimers = new DispatcherTimer();
            RamInfoTimers.Interval = TimeSpan.FromSeconds(1);
            RamInfoTimers.Tick += RamInfoTimer_Tick1;
            RamInfoTimers.Start();
        }

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
            List<string> HighingAppUsingNames = new List<string>(2);
            List<string> RamUsageFromApps = new List<string>(2);
            this.RamLoad = (GC.GetGCMemoryInfo().MemoryLoadBytes) / 1024 / 1024 / 1024;
            if (Convert.ToInt32((this.RamLoad / RamSize) * 100) > this.MaxRamLoad)
                this.MaxRamLoad = Convert.ToInt32((this.RamLoad / RamSize) * 100);
            this.RamUsage = Convert.ToInt32((this.RamLoad / RamSize) * 100);
            Process[] procesess = Process.GetProcesses();
            Dictionary<string, long> ProcMap = new Dictionary<string, long>();
            foreach (Process process in procesess)
            {
                if (ProcMap.ContainsKey(process.ProcessName))
                {
                    ProcMap[process.ProcessName] += process.WorkingSet64;
                }
                else
                    ProcMap.Add(process.ProcessName, process.WorkingSet64);
            }
            ProcMap = ProcMap.OrderByDescending(proc => proc.Value).ToDictionary(x => x.Key, x => x.Value);
            string memorytype = "KB";
            decimal MemoryUsed = 0;
            int index = 0;
            foreach (KeyValuePair<string, long> kvp in ProcMap)
            {
                if (index >= 2)
                    break;
                HighingAppUsingNames.Add(kvp.Key);
                MemoryUsed = kvp.Value / 1024;
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
                RamUsageFromApps.Add(MemoryUsed + memorytype);
                index++;
            }
            this.HighestAppUsing = new ObservableCollection<string>(HighingAppUsingNames);
            this.RamUsageFromTheApp = new ObservableCollection<string>(RamUsageFromApps);
            GetChangingInfo.CancelAsync();

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
