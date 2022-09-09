using PcInfoApp.PcInfoClasses;
namespace PcInfoApp
{
    public class PcInfo
    {
        static public GpuClass GpuInfo { get; set; } = new();
        static public CpuClass CpuInfo { get; set; } = new();
        static public RamClass RamInfo { get; set; } = new();
        static public MBClass MBInfo { get; set; } = new();
        static public StorageClass StorageInfo { get; set; } = new();
        static public NetworkClass NetworkInfo { get; set; } = new();
    }
}
