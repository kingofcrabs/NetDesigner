using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW.Cad.Actions;
using WW.Cad.IO;
using WW.Cad.Model;
using WW.Cad.Model.Entities;
using WW.Math;

namespace FishingNetDesigner.Data
{
    class Dwg
    {
        public static void Save(string filePath, List<Line> lines)
        {
            DxfModel dxfModel = new DxfModel(DxfVersion.Dxf13);
            lines.ForEach(l=> AddEntity(dxfModel,l));
            
            DwgWriter.Write(filePath, dxfModel);
        }

        private static void AddEntity(DxfModel dxfModel,Line l)
        {
            DxfLine line = new DxfLine(l.ptStart,l.ptEnd);
            line.Thickness = l.thickness;
            dxfModel.Entities.Add(line);
        }
    }
}
