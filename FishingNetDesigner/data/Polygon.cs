using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WW.Cad.Actions;

namespace FishingNetDesigner.Data
{
    class Polygon
    {
        private static bool IsInPolygon(PointF[] poly, PointF point)
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

        public static bool IsInPolygon(IControlPointCollection pts2Test, IControlPointCollection polyPts)
        {
            PointF[] pts = new PointF[polyPts.Count];
            for (int i = 0; i < polyPts.Count; i++)
            {
                var pt3D = polyPts.Get(i);
                pts[i] = new PointF((float)pt3D.X, (float)pt3D.Y);
            }

            bool isInside = false;
            for (int i = 0; i < pts2Test.Count; i++)
            {
                var pt3D = pts2Test.Get(i);
                PointF ptF = new PointF((float)pt3D.X, (float)pt3D.Y);
                if (IsInPolygon(pts, ptF))
                {
                    isInside = true;
                    break;
                }
            }
            return isInside;

        }
    }
}
