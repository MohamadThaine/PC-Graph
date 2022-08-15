using LibreHardwareMonitor.Hardware;
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace PcInfoApp.PcInfoClasses
{
    public class RamClass : INotifyPropertyChanged
    {
        public double RamSize { get; set; }
        public double RamLoad { get; set; }
        public double RamSpeed { get; set; }
        public int MaxRamLoad { get; set; }
        public int RamStickCounter { get; set; } = 0;
        public RamClass()
        {
            GetRamInfo();
            DispatcherTimer RamInfoTimers = new DispatcherTimer();
            RamInfoTimers.Interval = TimeSpan.FromSeconds(1);
            RamInfoTimers.Tick += RamInfoTimer_Tick1;
            RamInfoTimers.Start();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RamInfoTimer_Tick1(object? sender, EventArgs e)
        {
            GetLoadInfo();
            OnPropertyChanged("RamLoad");
            OnPropertyChanged("MaxRamLoad");
        }
        private void GetRamInfo()
        {
            Computer pc = new Computer
            {
                IsMotherboardEnabled = true
            };
            pc.Open();
            pc.GetReport();
            foreach (LibreHardwareMonitor.Hardware.Motherboard.Motherboard motherboard in pc.Hardware)
            {
                foreach (MemoryDevice ram in motherboard.SMBios.MemoryDevices)
                {
                    if (ram.ManufacturerName != "")
                    {
                        this.RamSpeed = ram.Speed;
                        this.RamSize += ram.Size / 1024;
                        RamStickCounter++;
                    }
                }
            }
            GetLoadInfo();
        }
        public void GetLoadInfo()
        {
            this.RamLoad = (GC.GetGCMemoryInfo().MemoryLoadBytes) / 1024 / 1024 / 1024;
            if(Convert.ToInt32((this.RamLoad / this.RamSize) * 100) > this.MaxRamLoad)
                this.MaxRamLoad = Convert.ToInt32((this.RamLoad / this.RamSize) * 100);

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
