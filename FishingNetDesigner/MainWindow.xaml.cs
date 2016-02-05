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
            DataContext = viewModel;
            this.Loaded += MainWindow_Loaded;
        }

      

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //viewModel.AddFishingNet(40, 10, 5, 5, 3);
        }
      

        #region navigation
        private void btnCutLine_Click(object sender, RoutedEventArgs e)
        {
            onChangeUserControl(Stage.Cutting);

        }
        private void btnDefineFishingNet_Click(object sender, RoutedEventArgs e)
        {
            onChangeUserControl(Stage.Define);
        }

        private void onChangeUserControl(Stage curStage)
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
                    throw new Exception("找不当当前步骤对应的控件！");

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
                    throw new Exception("找不当当前步骤对应的控件！");

            }
        }

      
        #endregion
    }

  
}
