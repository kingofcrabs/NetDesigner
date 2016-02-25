using System.Collections.Generic;
using System.Linq;
using WW.Cad.Actions;
using WW.Math;
using OxyPlot;
using OxyPlot.Series;
using System;
using FishingNetDesigner.data;

namespace FishingNetDesigner.Data
{
    class CuttingByPolygon
    {
        private static CuttingByPolygon instance;
        public List<Point2D> Points { get; set; }
        public OxyPlot.Series.LineSeries Whole { get; set; }
        public bool Completed { get; set; }
        private CuttingByPolygon()
        {
            Points = new List<Point2D>();
            Whole = new LineSeries { MarkerType = MarkerType.Circle, Color=OxyColors.Red, MarkerFill = OxyColors.Red };
        }

        public static CuttingByPolygon Instance
        {
            get
            {
                if (instance == null)
                    instance = new CuttingByPolygon();
                return instance;
            }
        }


        private bool IsInPolygon(Point2D[] poly, Point2D point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.Y - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }

        public bool IsInPolygon(IControlPointCollection pts2Test, IControlPointCollection polyPts)
        {
            Point2D[] pts = new Point2D[polyPts.Count];
            for (int i = 0; i < polyPts.Count; i++)
            {
                var pt3D = polyPts.Get(i);
                pts[i] = new Point2D((float)pt3D.X, (float)pt3D.Y);
            }

            bool isInside = false;
            for (int i = 0; i < pts2Test.Count; i++)
            {
                var pt3D = pts2Test.Get(i);
                Point2D ptF = new Point2D((float)pt3D.X, (float)pt3D.Y);
                if (IsInPolygon(pts, ptF))
                {
                    isInside = true;
                    break;
                }
            }
            return isInside;

        }
        public List<LineSeries> AllLineSeries
        {
            get
            {
                return new List<LineSeries>() { Whole};
            }
        }
        internal void Clear()
        {
            Whole.Points.Clear();
        }

        internal void AddPoint(DataPoint clickPoint)
        {
            if (FishingNet.Instance.Current == null || FishingNet.Instance.Current.Count == 0)
            {
                throw new Exception("未定义渔网结构！");
            }
            Whole.Points.Add(clickPoint);
        }

        internal void Complete()
        {
            if (Whole.Points.Count < 3)
                throw new Exception("补全多边形需要3个或以上的已定义点！");
            Whole.Points.Add(Whole.Points.First());
            Completed = true;
        }

        internal List<Line> Delete()
        {
            List<Line> survivedLines = new List<Line>();
            List<Point2D> polygonPts = new List<Point2D>();
            Whole.Points.ForEach(pt => polygonPts.Add(new Point2D(pt.X, pt.Y)));

            FishingNet.Instance.Current.ForEach(l => CheckThenAdd(survivedLines, polygonPts,l));
            Memo.Instance.Update(survivedLines);
            return survivedLines;
        }

        private void CheckThenAdd(List<Line> survivedLines,List<Point2D> polygonPts,Line l)
        {
            Point2D pt = new Point2D((l.ptStart.X + l.ptEnd.X) / 2, (l.ptStart.Y + l.ptEnd.Y) / 2);
            if(!Polygon.IsInPolygon(polygonPts,pt))
                survivedLines.Add(l);
        }
    }
}
