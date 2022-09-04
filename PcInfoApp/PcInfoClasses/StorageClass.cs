using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using System;
using System.Collections.Generic;

namespace PcInfoApp.PcInfoClasses
{
    public class StorageClass
    {
        public List<String> StorageNames { get; set; } = new List<String>();
        public static List<int> StorageSpace { get; set; } = new List<int>();
        public static List<int> StorageFreeSpace { get; set; } = new List<int>();
        public StorageClass()
        {
            GetStorageInfo();
        }
        private void GetStorageInfo()
        {
            Computer pc = new Computer()
            {
                IsStorageEnabled = true
            };
            pc.Open();
            foreach (AbstractStorage Storage in pc.Hardware)
            {
                int TotalSize = 0;
                int TotalFreeSize = 0;
                StorageNames.Add(Storage.Name);
                foreach (var Info in Storage.DriveInfos)
                {
                    TotalSize += Convert.ToInt32(Info.TotalSize / 1024 / 1024 / 1024);
                    TotalFreeSize += Convert.ToInt32(Info.TotalFreeSpace / 1024 / 1024 / 1024);
                }
                StorageSpace.Add(TotalSize);
                StorageFreeSpace.Add(TotalFreeSize);
            }
        }
        public static double GetStorageTemp(string name)
        {
            double Temp = 0;
            Computer pc = new Computer()
            {
                IsStorageEnabled = true
            };
            pc.Open();
            foreach (var Storage in pc.Hardware)
            {
                if (Storage.Name != name)
                    continue;
                Storage.Update();
                foreach (var sensor in Storage.Sensors)
                {
                    if (sensor.Name == "Temperature")
                    {
                        Temp = Convert.ToDouble(sensor.Value);
                        break;
                    }
                }
                break;
            }
            return Temp;
        }
    }
}
