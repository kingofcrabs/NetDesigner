using FishingNetDesigner.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FishingNetDesigner.Data
{
    class CommandController : BindableBase
    {
        Visibility cuttingSubCommandsVisible = Visibility.Hidden;
        public Visibility CuttingSubCommandsVisible
        {
            get
            {
                return cuttingSubCommandsVisible;
            }
            set
            {
                SetProperty(ref cuttingSubCommandsVisible, value);
            }
        }
        
    }
    public enum Stage
    {
        Define,
        Cutting
    }
    public enum SubStage
    {
        Polygon,
        Half,
        Dummy,
    }
}
