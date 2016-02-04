using FishingNetDesigner.userControls;
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
        

        private void btnCutLine_Click(object sender, RoutedEventArgs e)
        {
            userControlHost.Children.Clear();
            userControlHost.Children.Add(editCuttingLineUserControl);
        }

        private void btnDefineFishingNet_Click(object sender, RoutedEventArgs e)
        {
            userControlHost.Children.Clear();
            userControlHost.Children.Add(defineFishingNetUserControl);
        }
    }
}
