using LibreHardwareMonitor.Hardware;
using System;

namespace PCGraph.PcInfoClasses
{
    public class MBClass
    {
        public string MBName { get; set; }
        public string MBBiosVersion { get; set; }
        public string MBBiosDate { get; set; }
        public MBClass()
        {
            GetMotherBoardInfo();
        }
        private void GetMotherBoardInfo()
        {
            DateTime BiosDate = DateTime.Now;
            Computer pc = new Computer
            {
                IsMotherboardEnabled = true,
            };
            pc.Open();
            pc.GetReport();
            foreach (LibreHardwareMonitor.Hardware.Motherboard.Motherboard motherboard in pc.Hardware)
            {
                this.MBName = motherboard.Name;
                if (this.MBName.Length > 35)
                    this.MBName = this.MBName.Substring(0, 35) + "...";
                this.MBBiosVersion = motherboard.SMBios.Bios.Version;
                MBBiosDate = motherboard.SMBios.Bios.Date.Value.ToString("dd/MM/yyyy");
            }
        }
    }
}
