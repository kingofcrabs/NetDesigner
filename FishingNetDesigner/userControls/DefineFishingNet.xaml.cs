using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FishingNetDesigner.ViewModels;
using FishingNetDesigner.Data;

namespace FishingNetDesigner.userControls
{
    /// <summary>
    /// Interaction logic for DefineFishingNet.xaml
    /// </summary>
    public partial class DefineFishingNet : UserControl
    {
        private Model viewModel;

        public DefineFishingNet()
        {
            InitializeComponent();
            Net = new FishingNet(1, 1, 10, 10, 1);
            this.DataContext = Net;
        }

        public DefineFishingNet(Model viewModel):this()
        {
            this.viewModel = viewModel;
        }

        FishingNet Net{get; set;}

        private void btnCell_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddCell(Net.WidthUnit, Net.HeightUnit,Net.Thickness);
            btnExpand.IsEnabled = true;
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Expand(Net);
        }
    }
}
