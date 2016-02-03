using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Media;
using WW.Math;

namespace FishingNetDesigner.ViewModels
{
    public class Model : INotifyPropertyChanged
    {
         public event PropertyChangedEventHandler PropertyChanged;
         private PlotModel plotModel;
         FishingNet fishingNet = new FishingNet();
         Timer timer = new Timer(1000);
         #region cutting event
         public delegate void onCuttingLine(CuttingOperation op);
         public event onCuttingLine onCutting;
         #endregion
         public PlotModel PlotModel 
         { 
             get { return plotModel; } 
             set { plotModel = value; OnPropertyChanged("PlotModel"); } 
         }

         public Model() 
         {
            plotModel = SetUpModel();
            plotModel.KeyDown += plotModel_KeyDown;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
         }

         void plotModel_KeyDown(object sender, OxyKeyEventArgs e)
         {
             ExpandCuttingLine(e.Key);
         }

         private void ExpandCuttingLine(OxyKey key)
         {
             if (CuttingLineSeries.Instance.Current.Points.Count == 0)
                 return;

             double xOffSet = 0;
             double yOffset = 0;
          
             CuttingOperation op = CuttingOperation.Up;
             if (CuttingLineSeries.Instance.Current.Points[0].Y > fishingNet.HeightUnit * fishingNet.YNum)
                 return;

             bool isValidKey = GetOffSetAndOperation(key,ref xOffSet, ref yOffset, ref op);
             if (!isValidKey)
                 return;
             
             if (CuttingLineSeries.Instance.CanGo(xOffSet, yOffset))
             {
                 var currentPt = CuttingLineSeries.Instance.Current.Points[0];
                 Point2D latestCurrent = new Point2D(currentPt.X + xOffSet, currentPt.Y + yOffset);
                 var reachablePts = fishingNet.GetReachablePts(latestCurrent, new Point2D(currentPt.X, currentPt.Y));
                 CuttingLineSeries.Instance.UpdateCurrent(latestCurrent, reachablePts, fishingNet.Thickness, true);
                 if (onCutting != null)
                     onCutting(op);
                 plotModel.InvalidatePlot(false);
             }
         }

         private bool GetOffSetAndOperation(OxyKey key, ref double xOffSet, ref double yOffset, ref CuttingOperation op)
         {
             double unitX = fishingNet.WidthUnit / 2;
             double unitY = fishingNet.HeightUnit / 2;
             bool validKey = true;
             switch (key)
             {
                 case OxyKey.NumPad8://up
                 case OxyKey.Up:
                     yOffset = unitY;
                     break;
                 case OxyKey.Right:
                 case OxyKey.NumPad6: //right
                     xOffSet = unitX;
                     op = CuttingOperation.Right;
                     break;
                 //case OxyKey.Left:
                 //case OxyKey.NumPad4: //left
                 //    xOffSet = -unitX;
                 //    op = CuttingOperation.Left;
                 //    break;
                 case OxyKey.NumPad9: //up right
                     xOffSet = unitX;
                     yOffset = unitY;
                     op = CuttingOperation.UpRight;
                     break;
                 //case OxyKey.NumPad7: //up left
                 //    xOffSet = -unitX;
                 //    yOffset = unitY;
                 //    op = CuttingOperation.UpLeft;
                 //    break;
                 default:
                     validKey = false;
                     break;
             }
             return validKey;
         }

         void timer_Elapsed(object sender, ElapsedEventArgs e)
         {
             var pts = CuttingLineSeries.Instance.Reachable.Points;
             if (pts.Count == 0)
                 return;
             
             var curSize = pts[0].Size;
             double boldThickness =  fishingNet.Thickness * 1.2;
             var newSize = curSize == fishingNet.Thickness ?  boldThickness : fishingNet.Thickness;
             pts.ForEach(x => x.Size = newSize);
             plotModel.InvalidatePlot(false);
         }

         private PlotModel SetUpModel()
         {
             PlotModel plotModel1 = new PlotModel();
             plotModel1.Title = "Fishing Net";
             var linearAxis1 = new OxyPlot.Axes.LinearAxis();
             linearAxis1.MajorGridlineStyle = LineStyle.Solid;
             linearAxis1.MinimumPadding = 0;
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
            lineSeries1.Color = OxyColors.Black;
            lineSeries1.LineStyle = LineStyle.Solid;
            lineSeries1.MarkerSize = thickness;
            lineSeries1.MarkerStroke = OxyColors.Black;
            lineSeries1.MarkerStrokeThickness = thickness;
            lineSeries1.MarkerType = MarkerType.Circle;
            lineSeries1.Title = "Net Lines";
            
            return lineSeries1;
        }
        
        public void AddFishingNet(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            plotModel.Series.Clear();
            var netLines = fishingNet.Create(xNum, yNum, xLen, yLen, thickness);
            OxyPlot.Series.LineSeries lineSeries = CreateDefaultLineSeries(netLines.First().thickness);
            foreach (var line in netLines)
            {
                lineSeries.Points.Add(new DataPoint(line.ptStart.X, line.ptStart.Y));
                lineSeries.Points.Add(new DataPoint(line.ptEnd.X,line.ptEnd.Y));
                lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
            }
            AdjustAxes(xNum * xLen, yNum * yLen);
            plotModel.Series.Add(lineSeries);
            CuttingLineSeries.Instance.AllScatterSeries.ForEach(x => plotModel.Series.Add(x));
            CuttingLineSeries.Instance.AllLineSeries.ForEach(x => plotModel.Series.Add(x));
            lineSeries.MouseDown += lineSeries_MouseDown;
           
        }

        private void AdjustAxes(double xMax, double yMax)
        {
            var xAxis = plotModel.Axes.First(x => x.Position == AxisPosition.Bottom);
            xAxis.Maximum = xMax * 1.1;
            xAxis.Minimum = -xMax * 0.1;
            var yAxis = plotModel.Axes.First(x => x.Position != AxisPosition.Bottom);
            yAxis.Maximum = yMax * 1.1;
            yAxis.Minimum = -yMax * 0.1;
        }

        void lineSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var lineSeries = (OxyPlot.Series.LineSeries)sender;
            DataPoint clickPoint = lineSeries.InverseTransform(e.Position);
            if (clickPoint.Y > fishingNet.HeightUnit)
                return;

            if(fishingNet.GeneratedLines == null || fishingNet.GeneratedLines.Count == 0)
            {
                throw new Exception("未定义渔网结构！");
            }
   
            var anchorPt = GetAnchorPos(clickPoint);
            fishingNet.CuttingLine.Clear();
            fishingNet.CuttingLine.Add(anchorPt);
            List<Point2D> reachablePts = fishingNet.GetReachablePts(anchorPt,new Point2D(-1,-1));
            CuttingLineSeries.Instance.UpdateCurrent(anchorPt, reachablePts,fishingNet.Thickness);
            plotModel.InvalidatePlot(true);
        }

        private Point2D GetAnchorPos(DataPoint clickPoint)
        {
            double halfWidth = fishingNet.WidthUnit / 2.0d;
            double quarterHeight = fishingNet.HeightUnit / 4.0d;
            double firstLineOffset = fishingNet.WidthUnit / 4.0d;
            double x = (int)(Math.Round((clickPoint.X - firstLineOffset) / halfWidth)) * halfWidth + firstLineOffset;
            double y = quarterHeight;
            return new Point2D(x, y);
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        internal void ExecuteCutCommand(string commands, int repeatTimes)
        {
            for (int i = 0; i < repeatTimes; i++ )
            {
                foreach (char ch in commands)
                {
                    var key = ch.ToKey();
                    ExpandCuttingLine(key);
                }
            }
        }
    }
}
