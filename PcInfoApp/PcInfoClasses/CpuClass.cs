using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace PCGraph.PcInfoClasses
{
    public class CpuClass : INotifyPropertyChanged
    {
        public string CpuName { get; set; }
        public double CpuTemp { get; set; }
        public int MaxCpuTemp { get; set; }
        public int CpuCurrentClock { get; set; }
        public int CpuMaxClock { get; set; }
        public string CpuVolatge { get; set; }
        public double CpuLoad { get; set; }
        public int MaxCpuLoad { get; set; }
        public int CoresCount { get; set; }
        private BackgroundWorker GetChangingInfo;
        public CpuClass()
        {
            GetCpuInfo();
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.WorkerSupportsCancellation = true;
            GetChangingInfo.DoWork += GetChangingInfo_DoWork;
            GetChangingInfo.RunWorkerAsync();
            DispatcherTimer CpuInfoTimer = new DispatcherTimer();
            CpuInfoTimer.Interval = TimeSpan.FromSeconds(1);
            CpuInfoTimer.Tick += CpuInfoTimer_Tick1;
            CpuInfoTimer.Start();
        }
        public void GetCpuInfo()
        {
            Computer pc = new Computer
            {
                IsCpuEnabled = true,
            };
            pc.Open();
            CpuName = pc.Hardware[0].Name;
            CoresCount = pc.SMBios.Processors[0].CoreCount;
            CpuMaxClock = pc.SMBios.Processors[0].MaxSpeed;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void CpuInfoTimer_Tick1(object? sender, EventArgs e)
        {
            GetChangingInfo = new BackgroundWorker();
            GetChangingInfo.WorkerSupportsCancellation = true;
            GetChangingInfo.DoWork += GetChangingInfo_DoWork;
            GetChangingInfo.RunWorkerAsync();
            OnPropertyChanged("CpuTemp");
            OnPropertyChanged("CpuCurrentClock");
            OnPropertyChanged("CpuVolatge");
            OnPropertyChanged("CpuLoad");
            OnPropertyChanged("MaxCpuLoad");
            OnPropertyChanged("MaxCpuTemp");
        }
        private void GetChangingInfo_DoWork(object? sender, DoWorkEventArgs e)
        {
            bool VoltageFirstTime = true, ClockFirstTime = true;
            string CpuTempS, CpuLoadS, CpuClockS;
            Computer pc = new Computer
            {
                IsCpuEnabled = true,
            };
            pc.Open();
            foreach (var HardWareItem in pc.Hardware)
            {
                foreach (var sensor in HardWareItem.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name == "Core Average")
                    {
                        this.CpuTemp = Convert.ToDouble(sensor.Value);
                    }
                    else if (sensor.SensorType == SensorType.Voltage && VoltageFirstTime == true)
                    {
                        VoltageFirstTime = false;
                        this.CpuVolatge = sensor.Value.ToString();
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
                    {
                        this.CpuLoad = Convert.ToDouble(sensor.Value);
                    }
                    else if (sensor.SensorType == SensorType.Clock && ClockFirstTime == true)
                    {
                        ClockFirstTime = false;
                        this.CpuCurrentClock = Convert.ToInt32(sensor.Value);
                    }
                }
            }
            CpuTempS = CpuTemp.ToString();
            if (CpuTempS.Length > 3)
                CpuTempS = CpuTempS.Substring(0, 4);
            CpuTemp = Convert.ToDouble(CpuTempS);
            if (Convert.ToInt32(this.CpuTemp) > this.MaxCpuTemp)
                this.MaxCpuTemp = Convert.ToInt32(this.CpuTemp);
            if (this.CpuVolatge.Length > 3)
                this.CpuVolatge = CpuVolatge.Substring(0, 4) + "V";
            else
                this.CpuVolatge += "V";
            CpuLoadS = CpuLoad.ToString();
            if (CpuLoadS.Length > 2)
                CpuLoadS = CpuLoadS.Substring(0, 2);
            CpuLoad = Convert.ToDouble(CpuLoadS);
            if (Convert.ToInt32(this.CpuLoad) > this.MaxCpuLoad)
                this.MaxCpuLoad = Convert.ToInt32(this.CpuLoad);
            GetChangingInfo.CancelAsync();
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
