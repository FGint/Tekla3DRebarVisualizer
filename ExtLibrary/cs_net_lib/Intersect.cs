using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Solid;

namespace cs_net_lib
{
    public class Intersect
    {
        public Intersect()
        {
        }

        public static List<List<Point>> GetPartFacesPoints(Part part, Tekla.Structures.Model.Solid.SolidCreationTypeEnum solidType = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
        {
            Tekla.Structures.Model.Solid solid = part.GetSolid(solidType);
            List<List<Point>> lists = new List<List<Point>>();
            List<Point> points = new List<Point>();
            FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
            while (faceEnumerator.MoveNext())
            {
                Face current = faceEnumerator.Current;
                if (current == null)
                {
                    continue;
                }
                LoopEnumerator loopEnumerator = current.GetLoopEnumerator();
                while (loopEnumerator.MoveNext())
                {
                    Loop loop = loopEnumerator.Current;
                    if (loop == null)
                    {
                        continue;
                    }
                    VertexEnumerator vertexEnumerator = loop.GetVertexEnumerator();
                    while (vertexEnumerator.MoveNext())
                    {
                        Point point = vertexEnumerator.Current;
                        if (point == null)
                        {
                            continue;
                        }
                        points.Add(point);
                    }
                    lists.Add(new List<Point>(points));
                    points.Clear();
                }
            }
            return lists;
        }

        [Obsolete("use Intersect.GetPartFacesPoints(TSM.Part, TSM.Solid.SolidCreationTypeEnum.NORMAL")]
        public static List<List<Point>> GetPartFacesPointsWithCuts(Part part)
        {
            return Intersect.GetPartFacesPoints(part, Tekla.Structures.Model.Solid.SolidCreationTypeEnum.NORMAL);
        }

        [Obsolete("use Intersect.GetPartFacesPoints(TSM.Part, TSM.Solid.SolidCreationTypeEnum.RAW")]
        public static List<List<Point>> GetPartFacesPointsWithoutCuts(Part part)
        {
            return Intersect.GetPartFacesPoints(part, Tekla.Structures.Model.Solid.SolidCreationTypeEnum.RAW);
        }

        public static ArrayList IntersectAllFaces(Tekla.Structures.Model.Solid solid, Point point1, Point point2, Point point3)
        {
            IEnumerator enumerator = solid.IntersectAllFaces(point1, point2, point3);
            ArrayList arrayLists = new ArrayList();
            while (enumerator.MoveNext())
            {
                arrayLists = enumerator.Current as ArrayList;
            }
            return arrayLists;
        }

        public static bool IntersectLineSegmentToLineSegment2D(LineSegment lineSegment1, LineSegment lineSegment2, ref List<Point> intersectPoints)
        {
            intersectPoints.Clear();
            if (!Geo.CompareTwoLinesSegment2D(lineSegment1, lineSegment2))
            {
                Line line = new Line(lineSegment1);
                LineSegment lineSegment = Intersection.LineToLine(line, new Line(lineSegment2));
                if (lineSegment != null)
                {
                    if (!Geo.CompareTwoPoints2D(lineSegment.Point1, lineSegment.Point2))
                    {
                        intersectPoints.Add(lineSegment.Point1);
                        intersectPoints.Add(lineSegment.Point2);
                    }
                    else
                    {
                        intersectPoints.Add(lineSegment.Point1);
                    }
                    for (int i = 0; i < intersectPoints.Count; i++)
                    {
                        if (!Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, intersectPoints[i]) || !Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, intersectPoints[i]))
                        {
                            intersectPoints.RemoveAt(i);
                            i--;
                        }
                    }
                }
                if (Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point1))
                {
                    intersectPoints.Add(new Point(lineSegment2.Point1));
                }
                if (Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point2))
                {
                    intersectPoints.Add(new Point(lineSegment2.Point2));
                }
                if (Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point1))
                {
                    intersectPoints.Add(new Point(lineSegment1.Point1));
                }
                if (Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point2))
                {
                    intersectPoints.Add(new Point(lineSegment1.Point2));
                }
                for (int j = 0; j < intersectPoints.Count; j++)
                {
                    for (int k = j + 1; k < intersectPoints.Count; k++)
                    {
                        if (Geo.CompareTwoPoints2D(intersectPoints[j], intersectPoints[k]))
                        {
                            intersectPoints.RemoveAt(k);
                            k--;
                        }
                    }
                }
                Geo.SortPoints2D(intersectPoints, lineSegment1.Point1);
                for (int l = 0; l < intersectPoints.Count - 2; l++)
                {
                    if (Geo.IsPointInLineSegment2D(intersectPoints[l], intersectPoints[l + 2], intersectPoints[l + 1]))
                    {
                        intersectPoints.RemoveAt(l + 1);
                        l--;
                    }
                }
            }
            else
            {
                intersectPoints.Add(lineSegment1.Point1);
                intersectPoints.Add(lineSegment1.Point2);
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineSegmentToLineSegment2D(Point lineSegment1Point1, Point lineSegment1Point2, Point lineSegment2Point1, Point lineSegment2Point2, ref List<Point> intersectPoints)
        {
            LineSegment lineSegment = new LineSegment(lineSegment1Point1, lineSegment1Point2);
            return Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, new LineSegment(lineSegment2Point1, lineSegment2Point2), ref intersectPoints);
        }

        public static bool IntersectLineSegmentToLineSegment3D(LineSegment lineSegment1, LineSegment lineSegment2, ref List<Point> intersectPoints)
        {
            intersectPoints.Clear();
            if (!Geo.CompareTwoLinesSegment3D(lineSegment1, lineSegment2))
            {
                Line line = new Line(lineSegment1);
                LineSegment lineSegment = Intersection.LineToLine(line, new Line(lineSegment2));
                if (lineSegment != null)
                {
                    if (!Geo.CompareTwoPoints3D(lineSegment.Point1, lineSegment.Point2))
                    {
                        intersectPoints.Add(lineSegment.Point1);
                        intersectPoints.Add(lineSegment.Point2);
                    }
                    else
                    {
                        intersectPoints.Add(lineSegment.Point1);
                    }
                    for (int i = 0; i < intersectPoints.Count; i++)
                    {
                        if (!Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, intersectPoints[i]) || !Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, intersectPoints[i]))
                        {
                            intersectPoints.RemoveAt(i);
                            i--;
                        }
                    }
                }
                if (Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point1))
                {
                    intersectPoints.Add(new Point(lineSegment2.Point1));
                }
                if (Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point2))
                {
                    intersectPoints.Add(new Point(lineSegment2.Point2));
                }
                if (Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point1))
                {
                    intersectPoints.Add(new Point(lineSegment1.Point1));
                }
                if (Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point2))
                {
                    intersectPoints.Add(new Point(lineSegment1.Point2));
                }
                for (int j = 0; j < intersectPoints.Count; j++)
                {
                    for (int k = j + 1; k < intersectPoints.Count; k++)
                    {
                        if (Geo.CompareTwoPoints2D(intersectPoints[j], intersectPoints[k]))
                        {
                            intersectPoints.RemoveAt(k);
                            k--;
                        }
                    }
                }
                Geo.SortPoints3D(intersectPoints, lineSegment1.Point1);
                for (int l = 0; l < intersectPoints.Count - 2; l++)
                {
                    if (Geo.IsPointInLineSegment3D(intersectPoints[l], intersectPoints[l + 2], intersectPoints[l + 1]))
                    {
                        intersectPoints.RemoveAt(l + 1);
                        l--;
                    }
                }
            }
            else
            {
                intersectPoints.Add(new Point(lineSegment1.Point1));
                intersectPoints.Add(new Point(lineSegment1.Point2));
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineSegmentToLineSegment3D(Point lineSegment1Point1, Point lineSegment1Point2, Point lineSegment2Point1, Point lineSegment2Point2, ref List<Point> intersectPoints)
        {
            LineSegment lineSegment = new LineSegment(lineSegment1Point1, lineSegment1Point2);
            return Intersect.IntersectLineSegmentToLineSegment3D(lineSegment, new LineSegment(lineSegment2Point1, lineSegment2Point2), ref intersectPoints);
        }

        public static bool IntersectLineToCircle2D(Point linePoint1, Point linePoint2, Point centre, double radius, ref List<Point> intersectPoints)
        {
            intersectPoints.Clear();
            double x = linePoint2.X - linePoint1.X;
            double y = linePoint2.Y - linePoint1.Y;
            double num = x * x + y * y;
            double x1 = 2 * (x * (linePoint1.X - centre.X) + y * (linePoint1.Y - centre.Y));
            double y1 = (linePoint1.X - centre.X) * (linePoint1.X - centre.X);
            y1 = y1 + (linePoint1.Y - centre.Y) * (linePoint1.Y - centre.Y);
            y1 = y1 - radius * radius;
            double[] numArray = Intersect.QuadraticEquation(num, x1, y1);
            if (!double.IsNaN(numArray[0]))
            {
                intersectPoints.Add(new Point(linePoint1.X + numArray[0] * x, linePoint1.Y + numArray[0] * y));
            }
            if (!double.IsNaN(numArray[1]))
            {
                intersectPoints.Add(new Point(linePoint1.X + numArray[1] * x, linePoint1.Y + numArray[1] * y));
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineToLineSegment2D(Point linePoint1, Point linePoint2, Point lineSegmentPoint1, Point lineSegmentPoint2, ref List<Point> intersectPoints)
        {
            Line line = new Line(linePoint1, linePoint2);
            return Intersect.IntersectLineToLineSegment2D(line, new LineSegment(lineSegmentPoint1, lineSegmentPoint2), ref intersectPoints);
        }

        public static bool IntersectLineToLineSegment2D(Line line, LineSegment lineSegment, ref List<Point> intersectPoints)
        {
            LineSegment lineSegment1 = Intersection.LineToLine(line, new Line(lineSegment));
            intersectPoints.Clear();
            if (lineSegment1 == null)
            {
                Point point = Projection.PointToLine(lineSegment.Point1, line);
                if (Geo.CompareTwoPoints2D(point, lineSegment.Point1))
                {
                    intersectPoints.Add(point);
                }
                point = Projection.PointToLine(lineSegment.Point2, line);
                if (Geo.CompareTwoPoints2D(point, lineSegment.Point2))
                {
                    intersectPoints.Add(point);
                }
            }
            else
            {
                if (!Geo.CompareTwoPoints2D(lineSegment1.Point1, lineSegment1.Point2))
                {
                    intersectPoints.Add(lineSegment1.Point1);
                    intersectPoints.Add(lineSegment1.Point2);
                }
                else
                {
                    intersectPoints.Add(lineSegment1.Point1);
                }
                for (int i = 0; i < intersectPoints.Count; i++)
                {
                    if (!Geo.IsPointInLineSegment2D(lineSegment.Point1, lineSegment.Point2, intersectPoints[i]))
                    {
                        intersectPoints.RemoveAt(i);
                        i--;
                    }
                }
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineToLineSegment3D(Point linePoint1, Point linePoint2, Point lineSegmentPoint1, Point lineSegmentPoint2, ref List<Point> intersectPoints)
        {
            Line line = new Line(linePoint1, linePoint2);
            return Intersect.IntersectLineToLineSegment3D(line, new LineSegment(lineSegmentPoint1, lineSegmentPoint2), ref intersectPoints);
        }

        public static bool IntersectLineToLineSegment3D(Line line, LineSegment lineSegment, ref List<Point> intersectPoints)
        {
            LineSegment lineSegment1 = Intersection.LineToLine(line, new Line(lineSegment));
            intersectPoints.Clear();
            if (lineSegment1 == null)
            {
                if (Geo.CompareTwoPoints3D(Projection.PointToLine(lineSegment.Point1, line), lineSegment.Point1))
                {
                    intersectPoints.Add(new Point(lineSegment.Point1));
                }
                if (Geo.CompareTwoPoints3D(Projection.PointToLine(lineSegment.Point2, line), lineSegment.Point2))
                {
                    intersectPoints.Add(new Point(lineSegment.Point2));
                }
            }
            else
            {
                if (!Geo.CompareTwoPoints3D(lineSegment1.Point1, lineSegment1.Point2))
                {
                    intersectPoints.Add(lineSegment1.Point1);
                    intersectPoints.Add(Geo.GetCenterPoint3D(lineSegment1.Point1, lineSegment1.Point2));
                    intersectPoints.Add(lineSegment1.Point2);
                }
                else
                {
                    intersectPoints.Add(lineSegment1.Point1);
                }
                for (int i = 0; i < intersectPoints.Count; i++)
                {
                    if (Geo.IsPointInLineSegment3D(lineSegment.Point1, lineSegment.Point2, intersectPoints[i]))
                    {
                        Point point = Projection.PointToLine(intersectPoints[i], line);
                        if (!Compare.GT(Geo.GetDistanceBeetveenTwoPoints3D(point, intersectPoints[i]), 50 * Constants.CS_EPSILON))
                        {
                            intersectPoints[i] = point;
                        }
                        else
                        {
                            intersectPoints.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        intersectPoints.RemoveAt(i);
                        i--;
                    }
                }
            }
            return intersectPoints.Count > 0;
        }

        public static Line IntersectPlaneToPlane(GeometricPlane plane1, GeometricPlane plane2)
        {
            Line line = null;
            Vector vector = new Vector(plane1.Normal.Y * plane2.Normal.Z - plane1.Normal.Z * plane2.Normal.Y, plane1.Normal.Z * plane2.Normal.X - plane1.Normal.X * plane2.Normal.Z, plane1.Normal.X * plane2.Normal.Y - plane1.Normal.Y * plane2.Normal.X);
            Vector vector1 = new Vector(plane2.Normal.Y * vector.Z - plane2.Normal.Z * vector.Y, plane2.Normal.Z * vector.X - plane2.Normal.X * vector.Z, plane2.Normal.X * vector.Y - plane2.Normal.Y * vector.X);
            double x = plane1.Normal.X * vector1.X + plane1.Normal.Y * vector1.Y + plane1.Normal.Z * vector1.Z;
            if (Math.Abs(x) > 0)
            {
                Vector vectorLineSegment = Geo.GetVectorLineSegment(plane1.Origin, plane2.Origin);
                double num = plane1.Normal.X * vectorLineSegment.X + plane1.Normal.Y * vectorLineSegment.Y + plane1.Normal.Z * vectorLineSegment.Z;
                num = num / x;
                Point origin = plane2.Origin + (num * vector1);
                line = new Line(origin, vector);
            }
            return line;
        }

        private static double[] QuadraticEquation(double operandA, double operandB, double operandC)
        {
            double[] numArray = new double[2];
            double num = operandB * operandB - 4 * operandA * operandC;
            if (Compare.LT(num, 0))
            {
                numArray[0] = double.NaN;
                numArray[1] = double.NaN;
            }
            else if (!Compare.EQ(num, 0))
            {
                numArray[0] = (-operandB + num) / (2 * operandA);
                numArray[1] = (-operandB - num) / (2 * operandA);
            }
            else
            {
                numArray[0] = (-operandB + num) / (2 * operandA);
                numArray[1] = double.NaN;
            }
            return numArray;
        }

        public class LinearEquationsWithUnknowns
        {
            public int CountOfUnknowns
            {
                get;
                private set;
            }

            public double[,] Data
            {
                get;
                set;
            }

            public double[] Result
            {
                get;
                private set;
            }

            public LinearEquationsWithUnknowns(int numberOfUnknowns)
            {
                this.CountOfUnknowns = numberOfUnknowns;
                this.Data = new double[numberOfUnknowns, numberOfUnknowns + 1];
                this.Result = new double[numberOfUnknowns];
            }

            private void AddLine(int line1, int line2)
            {
                if (line1 == line2)
                {
                    return;
                }
                for (int i = 0; i < this.CountOfUnknowns + 1; i++)
                {
                    this.Data[line1, i] = this.Data[line1, i] + this.Data[line2, i];
                }
            }

            private void ChangeLine(int line1, int line2)
            {
                if (line1 == this.CountOfUnknowns || line2 == this.CountOfUnknowns)
                {
                    return;
                }
                if (line1 == line2)
                {
                    return;
                }
                double[] data = new double[this.CountOfUnknowns + 1];
                for (int i = 0; i < this.CountOfUnknowns + 1; i++)
                {
                    data[i] = this.Data[line2, i];
                }
                for (int j = 0; j < this.CountOfUnknowns + 1; j++)
                {
                    this.Data[line2, j] = this.Data[line1, j];
                }
                for (int k = 0; k < this.CountOfUnknowns + 1; k++)
                {
                    this.Data[line1, k] = data[k];
                }
            }

            private void GetNonZero(int unknown)
            {
                int num = unknown;
                int num1 = unknown;
                while (num1 < this.CountOfUnknowns && this.Data[num1, unknown] == 0)
                {
                    num1++;
                }
                this.ChangeLine(num1, num);
            }

            private void MultiplyLine(int line, double constant)
            {
                for (int i = 0; i < this.CountOfUnknowns + 1; i++)
                {
                    this.Data[line, i] = this.Data[line, i] * constant;
                }
            }

            public void Sum()
            {
                for (int i = 0; i < this.CountOfUnknowns - 1; i++)
                {
                    this.TerminateUnknown(i);
                }
                for (int j = this.CountOfUnknowns - 1; j >= 0; j--)
                {
                    double num = this.SummaryUnknovn(j, j);
                    this.Result[j] = (this.Data[j, this.CountOfUnknowns] - num) / this.Data[j, j];
                }
            }

            private double SummaryUnknovn(int unknovn, int line)
            {
                if (line == this.CountOfUnknowns - 1)
                {
                    return 0;
                }
                return this.SummaryUnknovn(unknovn, line + 1) + this.Data[unknovn, line + 1] * this.Result[line + 1];
            }

            private void TerminateUnknown(int unknown)
            {
                this.GetNonZero(unknown);
                double data = 0;
                for (int i = unknown + 1; i < this.CountOfUnknowns; i++)
                {
                    if (this.Data[i, unknown] != 0)
                    {
                        data = this.Data[unknown, unknown] / -this.Data[i, unknown];
                        this.MultiplyLine(i, data);
                        this.AddLine(i, unknown);
                    }
                }
            }
        }

        public class LineSolidIntersect
        {
            private readonly Tekla.Structures.Model.Model model;

            private List<Intersect.LineSolidIntersect.IntersectPointResult> intersectResults;

            private List<Intersect.SummaryPlane> partPlanes;

            private List<Point> results;

            public LineSolidIntersect(Tekla.Structures.Model.Model actualModel)
            {
                this.model = actualModel;
            }

            private bool IsInLine(List<Point> polygonPoints, Point testPoint)
            {
                for (int i = 0; i < polygonPoints.Count; i++)
                {
                    int count = (i + 1) % polygonPoints.Count;
                    if (Geo.IsPointInLineSegment3D(polygonPoints[i], polygonPoints[count], testPoint))
                    {
                        return true;
                    }
                }
                return false;
            }

            private bool IsNear(ref Point point1, ref Point point2, Point beginLinePoint)
            {
                double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point1, beginLinePoint);
                if (Geo.GetDistanceBeetveenTwoPoints3D(point2, beginLinePoint) >= distanceBeetveenTwoPoints3D)
                {
                    return false;
                }
                Point point = point2;
                point2 = point1;
                point1 = point;
                return true;
            }

            public List<Point> LineIntersect(List<List<Point>> partFacesPoints, Point beginLinePoint, Point endLinePoint)
            {
                this.partPlanes = new List<Intersect.SummaryPlane>();
                this.intersectResults = new List<Intersect.LineSolidIntersect.IntersectPointResult>();
                this.results = new List<Point>();
                this.partPlanes = Intersect.SummaryPlane.CreateSummaryPlanes(partFacesPoints);
                for (int i = 0; i < this.partPlanes.Count; i++)
                {
                    Point point = this.partPlanes[i].IntersectLineSegment(beginLinePoint, endLinePoint);
                    if (!double.IsNaN(point.X) && !double.IsNaN(point.Y) && !double.IsNaN(point.Z) && this.TestIsPointValid(this.partPlanes[i].Polygon, point))
                    {
                        bool flag = false;
                        foreach (Intersect.SummaryPlane samePlane in this.partPlanes[i].SamePlanes)
                        {
                            if (!this.TestIsPointValid(samePlane.Polygon, point))
                            {
                                continue;
                            }
                            flag = true;
                            break;
                        }
                        if (!flag)
                        {
                            Intersect.LineSolidIntersect.IntersectPointResult intersectPointResult = new Intersect.LineSolidIntersect.IntersectPointResult(point, i);

                            intersectPointResult.IsInLine = this.IsInLine(this.partPlanes[i].Polygon, intersectPointResult.Point);

                            this.intersectResults.Add(intersectPointResult);
                        }
                    }
                }
                this.UniquePoints();
                for (int j = 0; j < this.intersectResults.Count; j++)
                {
                    if (this.intersectResults[j].IsInLine)
                    {
                        this.results.Add(this.intersectResults[j].Point);
                    }
                    else if (this.intersectResults[j].ParentFaces.Count == 1)
                    {
                        this.results.Add(this.intersectResults[j].Point);
                    }
                }
                this.SortResults(beginLinePoint);
                return this.results;
            }

            public List<Point> LineIntersect(Part partToIntersect, Point beginLinePoint, Point endLinePoint, Tekla.Structures.Model.Solid.SolidCreationTypeEnum solidType = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                return this.LineIntersect(Intersect.GetPartFacesPoints(partToIntersect, solidType), beginLinePoint, endLinePoint);
            }

            public List<Point> LineIntersect(Part partToIntersect, LineSegment lineSegment, Tekla.Structures.Model.Solid.SolidCreationTypeEnum solidType = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                return this.LineIntersect(partToIntersect, lineSegment.Point1, lineSegment.Point2, solidType);
            }

            public List<Point> LineIntersect(Part partToIntersect, Line line, Tekla.Structures.Model.Solid.SolidCreationTypeEnum solidType = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                Vector vector = new Vector(line.Direction);
                vector.Normalize();
                double num = 1000;
                Point point = new Point(line.Origin.X + vector.X * num, line.Origin.Y + vector.Y * num, line.Origin.Z + vector.Z * num);
                Point point1 = new Point(line.Origin.X - vector.X * num, line.Origin.Y - vector.Y * num, line.Origin.Z - vector.Z * num);
                return this.LineIntersect(partToIntersect, point1, point, solidType);
            }

            private void SortResults(Point beginLinePoint)
            {
                Point[] item = new Point[this.results.Count];
                for (int i = 0; i < this.results.Count; i++)
                {
                    item[i] = this.results[i];
                }
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int j = 0; j < this.results.Count; j++)
                    {
                        if (j + 1 < this.results.Count && this.IsNear(ref item[j], ref item[j + 1], beginLinePoint))
                        {
                            flag = true;
                        }
                    }
                }
                int count = this.results.Count;
                this.results.Clear();
                for (int k = 0; k < count; k++)
                {
                    this.results.Add(item[k]);
                }
            }

            private bool TestIsPointValid(List<Point> polygonPoints, Point testPoint)
            {
                bool flag = false;
                if (this.IsInLine(polygonPoints, testPoint))
                {
                    return !flag;
                }
                SetPlane setPlane = new SetPlane(this.model);
                foreach (Point polygonPoint in polygonPoints)
                {
                    setPlane.AddPoints(new Point[] { polygonPoint });
                }
                setPlane.AddPoints(new Point[] { testPoint });
                Vector vectorLineSegment = Geo.GetVectorLineSegment(polygonPoints[1], polygonPoints[0]);
                Vector vector = Geo.GetVectorLineSegment(polygonPoints[2], polygonPoints[0]);
                setPlane.Begin(polygonPoints[0], vectorLineSegment, vector);
                try
                {
                    flag = Geo.IsPointInsidePolygon2D(polygonPoints, testPoint);
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
                setPlane.End();
                return flag;
            }

            private void UniquePoints()
            {
                for (int i = 0; i < this.intersectResults.Count; i++)
                {
                    for (int j = 0; j < this.intersectResults.Count; j++)
                    {
                        if (j != i && Geo.CompareTwoPoints3D(this.intersectResults[i].Point, this.intersectResults[j].Point))
                        {
                            if (this.intersectResults[i].IsInLine || this.intersectResults[j].IsInLine)
                            {
                                this.intersectResults[i].IsInLine = true;
                            }
                            this.intersectResults[i].ParentFaces.AddRange(this.intersectResults[j].ParentFaces);
                            this.intersectResults.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }

            private class IntersectPointResult
            {
                public bool IsInLine
                {
                    get;
                    set;
                }

                public List<int> ParentFaces
                {
                    get;
                    set;
                }

                public Point Point
                {
                    get;
                    set;
                }

                public IntersectPointResult(Point resultPoint, int parentface)
                {
                    this.Point = resultPoint;
                    this.IsInLine = false;
                    this.ParentFaces = new List<int>()
                    {
                        parentface
                    };
                }
            }
        }

        public class PlaneSolidIntersect
        {
            private Intersect.SummaryPlane intersectPlane;

            private Tekla.Structures.Model.Model model;

            private List<Intersect.SummaryPlane> partPlanes;

            private PolygonOperation po;

            public PlaneSolidIntersect(Tekla.Structures.Model.Model actualModel)
            {
                this.model = actualModel;
            }

            public ArrayList PlaneIntersect(Part partToIntersect, Point point1, Point point2, Point point3, Tekla.Structures.Model.Solid.SolidCreationTypeEnum typeSolid = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                List<List<Point>> lists = this.PlaneIntersectList(partToIntersect, point1, point2, point3, typeSolid);
                ArrayList arrayLists = new ArrayList(lists.Count);
                foreach (List<Point> points in lists)
                {
                    if (points.Count <= 0)
                    {
                        continue;
                    }
                    arrayLists.Add(new ArrayList(points));
                }
                return arrayLists;
            }

            public ArrayList PlaneIntersect(List<List<Point>> partToIntersectFaces, Point point1, Point point2, Point point3)
            {
                List<List<Point>> lists = this.PlaneIntersectList(partToIntersectFaces, point1, point2, point3);
                ArrayList arrayLists = new ArrayList(lists.Count);
                foreach (List<Point> points in lists)
                {
                    if (points.Count <= 0)
                    {
                        continue;
                    }
                    arrayLists.Add(new ArrayList(points));
                }
                return arrayLists;
            }

            public List<List<Point>> PlaneIntersectList(Part partToIntersect, Point point1, Point point2, Point point3, Tekla.Structures.Model.Solid.SolidCreationTypeEnum typeSolid = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                return this.PlaneIntersectList(Intersect.GetPartFacesPoints(partToIntersect, typeSolid), point1, point2, point3);
            }

            public List<List<Point>> PlaneIntersectList(List<List<Point>> partToIntersectFaces, Point point1, Point point2, Point point3)
            {
                List<List<List<Point>>> lists = this.PlaneIntersectPolygonsWithHolesList(partToIntersectFaces, point1, point2, point3);
                List<List<Point>> lists1 = new List<List<Point>>();
                for (int i = lists.Count - 1; i >= 0; i--)
                {
                    for (int j = 0; j < lists[i].Count; j++)
                    {
                        if (j != 0)
                        {
                            lists1.Add(lists[i][j]);
                        }
                        else
                        {
                            lists1.Insert(0, lists[i][j]);
                        }
                    }
                }
                return lists1;
            }

            public ArrayList PlaneIntersectPolygonsWithHoles(Part partToIntersect, Point point1, Point point2, Point point3, Tekla.Structures.Model.Solid.SolidCreationTypeEnum typeSolid = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                List<List<Point>> partFacesPoints = Intersect.GetPartFacesPoints(partToIntersect, typeSolid);
                List<List<List<Point>>> lists = this.PlaneIntersectPolygonsWithHolesList(partFacesPoints, point1, point2, point3);
                ArrayList arrayLists = new ArrayList(lists.Count);
                foreach (List<List<Point>> lists1 in lists)
                {
                    ArrayList arrayLists1 = new ArrayList(lists1.Count);
                    foreach (List<Point> points in lists1)
                    {
                        if (points.Count <= 0)
                        {
                            continue;
                        }
                        arrayLists1.Add(new ArrayList(points));
                    }
                    arrayLists.Add(arrayLists1);
                }
                return arrayLists;
            }

            public ArrayList PlaneIntersectPolygonsWithHoles(List<List<Point>> partToIntersectFaces, Point point1, Point point2, Point point3)
            {
                List<List<List<Point>>> lists = this.PlaneIntersectPolygonsWithHolesList(partToIntersectFaces, point1, point2, point3);
                ArrayList arrayLists = new ArrayList(lists.Count);
                foreach (List<List<Point>> lists1 in lists)
                {
                    ArrayList arrayLists1 = new ArrayList(lists1.Count);
                    foreach (List<Point> points in lists1)
                    {
                        if (points.Count <= 0)
                        {
                            continue;
                        }
                        arrayLists1.Add(new ArrayList(points));
                    }
                    arrayLists.Add(arrayLists1);
                }
                return arrayLists;
            }

            public List<List<List<Point>>> PlaneIntersectPolygonsWithHolesList(Part partToIntersect, Point point1, Point point2, Point point3, Tekla.Structures.Model.Solid.SolidCreationTypeEnum typeSolid = (Tekla.Structures.Model.Solid.SolidCreationTypeEnum)2)
            {
                return this.PlaneIntersectPolygonsWithHolesList(Intersect.GetPartFacesPoints(partToIntersect, typeSolid), point1, point2, point3);
            }

            public List<List<List<Point>>> PlaneIntersectPolygonsWithHolesList(List<List<Point>> partToIntersectFaces, Point point1, Point point2, Point point3)
            {
                List<List<List<Point>>> lists = new List<List<List<Point>>>();
                SetPlane setPlane = new SetPlane(this.model);
                setPlane.AddPoints(new Point[] { point1, point2, point3 });
                setPlane.AddList(partToIntersectFaces);
                Vector vectorLineSegment = Geo.GetVectorLineSegment(point1, point2);
                Vector vector = vectorLineSegment.Cross(Geo.GetVectorLineSegment(point1, point3));
                setPlane.Begin(point1, vectorLineSegment, vector);
                try
                {
                    this.model = new Tekla.Structures.Model.Model();
                    this.po = new PolygonOperation();
                    this.partPlanes = new List<Intersect.SummaryPlane>();
                    List<Point> points = new List<Point>(3)
                    {
                        new Point(point1),
                        new Point(point2),
                        new Point(point3)
                    };
                    this.intersectPlane = new Intersect.SummaryPlane(points, this.model);
                    this.partPlanes = Intersect.SummaryPlane.CreateSummaryPlanes(partToIntersectFaces);
                    this.UpdateSummaryPlanes();
                    List<LineSegment> lineSegments = new List<LineSegment>(10);
                    List<List<Point>> lists1 = new List<List<Point>>();
                    foreach (Intersect.SummaryPlane partPlane in this.partPlanes)
                    {
                        if (partPlane.IsIntersectFace)
                        {
                            lists1.Add(new List<Point>(partPlane.Polygon));
                        }
                        else
                        {
                            Line line = this.intersectPlane.IntersectPlane(partPlane);
                            if (line == null)
                            {
                                continue;
                            }
                            lineSegments.AddRange(partPlane.IntersectLine(line));
                        }
                    }
                    List<Polygon> polygons = new List<Polygon>(lists1.Count);
                    foreach (List<Point> list in lists1)
                    {
                        polygons.Add(Geo.ConvertPolygonFromListPoint(list));
                        for (int i = 0; i < list.Count; i++)
                        {
                            int count = (i + 1) % list.Count;
                            lineSegments.Add(new LineSegment(new Point(list[i]), new Point(list[count])));
                        }
                    }
                    SetPlane setPlane1 = new SetPlane(this.model);
                    setPlane1.AddList(polygons);
                    this.intersectPlane.Begin(setPlane1);
                    for (int j = 0; j < polygons.Count; j++)
                    {
                        for (int k = j + 1; k < polygons.Count; k++)
                        {
                            if (this.po.CsCmpTwoPolygons(polygons[j], polygons[k]) == PolygonOperation.ComparePolygonTypeEnum.POL1_COLLIDE_POL2)
                            {
                                List<PolygonOperation.PolygonWithHoles> polygonWithHoles = this.po.PolygonOperations(polygons[j], polygons[k], PolygonOperation.PolygonOperationsEnum.UNION);
                                if (polygonWithHoles.Count == 1)
                                {
                                    polygons.RemoveAt(k);
                                    polygons[j] = polygonWithHoles[0].contourPolygon;
                                    j--;
                                    if (polygonWithHoles[0].innerPolygons.Count <= 0)
                                    {
                                        break;
                                    }
                                    polygons.AddRange(polygonWithHoles[0].innerPolygons);
                                    break;
                                }
                            }
                        }
                    }
                    setPlane1.RemoveAllPolygons();
                    setPlane1.AddList(polygons);
                    setPlane1.End();
                    List<Polygon> polygons1 = new List<Polygon>();
                    (new PolygonOperation.CreatePolygons()).Create(lineSegments, ref polygons1);
                    foreach (Polygon polygon in polygons1)
                    {
                        PolygonOperation.RemoveUnnecessaryPolygonPoints(polygon);
                    }
                    polygons1.AddRange(polygons);
                    foreach (List<Polygon> polygons2 in this.PreparePolygon(polygons1))
                    {
                        List<List<Point>> lists2 = new List<List<Point>>();
                        foreach (Polygon polygon1 in polygons2)
                        {
                            List<Point> points1 = new List<Point>();
                            foreach (Point point in polygon1.Points)
                            {
                                points1.Add(point);
                                setPlane.AddPoints(new Point[] { point });
                            }
                            lists2.Add(points1);
                        }
                        lists.Add(lists2);
                    }
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
                setPlane.End();
                return lists;
            }

            private List<List<Polygon>> PreparePolygon(List<Polygon> polygons)
            {
                List<List<Polygon>> lists = new List<List<Polygon>>();
                SetPlane setPlane = new SetPlane(this.model);
                setPlane.AddPolygons(polygons.ToArray());
                setPlane.Begin(this.intersectPlane.Polygon[0], this.intersectPlane.VectorS, this.intersectPlane.VectorT);
                try
                {
                    foreach (Polygon polygon in polygons)
                    {
                        if (polygon.Points.Count != 2 || !Geo.CompareTwoPoints3D(polygon.Points[0] as Point, polygon.Points[1] as Point))
                        {
                            continue;
                        }
                        polygon.Points.RemoveAt(1);
                    }
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        for (int j = i + 1; j < polygons.Count; j++)
                        {
                            PolygonOperation.ComparePolygonTypeEnum comparePolygonTypeEnum = this.po.CsCmpTwoPolygons(polygons[i], polygons[j]);
                            if (comparePolygonTypeEnum == PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2)
                            {
                                polygons.RemoveAt(j);
                                j--;
                            }
                            else if (comparePolygonTypeEnum == PolygonOperation.ComparePolygonTypeEnum.POL1_COLLIDE_POL2)
                            {
                                List<PolygonOperation.PolygonWithHoles> polygonWithHoles = this.po.PolygonOperations(polygons[i], polygons[j], PolygonOperation.PolygonOperationsEnum.UNION);
                                if (polygonWithHoles.Count == 1)
                                {
                                    polygons[i] = polygonWithHoles[0].contourPolygon;
                                    polygons.RemoveAt(j);
                                    i--;
                                    break;
                                }
                            }
                            else if (comparePolygonTypeEnum == PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                            {
                                Polygon item = polygons[i];
                                polygons[i] = polygons[j];
                                polygons[j] = item;
                                i--;
                                break;
                            }
                        }
                    }
                    lists = this.SplitPolygonHoles(polygons);
                    setPlane.RemoveAllPolygons();
                    setPlane.AddPolygons(polygons.ToArray());
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
                setPlane.End();
                return lists;
            }

            private List<List<Polygon>> SplitPolygonHoles(List<Polygon> polygons)
            {
                List<List<Polygon>> lists = new List<List<Polygon>>();
                if (polygons.Count > 1)
                {
                    bool polygonOrientation = PolygonOperation.GetPolygonOrientation(polygons[0]);
                    for (int i = 0; i < polygons.Count; i++)
                    {
                        bool flag = true;
                        bool polygonOrientation1 = PolygonOperation.GetPolygonOrientation(polygons[i]);
                        int num = 0;
                        while (num < polygons.Count)
                        {
                            if (i == num || this.po.CsCmpTwoPolygons(polygons[i], polygons[num]) != PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                            {
                                num++;
                            }
                            else
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag && polygonOrientation1 != polygonOrientation || !flag && polygonOrientation1 == polygonOrientation)
                        {
                            polygons[i].Points.Reverse();
                        }
                        if (flag)
                        {
                            lists.Add(new List<Polygon>()
                            {
                                polygons[i]
                            });
                        }
                    }
                    foreach (Polygon polygon in polygons)
                    {
                        foreach (List<Polygon> list in lists)
                        {
                            if (polygon == list[0] || this.po.CsCmpTwoPolygons(polygon, list[0]) != PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                            {
                                continue;
                            }
                            list.Add(polygon);
                            break;
                        }
                    }
                }
                else if (polygons.Count == 1)
                {
                    lists.Add(new List<Polygon>()
                    {
                        polygons[0]
                    });
                }
                return lists;
            }

            private void UpdateSummaryPlanes()
            {
                SetPlane setPlane = new SetPlane(this.model);
                setPlane.AddList(this.intersectPlane.Polygon);
                foreach (Intersect.SummaryPlane partPlane in this.partPlanes)
                {
                    setPlane.AddList(partPlane.Polygon);
                }
                setPlane.Begin(this.intersectPlane.Polygon[0], this.intersectPlane.VectorT, this.intersectPlane.VectorS);
                try
                {
                    foreach (Intersect.SummaryPlane summaryPlane in this.partPlanes)
                    {
                        foreach (Point polygon in summaryPlane.Polygon)
                        {
                            if (!Compare.NE(polygon.Z, 0))
                            {
                                continue;
                            }
                            summaryPlane.IsIntersectFace = false;
                            break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
                setPlane.End();
            }
        }

        private class SummaryPlane
        {
            private readonly Tekla.Structures.Model.Model model;

            public bool IsIntersectFace
            {
                get;
                set;
            }

            private Vector Normal
            {
                get;
                set;
            }

            public List<Point> Polygon
            {
                get;
                private set;
            }

            public List<Intersect.SummaryPlane> SamePlanes
            {
                get;
                set;
            }

            public Vector VectorS
            {
                get;
                set;
            }

            public Vector VectorT
            {
                get;
                set;
            }

            public SummaryPlane(List<Point> facePolygon, Tekla.Structures.Model.Model actualModel)
            {
                this.model = actualModel;
                this.IsIntersectFace = true;
                this.SamePlanes = new List<Intersect.SummaryPlane>();
                this.Polygon = new List<Point>(facePolygon);
                this.VectorS = Geo.GetVectorLineSegment(this.Polygon[1], this.Polygon[0]).GetNormal();
                this.VectorT = Geo.GetVectorLineSegment(this.Polygon[2], this.Polygon[0]).GetNormal();
                for (int i = 1; Compare.LT(this.VectorS.GetLength(), 0.5) && i < this.Polygon.Count; i++)
                {
                    this.VectorS = Geo.GetVectorLineSegment(this.Polygon[i], this.Polygon[i - 1]).GetNormal();
                }
                Vector vector = new Vector(Math.Abs(this.VectorS.X), Math.Abs(this.VectorS.Y), Math.Abs(this.VectorS.Z));
                Vector vector1 = new Vector(Math.Abs(this.VectorT.X), Math.Abs(this.VectorT.Y), Math.Abs(this.VectorT.Z));
                bool flag = (Geo.CompareTwoPoints3D(vector, vector1) ? true : Compare.LT(vector1.GetLength(), 0.5));
                for (int j = 1; j < this.Polygon.Count && flag; j++)
                {
                    this.VectorT = Geo.GetVectorLineSegment(this.Polygon[j], this.Polygon[j - 1]).GetNormal();
                    vector1 = new Vector(Math.Abs(this.VectorT.X), Math.Abs(this.VectorT.Y), Math.Abs(this.VectorT.Z));
                    flag = (Geo.CompareTwoPoints3D(vector, vector1) ? true : Compare.LT(vector1.GetLength(), 0.5));
                }
                this.Normal = new Vector(this.VectorT.Y * this.VectorS.Z - this.VectorT.Z * this.VectorS.Y, this.VectorT.Z * this.VectorS.X - this.VectorT.X * this.VectorS.Z, this.VectorT.X * this.VectorS.Y - this.VectorT.Y * this.VectorS.X);
            }

            public void Begin(SetPlane setPlane)
            {
                setPlane.Begin(this.Polygon[0], this.VectorS, this.VectorT);
            }

            private static void ComparePlanes(List<Intersect.SummaryPlane> planes, int index1, PolygonOperation po)
            {
                try
                {
                    for (int i = index1 + 1; i < planes.Count; i++)
                    {
                        bool flag = true;
                        foreach (Point polygon in planes[i].Polygon)
                        {
                            if (!Compare.NE(polygon.Z, 0))
                            {
                                continue;
                            }
                            flag = false;
                            break;
                        }
                        if (flag)
                        {
                            Polygon polygon1 = Geo.ConvertPolygonFromListPoint(planes[index1].Polygon);
                            Polygon polygon2 = Geo.ConvertPolygonFromListPoint(planes[i].Polygon);
                            PolygonOperation.ComparePolygonTypeEnum comparePolygonTypeEnum = po.CsCmpTwoPolygons(polygon1, polygon2);
                            if (comparePolygonTypeEnum == PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1)
                            {
                                planes[index1].SamePlanes.Add(planes[i]);
                                planes.RemoveAt(i);
                                i--;
                            }
                            else if (comparePolygonTypeEnum == PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                            {
                                planes[i].SamePlanes.Add(planes[index1]);
                                planes.RemoveAt(index1);
                                index1--;
                                break;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    exception.ToString();
                }
            }

            private static List<Intersect.SummaryPlane> CreatePlanes(List<List<Point>> faces)
            {
                Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
                PolygonOperation polygonOperation = new PolygonOperation();
                List<Intersect.SummaryPlane> summaryPlanes = new List<Intersect.SummaryPlane>();
                foreach (List<Point> face in faces)
                {
                    summaryPlanes.Add(new Intersect.SummaryPlane(face, model));
                }
                for (int i = 0; i < summaryPlanes.Count; i++)
                {
                    SetPlane setPlane = new SetPlane(model);
                    setPlane.AddPoints(summaryPlanes[i].Polygon.ToArray());
                    for (int j = i + 1; j < summaryPlanes.Count; j++)
                    {
                        setPlane.AddPoints(summaryPlanes[j].Polygon.ToArray());
                    }
                    setPlane.Begin(summaryPlanes[i].Polygon[0], summaryPlanes[i].VectorT, summaryPlanes[i].VectorS);
                    Intersect.SummaryPlane.ComparePlanes(summaryPlanes, i, polygonOperation);
                    setPlane.End();
                }
                return summaryPlanes;
            }

            public static List<Intersect.SummaryPlane> CreateSummaryPlanes(List<List<Point>> faces)
            {
                List<List<Point>> lists = new List<List<Point>>(faces.Count);
                foreach (List<Point> face in faces)
                {
                    List<Point> points = new List<Point>();
                    foreach (Point point in face)
                    {
                        points.Add(new Point(point));
                    }
                    if (points.Count <= 0)
                    {
                        continue;
                    }
                    lists.Add(points);
                }
                Intersect.SummaryPlane.RemoveLinesInFaces(lists);
                return Intersect.SummaryPlane.CreatePlanes(lists);
            }

            public List<LineSegment> IntersectLine(Line line)
            {
                List<Point> points = new List<Point>();
                List<LineSegment> lineSegments = new List<LineSegment>();
                for (int i = 0; i < this.Polygon.Count; i++)
                {
                    int count = (i + 1) % this.Polygon.Count;
                    List<Point> points1 = new List<Point>();
                    Intersect.IntersectLineToLineSegment3D(line, new LineSegment(this.Polygon[i], this.Polygon[count]), ref points1);
                    points.AddRange(points1);
                }
                points = this.SortPoints(points);
                if (points.Count > 0)
                {
                    SetPlane setPlane = new SetPlane(this.model);
                    setPlane.AddPoints(points.ToArray());
                    setPlane.AddPoints(this.Polygon.ToArray());
                    foreach (Intersect.SummaryPlane samePlane in this.SamePlanes)
                    {
                        setPlane.AddPoints(samePlane.Polygon.ToArray());
                    }
                    setPlane.Begin(this.Polygon[0], this.VectorT, this.VectorS);
                    try
                    {
                        if (points.Count != 1)
                        {
                            for (int j = 0; j < points.Count - 1; j++)
                            {
                                int num = j + 1;
                                if (Geo.IsPointInsidePolygon2D(this.Polygon, Geo.GetCenterPoint2D(points[j], points[num]), true))
                                {
                                    lineSegments.Add(new LineSegment(new Point(points[j]), new Point(points[num])));
                                }
                            }
                            for (int k = 0; k < lineSegments.Count - 1; k++)
                            {
                                int num1 = k + 1;
                                if (Geo.CompareTwoPoints3D(lineSegments[k].Point2, lineSegments[num1].Point1))
                                {
                                    lineSegments[k].Point2 = lineSegments[num1].Point2;
                                    lineSegments.RemoveAt(num1);
                                    k--;
                                }
                            }
                            foreach (Intersect.SummaryPlane summaryPlane in this.SamePlanes)
                            {
                                List<LineSegment> lineSegments1 = new List<LineSegment>();
                                foreach (LineSegment lineSegment in lineSegments)
                                {
                                    lineSegments1.AddRange(Geo.CutLineByPolygon2D(lineSegment, summaryPlane.Polygon, false));
                                }
                                lineSegments = lineSegments1;
                            }
                        }
                        else
                        {
                            bool flag = true;
                            foreach (Intersect.SummaryPlane samePlane1 in this.SamePlanes)
                            {
                                if (!Geo.IsPointInsidePolygon2D(samePlane1.Polygon, points[0]))
                                {
                                    continue;
                                }
                                flag = false;
                                break;
                            }
                            if (flag)
                            {
                                lineSegments.Add(new LineSegment(points[0], new Point(points[0])));
                            }
                        }
                        setPlane.RemoveAllPoints();
                        setPlane.AddPoints(this.Polygon.ToArray());
                        foreach (Intersect.SummaryPlane summaryPlane1 in this.SamePlanes)
                        {
                            setPlane.AddList(summaryPlane1.Polygon);
                        }
                        foreach (LineSegment lineSegment1 in lineSegments)
                        {
                            Point[] point1 = new Point[] { lineSegment1.Point1, lineSegment1.Point2 };
                            setPlane.AddPoints(point1);
                        }
                    }
                    catch (Exception exception)
                    {
                        exception.ToString();
                    }
                    setPlane.End();
                }
                return lineSegments;
            }

            public Point IntersectLineSegment(Point startPoint, Point endPoint)
            {
                double[] x = new double[] { startPoint.X - endPoint.X, startPoint.Y - endPoint.Y, startPoint.Z - endPoint.Z };
                double[] numArray = x;
                double[] x1 = new double[] { this.Polygon[1].X - this.Polygon[0].X, this.Polygon[1].Y - this.Polygon[0].Y, this.Polygon[1].Z - this.Polygon[0].Z };
                double[] numArray1 = x1;
                double[] x2 = new double[] { this.Polygon[2].X - this.Polygon[0].X, this.Polygon[2].Y - this.Polygon[0].Y, this.Polygon[2].Z - this.Polygon[0].Z };
                double[] numArray2 = x2;
                double[] numArray3 = new double[] { numArray1[1] * numArray2[2] - numArray1[2] * numArray2[1], numArray1[2] * numArray2[0] - numArray1[0] * numArray2[2], numArray1[0] * numArray2[1] - numArray1[1] * numArray2[0] };
                double[] numArray4 = numArray3;
                double num = numArray4[0] * this.Polygon[0].X + numArray4[1] * this.Polygon[0].Y + numArray4[2] * this.Polygon[0].Z;
                double num1 = numArray4[0] * startPoint.X + numArray4[1] * startPoint.Y + numArray4[2] * startPoint.Z - num;
                double num2 = numArray4[0] * numArray[0] + numArray4[1] * numArray[1] + numArray4[2] * numArray[2];
                double num3 = -(num1 / num2);
                return new Point(startPoint.X + numArray[0] * num3, startPoint.Y + numArray[1] * num3, startPoint.Z + numArray[2] * num3);
            }

            public Line IntersectPlane(Intersect.SummaryPlane other)
            {
                GeometricPlane geometricPlane = new GeometricPlane(this.Polygon[0], this.Normal);
                GeometricPlane geometricPlane1 = new GeometricPlane(other.Polygon[0], other.Normal);
                return Intersect.IntersectPlaneToPlane(geometricPlane, geometricPlane1);
            }

            private static void RemoveLinesInFaces(List<List<Point>> faces)
            {
                for (int i = 0; i < faces.Count; i++)
                {
                    if (faces[i].Count > 3)
                    {
                        PolygonOperation.RemoveUnnecessaryPolygonPoints(faces[i]);
                    }
                    if (faces[i].Count < 3)
                    {
                        faces.RemoveAt(i);
                        i--;
                    }
                }
            }

            private List<Point> SortPoints(List<Point> points)
            {
                List<Point> points1 = new List<Point>();
                List<Point> points2 = new List<Point>(points);
                for (int i = 0; i < points2.Count; i++)
                {
                    for (int j = i + 1; j < points2.Count; j++)
                    {
                        if (Geo.CompareTwoPoints3D(points2[i], points2[j]))
                        {
                            points2.RemoveAt(j);
                            j--;
                        }
                    }
                }
                if (points2.Count < 3)
                {
                    points1 = points2;
                }
                else
                {
                    List<List<double>> lists = new List<List<double>>();
                    for (int k = 0; k < points2.Count; k++)
                    {
                        List<double> nums = new List<double>();
                        foreach (Point point in points2)
                        {
                            nums.Add(Geo.GetDistanceBeetveenTwoPoints3D(points2[k], point));
                        }
                        lists.Add(nums);
                    }
                    double item = 0;
                    int num = 0;
                    for (int l = 0; l < lists.Count; l++)
                    {
                        for (int m = 0; m < lists[l].Count; m++)
                        {
                            if (item < lists[l][m])
                            {
                                item = lists[l][m];
                                num = m;
                            }
                        }
                    }
                    points1.Add(new Point(points2[num]));
                    item = double.MaxValue;
                    double num1 = 0;
                    int num2 = 0;
                    for (int n = 1; n < points2.Count; n++)
                    {
                        num2 = 0;
                        for (int o = 0; o < points2.Count; o++)
                        {
                            if (lists[num][o] > num1 && lists[num][o] < item)
                            {
                                item = lists[num][o];
                                num2 = o;
                            }
                        }
                        num1 = item;
                        item = double.MaxValue;
                        points1.Add(new Point(points2[num2]));
                    }
                }
                return points1;
            }
        }
    }
}