using System.Collections.Generic;
using System.Linq;
using WW.Cad.Actions;
using WW.Math;
using OxyPlot;
using OxyPlot.Series;

namespace FishingNetDesigner.Data
{
    class CuttingByPolygon
    {
        private static CuttingByPolygon instance;
        public List<Point2D> Points { get; set; }
        public OxyPlot.Series.LineSeries Whole { get; set; }

        private CuttingByPolygon()
        {
            Points = new List<Point2D>();
            Whole = new LineSeries { MarkerType = MarkerType.Circle, MarkerFill = OxyColors.Red };
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
        internal void Reset()
        {
            instance = new CuttingByPolygon();
        }
    }
}
