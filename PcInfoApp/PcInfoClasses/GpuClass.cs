﻿using Cudafy;
using Cudafy.Host;
using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Management;
using System.Windows.Threading;

namespace PcInfoApp.PcInfoClasses
{
    public class GpuClass : INotifyPropertyChanged
    {
        public string GpuName { get; set; }
        public double GpuTemp { get; set; }
        public int MaxGpuTemp { get; set; }
        public double GpuLoad { get; set; }
        public int MaxGpuLoad { get; set; }
        public double GpuMaxClock { get; set; }
        public double GpuMemoryMaxSize { get; set; }
        public string VersionDate { get; set; }
        public double GpuFansControl { get; set; }
        public double CurrentClockRate { get; set; }
        public double CurrentVramUsage { get; set; }
        public GpuClass()
        {
            GetGpuInfo();
            DispatcherTimer GpuTempTimer = new DispatcherTimer();
            GpuTempTimer.Interval = TimeSpan.FromSeconds(1);
            GpuTempTimer.Tick += GpuTempTimer_Tick1;
            GpuTempTimer.Start();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void GpuTempTimer_Tick1(object? sender, EventArgs e)
        {
            GetGpuChangedData();
            OnPropertyChanged("GpuTemp");
            OnPropertyChanged("GpuLoad");
            OnPropertyChanged("GpuFansControl");
            OnPropertyChanged("CurrentClockRate");
            OnPropertyChanged("CurrentVramUsage");
            OnPropertyChanged("MaxGpuTemp");
            OnPropertyChanged("MaxGpuLoad");
        }
        private void GetGpuInfo()
        {
            GPGPU Gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
            var GpuInfo = Gpu.GetDeviceProperties(true);
            var GpuVRamSize = (GpuInfo.TotalMemory / 1024 / 1024 + 1) / 1024;
            GpuName = GpuInfo.Name;
            GetGpuChangedData();
            GpuMaxClock = (GpuInfo.ClockRate / 1000);
            GpuMemoryMaxSize = GpuVRamSize;
            VersionDate = GetDriverVersionDate().ToString("dd/MM/yyyy");
        }
        private DateTime GetDriverVersionDate()
        {
            string DriverVersion = null;
            ManagementObjectSearcher GpuVersion = new ManagementObjectSearcher("SELECT DriverDate FROM Win32_VideoController ");
            foreach (ManagementObject Version in GpuVersion.Get())
            {
                DriverVersion = Version["DriverDate"].ToString();
            }
            string DriverVersionYear = DriverVersion.Substring(0, 4);
            string DriverVersionMonth = DriverVersion.Substring(4, 2);
            string DriverVersionDay = DriverVersion.Substring(6, 2);
            DriverVersion = DriverVersionYear + "/" + DriverVersionMonth + "/" + DriverVersionDay;
            return Convert.ToDateTime(DriverVersion);

        }
        public void GetGpuChangedData()
        {
            this.GpuFansControl = 0;
            int FansCounter = 0;
            string GpuTempS = "", GpuLoadS = "", GpuMemoryS = "";
            Computer pc = new Computer
            {
                IsGpuEnabled = true,
            };
            pc.Open();
            foreach (LibreHardwareMonitor.Hardware.Gpu.GenericGpu gpu in pc.Hardware)
            {
                foreach (var sensor in gpu.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature && sensor.Name == "GPU Core")
                    {
                        GpuTempS = sensor.Value.ToString();
                    }
                    else if (sensor.SensorType == SensorType.Load && sensor.Name == "GPU Core")
                    {
                        GpuLoadS = sensor.Value.ToString();
                    }
                    else if (sensor.SensorType == SensorType.Control)
                    {
                        FansCounter++;
                        this.GpuFansControl += Convert.ToDouble(sensor.Value);
                    }
                    else if (sensor.SensorType == SensorType.Clock && sensor.Name == "GPU Core")
                    {
                        this.CurrentClockRate = Convert.ToDouble(sensor.Value);
                    }
                    else if (sensor.SensorType == SensorType.SmallData && sensor.Name == "GPU Memory Used")
                    {
                        GpuMemoryS = sensor.Value.ToString();
                    }
                }
            }

            if (GpuTempS.Length > 4)
                GpuTempS = GpuTempS.Substring(0, 4);
            this.GpuTemp = Convert.ToDouble(GpuTempS);
            if (Convert.ToInt32(this.GpuTemp) > this.MaxGpuTemp)
                this.MaxGpuTemp = Convert.ToInt32(this.GpuTemp);
            if (GpuLoadS.Length > 2)
                GpuLoadS = GpuLoadS.Substring(0, 2);
            if (Convert.ToInt32(this.GpuLoad) > this.MaxGpuLoad)
                this.MaxGpuLoad = Convert.ToInt32(this.GpuLoad);
            this.GpuLoad = Convert.ToDouble(GpuLoadS);
            this.GpuFansControl /= FansCounter;
            this.CurrentVramUsage = Convert.ToDouble(GpuMemoryS) / 1024;
            this.CurrentVramUsage = Math.Round(this.CurrentVramUsage, 2);
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}