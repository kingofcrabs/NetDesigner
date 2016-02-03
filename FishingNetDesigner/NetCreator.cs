using System;
using System.Collections.Generic;
using System.Drawing;
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
    class FishingNet
    {
        int xNum;
        int yNum;
        double xLen;
        double yLen;
        double thickness;
        #region
        public int XNum
        {
            get
            {
                return xNum;
            }
        }

        public int YNum
        {
            get
            {
                return yNum;
            }
        }

        public double Thickness
        {
            get
            {
                return thickness;
            }
        }

        public double WidthUnit 
        {
            get
            { 
                return xLen; 
            } 
        }

        public double HeightUnit
        {
            get
            {
                return yLen;
            }
        }
        #endregion

        public FishingNet()
        {
            CuttingLine = new List<Point2D>();
            GeneratedLines = null;
        }
        public List<Line> Create(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            this.xNum = xNum;
            this.yNum = yNum;
            this.xLen = xLen;
            this.yLen = yLen;
            this.thickness = thickness;
            List<Line> lines = new List<Line>();
            List<List<Point2D>> ptsVector = new List<List<Point2D>>();
            int rowCnt = yNum * 2 + 1;
            for (int row = 0; row < rowCnt; row++)
            {
                List<Point2D> ptsSameRow = new List<Point2D>();
                bool isEvenLine = row % 2 == 0;
                double xOffset = isEvenLine ? xLen / 2.0 : 0;
                for (int col = 0; col < xNum; col++)
                {
                    double xPos = xOffset + col * xLen;
                    double yPos = row * yLen/2;
                    ptsSameRow.Add(new Point2D(xPos, yPos));
                }
                ptsVector.Add(ptsSameRow);
            }

            for (int r = 0; r < rowCnt - 1; r++)
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
            GeneratedLines = lines;
            return lines;
        }
        public List<Point2D> CuttingLine { get; set; }
        public List<Line> GeneratedLines { get; set; }

        internal List<Point2D> GetReachablePts(Point2D anchorPt,Point2D invalidPt)
        {
            double x = anchorPt.X;
            int xIndex = (int)Math.Round((x - xLen / 4)  / (xLen/2));
            double y = anchorPt.Y;
            int yIndex = (int)Math.Round((y-yLen/4) / (yLen/2));
            bool isOnLowerEdge = yIndex % 2 == 0;
            bool isOnLeftEdge = xIndex % 2 == 0;
            bool canGoUpRight;
            canGoUpRight = isOnLowerEdge == isOnLeftEdge; //on left,low edge or on right,upper edge we can go ↗
            bool canGoUpLeft = !canGoUpRight;
            double eps = 0.000001;
            bool mostRight = Math.Abs(x + xLen / 4 - xLen * xNum) < eps;
            bool mostLeft = Math.Abs(x - xLen / 4) < eps;
            bool mostTop = Math.Abs(y + yLen / 4 - yLen * yNum) < eps;

            List<Point2D> pts = new List<Point2D>();
            if(!mostTop) //go ↑
            {
                pts.Add(new Point2D(anchorPt.X, anchorPt.Y + yLen/2));
            }
            if(!mostRight && !mostTop && canGoUpRight) //go ↗
            {
                pts.Add(new Point2D(anchorPt.X + xLen/2, anchorPt.Y + yLen/2));
            }
            //if(!mostLeft && !mostTop && canGoUpLeft) //go ↖
            //{
            //    pts.Add(new Point2D(anchorPt.X - xLen/2, anchorPt.Y + yLen/2));
            //}
            if( !mostRight)// go right
            {
                pts.Add(new Point2D(anchorPt.X + xLen/2, anchorPt.Y));
            }
            //if( !mostLeft && canGoUpLeft) //go left
            //{
            //    pts.Add(new Point2D(anchorPt.X - xLen/2, anchorPt.Y));
            //}
            pts.Remove(invalidPt);
            return pts;
        }
    }
}
