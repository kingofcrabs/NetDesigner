using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WW.Math;

namespace FishingNetDesigner.Data
{
    public enum CuttingOperation
    {
        Up = 0,
        Right = 1,
        UpRight = 2
    }
    public static class CuttingOperationExt
    {
        public static string ToFriendlyString(this CuttingOperation me)
        {
            switch (me)
            {
                case CuttingOperation.Right:
                    return "T";
                case CuttingOperation.Up:
                    return "N";
                case CuttingOperation.UpRight:
                    return "B";
                default:
                    throw new Exception("unknown CuttingOperation enum type!");
            }
        }

        public static OxyKey ToKey(this char ch)
        {
            switch(ch)
            {
                case 'N':
                    return OxyKey.NumPad8;
                case 'T':
                    return OxyKey.NumPad6;
                case 'B':
                    return OxyKey.NumPad9;
                default:
                    throw new Exception("unknown CuttingOperation enum type!");
            }
        }
    }

    class CuttingBySide
    {
        private static CuttingBySide instance = null;
        ScatterSeries current;
        public ScatterSeries Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value;
            }
        }
        public ScatterSeries Reachable { get; set; }
        public LineSeries Whole { get; set; }
        public LineSeries SelectionBoundary { get; set; }
        public double Thickness { get; set; }
        public static CuttingBySide Instance
        {
            get
            {
                if (instance == null)
                    instance = new CuttingBySide();
                return instance;
            }
        }


        private CuttingBySide()
        {
            Current = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red };
            Reachable = new ScatterSeries { MarkerType = MarkerType.Square, MarkerFill = OxyColors.Green };
            Whole = new LineSeries { Title = "Cutting Line", Color = OxyColors.Red };
            SelectionBoundary = new LineSeries {Title= "Cutting Boundary",Color = OxyColors.Red };
        }


        public List<LineSeries> AllLineSeries
        {
            get
            {
                return new List<LineSeries>() { Whole,SelectionBoundary };
            }
        }
        public List<ScatterSeries> AllScatterSeries
        {
            get
            {
                return new List<ScatterSeries>() { Reachable, Current };
            }
        }

        internal void Clear()
        {
            Reachable.Points.Clear();
            Whole.Points.Clear();
            Current.Points.Clear();
            SelectionBoundary.Points.Clear();
        }

        public void SelectSide(bool left)
        {
            double maxX = FishingNet.Instance.XNum * FishingNet.Instance.WidthUnit;
            double maxY = FishingNet.Instance.YNum * FishingNet.Instance.HeightUnit;
            if (Whole.Points.Count < 2)
                throw new Exception("剪裁点数小于2！");
            DataPoint ptStart = Whole.Points.Last();
            DataPoint ptTop = new DataPoint(ptStart.X, maxY);
            SelectionBoundary.Points.Clear();
            SelectionBoundary.Points.Add(ptStart);
            SelectionBoundary.Points.Add(ptTop);
            double sidePos = left ? 0 : maxX;
            SelectionBoundary.Points.Add(new DataPoint(sidePos, ptTop.Y));
            SelectionBoundary.Points.Add(new DataPoint(sidePos,0));
            SelectionBoundary.Points.Add(new DataPoint(Whole.Points.First().X, 0));
            SelectionBoundary.Points.Add(Whole.Points.First());
        }
   
        public void UpdateCurrent(Point2D currentPt, List<Point2D> reachablePts, bool fromKeyboard = false)
        {
            Current.Points.Clear();
            Thickness = FishingNet.Instance.Thickness * 2;
            var scatterPt = new ScatterPoint(currentPt.X, currentPt.Y, Thickness);
            Current.Points.Add(scatterPt);
            Reachable.Points.Clear();

            foreach(var pt in reachablePts)
            {
                Reachable.Points.Add(new ScatterPoint(pt.X, pt.Y, Thickness));
            }
            if (!fromKeyboard) //user create a new cutting line
                Whole.Points.Clear();

            Whole.Points.Add(new DataPoint(currentPt.X,currentPt.Y));
        }

       
        private bool IsSamePt(ScatterPoint pt, ScatterPoint currentPt, double xOffSet, double yOffSet)
        {
            return pt.X == currentPt.X + xOffSet && pt.Y == currentPt.Y + yOffSet;
        }

        public string DeleteSide { get; set; }

        internal bool Extend(OxyKey key,ref CuttingOperation op)
        {
            if (Current.Points.Count == 0)
                return false;

            double xOffSet = 0;
            double yOffset = 0;
            double maxY = FishingNet.Instance.HeightUnit * FishingNet.Instance.YNum;
            op = CuttingOperation.Up;
            if (CuttingBySide.Instance.Current.Points[0].Y > maxY)
                return false;

            bool isValidKey = GetOffSetAndOperation(key, ref xOffSet, ref yOffset, ref op);
            if (!isValidKey)
                return false;

            bool bCango = Cango(xOffSet,yOffset);
            if(bCango)
            {
                var currentPt = CuttingBySide.Instance.Current.Points[0];
                Point2D latestCurrent = new Point2D(currentPt.X + xOffSet, currentPt.Y + yOffset);
                var reachablePts = FishingNet.Instance.GetReachablePts(latestCurrent, new Point2D(currentPt.X, currentPt.Y));
                CuttingBySide.Instance.UpdateCurrent(latestCurrent, reachablePts, true);
            }
            return bCango;
        }

        internal bool Cango(double xOffSet, double yOffSet)
        {
            if (Current.Points.Count == 0 || Reachable.Points.Count == 0)
                return false;
            var currentPt = Current.Points[0];
            return Reachable.Points.Exists(pt => IsSamePt(pt, currentPt, xOffSet, yOffSet));
        }

   


        private bool GetOffSetAndOperation(OxyKey key, ref double xOffSet, ref double yOffset, ref CuttingOperation op)
        {
            double unitX = FishingNet.Instance.WidthUnit / 2;
            double unitY = FishingNet.Instance.HeightUnit / 2;
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

        public void StartCutting(DataPoint clickPoint)
        {
            if (!FishingNet.Instance.IsInside(clickPoint))
                return;

            if (FishingNet.Instance.Current == null || FishingNet.Instance.Current.Count == 0)
            {
                throw new Exception("未定义渔网结构！");
            }
            Point2D anchorPt = FishingNet.Instance.GetAnchorPos(new Point2D(clickPoint.X, clickPoint.Y));
            List<Point2D> reachablePts = FishingNet.Instance.GetReachablePts(anchorPt, new Point2D(-1, -1));
            CuttingBySide.Instance.UpdateCurrent(anchorPt, reachablePts);
        }
    }
}
