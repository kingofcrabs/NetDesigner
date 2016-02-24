using FishingNetDesigner.Data;
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
        CuttingDeleteHalf editCuttingLineUserControl = null;
        DefineFishingNet defineFishingNetUserControl = null;
        CommandController commandController = new CommandController();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModels.Model();
            editCuttingLineUserControl = new CuttingDeleteHalf(viewModel);
            defineFishingNetUserControl = new DefineFishingNet(viewModel);
            defineFishingNetUserControl.onNavigation += defineFishingNetUserControl_onNavigation;
            defineFishingNetUserControl.onNotify += defineFishingNetUserControl_onNotify;
            cuttingApproach.DataContext = commandController;
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

        void SetInfo(string s, bool isError = true)
        {
            txtInfo.Foreground = isError ? Brushes.Red : Brushes.Black;
            txtInfo.Text = s;
        }
        #region userControl event handler
        void defineFishingNetUserControl_onNotify(string info)
        {
            SetInfo(info);
        }

        void defineFishingNetUserControl_onNavigation(Stage dstStage)
        {
            Navigate(dstStage,SubStage.Dummy);
        }
        #endregion

        
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //viewModel.AddFishingNet(40, 10, 5, 5, 3);
        }
      

        #region navigation
        private void btnCutLine_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Cutting,SubStage.Polygon);

        }
        private void btnDefineFishingNet_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Define,SubStage.Dummy);
        }

        private void btnCutByPolygon_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Cutting, SubStage.Polygon);
        }

        private void btnDeleteOneSide_Click(object sender, RoutedEventArgs e)
        {
            Navigate(Stage.Cutting, SubStage.Half);
        }
        private void Navigate(Stage mainStage,SubStage subStage)
        {
            userControlHost.Children.Clear();
            var curUserControl = GetCurrentControl(mainStage, subStage);
            if(curUserControl != null)
                userControlHost.Children.Add(curUserControl);
            List<Button> stageBtns = new List<Button>() { btnCutLine, btnDefineFishingNet };
            List<Button> subStageBtns = new List<Button>() { btnCutByPolygon,btnDeleteOneSide };
            
            SolidColorBrush white = new SolidColorBrush(Colors.White);
            SolidColorBrush blue = new SolidColorBrush(Colors.LightBlue);
            stageBtns.ForEach(x => x.Background = white);
            subStageBtns.ForEach(x => x.Background = white);
            var dstButton = GetStageButton(mainStage);
            dstButton.Background = blue;
            var subStageBtn = GetStageButton(subStage);
            if (subStageBtn != null)
                subStageBtn.Background = blue;
            viewModel.CurMainStage = mainStage;
            viewModel.CurSubStage = subStage;
            commandController.CuttingSubCommandsVisible = mainStage == Stage.Cutting ? Visibility.Visible : Visibility.Hidden;
                
        }
        Button GetStageButton(SubStage subStage)
        {
            switch (subStage)
            {
                case SubStage.Half:
                    return btnDeleteOneSide;
                case SubStage.Polygon:
                    return btnCutByPolygon;
                default:
                    return null;

            }
        }
        Button GetStageButton(Stage mainStage)
        {
            switch (mainStage)
            {
                case Stage.Cutting:
                    return btnCutLine;
                case Stage.Define:
                    return btnDefineFishingNet;
                default:
                    throw new Exception("找不到当前步骤对应的控件！");

            }
        }
        UserControl GetCurrentControl(Stage mainStage,SubStage subStage)
        {
            if (mainStage == Stage.Define)
                return defineFishingNetUserControl;
            if( mainStage == Stage.Cutting)
            {
                if (subStage == SubStage.Half)
                    return editCuttingLineUserControl;
            }
            return null;
        }

      
        #endregion

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if(Memo.Instance.HistoryLines.Count == 0)
            {
                SetInfo("无可以导出的数据！");
                return;
            }
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
