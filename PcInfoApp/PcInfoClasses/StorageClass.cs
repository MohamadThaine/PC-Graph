using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using System;
using System.Collections.Generic;

namespace PCGraph.PcInfoClasses
{
    public class StorageClass
    {
        public int StorageCounter { get; set; }
        public static List<Storage> Storages { get; set; } = new List<Storage>();
        public StorageClass()
        {
            GetStorageInfo();
            StorageCounter = Storages.Count;
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
                long TotalBtyes = 0;
                long FreeBytes = 0;
                foreach (var Info in Storage.DriveInfos)
                {
                    TotalSize += Convert.ToInt32(Info.TotalSize / 1024 / 1024 / 1024);
                    TotalBtyes += long.Parse(Info.TotalSize.ToString());
                    TotalFreeSize += Convert.ToInt32(Info.TotalFreeSpace / 1024 / 1024 / 1024);
                    FreeBytes += long.Parse(Info.TotalFreeSpace.ToString());
                }
                Storages.Add(new Storage(Storage.Name, TotalSize, TotalFreeSize, TotalBtyes, FreeBytes));
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
    public class Storage
    {
        public string Name { get; set; }
        public string TotalSpace { get; set; }
        public string FreeSpace { get; set; }
        public long SpaceInBytes { get; set; }
        public long FreeSpaceInBytes { get; set; }
        public Storage(string name, int totalSpace, int freeSpace, long spaceInBytes, long freeSpaceInBytes)
        {
            Name = name;
            TotalSpace = totalSpace + "GB";
            FreeSpace = freeSpace + "GB";
            SpaceInBytes = spaceInBytes;
            FreeSpaceInBytes = freeSpaceInBytes;
        }
    }
}
