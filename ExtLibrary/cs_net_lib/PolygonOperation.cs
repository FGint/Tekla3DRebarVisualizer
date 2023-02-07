using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
    public class PolygonOperation
    {
        private const double AngleTollerance = 1.5;

        private readonly PolygonOperation.CreatePolygons createPolygons;

        public PolygonOperation()
        {
            this.createPolygons = new PolygonOperation.CreatePolygons();
        }

        private static Polygon CopyPolygon(Polygon origin, double valueZ = 0)
        {
            Polygon polygon = new Polygon();
            foreach (Point point in origin.Points)
            {
                polygon.Points.Add(new Point(point.X, point.Y, valueZ));
            }
            return polygon;
        }

        public PolygonOperation.ComparePolygonTypeEnum CsCmpTwoPolygons(Polygon polygon1, Polygon polygon2)
        {
            if (polygon1.Points.Count == 0 && polygon2.Points.Count == 0)
            {
                return PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2;
            }
            if (polygon1.Points.Count == 0)
            {
                return PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2;
            }
            if (polygon2.Points.Count == 0)
            {
                return PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1;
            }
            int i = 0;
            int num = 0;
            if (!Compare.EQ(PolygonOperation.CsTwoPolygonsMinDist2D(polygon1, polygon2), 0))
            {
                i = 0;
                while (i < polygon1.Points.Count && Geo.IsPointInsidePolygon2D(polygon2, polygon1.Points[i] as Point, false))
                {
                    i++;
                }
                if (i == polygon1.Points.Count)
                {
                    return PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2;
                }
                i = 0;
                while (i < polygon2.Points.Count && Geo.IsPointInsidePolygon2D(polygon1, polygon2.Points[i] as Point, false))
                {
                    i++;
                }
                if (i == polygon2.Points.Count)
                {
                    return PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1;
                }
                return PolygonOperation.ComparePolygonTypeEnum.POL_OUTSIDE;
            }
            if (polygon1.Points.Count == polygon2.Points.Count)
            {
                bool flag = false;
                for (i = 0; i < polygon1.Points.Count; i++)
                {
                    num = 0;
                    while (num < polygon2.Points.Count)
                    {
                        if (!Geo.CompareTwoPoints2D(polygon1.Points[i] as Point, polygon2.Points[num] as Point))
                        {
                            num++;
                        }
                        else
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (i != polygon1.Points.Count || num != polygon2.Points.Count)
                {
                    int num1 = 0;
                    while (num1 < polygon1.Points.Count && Geo.CompareTwoPoints2D(polygon1.Points[(num1 + i) % polygon1.Points.Count] as Point, polygon2.Points[(num1 + num) % polygon2.Points.Count] as Point))
                    {
                        num1++;
                    }
                    if (num1 == polygon1.Points.Count)
                    {
                        return PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2;
                    }
                    num1 = 0;
                    while (num1 < polygon1.Points.Count && Geo.CompareTwoPoints2D(polygon1.Points[(i + num1 + polygon1.Points.Count) % polygon1.Points.Count] as Point, polygon2.Points[(num - num1 + polygon2.Points.Count) % polygon2.Points.Count] as Point))
                    {
                        num1++;
                    }
                    if (num1 == polygon1.Points.Count)
                    {
                        return PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2;
                    }
                }
            }
            return PolygonOperation.ComparePolygonTypeEnum.POL1_COLLIDE_POL2;
        }

        private static void CsGetLinesList(List<LineSegment> lines1, List<LineSegment> lines2, List<LineSegment> lines)
        {
            List<LineSegment> lineSegments = new List<LineSegment>(lines1.Count);
            lineSegments.AddRange(lines1);
            for (int i = 0; i < lines2.Count; i++)
            {
                List<LineSegment> lineSegments1 = new List<LineSegment>(lineSegments.Count);
                for (int j = 0; j < lineSegments.Count; j++)
                {
                    List<Point> points = new List<Point>();
                    Intersect.IntersectLineSegmentToLineSegment2D(lineSegments[j], lines2[i], ref points);
                    if (points.Count == 1 && !Geo.CompareTwoPoints2D(lineSegments[j].Point1, points[0]) && !Geo.CompareTwoPoints2D(lineSegments[j].Point2, points[0]))
                    {
                        lineSegments1.Add(new LineSegment(new Point(lineSegments[j].Point1), new Point(points[0])));
                        lineSegments1.Add(new LineSegment(new Point(points[0]), new Point(lineSegments[j].Point2)));
                    }
                    else if (points.Count != 2 || Geo.CompareTwoPoints2D(lineSegments[j].Point1, points[0]) && Geo.CompareTwoPoints2D(lineSegments[j].Point2, points[1]))
                    {
                        lineSegments1.Add(new LineSegment(new Point(lineSegments[j].Point1), new Point(lineSegments[j].Point2)));
                    }
                    else
                    {
                        if (!Geo.CompareTwoPoints2D(lineSegments[j].Point1, points[0]))
                        {
                            lineSegments1.Add(new LineSegment(new Point(lineSegments[j].Point1), new Point(points[0])));
                        }
                        lineSegments1.Add(new LineSegment(new Point(points[0]), new Point(points[1])));
                        if (!Geo.CompareTwoPoints2D(lineSegments[j].Point2, points[1]))
                        {
                            lineSegments1.Add(new LineSegment(new Point(points[1]), new Point(lineSegments[j].Point2)));
                        }
                    }
                }
                lineSegments = lineSegments1;
            }
            foreach (LineSegment lineSegment in lineSegments)
            {
                lines.Add(lineSegment);
            }
        }

        private static void CsGetLinesList(Polygon polygonOne, Polygon polygonTwo, List<LineSegment> lines, PolygonOperation.SelectionTypeEnum selectionType)
        {
            List<LineSegment> lineSegments = new List<LineSegment>();
            List<LineSegment> lineSegments1 = new List<LineSegment>();
            for (int i = 0; i < polygonOne.Points.Count; i++)
            {
                int count = (i + 1) % polygonOne.Points.Count;
                lineSegments.Add(new LineSegment(new Point(polygonOne.Points[i] as Point), new Point(polygonOne.Points[count] as Point)));
            }
            for (int j = 0; j < polygonTwo.Points.Count; j++)
            {
                int num = (j + 1) % polygonTwo.Points.Count;
                lineSegments1.Add(new LineSegment(new Point(polygonTwo.Points[j] as Point), new Point(polygonTwo.Points[num] as Point)));
            }
            for (int k = 0; k < lineSegments1.Count; k++)
            {
                List<LineSegment> lineSegments2 = new List<LineSegment>(lineSegments.Count);
                for (int l = 0; l < lineSegments.Count; l++)
                {
                    List<Point> points = new List<Point>();
                    Intersect.IntersectLineSegmentToLineSegment2D(lineSegments[l], lineSegments1[k], ref points);
                    if (points.Count == 1 && !Geo.CompareTwoPoints2D(lineSegments[l].Point1, points[0]) && !Geo.CompareTwoPoints2D(lineSegments[l].Point2, points[0]))
                    {
                        lineSegments2.Add(new LineSegment(new Point(lineSegments[l].Point1), new Point(points[0])));
                        lineSegments2.Add(new LineSegment(new Point(points[0]), new Point(lineSegments[l].Point2)));
                    }
                    else if (points.Count != 2 || Geo.CompareTwoPoints2D(lineSegments[l].Point1, points[0]) && Geo.CompareTwoPoints2D(lineSegments[l].Point2, points[1]))
                    {
                        lineSegments2.Add(new LineSegment(new Point(lineSegments[l].Point1), new Point(lineSegments[l].Point2)));
                    }
                    else
                    {
                        if (!Geo.CompareTwoPoints2D(lineSegments[l].Point1, points[0]))
                        {
                            lineSegments2.Add(new LineSegment(new Point(lineSegments[l].Point1), new Point(points[0])));
                        }
                        lineSegments2.Add(new LineSegment(new Point(points[0]), new Point(points[1])));
                        if (!Geo.CompareTwoPoints2D(lineSegments[l].Point2, points[1]))
                        {
                            lineSegments2.Add(new LineSegment(new Point(points[1]), new Point(lineSegments[l].Point2)));
                        }
                    }
                }
                lineSegments = lineSegments2;
            }
            if (selectionType == PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES)
            {
                for (int m = lineSegments.Count - 1; m >= 0; m--)
                {
                    if (!Geo.IsPointInsidePolygon2D(polygonTwo, Geo.GetCenterPoint2D(lineSegments[m].Point1, lineSegments[m].Point2), true))
                    {
                        lineSegments.RemoveAt(m);
                    }
                }
            }
            foreach (LineSegment lineSegment in lineSegments)
            {
                lines.Add(lineSegment);
            }
        }

        private static double CsTwoPolygonsMinDist2D(Polygon polygon1, Polygon polygon2)
        {
            double num = double.MaxValue;
            for (int i = 0; i < polygon1.Points.Count; i++)
            {
                for (int j = 0; j < polygon2.Points.Count; j++)
                {
                    double distanceBetweenTwoLineSegments3D = Geo.GetDistanceBetweenTwoLineSegments3D(polygon1.Points[i] as Point, polygon1.Points[(i + 1) % polygon1.Points.Count] as Point, polygon2.Points[j] as Point, polygon2.Points[(j + 1) % polygon2.Points.Count] as Point);
                    if (num > distanceBetweenTwoLineSegments3D)
                    {
                        num = distanceBetweenTwoLineSegments3D;
                        if (num == 0)
                        {
                            return num;
                        }
                    }
                }
            }
            return num;
        }

        private void Difference(Polygon polygon1, List<Polygon> finalPolygons, List<LineSegment> intersectLines, List<LineSegment> pol1Lines)
        {
            List<LineSegment> lineSegments = new List<LineSegment>();
            List<Polygon> polygons = new List<Polygon>();
            this.createPolygons.Create(intersectLines, ref polygons);
            if (polygons.Count == 0)
            {
                lineSegments = pol1Lines;
            }
            else
            {
                for (int i = 0; i < polygons.Count; i++)
                {
                    PolygonOperation.CsGetLinesList(polygons[i], polygon1, intersectLines, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                }
                PolygonOperation.RemoveUsefulLines(intersectLines);
                for (int j = 0; j < pol1Lines.Count; j++)
                {
                    bool flag = false;
                    int num = 0;
                    while (num < intersectLines.Count)
                    {
                        if (!Geo.CompareTwoLinesSegment2D(pol1Lines[j], intersectLines[num]))
                        {
                            num++;
                        }
                        else
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        lineSegments.Add(pol1Lines[j]);
                    }
                }
                for (int k = 0; k < intersectLines.Count; k++)
                {
                    bool flag1 = false;
                    int num1 = 0;
                    while (num1 < pol1Lines.Count)
                    {
                        if (!Geo.CompareTwoLinesSegment2D(intersectLines[k], pol1Lines[num1]))
                        {
                            num1++;
                        }
                        else
                        {
                            flag1 = true;
                            break;
                        }
                    }
                    if (!flag1)
                    {
                        lineSegments.Add(intersectLines[k]);
                    }
                }
            }
            this.createPolygons.Create(lineSegments, ref finalPolygons);
        }

        public static bool GetPolygonOrientation(Polygon polygon)
        {
            return PolygonOperation.GetPolygonOrientation(Geo.ConvertListPointsFromPolygon(polygon));
        }

        public static bool GetPolygonOrientation(List<Point> polygon)
        {
            if (polygon.Count < 3)
            {
                return true;
            }
            int num = 0;
            int count = num + 1;
            double x = 0;
            while (num < polygon.Count)
            {
                Point item = polygon[num];
                Point point = polygon[count];
                x = x + (point.X - item.X) * (point.Y + item.Y);
                num++;
                count++;
                if (count < polygon.Count)
                {
                    continue;
                }
                count = count - polygon.Count;
            }
            return Compare.GT(x, 0);
        }

        public static void PolygonOffset(Polygon polygon, List<double> offsets, bool negative, bool openPolygon)
        {
            int i;
            if (offsets == null || offsets.Count < 1 || polygon == null || polygon.Points.Count < 1)
            {
                return;
            }
            Polygon point = new Polygon();
            foreach (Point point1 in polygon.Points)
            {
                point.Points.Add(new Point(point1));
            }
            bool flag = false;
            int count = polygon.Points.Count;
            if (Geo.CompareTwoPoints3D(polygon.Points[0] as Point, polygon.Points[count - 1] as Point))
            {
                count--;
                flag = true;
            }
            double item = offsets[0];
            for (i = 0; i < count; i++)
            {
                if (i >= offsets.Count)
                {
                    offsets.Add(item);
                }
                else
                {
                    item = offsets[i];
                }
            }
            int num = count;
            if (openPolygon)
            {
                num--;
            }
            for (i = 0; i < num; i++)
            {
                Vector vector = new Vector((polygon.Points[(i + 1) % count] as Point).X - (polygon.Points[i % count] as Point).X, (polygon.Points[(i + 1) % count] as Point).Y - (polygon.Points[i % count] as Point).Y, (polygon.Points[(i + 1) % count] as Point).Z - (polygon.Points[i % count] as Point).Z);
                Vector vector1 = new Vector(0, 0, -1000);
                vector1.Normalize();
                vector.Normalize();
                CoordinateSystem coordinateSystem = new CoordinateSystem()
                {
                    Origin = polygon.Points[i] as Point,
                    AxisX = vector1.Cross(vector),
                    AxisY = vector
                };
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                setPlane.AddPolygons(new Polygon[] { polygon });
                setPlane.AddPolygons(new Polygon[] { point });
                setPlane.Begin(coordinateSystem);
                try
                {
                    Point point2 = new Point();
                    Point point3 = new Point();
                    Point point4 = new Point();
                    double item1 = offsets[i];
                    if (!Geo.IsPointInsidePolygon2D(polygon, new Point(((polygon.Points[i] as Point).X + (polygon.Points[(i + 1) % count] as Point).X) / 2, ((polygon.Points[i] as Point).Y + (polygon.Points[(i + 1) % count] as Point).Y) / 2 + 1, ((polygon.Points[i] as Point).Z + (polygon.Points[(i + 1) % count] as Point).Z) / 2), false))
                    {
                        item1 = -item1;
                    }
                    if (negative)
                    {
                        item1 = -item1;
                    }
                    Geo.PointParallel(ref point2, ref point3, polygon.Points[i] as Point, polygon.Points[(i + 1) % count] as Point, item1);
                    if (i == 0)
                    {
                        point.Points[i] = point2;
                        point.Points[i + 1] = point3;
                    }
                    else if (i != num - 1)
                    {
                        if (Distance.PointToPoint(point.Points[i] as Point, point2) != 0)
                        {
                            LineSegment line = Intersection.LineToLine(new Line(point.Points[i - 1] as Point, point.Points[i] as Point), new Line(point2, point3));
                            if (line != null && line.Point1 != null)
                            {
                                point4 = new Point(line.Point1);
                            }
                        }
                        else
                        {
                            point4 = new Point(point2);
                        }
                        point.Points[i] = new Point(point4);
                        point.Points[i + 1] = new Point(point3);
                    }
                    else
                    {
                        if (Distance.PointToPoint(point.Points[i] as Point, point2) != 0)
                        {
                            LineSegment lineSegment = Intersection.LineToLine(new Line(point.Points[i - 1] as Point, point.Points[i] as Point), new Line(point2, point3));
                            if (lineSegment != null && lineSegment.Point1 != null)
                            {
                                point4 = new Point(lineSegment.Point1);
                            }
                        }
                        else
                        {
                            point4 = new Point(point2);
                        }
                        point.Points[i] = new Point(point4);
                        if (openPolygon)
                        {
                            point.Points[i + 1] = new Point(point3);
                        }
                        else
                        {
                            LineSegment line1 = Intersection.LineToLine(new Line(point.Points[0] as Point, point.Points[1] as Point), new Line(point2, point3));
                            if (line1 != null && line1.Point1 != null)
                            {
                                point4 = new Point(line1.Point1);
                            }
                            point.Points[0] = new Point(point4);
                        }
                    }
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
                setPlane.End();
            }
            if (flag)
            {
                point.Points[count] = new Point(point.Points[0] as Point);
                count++;
            }
            for (i = 0; i < count; i++)
            {
                polygon.Points[i] = new Point(point.Points[i] as Point);
            }
        }

        public List<PolygonOperation.PolygonWithHoles> PolygonOperations(Polygon polygon1, Polygon polygon2, PolygonOperation.PolygonOperationsEnum operation)
        {
            double z = 0;
            if (polygon1.Points.Count > 0)
            {
                z = (polygon1.Points[0] as Point).Z;
            }
            Polygon polygon = PolygonOperation.CopyPolygon(polygon1, z);
            Polygon polygon3 = PolygonOperation.CopyPolygon(polygon2, z);
            List<PolygonOperation.PolygonWithHoles> polygonWithHoles = new List<PolygonOperation.PolygonWithHoles>();
            List<Polygon> polygons = new List<Polygon>();
            if (operation != PolygonOperation.PolygonOperationsEnum.UNION)
            {
                List<LineSegment> lineSegments = new List<LineSegment>();
                List<LineSegment> lineSegments1 = new List<LineSegment>();
                if (operation == PolygonOperation.PolygonOperationsEnum.DIFFERENCE)
                {
                    PolygonOperation.CsGetLinesList(polygon, polygon3, lineSegments1, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                }
                PolygonOperation.CsGetLinesList(polygon, polygon3, lineSegments, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                PolygonOperation.CsGetLinesList(polygon3, polygon, lineSegments, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                PolygonOperation.RemoveUsefulLines(lineSegments);
                if (operation == PolygonOperation.PolygonOperationsEnum.DIFFERENCE)
                {
                    this.Difference(polygon, polygons, lineSegments, lineSegments1);
                }
                else if (operation == PolygonOperation.PolygonOperationsEnum.INTERSECT)
                {
                    this.createPolygons.Create(lineSegments, ref polygons);
                }
            }
            else
            {
                this.Union(polygon, polygon3, polygons);
            }
            this.SeparateHoles(polygons, polygonWithHoles);
            return polygonWithHoles;
        }

        public static List<Polygon> ReduseSizePolygon(List<Point> inputPolygon, double offset)
        {
            List<Point> points = new List<Point>(inputPolygon.Count);
            List<Line> lines = new List<Line>();
            List<Polygon> polygons = new List<Polygon>();
            bool polygonOrientation = PolygonOperation.GetPolygonOrientation(inputPolygon);
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
            }
            for (int i = 0; i < inputPolygon.Count; i++)
            {
                int num = i + 1;
                if (num >= inputPolygon.Count)
                {
                    num = 0;
                }
                Vector vectorLineSegment = Geo.GetVectorLineSegment(inputPolygon[num], inputPolygon[i]);
                Vector vector = new Vector(vectorLineSegment.Y, -vectorLineSegment.X, vectorLineSegment.Z);
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                Point[] item = new Point[] { inputPolygon[i], inputPolygon[num] };
                setPlane.AddPoints(item);
                setPlane.Begin(new Point(inputPolygon[i]), vectorLineSegment, vector);
                Point point = new Point(inputPolygon[i].X, inputPolygon[i].Y + offset, inputPolygon[i].Z);
                Point point1 = new Point(inputPolygon[num].X, inputPolygon[num].Y + offset, inputPolygon[num].Z);
                setPlane.AddPoints(new Point[] { point, point1 });
                setPlane.End();
                lines.Add(new Line(point, point1));
            }
            for (int j = 0; j < lines.Count; j++)
            {
                int num1 = j + 1;
                if (num1 >= lines.Count)
                {
                    num1 = 0;
                }
                LineSegment line = Intersection.LineToLine(lines[j], lines[num1]);
                if (line != null && Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                {
                    points.Add(new Point(line.Point1));
                }
            }
            if (points.Count == inputPolygon.Count)
            {
                Point item1 = points[points.Count - 1];
                points.RemoveAt(points.Count - 1);
                points.Insert(0, item1);
                Polygon polygon = Geo.ConvertPolygonFromListPoint(points);
                Polygon polygon1 = new Polygon();
                Polygon polygon2 = new Polygon();
                foreach (Point point2 in polygon.Points)
                {
                    polygon1.Points.Add(new Point(point2));
                }
                foreach (Point point3 in polygon.Points)
                {
                    polygon2.Points.Add(new Point(point3));
                }
                List<LineSegment> lineSegments = new List<LineSegment>();
                PolygonOperation.CsGetLinesList(polygon1, polygon2, lineSegments, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                double num2 = Math.Abs(offset);
                for (int k = lineSegments.Count - 1; k >= 0; k--)
                {
                    int num3 = 0;
                    while (num3 < inputPolygon.Count)
                    {
                        int count = num3 + 1;
                        if (count >= inputPolygon.Count)
                        {
                            count = count - inputPolygon.Count;
                        }
                        if (!Compare.LT(Geo.GetDistanceBetweenTwoLineSegments3D(lineSegments[k].Point1, lineSegments[k].Point2, inputPolygon[num3], inputPolygon[count]), num2))
                        {
                            num3++;
                        }
                        else
                        {
                            lineSegments.RemoveAt(k);
                            break;
                        }
                    }
                }
                (new PolygonOperation.CreatePolygons()).Create(lineSegments, ref polygons);
                if (polygons.Count == 1)
                {
                    points = Geo.ConvertListPointsFromPolygon(polygons[0]);
                    PolygonOperation.RemoveUnnecessaryPolygonPoints(points);
                    if (polygonOrientation != PolygonOperation.GetPolygonOrientation(points))
                    {
                        points.Reverse();
                    }
                }
            }
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
            }
            return polygons;
        }

        public static void RemoveUnnecessaryPolygonPoints(Polygon polygon)
        {
            List<Point> points = Geo.ConvertListPointsFromPolygon(polygon);
            polygon.Points.Clear();
            PolygonOperation.RemoveUnnecessaryPolygonPoints(points);
            polygon.Points.AddRange(points);
        }

        public static void RemoveUnnecessaryPolygonPoints(List<Point> polygon)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < polygon.Count; i++)
            {
                if (polygon.Count != 1 && Geo.CompareTwoPoints3D(polygon[i], polygon[(i + 1) % polygon.Count]))
                {
                    polygon.RemoveAt(i);
                    i--;
                }
            }
            if (polygon.Count > 3)
            {
                for (int j = 0; j < polygon.Count; j++)
                {
                    int count = j - 1;
                    int num = j + 1;
                    if (count < 0)
                    {
                        count = count + polygon.Count;
                    }
                    if (num >= polygon.Count)
                    {
                        num = num - polygon.Count;
                    }
                    Point item = polygon[count];
                    Point point = polygon[j];
                    Point item1 = polygon[num];
                    double angle3D = Geo.GetAngle3D(point, item, item1) * Constants.RAD_TO_DEG;
                    if (!Compare.IE(angle3D, 178.5, 181.5) && !Compare.IE(angle3D, -1.5, 1.5))
                    {
                        points.Add(point);
                    }
                    else if (Compare.NZ(angle3D) && Compare.NE(angle3D, 180))
                    {
                        double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(item, point);
                        double distanceBeetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(point, item1);
                        if (Compare.GT(distanceBeetveenTwoPoints3D, 4 * distanceBeetveenTwoPoints3D1) || Compare.GT(distanceBeetveenTwoPoints3D1, 4 * distanceBeetveenTwoPoints3D))
                        {
                            points.Add(point);
                        }
                    }
                }
                polygon.Clear();
                polygon.AddRange(points);
            }
        }

        private static void RemoveUsefulLines(List<LineSegment> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (Geo.CompareTwoPoints2D(lines[i].Point1, lines[i].Point2))
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }
            for (int j = 0; j < lines.Count; j++)
            {
                for (int k = j + 1; k < lines.Count; k++)
                {
                    if (Geo.CompareTwoLinesSegment2D(lines[j], lines[k]))
                    {
                        lines.RemoveAt(k);
                        k--;
                    }
                }
            }
        }

        [Obsolete("use PolygonOperation.ReduseSizePolygon(List<TSG.Point> inputPolygon, double offset")]
        public static List<Point> ResizePolygon(List<Point> inputPolygon, double offset)
        {
            List<Point> points = new List<Point>(inputPolygon.Count);
            List<Line> lines = new List<Line>();
            bool polygonOrientation = PolygonOperation.GetPolygonOrientation(inputPolygon);
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
            }
            for (int i = 0; i < inputPolygon.Count; i++)
            {
                int num = i + 1;
                if (num >= inputPolygon.Count)
                {
                    num = 0;
                }
                Vector vectorLineSegment = Geo.GetVectorLineSegment(inputPolygon[num], inputPolygon[i]);
                Vector vector = new Vector(vectorLineSegment.Y, -vectorLineSegment.X, vectorLineSegment.Z);
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                Point[] item = new Point[] { inputPolygon[i], inputPolygon[num] };
                setPlane.AddPoints(item);
                setPlane.Begin(new Point(inputPolygon[i]), vectorLineSegment, vector);
                Point point = new Point(inputPolygon[i].X, inputPolygon[i].Y + offset, inputPolygon[i].Z);
                Point point1 = new Point(inputPolygon[num].X, inputPolygon[num].Y + offset, inputPolygon[num].Z);
                setPlane.AddPoints(new Point[] { point, point1 });
                setPlane.End();
                lines.Add(new Line(point, point1));
            }
            for (int j = 0; j < lines.Count; j++)
            {
                int num1 = j + 1;
                if (num1 >= lines.Count)
                {
                    num1 = 0;
                }
                LineSegment line = Intersection.LineToLine(lines[j], lines[num1]);
                if (line != null && Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                {
                    points.Add(new Point(line.Point1));
                }
            }
            if (points.Count != inputPolygon.Count)
            {
                points = null;
            }
            else
            {
                Point item1 = points[points.Count - 1];
                points.RemoveAt(points.Count - 1);
                points.Insert(0, item1);
                Polygon polygon = Geo.ConvertPolygonFromListPoint(points);
                Polygon polygon1 = new Polygon();
                Polygon polygon2 = new Polygon();
                foreach (Point point2 in polygon.Points)
                {
                    polygon1.Points.Add(new Point(point2));
                }
                foreach (Point point3 in polygon.Points)
                {
                    polygon2.Points.Add(new Point(point3));
                }
                List<LineSegment> lineSegments = new List<LineSegment>();
                PolygonOperation.CsGetLinesList(polygon1, polygon2, lineSegments, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                double num2 = Math.Abs(offset);
                for (int k = lineSegments.Count - 1; k >= 0; k--)
                {
                    int num3 = 0;
                    while (num3 < inputPolygon.Count)
                    {
                        int count = num3 + 1;
                        if (count >= inputPolygon.Count)
                        {
                            count = count - inputPolygon.Count;
                        }
                        if (!Compare.LT(Geo.GetDistanceBetweenTwoLineSegments3D(lineSegments[k].Point1, lineSegments[k].Point2, inputPolygon[num3], inputPolygon[count]), num2))
                        {
                            num3++;
                        }
                        else
                        {
                            lineSegments.RemoveAt(k);
                            break;
                        }
                    }
                }
                List<Polygon> polygons = new List<Polygon>();
                (new PolygonOperation.CreatePolygons()).Create(lineSegments, ref polygons);
                if (polygons.Count != 1)
                {
                    points = null;
                }
                else
                {
                    points = Geo.ConvertListPointsFromPolygon(polygons[0]);
                    PolygonOperation.RemoveUnnecessaryPolygonPoints(points);
                    if (polygonOrientation != PolygonOperation.GetPolygonOrientation(points))
                    {
                        points.Reverse();
                    }
                }
            }
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
            }
            return points;
        }

        public static List<Point> ResizePolygon(List<Point> inputPolygon, List<double> offsets)
        {
            if (offsets == null || offsets.Count < 1 || inputPolygon == null || inputPolygon.Count < 3)
            {
                return null;
            }
            bool polygonOrientation = PolygonOperation.GetPolygonOrientation(inputPolygon);
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
                inputPolygon.Insert(0, inputPolygon[inputPolygon.Count - 1]);
                inputPolygon.RemoveAt(inputPolygon.Count - 1);
                offsets.Reverse();
            }
            List<Point> points = new List<Point>(inputPolygon.Count);
            List<Line> lines = new List<Line>();
            List<double> nums = new List<double>();
            for (int i = 0; i < inputPolygon.Count; i++)
            {
                int num = i + 1;
                if (num >= inputPolygon.Count)
                {
                    num = 0;
                }
                double num1 = 0;
                num1 = (i >= offsets.Count ? offsets[offsets.Count - 1] : offsets[i]);
                Vector vectorLineSegment = Geo.GetVectorLineSegment(inputPolygon[num], inputPolygon[i]);
                Vector vector = new Vector(vectorLineSegment.Y, -vectorLineSegment.X, vectorLineSegment.Z);
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                Point[] item = new Point[] { inputPolygon[i], inputPolygon[num] };
                setPlane.AddPoints(item);
                setPlane.Begin(new Point(inputPolygon[i]), vectorLineSegment, vector);
                Point point = new Point(inputPolygon[i].X, inputPolygon[i].Y + num1, inputPolygon[i].Z);
                Point point1 = new Point(inputPolygon[num].X, inputPolygon[num].Y + num1, inputPolygon[num].Z);
                setPlane.AddPoints(new Point[] { point, point1 });
                setPlane.End();
                lines.Add(new Line(point, point1));
                nums.Add(num1);
            }
            for (int j = 0; j < lines.Count; j++)
            {
                int num2 = j + 1;
                if (num2 >= lines.Count)
                {
                    num2 = 0;
                }
                LineSegment line = Intersection.LineToLine(lines[j], lines[num2]);
                if (line != null && Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                {
                    points.Add(new Point(line.Point1));
                }
            }
            if (points.Count != inputPolygon.Count)
            {
                points = null;
            }
            else
            {
                Point item1 = points[points.Count - 1];
                points.RemoveAt(points.Count - 1);
                points.Insert(0, item1);
                List<LineSegment> lineSegments = new List<LineSegment>();
                for (int k = 0; k < points.Count; k++)
                {
                    int num3 = k + 1;
                    if (num3 >= points.Count)
                    {
                        num3 = 0;
                    }
                    lineSegments.Add(new LineSegment(new Point(points[k]), new Point(points[num3])));
                }
                List<LineSegment> lineSegments1 = new List<LineSegment>();
                for (int l = 0; l < points.Count; l++)
                {
                    int num4 = l + 1;
                    if (num4 >= points.Count)
                    {
                        num4 = 0;
                    }
                    lineSegments1.Add(new LineSegment(new Point(points[l]), new Point(points[num4])));
                }
                List<LineSegment> lineSegments2 = new List<LineSegment>();
                PolygonOperation.CsGetLinesList(lineSegments, lineSegments1, lineSegments2);
                for (int m = lineSegments2.Count - 1; m >= 0; m--)
                {
                    int num5 = 0;
                    while (num5 < inputPolygon.Count)
                    {
                        int count = num5 + 1;
                        if (count >= inputPolygon.Count)
                        {
                            count = count - inputPolygon.Count;
                        }
                        double num6 = Math.Abs(nums[num5]);
                        if (!Compare.LT(Geo.GetDistanceBetweenTwoLineSegments3D(lineSegments2[m].Point1, lineSegments2[m].Point2, inputPolygon[num5], inputPolygon[count]), num6))
                        {
                            num5++;
                        }
                        else
                        {
                            Vector vectorLineSegment1 = Geo.GetVectorLineSegment(lineSegments2[m]);
                            Vector vector1 = Geo.GetVectorLineSegment(inputPolygon[num5], inputPolygon[count]);
                            if (!Compare.GT(vectorLineSegment1.Dot(vector1) / (vectorLineSegment1.GetLength() * vector1.GetLength()), 1 - Constants.CS_EPSILON))
                            {
                                break;
                            }
                            lineSegments2.RemoveAt(m);
                            break;
                        }
                    }
                }
                List<LineSegment> lineSegments3 = new List<LineSegment>(lineSegments2);
                List<Polygon> polygons = new List<Polygon>();
                (new PolygonOperation.CreatePolygons()).Create(lineSegments3, ref polygons);
                if (polygons.Count != 1)
                {
                    points = null;
                }
                else
                {
                    points = Geo.ConvertListPointsFromPolygon(polygons[0]);
                    PolygonOperation.RemoveUnnecessaryPolygonPoints(points);
                    if (polygonOrientation != PolygonOperation.GetPolygonOrientation(points))
                    {
                        points.Reverse();
                    }
                }
            }
            if (polygonOrientation)
            {
                inputPolygon.Reverse();
                inputPolygon.Insert(0, inputPolygon[inputPolygon.Count - 1]);
                inputPolygon.RemoveAt(inputPolygon.Count - 1);
                offsets.Reverse();
            }
            return points;
        }

        private void SeparateHoles(List<Polygon> allPolygons, List<PolygonOperation.PolygonWithHoles> resultPolygons)
        {
            List<Polygon> polygons = new List<Polygon>();
            List<Polygon> polygons1 = new List<Polygon>();
            for (int i = 0; i < allPolygons.Count; i++)
            {
                int j = 0;
                for (j = 0; j < allPolygons.Count; j++)
                {
                    if (i != j)
                    {
                        Polygon polygon = new Polygon();
                        polygon.Points.AddRange(allPolygons[j].Points);
                        Polygon polygon1 = new Polygon();
                        polygon1.Points.AddRange(allPolygons[i].Points);
                        if (this.CsCmpTwoPolygons(polygon, polygon1) == PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1)
                        {
                            break;
                        }
                    }
                }
                Polygon polygon2 = new Polygon();
                polygon2.Points.AddRange(allPolygons[i].Points);
                if (j == allPolygons.Count)
                {
                    polygons.Add(polygon2);
                }
                else
                {
                    polygons1.Add(polygon2);
                }
            }
            for (int k = 0; k < polygons.Count; k++)
            {
                PolygonOperation.PolygonWithHoles polygonWithHole = new PolygonOperation.PolygonWithHoles()
                {
                    contourPolygon = polygons[k]
                };
                for (int l = 0; l < polygons1.Count; l++)
                {
                    if (this.CsCmpTwoPolygons(polygons[k], polygons1[l]) == PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1)
                    {
                        polygonWithHole.innerPolygons.Add(polygons1[l]);
                    }
                }
                resultPolygons.Add(polygonWithHole);
            }
        }

        public static double SummaryPolygonArea2D(Polygon polygon)
        {
            if (polygon.Points.Count < 3)
            {
                return 0;
            }
            List<Point> points = Geo.ConvertListPointsFromPolygon(polygon);
            points.Add(new Point(points[0]));
            return Math.Abs(PolygonOperation.SummaryPolygonArea2DStep(points)) / 2;
        }

        public static double SummaryPolygonArea2D(List<Point> polygon)
        {
            if (polygon.Count < 3)
            {
                return 0;
            }
            List<Point> points = new List<Point>(polygon)
            {
                new Point(polygon[0])
            };
            return Math.Abs(PolygonOperation.SummaryPolygonArea2DStep(points)) / 2;
        }

        private static double SummaryPolygonArea2DStep(List<Point> polygon)
        {
            if (polygon.Count < 2)
            {
                return 0;
            }
            double x = polygon[0].X * polygon[1].Y - polygon[1].X * polygon[0].Y;
            List<Point> points = new List<Point>(polygon);
            points.RemoveAt(0);
            return x + PolygonOperation.SummaryPolygonArea2DStep(points);
        }

        private static List<T> ToList<T>(ArrayList arrayList)
        {
            List<T> ts = new List<T>(arrayList.Count);
            foreach (T t in arrayList)
            {
                ts.Add(t);
            }
            return ts;
        }

        public void Union(Polygon polygon1, Polygon polygon2, List<Polygon> finalPolygons)
        {
            List<LineSegment> lineSegments = new List<LineSegment>();
            PolygonOperation.CsGetLinesList(polygon1, polygon2, lineSegments, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
            PolygonOperation.CsGetLinesList(polygon2, polygon1, lineSegments, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
            this.createPolygons.Create(lineSegments, ref finalPolygons);
            if (finalPolygons.Count == 1)
            {
                List<PolygonOperation.PolygonWithHoles> polygonWithHoles = this.PolygonOperations(finalPolygons[0], polygon1, PolygonOperation.PolygonOperationsEnum.DIFFERENCE);
                if (polygonWithHoles.Count == 1)
                {
                    polygonWithHoles = this.PolygonOperations(polygonWithHoles[0].contourPolygon, polygon2, PolygonOperation.PolygonOperationsEnum.DIFFERENCE);
                    foreach (PolygonOperation.PolygonWithHoles polygonWithHole in polygonWithHoles)
                    {
                        finalPolygons.Add(polygonWithHole.contourPolygon);
                    }
                }
            }
        }

        public enum ComparePolygonTypeEnum
        {
            POL1_EQ_POL2,
            POL1_COLLIDE_POL2,
            POL1_IN_POL2,
            POL2_IN_POL1,
            POL_OUTSIDE
        }

        public class CreatePolygons
        {
            private static double offset;

            private List<PolygonOperation.CreatePolygons.PolygonLine> allLines;

            static CreatePolygons()
            {
                PolygonOperation.CreatePolygons.offset = 25 * Constants.CS_EPSILON;
            }

            public CreatePolygons()
            {
                this.allLines = new List<PolygonOperation.CreatePolygons.PolygonLine>();
            }

            public void Create(List<LineSegment> lines, ref List<Polygon> returnPolygons)
            {
                returnPolygons.Clear();
                if (lines.Count > 2)
                {
                    SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                    Vector normal = Geo.GetVectorLineSegment(lines[0]).GetNormal();
                    Vector vector = Geo.GetVectorLineSegment(lines[1]).GetNormal();
                    this.InitializeSetPlane(setPlane, ref normal, ref vector, lines);
                    setPlane.Begin(lines[0].Point1, normal, vector);
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines = this.CreatePolygonLines(lines);
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    this.allLines = new List<PolygonOperation.CreatePolygons.PolygonLine>(polygonLines);
                    this.SplitLines();
                    this.FoundSameLines();
                    polygonLines = new List<PolygonOperation.CreatePolygons.PolygonLine>(this.allLines);
                    this.RemoveBlindLines(this.CreateConnectionBetweenLines(polygonLines, polygonLines1), polygonLines1);
                    List<PolygonOperation.CreatePolygons.PolygonData> polygonDatas = new List<PolygonOperation.CreatePolygons.PolygonData>()
                    {
                        new PolygonOperation.CreatePolygons.PolygonData()
                    };
                    foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygonLines1)
                    {
                        returnPolygons.AddRange(this.GeneratePolygons(polygonDatas, polygonLine, polygonLine.Neighbors1.Count > 0, true));
                    }
                    for (int i = returnPolygons.Count - 1; i >= 0; i--)
                    {
                        if (returnPolygons[i].Points.Count != 0)
                        {
                            foreach (Point point in returnPolygons[i].Points)
                            {
                                setPlane.AddPoints(new Point[] { point });
                            }
                        }
                        else
                        {
                            returnPolygons.RemoveAt(i);
                        }
                    }
                    setPlane.End();
                }
            }

            private List<PolygonOperation.CreatePolygons.PolygonLine> CreateConnectionBetweenLines(List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines, List<PolygonOperation.CreatePolygons.PolygonLine> polygonsByLines)
            {
                List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                while (polygonLines.Count > 0)
                {
                    PolygonOperation.CreatePolygons.PolygonLine item = polygonLines[0];
                    item.FoundNeighbors(polygonLines);
                    polygonsByLines.Add(item);
                    for (int i = polygonLines.Count - 1; i >= 0; i--)
                    {
                        if (polygonLines[i].Expanded)
                        {
                            if (polygonLines[i].Neighbors1.Count == 0 || polygonLines[i].Neighbors2.Count == 0)
                            {
                                polygonLines1.Add(polygonLines[i]);
                            }
                            polygonLines.RemoveAt(i);
                        }
                    }
                }
                return polygonLines1;
            }

            private List<PolygonOperation.CreatePolygons.PolygonLine> CreatePolygonLines(List<LineSegment> lines)
            {
                List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                foreach (LineSegment line in lines)
                {
                    bool flag = false;
                    foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygonLines)
                    {
                        double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, polygonLine.Point1);
                        double num = Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, polygonLine.Point2);
                        if (!Compare.LE(distanceBeetveenTwoPoints3D, PolygonOperation.CreatePolygons.offset) || !Compare.LE(num, PolygonOperation.CreatePolygons.offset))
                        {
                            distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, polygonLine.Point2);
                            num = Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, polygonLine.Point1);
                            if (!Compare.LE(distanceBeetveenTwoPoints3D, PolygonOperation.CreatePolygons.offset) || !Compare.LE(num, PolygonOperation.CreatePolygons.offset))
                            {
                                continue;
                            }
                            flag = true;
                            break;
                        }
                        else
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                    polygonLines.Add(new PolygonOperation.CreatePolygons.PolygonLine(line));
                }
                return polygonLines;
            }

            private void FoundInnerLines(PolygonOperation.CreatePolygons.PolygonData polygonData)
            {
                foreach (PolygonOperation.CreatePolygons.PolygonLine polygonDatum in polygonData)
                {
                    this.FoundInnerLines(polygonData, polygonDatum, polygonDatum.Neighbors1);
                    this.FoundInnerLines(polygonData, polygonDatum, polygonDatum.Neighbors2);
                }
            }

            private void FoundInnerLines(PolygonOperation.CreatePolygons.PolygonData polygonData, PolygonOperation.CreatePolygons.PolygonLine line, List<PolygonOperation.CreatePolygons.PolygonLine> neighbors)
            {
                for (int i = neighbors.Count - 1; i > -1; i--)
                {
                    if (!polygonData.Contains(neighbors[i]))
                    {
                        int num = 0;
                        List<PolygonOperation.CreatePolygons.PolygonLine>.Enumerator enumerator = polygonData.GetEnumerator();
                        try
                        {
                            do
                            {
                                Label0:
                                if (!enumerator.MoveNext())
                                {
                                    break;
                                }
                                PolygonOperation.CreatePolygons.PolygonLine current = enumerator.Current;
                                if (current != line && current.ContainsNeighbor(neighbors[i]))
                                {
                                    num++;
                                }
                                else
                                {
                                    goto Label0;
                                }
                            }
                            while (num <= 1);
                        }
                        finally
                        {
                            ((IDisposable)enumerator).Dispose();
                        }
                        if (num > 1)
                        {
                            Point centerPoint3D = Geo.GetCenterPoint3D(neighbors[i].Point1, neighbors[i].Point2);
                            if (Geo.IsPointInsidePolygon2D(polygonData.Polygon, centerPoint3D, true))
                            {
                                neighbors[i].RemoveNeighbor();
                            }
                        }
                    }
                }
            }

            private void FoundSameLines()
            {
                for (int i = 0; i < this.allLines.Count; i++)
                {
                    int num = -1;
                    int num1 = i + 1;
                    while (num1 < this.allLines.Count)
                    {
                        double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point1, this.allLines[num1].Point1);
                        double distanceBeetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point2, this.allLines[num1].Point2);
                        if (!Compare.LE(distanceBeetveenTwoPoints3D, PolygonOperation.CreatePolygons.offset) || !Compare.LE(distanceBeetveenTwoPoints3D1, PolygonOperation.CreatePolygons.offset))
                        {
                            distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point1, this.allLines[num1].Point2);
                            distanceBeetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point2, this.allLines[num1].Point1);
                            if (!Compare.LE(distanceBeetveenTwoPoints3D, PolygonOperation.CreatePolygons.offset) || !Compare.LE(distanceBeetveenTwoPoints3D1, PolygonOperation.CreatePolygons.offset))
                            {
                                num1++;
                            }
                            else
                            {
                                num = num1;
                                break;
                            }
                        }
                        else
                        {
                            num = num1;
                            break;
                        }
                    }
                    if (num != -1)
                    {
                        this.allLines.RemoveAt(num);
                    }
                }
            }

            private List<Polygon> GeneratePolygons(List<PolygonOperation.CreatePolygons.PolygonData> linesPolygons, PolygonOperation.CreatePolygons.PolygonLine actualLine, bool usePoint1, bool first)
            {
                List<Polygon> polygons = new List<Polygon>();
                PolygonOperation.CreatePolygons.PolygonData item = linesPolygons[linesPolygons.Count - 1];
                int num = item.IndexOf(actualLine);
                if (num != -1)
                {
                    PolygonOperation.CreatePolygons.PolygonData polygonDatum = new PolygonOperation.CreatePolygons.PolygonData(item.GetRange(num, item.Count - num));
                    if (!PolygonOperation.CreatePolygons.PolygonData.CheckIfExist(linesPolygons, polygonDatum))
                    {
                        polygonDatum.GetPolygon();
                        linesPolygons.Insert(0, polygonDatum);
                        this.FoundInnerLines(polygonDatum);
                    }
                }
                else
                {
                    item.Add(actualLine);
                    if (!usePoint1)
                    {
                        this.GeneratePolygons(linesPolygons, actualLine.Neighbors2, actualLine.Point2);
                    }
                    else
                    {
                        this.GeneratePolygons(linesPolygons, actualLine.Neighbors1, actualLine.Point1);
                    }
                    item.RemoveAt(item.Count - 1);
                }
                if (first)
                {
                    this.GeneratePolygonsFirst(linesPolygons, polygons);
                }
                return polygons;
            }

            private void GeneratePolygons(List<PolygonOperation.CreatePolygons.PolygonData> linesPolygons, List<PolygonOperation.CreatePolygons.PolygonLine> neighbors, Point actualPoint)
            {
                List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                for (int i = neighbors.Count - 1; i > -1; i--)
                {
                    if (i < neighbors.Count && !polygonLines.Contains(neighbors[i]))
                    {
                        polygonLines.Add(neighbors[i]);
                        double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(actualPoint, neighbors[i].Point1);
                        double num = Geo.GetDistanceBeetveenTwoPoints3D(actualPoint, neighbors[i].Point2);
                        this.GeneratePolygons(linesPolygons, neighbors[i], Compare.GT(distanceBeetveenTwoPoints3D, num), false);
                    }
                }
            }

            private void GeneratePolygonsFirst(List<PolygonOperation.CreatePolygons.PolygonData> linesPolygons, List<Polygon> result)
            {
                linesPolygons.RemoveAt(linesPolygons.Count - 1);
                double area = double.MinValue;
                foreach (PolygonOperation.CreatePolygons.PolygonData linesPolygon in linesPolygons)
                {
                    if (!Compare.GE(linesPolygon.Area, area))
                    {
                        continue;
                    }
                    area = linesPolygon.Area;
                    result.Clear();
                    result.Add(linesPolygon.Polygon);
                }
                if (linesPolygons.Count > 1)
                {
                    PolygonOperation polygonOperation = new PolygonOperation();
                    foreach (PolygonOperation.CreatePolygons.PolygonData polygonDatum in linesPolygons)
                    {
                        bool flag = true;
                        List<Polygon>.Enumerator enumerator = result.GetEnumerator();
                        try
                        {
                            do
                            {
                                if (!enumerator.MoveNext())
                                {
                                    break;
                                }
                                Polygon current = enumerator.Current;
                                flag = flag & polygonOperation.CsCmpTwoPolygons(current, polygonDatum.Polygon) == PolygonOperation.ComparePolygonTypeEnum.POL_OUTSIDE;
                            }
                            while (flag);
                        }
                        finally
                        {
                            ((IDisposable)enumerator).Dispose();
                        }
                        if (!flag || !Compare.GT(polygonDatum.Area, 0))
                        {
                            continue;
                        }
                        result.Add(polygonDatum.Polygon);
                    }
                }
                linesPolygons.Clear();
                linesPolygons.Add(new PolygonOperation.CreatePolygons.PolygonData());
            }

            private void InitializeSetPlane(SetPlane plane, ref Vector vX, ref Vector vY, List<LineSegment> lines)
            {
                for (int i = 1; Compare.LT(vX.GetLength(), 0.5) && i < lines.Count; i++)
                {
                    vX = Geo.GetVectorLineSegment(lines[i]).GetNormal();
                }
                Vector vector = new Vector(Math.Abs(vX.X), Math.Abs(vX.Y), Math.Abs(vX.Z));
                Vector vector1 = new Vector(Math.Abs(vY.X), Math.Abs(vY.Y), Math.Abs(vY.Z));
                bool flag = (Geo.CompareTwoPoints3D(vector, vector1) ? true : Compare.LT(vector1.GetLength(), 0.5));
                foreach (LineSegment line in lines)
                {
                    Point[] point1 = new Point[] { line.Point1, line.Point2 };
                    plane.AddPoints(point1);
                    if (!flag)
                    {
                        continue;
                    }
                    vY = Geo.GetVectorLineSegment(line).GetNormal();
                    vector1 = new Vector(Math.Abs(vY.X), Math.Abs(vY.Y), Math.Abs(vY.Z));
                    flag = (Geo.CompareTwoPoints3D(vector, vector1) ? true : Compare.LT(vector1.GetLength(), 0.5));
                }
            }

            private void RemoveBlindLines(List<PolygonOperation.CreatePolygons.PolygonLine> blindLines, List<PolygonOperation.CreatePolygons.PolygonLine> polygonsByLines)
            {
                List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines = new List<PolygonOperation.CreatePolygons.PolygonLine>(blindLines.Count);
                polygonLines.AddRange(blindLines);
                while (polygonLines.Count > 0)
                {
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonLines1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygonLines)
                    {
                        this.allLines.Remove(polygonLine);
                        if (!polygonsByLines.Contains(polygonLine))
                        {
                            continue;
                        }
                        if (polygonLine.Neighbors1.Count > 0)
                        {
                            polygonsByLines.Add(polygonLine.Neighbors1[0]);
                        }
                        else if (polygonLine.Neighbors2.Count > 0)
                        {
                            polygonsByLines.Add(polygonLine.Neighbors2[0]);
                        }
                        polygonsByLines.Remove(polygonLine);
                    }
                    foreach (PolygonOperation.CreatePolygons.PolygonLine allLine in this.allLines)
                    {
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine1 in polygonLines)
                        {
                            if (allLine.Neighbors1.Contains(polygonLine1))
                            {
                                allLine.Neighbors1.Remove(polygonLine1);
                            }
                            if (!allLine.Neighbors2.Contains(polygonLine1))
                            {
                                continue;
                            }
                            allLine.Neighbors2.Remove(polygonLine1);
                        }
                        if (allLine.Neighbors1.Count != 0 && allLine.Neighbors2.Count != 0)
                        {
                            continue;
                        }
                        polygonLines1.Add(allLine);
                    }
                    polygonLines = polygonLines1;
                }
            }

            private void SplitLines()
            {
                for (int i = 0; i < this.allLines.Count; i++)
                {
                    for (int j = i + 1; j < this.allLines.Count; j++)
                    {
                        List<Point> points = new List<Point>();
                        if (!this.allLines[i].ContainsNeighbor(this.allLines[j]) && Intersect.IntersectLineSegmentToLineSegment3D(this.allLines[i], this.allLines[j], ref points) && points.Count == 1)
                        {
                            double num = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point1, points[0]), Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[i].Point2, points[0]));
                            double num1 = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[j].Point1, points[0]), Geo.GetDistanceBeetveenTwoPoints3D(this.allLines[j].Point2, points[0]));
                            bool flag = Compare.LE(num, PolygonOperation.CreatePolygons.offset);
                            bool flag1 = Compare.LE(num1, PolygonOperation.CreatePolygons.offset);
                            if (!flag || !flag1)
                            {
                                if (flag)
                                {
                                    this.allLines.Add(this.allLines[j].Split(points[0]));
                                }
                                else if (!flag1)
                                {
                                    this.allLines.Add(this.allLines[i].Split(points[0]));
                                    this.allLines.Add(this.allLines[j].Split(points[0]));
                                }
                                else
                                {
                                    this.allLines.Add(this.allLines[i].Split(points[0]));
                                }
                            }
                        }
                    }
                }
            }

            private class PolygonData : List<PolygonOperation.CreatePolygons.PolygonLine>
            {
                public double Area
                {
                    get;
                    private set;
                }

                public Polygon Polygon
                {
                    get;
                    private set;
                }

                public PolygonData()
                {
                    this.Polygon = new Polygon();
                }

                public PolygonData(List<PolygonOperation.CreatePolygons.PolygonLine> input) : base(input)
                {
                    this.Polygon = new Polygon();
                }

                public static bool CheckIfExist(List<PolygonOperation.CreatePolygons.PolygonData> linesPolygons, PolygonOperation.CreatePolygons.PolygonData polygon)
                {
                    bool flag1 = false;
                    System.Threading.Tasks.Parallel.For(0, linesPolygons.Count - 1, (int ii) =>
                    {
                        if (linesPolygons[ii].Count == polygon.Count)
                        {
                            bool flag = true;
                            List<PolygonOperation.CreatePolygons.PolygonLine>.Enumerator enumerator = polygon.GetEnumerator();
                            try
                            {
                                do
                                {
                                    if (!enumerator.MoveNext())
                                    {
                                        break;
                                    }
                                    PolygonOperation.CreatePolygons.PolygonLine current = enumerator.Current;
                                    flag = linesPolygons[ii].Contains(current);
                                }
                                while (flag);
                            }
                            finally
                            {
                                ((IDisposable)enumerator).Dispose();
                            }
                            if (flag)
                            {
                                flag1 = true;
                            }
                        }
                    });
                    return flag1;
                }

                public void GetPolygon()
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        int count = (i + 1) % base.Count;
                        if (!base[i].Neighbors1.Contains(base[count]))
                        {
                            this.Polygon.Points.Add(new Point(base[i].Point2));
                        }
                        else
                        {
                            this.Polygon.Points.Add(new Point(base[i].Point1));
                        }
                    }
                    PolygonOperation.RemoveUnnecessaryPolygonPoints(this.Polygon);
                    for (int j = 0; j < this.Polygon.Points.Count; j++)
                    {
                        for (int k = j + 1; k < this.Polygon.Points.Count; k++)
                        {
                            if (Geo.CompareTwoPoints2D(this.Polygon.Points[j] as Point, this.Polygon.Points[k] as Point))
                            {
                                List<Point> list = PolygonOperation.ToList<Point>(this.Polygon.Points.GetRange(0, j));
                                list.AddRange(PolygonOperation.ToList<Point>(this.Polygon.Points.GetRange(k, this.Polygon.Points.Count - k)));
                                List<Point> points = PolygonOperation.ToList<Point>(this.Polygon.Points.GetRange(j, k - j));
                                if (PolygonOperation.GetPolygonOrientation(list) != PolygonOperation.GetPolygonOrientation(points))
                                {
                                    points.Reverse();
                                    points.Insert(0, points[points.Count - 1]);
                                    points.RemoveAt(points.Count - 1);
                                    this.Polygon = new Polygon();
                                    this.Polygon.Points.AddRange(list.GetRange(0, j));
                                    this.Polygon.Points.AddRange(points);
                                    this.Polygon.Points.AddRange(list.GetRange(j, list.Count - j));
                                }
                            }
                        }
                    }
                    this.Area = PolygonOperation.SummaryPolygonArea2D(this.Polygon);
                }
            }

            private class PolygonLine : LineSegment
            {
                public bool Expanded
                {
                    get;
                    set;
                }

                public List<PolygonOperation.CreatePolygons.PolygonLine> Neighbors1
                {
                    get;
                    set;
                }

                public List<PolygonOperation.CreatePolygons.PolygonLine> Neighbors2
                {
                    get;
                    set;
                }

                public PolygonLine(LineSegment line) : base(new Point(line.Point1), new Point(line.Point2))
                {
                    this.Neighbors1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    this.Neighbors2 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    this.Expanded = false;
                }

                public bool ContainsNeighbor(PolygonOperation.CreatePolygons.PolygonLine neighbor)
                {
                    if (this.Neighbors1.Contains(neighbor))
                    {
                        return true;
                    }
                    return this.Neighbors2.Contains(neighbor);
                }

                public void FoundNeighbors(List<PolygonOperation.CreatePolygons.PolygonLine> lines)
                {
                    if (!this.Expanded)
                    {
                        this.Expanded = true;
                        foreach (PolygonOperation.CreatePolygons.PolygonLine line in lines)
                        {
                            if (line == this)
                            {
                                continue;
                            }
                            double num = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, base.Point1), Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, base.Point1));
                            double num1 = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, base.Point2), Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, base.Point2));
                            if (!Compare.LE(num, num1))
                            {
                                if (!Compare.LE(num1, PolygonOperation.CreatePolygons.offset))
                                {
                                    continue;
                                }
                                this.Neighbors2.Add(line);
                                line.FoundNeighbors(lines);
                            }
                            else
                            {
                                if (!Compare.LE(num, PolygonOperation.CreatePolygons.offset))
                                {
                                    continue;
                                }
                                this.Neighbors1.Add(line);
                                line.FoundNeighbors(lines);
                            }
                        }
                        if (this.Neighbors1.Count == 0)
                        {
                            double num2 = double.MaxValue;
                            PolygonOperation.CreatePolygons.PolygonLine polygonLine = null;
                            foreach (PolygonOperation.CreatePolygons.PolygonLine line1 in lines)
                            {
                                if (line1 == this)
                                {
                                    continue;
                                }
                                double num3 = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line1.Point1, base.Point1), Geo.GetDistanceBeetveenTwoPoints3D(line1.Point2, base.Point1));
                                if (!Compare.LE(num3, num2) || !Compare.LE(num3, 1))
                                {
                                    continue;
                                }
                                num2 = num3;
                                polygonLine = line1;
                            }
                            if (polygonLine != null)
                            {
                                this.Neighbors1.Add(polygonLine);
                                polygonLine.FoundNeighbors(lines);
                            }
                        }
                        if (this.Neighbors2.Count == 0)
                        {
                            double num4 = double.MaxValue;
                            PolygonOperation.CreatePolygons.PolygonLine polygonLine1 = null;
                            foreach (PolygonOperation.CreatePolygons.PolygonLine line2 in lines)
                            {
                                if (line2 == this)
                                {
                                    continue;
                                }
                                double num5 = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line2.Point1, base.Point2), Geo.GetDistanceBeetveenTwoPoints3D(line2.Point2, base.Point2));
                                if (!Compare.LE(num5, num4) || !Compare.LE(num5, 1))
                                {
                                    continue;
                                }
                                num4 = num5;
                                polygonLine1 = line2;
                            }
                            if (polygonLine1 != null)
                            {
                                this.Neighbors2.Add(polygonLine1);
                                polygonLine1.FoundNeighbors(lines);
                            }
                        }
                    }
                }

                public void RemoveNeighbor()
                {
                    foreach (PolygonOperation.CreatePolygons.PolygonLine neighbors1 in this.Neighbors1)
                    {
                        int num = neighbors1.Neighbors1.IndexOf(this);
                        if (num != -1)
                        {
                            neighbors1.Neighbors1.RemoveAt(num);
                        }
                        num = neighbors1.Neighbors2.IndexOf(this);
                        if (num == -1)
                        {
                            continue;
                        }
                        neighbors1.Neighbors2.RemoveAt(num);
                    }
                    foreach (PolygonOperation.CreatePolygons.PolygonLine neighbors2 in this.Neighbors2)
                    {
                        int num1 = neighbors2.Neighbors1.IndexOf(this);
                        if (num1 != -1)
                        {
                            neighbors2.Neighbors1.RemoveAt(num1);
                        }
                        num1 = neighbors2.Neighbors2.IndexOf(this);
                        if (num1 == -1)
                        {
                            continue;
                        }
                        neighbors2.Neighbors2.RemoveAt(num1);
                    }
                }

                public PolygonOperation.CreatePolygons.PolygonLine Split(Point point)
                {
                    PolygonOperation.CreatePolygons.PolygonLine polygonLine = new PolygonOperation.CreatePolygons.PolygonLine(this)
                    {
                        Point2 = new Point(point)
                    };
                    base.Point1 = new Point(point);
                    return polygonLine;
                }
            }
        }

        public enum PolygonOperationsEnum
        {
            INTERSECT,
            UNION,
            DIFFERENCE
        }

        public class PolygonWithHoles
        {
            public Polygon contourPolygon;

            public List<Polygon> innerPolygons;

            public PolygonWithHoles()
            {
                this.contourPolygon = new Polygon();
                this.innerPolygons = new List<Polygon>();
            }
        }

        private enum SelectionTypeEnum
        {
            GET_ALL_LINES,
            GET_INTERSECTION_LINES
        }
    }
}