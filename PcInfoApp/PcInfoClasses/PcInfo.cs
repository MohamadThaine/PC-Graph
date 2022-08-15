using PcInfoApp.PcInfoClasses;
namespace PcInfoApp
{
    public class PcInfo
    {
        public GpuClass GpuInfo { get; set; } = new();
        public CpuClass CpuInfo { get; set; } = new();
        public RamClass RamInfo { get; set; } = new();
        public MBClass MBInfo { get; set; } = new();
        public StorageClass StorageInfo { get; set; } = new();
    }
}
