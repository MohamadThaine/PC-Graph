using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;

namespace PCGraph.PcInfoClasses
{
    public class SystemSpecs
    {
        public List<string> NetworkInterFaces { get; set; }
        public List<string> AudioDevice { get; set; }
        public int NetworkCount { get; set; }
        public int AudioCount { get; set; }
        public SystemSpecs()
        {
            NetworkInterFaces = GetNetworkInterfaces();
            AudioDevice = GetAudioDevices();
            NetworkCount = NetworkInterFaces.Count;
            AudioCount = AudioDevice.Count;
        }
        public List<string> GetNetworkInterfaces()
        {
            List<string> networkInterfaces = new List<string>();
            NetworkInterface[] networkInterfaceHolder = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface networkInterface in networkInterfaceHolder)
            {
                string Name = networkInterface.Name;
                if (Name.Length > 39)
                    Name = Name.Substring(0, 39) + "...";
                networkInterfaces.Add(Name);
            }
            return networkInterfaces;
        }
        public List<string> GetAudioDevices()
        {
            List<string> AudioDevices = new List<string>();
            ManagementObjectSearcher GetAuidoDevicesMo = new ManagementObjectSearcher("select * from Win32_SoundDevice");
            foreach (ManagementObject AudioDevice in GetAuidoDevicesMo.Get())
            {
                string Name = AudioDevice.GetPropertyValue("Name").ToString();
                if (Name.Length > 39)
                    Name = Name.Substring(0, 39) + "...";
                AudioDevices.Add(Name);
            }
            return AudioDevices;
        }
    }
}
