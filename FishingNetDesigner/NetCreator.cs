using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WW.Cad.Model.Entities;
using WW.Math;

namespace FishingNetDesigner
{
    public struct Line
    {
        public Point2D ptStart;
        public Point2D ptEnd;
        public double thickness;
        public Line(Point2D ptStart, Point2D ptEnd, double thickness)
        {
            this.ptStart = ptStart;
            this.ptEnd = ptEnd;
            this.thickness = thickness;
        }
    }
    class NetCreator
    {
        public static List<Line> Create(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            List<Line> lines = new List<Line>();
            List<List<Point2D>> ptsVector = new List<List<Point2D>>();
            for (int row = 0; row < yNum; row++)
            {
                List<Point2D> ptsSameRow = new List<Point2D>();
                bool isEvenLine = row % 2 == 0;
                double xOffset = isEvenLine ? xLen / 2.0 : 0;
                for (int col = 0; col < xNum; col++)
                {
                    double xPos = xOffset + col * xLen;
                    double yPos = row * yLen;
                    ptsSameRow.Add(new Point2D(xPos, yPos));
                }
                ptsVector.Add(ptsSameRow);
            }

            for (int r = 0; r < yNum - 1; r++)
            {
                List<Point2D> bottomRow = ptsVector[r];
                List<Point2D> upperRow = ptsVector[r+1];
                for (int c = 0; c < xNum; c++)
                {
                    bool isEvenLine = r % 2 == 0;
                    Point2D ptStart = bottomRow[c];
                    Point2D ptEnd1 = upperRow[c];
                    var line = new Line(ptStart, ptEnd1,thickness);
                    lines.Add(line);

                    int ptEnd2Col = isEvenLine ? c + 1 : c - 1;
                    if (ptEnd2Col < 0 || ptEnd2Col >= xNum)
                        continue;
                    Point2D ptEnd2 = upperRow[ptEnd2Col];
                    line = new Line(ptStart, ptEnd2,thickness);
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
