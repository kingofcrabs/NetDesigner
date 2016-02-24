using FishingNetDesigner.Data;
using FishingNetDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FishingNetDesigner.userControls
{
    public class StageControl : UserControl
    {
        public delegate void StageChange(Stage dstStage);
        public event StageChange onNavigation;
        public delegate void Notify(string info);
        public event Notify onNotify;
        public void NotifyInformation(string s)
        {
            if (onNotify != null)
                onNotify(s);
        }

        public void Navi2Stage(Stage stage)
        {
            if (onNavigation != null)
                onNavigation(stage);
        }
    }
}
