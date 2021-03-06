﻿using FishingNetDesigner.data;
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
         Timer timer = new Timer(1000);
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
             var pts = CuttingInsidePolygon.Instance.Reachable.Points;
             if (pts.Count == 0)
                 return;
             
             var curSize = pts[0].Size;
             double boldThickness =  CuttingInsidePolygon.Instance.Thickness * 2;
             var newSize = curSize == boldThickness ? CuttingInsidePolygon.Instance.Thickness : boldThickness;
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
             //linearAxis1.MinorGridlineStyle = LineStyle.Dot;
             linearAxis1.Position = AxisPosition.Bottom;
             plotModel1.Axes.Add(linearAxis1);
             var linearAxis2 = new OxyPlot.Axes.LinearAxis();
             linearAxis2.MajorGridlineStyle = LineStyle.Solid;
             linearAxis2.MaximumPadding = 0;
             linearAxis2.MinimumPadding = 0;
             //linearAxis2.MinorGridlineStyle = LineStyle.Dot;
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
            lineSeries1.StrokeThickness = thickness;
            lineSeries1.MarkerSize = thickness/3;
            lineSeries1.MarkerStroke = OxyColors.Black;
            lineSeries1.MarkerStrokeThickness = thickness;
            lineSeries1.MarkerType = MarkerType.Square;
            lineSeries1.Title = "Net Lines";
            return lineSeries1;
        }
        #endregion
        #region interface
        public Stage CurMainStage { get; set; }
        public void ClearCuttingLines()
        {
            CuttingInsidePolygon.Instance.Clear();
            plotModel.InvalidatePlot(true);
        }
       
        private void ExtendCuttingLine(OxyKey key)
        {
            bool bok = CuttingInsidePolygon.Instance.Extend(key);
            if (bok)
               plotModel.InvalidatePlot(false);
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
            FishingNet.Instance.Create(xNum, yNum, xLen, yLen, thickness);
            var netLines = FishingNet.Instance.Generate();
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
            CuttingInsidePolygon.Instance.Clear();
            CuttingInsidePolygon.Instance.AllScatterSeries.ForEach(x => plotModel.Series.Add(x));
            CuttingInsidePolygon.Instance.AllLineSeries.ForEach(x => plotModel.Series.Add(x));
            //CuttingByPolygon.Instance.Clear();
            //CuttingByPolygon.Instance.AllLineSeries.ForEach(x => plotModel.Series.Add(x));
            lineSeries.MouseDown += lineSeries_MouseDown;
            plotModel.InvalidatePlot(false);
        }
        #endregion
        #region keyboard & mouse
        bool IsControlDown()
        {
            return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        }

        void plotModel_KeyDown(object sender, OxyKeyEventArgs e)
        {
            onKeyDown(e);
        }

        private void onKeyDown(OxyKeyEventArgs e)
        {
            //onKeyDownCuttingByPolygon(e);
            onKeyDownCuttingInsidePolygon(e);
        }
        private void CompletePolygon()
        {
            CuttingInsidePolygon.Instance.Complete();
            plotModel.InvalidatePlot(false);
        }

     

        private void onKeyDownCuttingInsidePolygon(OxyKeyEventArgs e)
        {
            bool cuttingInsidePolygon = CurMainStage == Stage.Cutting; //&& CurSubStage == SubStage.Half;
            if (!cuttingInsidePolygon)
                return;
            //if (e.Key == OxyKey.L || e.Key == OxyKey.R && e.IsControlDown)
            //{
            //    Selection2DeleteBoundary(e.Key);
            //    return;
            //}
            if (e.Key == OxyKey.P && e.IsControlDown)
            {
                CompletePolygon();//
                return;
            }
            if (e.Key == OxyKey.Delete)
            {
                DeleteInsidePolygon();
                return;
            }
            ExtendCuttingLine(e.Key);
        }

        private void DeleteInsidePolygon()
        {
            if (!CuttingInsidePolygon.Instance.Completed)
                throw new Exception("多边形未补全，请在保证有3个点的条件下，按Ctrl+C补全！");
            List<Line> remainLines = CuttingInsidePolygon.Instance.Delete();
            UpdateLines(remainLines);
        }

    
      
        void lineSeries_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (CurMainStage == Stage.Cutting)
            {
                CutInsidePolygon((OxyPlot.Series.LineSeries)sender, e);
            }
        }

        private void CutInsidePolygon(OxyPlot.Series.LineSeries lineSeries, OxyMouseDownEventArgs e)
        {
            DataPoint clickPoint = lineSeries.InverseTransform(e.Position);
            CuttingInsidePolygon.Instance.StartCutting(clickPoint);
            plotModel.InvalidatePlot(false);
        }
      
        internal void ExecuteCutCommand(string commands, int repeatTimes)
        {
            for (int i = 0; i < repeatTimes; i++)
            {
                foreach (char ch in commands)
                {
                    var key = ch.ToKey();
                    ExtendCuttingLine(key);
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
}
