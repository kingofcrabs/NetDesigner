using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WW.Math;

namespace FishingNetDesigner
{
    public enum CuttingOperation
    {
        Up = 0,
        Right = 2,
        UpRight = 4
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

    class CuttingLineSeries
    {
        private static CuttingLineSeries instance = null;
        public ScatterSeries Current { get; set; }
        public ScatterSeries Reachable { get; set; }
        public LineSeries Whole { get; set; }
        public static CuttingLineSeries Instance
        {
            get
            {
                if (instance == null)
                    instance = new CuttingLineSeries();
                return instance;
            }
        }


        private CuttingLineSeries()
        {
            Current = new ScatterSeries { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red };
            Reachable = new ScatterSeries { MarkerType = MarkerType.Square, MarkerFill = OxyColors.Blue };
            Whole = new LineSeries { Title = "Cutting Line"};
            
            Whole.Color = OxyColors.Red;
        }
        public List<LineSeries> AllLineSeries
        {
            get
            {
                return new List<LineSeries>() { Whole };
            }
        }
        public List<ScatterSeries> AllScatterSeries
        {
            get
            {
                return new List<ScatterSeries>() { Reachable, Current };
            }
        }

        internal void Reset()
        {
            instance = new CuttingLineSeries();
        }

   
        public void UpdateCurrent(Point2D currentPt, List<Point2D> reachablePts, double thickness, bool fromKeyboard = false)
        {
            Current.Points.Clear();
            var scatterPt = new ScatterPoint(currentPt.X, currentPt.Y, thickness);
            Current.Points.Add(scatterPt);
            Reachable.Points.Clear();

            foreach(var pt in reachablePts)
            {
                Reachable.Points.Add(new ScatterPoint(pt.X,pt.Y,thickness));
            }
            if (!fromKeyboard) //user create a new cutting line
                Whole.Points.Clear();

            Whole.Points.Add(new DataPoint(currentPt.X,currentPt.Y));
        }

        internal bool CanGo(double xOffSet, double yOffSet)
        {
            if (Current.Points.Count == 0 || Reachable.Points.Count == 0)
                return false;
            var currentPt = Current.Points[0];
            return Reachable.Points.Exists(pt => IsSamePt(pt, currentPt, xOffSet, yOffSet));
        }

        private bool IsSamePt(ScatterPoint pt, ScatterPoint currentPt, double xOffSet, double yOffSet)
        {
            return pt.X == currentPt.X + xOffSet && pt.Y == currentPt.Y + yOffSet;
        }
    }
}
