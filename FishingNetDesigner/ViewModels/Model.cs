using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace FishingNetDesigner.ViewModels
{
    public class Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
         private PlotModel plotModel; 
         public PlotModel PlotModel 
         { 
             get { return plotModel; } 
             set { plotModel = value; OnPropertyChanged("PlotModel"); } 
         }

         public Model() 
         {
            plotModel = SetUpModel();
         }

         private PlotModel SetUpModel()
         {
             PlotModel plotModel1 = new PlotModel();
             plotModel1.Title = "Fishing Net";
             var linearAxis1 = new OxyPlot.Axes.LinearAxis();
             linearAxis1.MajorGridlineStyle = LineStyle.Solid;
             linearAxis1.MaximumPadding = 0;
             linearAxis1.MinimumPadding = 0;
             linearAxis1.MinorGridlineStyle = LineStyle.Dot;
             linearAxis1.Position = AxisPosition.Bottom;
             plotModel1.Axes.Add(linearAxis1);
             var linearAxis2 = new OxyPlot.Axes.LinearAxis();
             linearAxis2.MajorGridlineStyle = LineStyle.Solid;
             linearAxis2.MaximumPadding = 0;
             linearAxis2.MinimumPadding = 0;
             linearAxis2.MinorGridlineStyle = LineStyle.Dot;
             plotModel1.Axes.Add(linearAxis2);
             return plotModel1;
        }
         OxyPlot.Series.LineSeries CreateDefaultLineSeries(double thickness)
        {
            var lineSeries1 = new OxyPlot.Series.LineSeries();
            lineSeries1.Color = OxyColors.SkyBlue;
            lineSeries1.LineStyle = LineStyle.Solid;
            lineSeries1.MarkerFill = OxyColors.SkyBlue;
            lineSeries1.MarkerSize = 5;
            lineSeries1.MarkerStroke = OxyColors.White;
            lineSeries1.MarkerStrokeThickness = 1.5;
            lineSeries1.MarkerType = MarkerType.Circle;
            lineSeries1.StrokeThickness = thickness;
            lineSeries1.Title = "Net Lines";
            
            return lineSeries1;
        }
        
        public void AddFishingNet(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            var lines = NetCreator.Create(xNum, yNum, xLen, yLen, thickness);
            OxyPlot.Series.LineSeries lineSeries = CreateDefaultLineSeries(lines.First().thickness);
            foreach(var line in lines)
            {
                lineSeries.Points.Add(new DataPoint(line.ptStart.X, line.ptStart.Y));
                lineSeries.Points.Add(new DataPoint(line.ptEnd.X,line.ptEnd.Y));
                lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
            }
            plotModel.Series.Add(lineSeries);
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
