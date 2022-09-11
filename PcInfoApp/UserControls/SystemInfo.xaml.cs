using System.Windows;
using System.Windows.Controls;

namespace PCGraph.UserControls
{
    /// <summary>
    /// Interaction logic for SystemInfo.xaml
    /// </summary>
    public partial class SystemInfo : UserControl
    {

        public SystemInfo()
        {
            InitializeComponent();
        }

        private void ExpanderExpanded(object sender, RoutedEventArgs e)
        {
            double height = 210;
            Expander GetNameEx = (Expander)sender;
            string Name = GetNameEx.Name;
            if (Name == "CPUExpander")
                CpuBorderVisual.Height = height;
            else if (Name == "GPUExpander")
                GpuBorderVisual.Height = height;
            else if (Name == "MBExpander")
                MBBorderVisual.Height = height;
            else if (Name == "RAMExpander")
                RamBorderVisual.Height = height + 50;
            else if (Name == "STORAGEExpander")
                STORAGEBorderVusual.Height = height;
            else if (Name == "AUDIOExpander")
                AudioBorderVisual.Height = height;
            else if (Name == "NETWORKExpander")
                NETWORKBorderVisual.Height = height;
        }

        private void ExpanderCollepsed(object sender, RoutedEventArgs e)
        {
            double height = 110;
            Expander GetNameEx = (Expander)sender;
            string Name = GetNameEx.Name;
            if (Name == "CPUExpander")
                CpuBorderVisual.Height = height;
            else if (Name == "GPUExpander")
                GpuBorderVisual.Height = height;
            else if (Name == "MBExpander")
                MBBorderVisual.Height = height;
            else if (Name == "RAMExpander")
                RamBorderVisual.Height = height;
            else if (Name == "STORAGEExpander")
                STORAGEBorderVusual.Height = height;
            else if (Name == "AUDIOExpander")
                AudioBorderVisual.Height = height;
            else if (Name == "NETWORKExpander")
                NETWORKBorderVisual.Height = height;
        }
    }
}
