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
using FishingNetDesigner.Data;

namespace FishingNetDesigner.userControls
{
    /// <summary>
    /// Interaction logic for DefineFishingNet.xaml
    /// </summary>
    public partial class DefineFishingNet : StageControl
    {
        private Model viewModel;
       
        public DefineFishingNet()
        {
            InitializeComponent();
            FishingNet.Instance.Create(2, 3, 10, 10, 1);
            this.DataContext = FishingNet.Instance;
        }

        public DefineFishingNet(Model viewModel):this()
        {
            this.viewModel = viewModel;
        }

        FishingNet Net{get; set;}

        private void btnCell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckCellValidity();
            }
            catch (Exception ex)
            {
                NotifyInformation("单元参数非法: " + ex.Message);
                return;
            }
            viewModel.AddCell(Net.WidthUnit, Net.HeightUnit,Net.Thickness);
            btnExpand.IsEnabled = true;
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckExpandValidity();
            }
            catch(Exception ex)
            {
                NotifyInformation("扩展参数非法: "+ex.Message);
                return;
            }

            viewModel.Expand(Net);
            Navi2Stage(Stage.Cutting);
        }

        private void CheckCellValidity()
        {
            if (Net.WidthUnit <= 0)
                throw new Exception("单元宽度必须大于0！");
            if(Net.HeightUnit <= 0)
                throw new Exception("单元高度必须大于0！");
            if (Net.Thickness <= 0)
                throw new Exception("线宽必须大于0！");

        }
        private void CheckExpandValidity()
        {
            if (Net.XNum <= 0)
                throw new Exception("横目数量必须大于0！");
            if (Net.YNum <= 0)
                throw new Exception("纵目数量必须大于0！");
        }
    }
}
