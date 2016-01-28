using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WW.Cad.Model.Entities;
using WW.Math;

namespace WinFormsEditExample
{
    class NetCreator
    {
        public static List<DxfEntity> Create(int xNum, int yNum, double xLen, double yLen, double thickness)
        {
            List<DxfEntity> entities = new List<DxfEntity>();
            List<List<Point2D>> ptsVector = new List<List<Point2D>>();
            for (int row = 0; row < yNum; row++)
            {
                List<Point2D> ptsSameRow = new List<Point2D>();
                for (int col = 0; col < xNum; col++)
                {
                    double xOffset = col % 2 == 0 ? xLen / 2.0 : 0;
                    double xPos = xOffset + col * xLen;
                    double yPos = row * yLen;
                    ptsSameRow.Add(new Point2D(xPos, yPos));
                }
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
                    var line = new DxfLine(ptStart, ptEnd1);
                    line.Thickness = thickness;
                    entities.Add(line);

                    int ptEnd2Col = isEvenLine ? c + 1 : c - 1;
                    if (ptEnd2Col < 0 || ptEnd2Col >= xNum)
                        continue;
                    Point2D ptEnd2 = upperRow[ptEnd2Col];
                    line = new DxfLine(ptStart, ptEnd2);
                    line.Thickness = thickness;
                    entities.Add(line);
                }
            }
            return entities;
        }
    }
}
