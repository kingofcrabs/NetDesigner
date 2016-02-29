using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WW.Cad.Actions;
using WW.Math;

namespace FishingNetDesigner.Data
{
    class Polygon
    {
        public static bool IsInPolygon(List<Point2D> polygon, Point2D point)
        {
            if (polygon.Exists(pt => pt.X == point.X && pt.Y == point.Y))
                return true;
            bool isInside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++) 
            { 
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) 
                    && (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)) 
                { 
                    isInside = !isInside;
                }
            }
            return isInside;
        }
    }
}
