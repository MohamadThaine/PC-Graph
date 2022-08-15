using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcInfoApp.PcInfoClasses
{
    public class StorageClass
    {
        public List<String> StorageNames { get; set; } = new List<String>();
        public List<int> StorageSpace { get; set; } = new List<int>();
        public List<int> StorageFreeSpace { get; set; } = new List<int>();
        public StorageClass()
        {
            GetStorageInfo();
        }
        private void GetStorageInfo()
        {
            int TotalSize = 0, TotalFreeSize = 0;
            Computer pc = new Computer()
            {
                IsStorageEnabled = true
            };
            pc.Open();
            foreach (var Storage in pc.Hardware)
            {
               TotalSize = 0;
               TotalFreeSize = 0;
               if(Storage.Identifier.ToString().Contains("nvme"))
                {
                    NVMeGeneric NVME = (NVMeGeneric)Storage;
                    StorageNames.Add(NVME.Name);
                    foreach(var Info in NVME.DriveInfos)
                    {
                        TotalSize += Convert.ToInt32(Info.TotalSize / 1024 / 1024 / 1024);
                        TotalFreeSize += Convert.ToInt32(Info.TotalFreeSpace / 1024 / 1024 / 1024);
                    }
                    StorageSpace.Add(TotalSize);
                    StorageFreeSpace.Add(TotalFreeSize);
                }  
                else
                {
                    GenericHardDisk StorageDisk = (GenericHardDisk)Storage;
                    StorageNames.Add(StorageDisk.Name);
                    foreach (var Info in StorageDisk.DriveInfos)
                    {
                        TotalSize += Convert.ToInt32(Info.TotalSize / 1024 / 1024 / 1024);
                        TotalFreeSize += Convert.ToInt32(Info.TotalFreeSpace / 1024 / 1024 / 1024);
                    }
                    StorageSpace.Add(TotalSize);
                    StorageFreeSpace.Add(TotalFreeSize);
                } 
            }
        }
    }
}
