using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWS = System.Windows.Shapes;
using TSD = Tekla.Structures.Drawing;

namespace Tekla3DRebarVisualizer
{
    public class BoundingBoxOfLines
    {
        private List<SWS.Line> SWSLine { get; set; }
        private List<TSD.Line> TSDLine { get; set; }
        public double minX { get; set; }
        public double maxX { get; set; }
        public double minY { get; set; }
        public double maxY { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public BoundingBoxOfLines(List<SWS.Line> swsline)
        {
            SWSLine = swsline;
            ObtainMaxMinValuesSWS();
        }

        public BoundingBoxOfLines(List<TSD.Line> tsdline)
        {
            TSDLine = tsdline;
            ObtainMaxMinValuesTSD();
        }

        private void ObtainMaxMinValuesSWS()
        {
            double minX_1 = SWSLine.Min(x => x.X1);
            double minX_2 = SWSLine.Min(x => x.X2);
            minX = minX_1 < minX_2 ? minX_1 : minX_2;

            double maxX_1 = SWSLine.Max(x => x.X1);
            double maxX_2 = SWSLine.Max(x => x.X2);
            maxX = maxX_1 > maxX_2 ? maxX_1 : maxX_2;

            double minY_1 = SWSLine.Min(y => y.Y1);
            double minY_2 = SWSLine.Min(y => y.Y2);
            minY = minY_1 < minY_2 ? minY_1 : minY_2;

            double maxY_1 = SWSLine.Max(y => y.Y1);
            double maxY_2 = SWSLine.Max(y => y.Y2);
            maxY = maxY_1 > maxY_2 ? maxY_1 : maxY_2;

            Height = maxY - minY;
            Width = maxX - minX;
        }

        private void ObtainMaxMinValuesTSD()
        {
            double minX_1 = TSDLine.Min(x => x.StartPoint.X);
            double minX_2 = TSDLine.Min(x => x.EndPoint.X);
            minX = minX_1 < minX_2 ? minX_1 : minX_2;

            double maxX_1 = TSDLine.Max(x => x.StartPoint.X);
            double maxX_2 = TSDLine.Max(x => x.EndPoint.X);
            maxX = maxX_1 > maxX_2 ? maxX_1 : maxX_2;

            double minY_1 = TSDLine.Min(y => y.StartPoint.Y);
            double minY_2 = TSDLine.Min(y => y.EndPoint.Y);
            minY = minY_1 < minY_2 ? minY_1 : minY_2;

            double maxY_1 = TSDLine.Max(y => y.StartPoint.Y);
            double maxY_2 = TSDLine.Max(y => y.EndPoint.Y);
            maxY = maxY_1 > maxY_2 ? maxY_1 : maxY_2;

            Height = maxY - minY;
            Width = maxX - minX;
        }
    }
}
