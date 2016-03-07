using FishingNetDesigner.data;
using FishingNetDesigner.userControls;
using FishingNetDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FishingNetDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModels.Model viewModel;
        #region usercontrols
        CuttingDeleteInsidePolygon delPolygonUserControl = null;
        DefineFishingNet defineFishingNetUserControl = null;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModels.Model();
            delPolygonUserControl = new CuttingDeleteInsidePolygon(viewModel);
            defineFishingNetUserControl = new DefineFishingNet(viewModel);
            defineFishingNetUserControl.onNavigation += defineFishingNetUserControl_onNavigation;
            defineFishingNetUserControl.onNotify += defineFishingNetUserControl_onNotify;
            this.Loaded += MainWindow_Loaded;
            Plot1.Model = viewModel.PlotModel;

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
            Navigate(dstStage);
        }
        #endregion

        
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

        
        private void Navigate(Stage mainStage)
        {
            userControlHost.Children.Clear();
            var curUserControl = GetCurrentControl(mainStage);
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
            viewModel.CurMainStage = mainStage;
        }

        //Button GetStageButton(SubStage subStage)
        //{
        //    switch (subStage)
        //    {
        //        case SubStage.Half:
        //            return btnDeleteOneSide;
        //        case SubStage.Polygon:
        //            return btnCutByPolygon;
        //        default:
        //            return null;

        //    }
        //}

        Button GetStageButton(Stage mainStage)
        {
            Dictionary<Stage, Button> dict = new Dictionary<Stage, Button>();
            dict.Add(Stage.Cutting, btnCutLine);
            dict.Add(Stage.Define, btnDefineFishingNet);
            return dict[mainStage];
        }
        UserControl GetCurrentControl(Stage mainStage)
        {
            Dictionary<Stage, UserControl> dict = new Dictionary<Stage, UserControl>();
            dict.Add(Stage.Cutting, delPolygonUserControl);
            dict.Add(Stage.Define, defineFishingNetUserControl);
            return dict[mainStage];
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

        #region commands
        private void CommandHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();
        }

        private void CommandHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
        
    }



    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate() { };
        public static void ForceRefresh(this UIElement uiElement)
        {

            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);

        }

    }
  
}
