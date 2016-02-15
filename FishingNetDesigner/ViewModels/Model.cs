using FishingNetDesigner.data;
using FishingNetDesigner.Data;
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
using System.Windows.Input;
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

         void timer_Elapsed(object sender, ElapsedEventArgs e)
         {
             var pts = CuttingLineSeries.Instance.Reachable.Points;
             if (pts.Count == 0)
                 return;
             
             var curSize = pts[0].Size;
             double boldThickness =  CuttingLineSeries.Instance.Thickness * 2;
             var newSize = curSize == boldThickness ? CuttingLineSeries.Instance.Thickness : boldThickness;
             pts.ForEach(x => x.Size = newSize);
             plotModel.InvalidatePlot(false);
         }
        #region render
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
        private void AdjustAxes(double xMax, double yMax)
        {
            var max = Math.Max(xMax, yMax);
            var xAxis = plotModel.Axes.First(x => x.Position == AxisPosition.Bottom);
            xAxis.Maximum = max * 1.1;
            xAxis.Minimum = -max * 0.1;
            var yAxis = plotModel.Axes.First(x => x.Position != AxisPosition.Bottom);
            yAxis.Maximum = max * 1.1;
            yAxis.Minimum = -max * 0.1;
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
        #endregion
        #region interface
        public Stage CurrentStage { get; set; }
        private void ExtentCuttingLine(OxyKey key)
        {
            if (CuttingLineSeries.Instance.Current.Points.Count == 0)
                return;

            double xOffSet = 0;
            double yOffset = 0;

            CuttingOperation op = CuttingOperation.Up;
            if (CuttingLineSeries.Instance.Current.Points[0].Y > fishingNet.HeightUnit * fishingNet.YNum)
                return;

            bool isValidKey = GetOffSetAndOperation(key, ref xOffSet, ref yOffset, ref op);
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
        public void AddCell(double widthUnit, double heightUnit, double thickness)
        {
            AddFishingNet(1, 1, widthUnit, heightUnit, thickness);
        }
        internal void Expand(FishingNet net)
        {
            AddFishingNet(net.XNum, net.YNum, net.WidthUnit, net.HeightUnit, net.Thickness);
        }

        private void CheckValidity(FishingNet net)
        {
            throw new NotImplementedException();
        }
        public void AddFishingNet(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            fishingNet = new FishingNet(xNum, yNum, xLen, yLen, thickness);
            var netLines = fishingNet.Create();
            UpdateLines(netLines);
        }
        private void UpdateLines(List<Line> netLines)
        {
            plotModel.Series.Clear();
            OxyPlot.Series.LineSeries lineSeries = CreateDefaultLineSeries(netLines.First().thickness);
            double maxX = 0, maxY =0;
            foreach (var line in netLines)
            {
                maxX = Math.Max(Math.Max(line.ptStart.X, line.ptEnd.X), maxX);
                maxY = Math.Max(Math.Max(line.ptStart.Y, line.ptEnd.Y), maxY);
                lineSeries.Points.Add(new DataPoint(line.ptStart.X, line.ptStart.Y));
                lineSeries.Points.Add(new DataPoint(line.ptEnd.X, line.ptEnd.Y));
                lineSeries.Points.Add(new DataPoint(double.NaN, double.NaN));
            }
            AdjustAxes(maxX, maxY);
            plotModel.Series.Add(lineSeries);
            CuttingLineSeries.Instance.Reset();
            CuttingLineSeries.Instance.AllScatterSeries.ForEach(x => plotModel.Series.Add(x));
            CuttingLineSeries.Instance.AllLineSeries.ForEach(x => plotModel.Series.Add(x));
            lineSeries.MouseDown += lineSeries_MouseDown;
            plotModel.InvalidatePlot(false);
        }
        #endregion
        #region keyboard & mouse
        //internal void InvokeKeyDown(System.Windows.Input.KeyEventArgs e)
        //{
        //    OxyKeyEventArgs oxyKeyEvent = new OxyKeyEventArgs();
            
        //    if(e.Key == Key.R || e.Key == Key.L && IsControlDown())
        //    {
        //        oxyKeyEvent.Key = e.Key == Key.R ? OxyKey.R : OxyKey.L;
        //        oxyKeyEvent.ModifierKeys = OxyModifierKeys.Control;
        //    }
        //    onKeyDown(oxyKeyEvent);
        //}

        bool IsControlDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        void plotModel_KeyDown(object sender, OxyKeyEventArgs e)
        {
            onKeyDown(e);
          
            //if(e.Key == OxyKey.S && e.IsControlDown)
            //{
            //    Dwg.Save("f:\\test.dwg", Memo.Instance.HistoryLines.Last().Value);
            //}
        }

        private void onKeyDown(OxyKeyEventArgs e)
        {
            if (CurrentStage == Stage.Cutting)
            {
                if (e.Key == OxyKey.L || e.Key == OxyKey.R && e.IsControlDown)
                {
                    Selection2DeleteBoundary(e.Key);
                    return;
                }
                if (e.Key == OxyKey.Delete)
                {
                    if (CuttingLineSeries.Instance.SelectionBoundary.Points.Count == 0)
                        throw new Exception("请先选择要删除的边！");
                    DeleteHalf();
                }
                ExtentCuttingLine(e.Key);
            }
        }

        private void DeleteHalf()
        {
            List<Point2D> cuttingLine = new List<Point2D>();
            CuttingLineSeries.Instance.Whole.Points.ForEach(pt => cuttingLine.Add(new Point2D(pt.X, pt.Y)));
            List<Line> remainLines = fishingNet.DeleteHalf(CuttingLineSeries.Instance.DeleteSide, cuttingLine);
            UpdateLines(remainLines);
        }

        private void Selection2DeleteBoundary(OxyKey oxyKey)
        {
            double totalWidth = fishingNet.XNum * fishingNet.WidthUnit;
            double totalHeight = fishingNet.YNum * fishingNet.HeightUnit;
            CuttingLineSeries.Instance.DeleteSide = oxyKey.ToString();
            CuttingLineSeries.Instance.SelectSide(oxyKey == OxyKey.L, totalWidth, totalHeight);
            plotModel.InvalidatePlot(false);
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
        void lineSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (CurrentStage == Stage.Cutting)
            {
                ProcessCutting((OxyPlot.Series.LineSeries)sender,e);
            }
        }

        private void ProcessCutting(OxyPlot.Series.LineSeries lineSeries, OxyMouseDownEventArgs e)
        {
            DataPoint clickPoint = lineSeries.InverseTransform(e.Position);
            if (clickPoint.Y > fishingNet.HeightUnit)
                return;

            if (fishingNet.GeneratedLines == null || fishingNet.GeneratedLines.Count == 0)
            {
                throw new Exception("未定义渔网结构！");
            }

            var anchorPt = GetAnchorPos(clickPoint);
            List<Point2D> reachablePts = fishingNet.GetReachablePts(anchorPt, new Point2D(-1, -1));
            CuttingLineSeries.Instance.UpdateCurrent(anchorPt, reachablePts, fishingNet.Thickness);
            plotModel.InvalidatePlot(false);
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
        internal void ExecuteCutCommand(string commands, int repeatTimes)
        {
            for (int i = 0; i < repeatTimes; i++)
            {
                foreach (char ch in commands)
                {
                    var key = ch.ToKey();
                    ExtentCuttingLine(key);
                }
            }
        }
        #endregion
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

      
    }

    public enum Stage
    {
        Define,
        Cutting
    }
}
