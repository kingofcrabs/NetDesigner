using FishingNetDesigner.data;
using FishingNetDesigner.userControls;
using FishingNetDesigner.ViewModels;
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
        #region usercontrols
        EditCuttingLine editCuttingLineUserControl = null;
        DefineFishingNet defineFishingNetUserControl = null;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModels.Model();
            editCuttingLineUserControl = new EditCuttingLine(viewModel);
            defineFishingNetUserControl = new DefineFishingNet(viewModel);
            defineFishingNetUserControl.onNavigation += defineFishingNetUserControl_onNavigation;
            defineFishingNetUserControl.onNotify += defineFishingNetUserControl_onNotify;
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }



        void SetInfo(string s, bool isError = true)
        {
            txtInfo.Foreground = isError ? Brushes.Red : Brushes.Black;
            txtInfo.Text = s;
        }

        void defineFishingNetUserControl_onNotify(string info)
        {
            SetInfo(info);
        }

        void defineFishingNetUserControl_onNavigation(Stage dstStage)
        {
            Navigate(dstStage);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //viewModel.AddFishingNet(40, 10, 5, 5, 3);
        }
      

        #region navigation
        private void btnCutLine_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Cutting);

        }
        private void btnDefineFishingNet_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Define);
        }

        private void Navigate(Stage curStage)
        {
            userControlHost.Children.Clear();
            userControlHost.Children.Add(GetCurrentControl(curStage));
            List<Button> stageBtns = new List<Button>() { btnCutLine, btnDefineFishingNet };
            var dstButton = GetCurrentButton(curStage);
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            SolidColorBrush blue = new SolidColorBrush(Colors.LightBlue);
            viewModel.CurrentStage = curStage;
            stageBtns.ForEach(x => x.Background = white);
            dstButton.Background = blue;
        }

        Button GetCurrentButton(Stage currentStage)
        {
            switch (currentStage)
            {
                case Stage.Cutting:
                    return btnCutLine;
                case Stage.Define:
                    return btnDefineFishingNet;
                default:
                    throw new Exception("找不到当前步骤对应的控件！");

            }
        }
        UserControl GetCurrentControl(Stage currentStage)
        {
            switch(currentStage)
            {
                case Stage.Cutting:
                    return editCuttingLineUserControl;
                case Stage.Define:
                    return defineFishingNetUserControl;
                default:
                    throw new Exception("找不到当前步骤对应的控件！");

            }
        }

      
        #endregion

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            string filePath = OpenDialog();
            if (filePath != "")
                Dwg.Save(filePath, Memo.Instance.HistoryLines.Last().Value);
        }

        private string OpenDialog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dwg";
            dlg.FileName = "test";
            dlg.Filter = "AutoCAD Files (*.dwg)|*.dwg";
            // Display OpenFileDialog by calling ShowDialog method 
            bool bok = (bool)dlg.ShowDialog();
            return bok ? dlg.FileName : "";

        }
    }

  
}
