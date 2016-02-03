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

namespace FishingNetDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModels.Model viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModels.Model();
            viewModel.onCutting += viewModel_onCutting;
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        void viewModel_onCutting(CuttingOperation op)
        {
            txtOperations.Text += op.ToFriendlyString();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.AddFishingNet(40, 10, 5, 5, 3);
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            onCut();
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

            if (repeatTimes < 0)
                throw new Exception("重复次数必须大于0！");
            viewModel.ExecuteCutCommand(txtOperations.Text, repeatTimes);
        }
    }
}
