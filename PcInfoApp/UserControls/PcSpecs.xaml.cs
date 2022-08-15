using PcInfoApp.PcInfoClasses;
using Syncfusion.UI.Xaml.ProgressBar;
using System.Windows.Controls;
using System.Windows.Media;
namespace PcInfoApp.UserControls
{
    /// <summary>
    /// Interaction logic for PcSpecs.xaml
    /// </summary>
    public partial class PcSpecs : UserControl
    {
        private RamClass Ram;
        private StorageClass Storage;
        public PcSpecs()
        {
            Ram = new();
            Storage = new();
            InitializeComponent();
            PrepareRamBar();
        }
        private void PrepareRamBar()
        {
            double thirtyPrecentRam = Ram.RamSize * 0.3, SeventyPrecentRam = Ram.RamSize * 0.7;
            RangeColorCollection BarRangeColors = new();
            BarRangeColors.Add(new RangeColor() { Color = Colors.LightGreen, Start = 0, End = thirtyPrecentRam });
            BarRangeColors.Add(new RangeColor() { Color = Colors.Coral, Start = thirtyPrecentRam, End = SeventyPrecentRam });
            BarRangeColors.Add(new RangeColor() { Color = Colors.Crimson, Start = SeventyPrecentRam, End = Ram.RamSize });
            RamLoadBar.RangeColors = BarRangeColors;
        }

        private void StorageNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TotalSpaceTextBlock.Text = "Total Space is: " + Storage.StorageSpace[StorageNameComboBox.SelectedIndex].ToString() + "GB";
            TotalFreeSpaceTextBlock.Text = "Total Free Space is: " + Storage.StorageFreeSpace[StorageNameComboBox.SelectedIndex].ToString() + "GB";
        }
    }
}
