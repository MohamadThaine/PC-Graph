using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using System;
using System.Collections.Generic;

namespace PCGraph.PcInfoClasses
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
        public static double GetStorageTemp(int index)
        {
            double Temp = 0;
            Computer pc = new Computer()
            {
                IsStorageEnabled = true
            };
            pc.Open();
            pc.Hardware[index].Update();
            foreach (var sensor in pc.Hardware[index].Sensors)
            {
                if (sensor.Name == "Temperature")
                {
                    Temp = Convert.ToDouble(sensor.Value);
                    break;
                }
            }
            return Temp;
        }
    }
}
