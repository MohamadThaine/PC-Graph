using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Management;
using System.Windows.Threading;

namespace PcInfoApp.PcInfoClasses
{
    public class CpuClass : INotifyPropertyChanged
    {
        public string CpuName { get; set; }
        public double CpuTemp { get; set; }
        public int MaxCpuTemp { get; set; }
        public string CpuCurrentClock { get; set; }
        public string CpuVolatge { get; set; }
        public double CpuLoad { get; set; }
        public int MaxCpuLoad { get; set; }
        public int CoresCount { get; set; }
        public CpuClass()
        {
            GetCpuInfo();
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
            ManagementObjectSearcher CpuInfo = new ManagementObjectSearcher("SELECT * FROM Win32_Processor ");
            foreach (ManagementObject Cpu in CpuInfo.Get())
            {
                CpuName = Cpu["Name"].ToString();
                CoresCount = Convert.ToInt32(Cpu["NumberOfCores"]);
            }
            GetCpuOtherInfo();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void CpuInfoTimer_Tick1(object? sender, EventArgs e)
        {
            GetCpuOtherInfo();
            OnPropertyChanged("CpuTemp");
            OnPropertyChanged("CpuCurrentClock");
            OnPropertyChanged("CpuVolatge");
            OnPropertyChanged("CpuLoad");
            OnPropertyChanged("MaxCpuLoad");
            OnPropertyChanged("MaxCpuTemp");
        }
        public void GetCpuOtherInfo()
        {
            bool VoltageFirstTime = true, ClockFirstTime = true;
            string CpuTempS, CpuLoadS;
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
                        this.CpuCurrentClock = sensor.Value.ToString();
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
            if (this.CpuCurrentClock.Length > 5)
                this.CpuCurrentClock = CpuCurrentClock.Substring(0, 5) + "MHZ";
            else
                this.CpuCurrentClock = "MHZ";
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
