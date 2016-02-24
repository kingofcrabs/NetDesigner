using FishingNetDesigner.data;
using FishingNetDesigner.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WW.Cad.Model.Entities;
using WW.Math;

namespace FishingNetDesigner.Data
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
    
    class FishingNet: BindableBase
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
            set
            {
                SetProperty(ref xNum, value);
            }

        }
        public int YNum
        {
            get
            {
                return yNum;
            }
            set
            {
                SetProperty(ref yNum, value);
            }
        }
        public double Thickness
        {
            get
            {
                return thickness;
            }
            set
            {
                SetProperty(ref thickness, value);
            }
        }
        public double WidthUnit 
        {
            get
            { 
                return xLen; 
            }
            set
            {
                SetProperty(ref xLen, value);
            }
        }
        public double HeightUnit
        {
            get
            {
                return yLen;
            }
            set
            {
                SetProperty(ref yLen, value);
            }
        }
        static FishingNet instance = null;

        public List<Line> Current { get; set; }
        #endregion

        public FishingNet()
        {
            Current = null;
        }
        public static FishingNet Instance
        {
            get
            {
                if (instance == null)
                    instance = new FishingNet();
                return instance;
            }
        }

   
        private FishingNet(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            this.xNum = xNum;
            this.yNum = yNum;
            this.xLen = xLen;
            this.yLen = yLen;
            this.thickness = thickness;
            Current = new List<Line>();
        }
        public  List<Line> CreateCell()
        {
            List<Line> lines = new List<Line>();
            Point2D ptBottom = new Point2D(xLen / 2, 0);
            Point2D ptLeft = new Point2D(0, yLen / 2);
            Point2D ptRight = new Point2D(xLen, yLen / 2);
            Point2D ptTop = new Point2D(xLen / 2, yLen);
            return new List<Line>() {
                new Line(ptBottom,ptLeft,thickness),
                new Line(ptBottom, ptRight,thickness),
                new Line(ptLeft, ptTop,thickness),
                new Line(ptRight, ptTop,thickness)
            };
        }

        public FishingNet Create(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            instance = new FishingNet(xNum, yNum, xLen, yLen, thickness);
            return instance;
        }

        public List<Line> Generate()
        {
            if (xNum == 1 && yNum == 1)
            {
                Current = CreateCell();
                return Current;
            }
            List<Line> lines = new List<Line>();    
            List<List<Point2D>> ptsVector = new List<List<Point2D>>();
            int rowCnt = yNum * 2 + 1;
            for (int row = 0; row < rowCnt; row++)
            {
                List<Point2D> ptsSameRow = new List<Point2D>();
                bool isEvenLine = row % 2 == 0;
                double xOffset = isEvenLine ? xLen / 2.0 : 0;
                int colCnt = isEvenLine ? xNum : xNum + 1;
                for (int col = 0; col < colCnt; col++)
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
                bool isEvenLine = r % 2 == 0;
                int colCnt = isEvenLine ? xNum : xNum + 1;
                for (int c = 0; c < bottomRow.Count; c++)
                {
                    Point2D ptStart = bottomRow[c];
                    if (c < upperRow.Count)
                    {
                        Point2D ptEnd1 = upperRow[c];
                        var line = new Line(ptStart, ptEnd1, thickness);
                        lines.Add(line);
                    }

                    int ptEnd2Col = isEvenLine ? c + 1 : c - 1;
                    if (ptEnd2Col < 0 || ptEnd2Col >= upperRow.Count)
                        continue;
                    Point2D ptEnd2 = upperRow[ptEnd2Col];
                    var line2 = new Line(ptStart, ptEnd2,thickness);
                    lines.Add(line2);
                }
            }
            Memo.Instance.Create(lines);
            Current = lines;
            return lines;
        }
  

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
            if (!mostLeft && !mostTop && canGoUpLeft) //go ↖
            {
                pts.Add(new Point2D(anchorPt.X - xLen / 2, anchorPt.Y + yLen / 2));
            }
            if( !mostRight)// go right
            {
                pts.Add(new Point2D(anchorPt.X + xLen/2, anchorPt.Y));
            }
            if (!mostLeft) //go left
            {
                pts.Add(new Point2D(anchorPt.X - xLen / 2, anchorPt.Y));
            }
            pts.Remove(invalidPt);
            return pts;
        }

        public List<Line> DeleteHalf(string side,List<Point2D> cuttingLine)
        {
            List<Line> survivedLines = new List<Line>();
            Dictionary<int, double> eachLayerCuttingPositions = new Dictionary<int, double>();
            cuttingLine.ForEach(pt => CalculateEachLayer(pt, eachLayerCuttingPositions, side));
            Current.ForEach(l => CheckThenAdd(survivedLines, l,side == "L", eachLayerCuttingPositions));
            Memo.Instance.Update(survivedLines);
            return survivedLines;
        }

        private void CalculateEachLayer(Point2D pt, Dictionary<int, double> eachLayerCuttingPositions, string side)
        {
            int layerIndex = GetLayer(pt.Y);
            if (eachLayerCuttingPositions.ContainsKey(layerIndex))
            {
                if (side == "L" && pt.X < eachLayerCuttingPositions[layerIndex]) //left, update the point even left
                {
                    eachLayerCuttingPositions[layerIndex] = pt.X;
                }
                if (side == "R" && pt.X > eachLayerCuttingPositions[layerIndex])// right, update the point even right
                {
                    eachLayerCuttingPositions[layerIndex] = pt.X;
                }
            }
            else
                eachLayerCuttingPositions.Add(layerIndex, pt.X);
        }

        private int GetLayer(double yPos)
        {
            return (int)(yPos / (HeightUnit / 2));
        }

        private void CheckThenAdd(List<Line> survivedLines, Line l, bool wantBigger, Dictionary<int, double> eachLayerCuttingPositions)
        {
            Point2D middlePt = new Point2D((l.ptStart.X + l.ptEnd.X) / 2, (l.ptStart.Y + l.ptEnd.Y) / 2);
            int layer = GetLayer(middlePt.Y);
            if (!eachLayerCuttingPositions.ContainsKey(layer))
                return;
            bool bValid = false;
            if (wantBigger && middlePt.X > eachLayerCuttingPositions[layer])
                bValid = true;
            if (!wantBigger && middlePt.X < eachLayerCuttingPositions[layer])
                bValid = true;
            if (bValid)
                survivedLines.Add(l);
        }


        internal bool IsInside(OxyPlot.DataPoint clickPoint)
        {
            var xMax = xNum * WidthUnit;
            var yMax = yNum * HeightUnit;
            var x = clickPoint.X;
            var y = clickPoint.Y;
            return x < xMax && x > 0 && y > 0 && y < yMax;
        }

        internal Point2D GetAnchorPos(Point2D pt)
        {
            double halfWidth = WidthUnit / 2.0d;
            double halfHeight = HeightUnit / 2.0d;
            double quarterHeight = HeightUnit / 4.0d;
            double firstLineOffset = WidthUnit / 4.0d;
            double x = (int)(Math.Round((pt.X - firstLineOffset) / halfWidth)) * halfWidth + firstLineOffset;
            double y = (int)(pt.Y / halfHeight) * halfHeight + quarterHeight;
            return new Point2D(x, y);
        }

        
    }
}
