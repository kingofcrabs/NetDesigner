﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FishingNetDesigner
{
    /// <summary>
    /// Interaction logic for HelpForm.xaml
    /// </summary>
    public partial class HelpForm : Window
    {
        public HelpForm()
        {
            InitializeComponent();
            lblDescription.Content = "版本号：" + strings.version;
        }
    }
}
