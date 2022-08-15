using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcInfoApp.PcInfoClasses
{
    public class MBClass
    {
        public string MBName { get; set; }
        public string MBBiosVersion { get; set; }
        public string MBBiosDate { get; set; }
        public string[] trying { get; set; }
        public MBClass()
        {
            trying = new string[4] {"hi" , "bye" , "hello" , "how are you"} ;
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
                this.MBBiosVersion = motherboard.SMBios.Bios.Version;
                MBBiosDate = motherboard.SMBios.Bios.Date.Value.ToString("dd/MM/yyyy");
            }
        }
    }
}
