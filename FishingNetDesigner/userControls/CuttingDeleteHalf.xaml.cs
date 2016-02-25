using FishingNetDesigner.Data;
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

namespace FishingNetDesigner.userControls
{
    /// <summary>
    /// Interaction logic for EditCuttingLine.xaml
    /// </summary>
    public partial class CuttingDeleteHalf : StageControl
    {
        ViewModels.Model viewModel;
        public CuttingDeleteHalf()
        {
            InitializeComponent();
        }

        public CuttingDeleteHalf(ViewModels.Model viewModel):this()
        {
            this.viewModel = viewModel;
            CuttingBySide.Instance.onCutting += Instance_onCutting;
        }

        void Instance_onCutting(CuttingOperation op)
        {
            txtOperations.Text += op.ToFriendlyString();
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                onCut();
            }
            catch(Exception ex)
            {
                NotifyInformation("剪切时发生错误！" + ex.Message);
            }
        }

        private void onCut()
        {
            string txtOperation = txtOperations.Text;
            if (txtOperation == "")
                throw new Exception("无法执行空操作");
            string sRepeatTimes = txtRepeatTimes.Text;
            int repeatTimes = 0;
            bool bok = int.TryParse(sRepeatTimes, out repeatTimes);
            if (!bok)
                throw new Exception("重复次数必须为数字！");

            if (repeatTimes <= 0)
                throw new Exception("重复次数必须大于0！");
            viewModel.ExecuteCutCommand(txtOperations.Text, repeatTimes);
        }
    }
}
