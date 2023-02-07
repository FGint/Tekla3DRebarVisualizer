using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
    public static class Geo
    {
        private static Point comparePoint;

        static Geo()
        {
            Geo.comparePoint = new Point(0, 0, 0);
        }

        public static bool CompareTwoLinesSegment2D(LineSegment lineSegment1, LineSegment lineSegment2)
        {
            bool flag = (!Geo.CompareTwoPoints2D(lineSegment1.Point1, lineSegment2.Point1) ? false : Geo.CompareTwoPoints2D(lineSegment1.Point2, lineSegment2.Point2));
            if (!flag)
            {
                flag = (!Geo.CompareTwoPoints2D(lineSegment1.Point1, lineSegment2.Point2) ? false : Geo.CompareTwoPoints2D(lineSegment1.Point2, lineSegment2.Point1));
            }
            return flag;
        }

        public static bool CompareTwoLinesSegment3D(LineSegment lineSegment1, LineSegment lineSegment2)
        {
            bool flag = (!Geo.CompareTwoPoints3D(lineSegment1.Point1, lineSegment2.Point1) ? false : Geo.CompareTwoPoints3D(lineSegment1.Point2, lineSegment2.Point2));
            if (!flag)
            {
                flag = (!Geo.CompareTwoPoints3D(lineSegment1.Point1, lineSegment2.Point2) ? false : Geo.CompareTwoPoints3D(lineSegment1.Point2, lineSegment2.Point1));
            }
            return flag;
        }

        public static bool CompareTwoPoints2D(Point point1, Point point2)
        {
            if (!Compare.EQ(point1.X, point2.X))
            {
                return false;
            }
            return Compare.EQ(point1.Y, point2.Y);
        }

        public static bool CompareTwoPoints3D(Point point1, Point point2)
        {
            if (!Compare.EQ(point1.X, point2.X) || !Compare.EQ(point1.Y, point2.Y))
            {
                return false;
            }
            return Compare.EQ(point1.Z, point2.Z);
        }

        public static List<Point> ConvertListPointsFromPolygon(Polygon polygon)
        {
            List<Point> points = new List<Point>(polygon.Points.Count);
            foreach (Point point in polygon.Points)
            {
                points.Add(new Point(point));
            }
            return points;
        }

        public static Polygon ConvertPolygonFromListPoint(List<Point> polygonPoints)
        {
            Polygon polygon = new Polygon();
            foreach (Point polygonPoint in polygonPoints)
            {
                polygon.Points.Add(new Point(polygonPoint));
            }
            return polygon;
        }

        public static void ConvexHull(ref List<Point> convexHull, bool deleteCollinearPoints = true)
        {
            Geo.ConvexHullSummary(ref convexHull);
            if (convexHull.Count > 2 && deleteCollinearPoints)
            {
                for (int i = 0; i < convexHull.Count; i++)
                {
                    if (Geo.IsPointsCollinear(convexHull[i], convexHull[(i + 1) % convexHull.Count], convexHull[(i + 2) % convexHull.Count]))
                    {
                        convexHull.RemoveAt((i + 1) % convexHull.Count);
                        i--;
                    }
                }
            }
        }

        private static void ConvexHullSummary(ref List<Point> convexHull)
        {
            List<Point> points = new List<Point>();
            List<Point> points1 = new List<Point>();
            points1.AddRange(convexHull);
            List<Point> points2 = new List<Point>();
            Point item = points1[0];
            Point item1 = points1[0];
            List<Point> points3 = new List<Point>();
            if (convexHull.Count < 3)
            {
                return;
            }
            convexHull.Clear();
            for (int i = 1; i < points1.Count; i++)
            {
                if (Compare.GT(points1[i].Y, item.Y))
                {
                    item = points1[i];
                }
                if (Compare.LT(points1[i].Y, item1.Y))
                {
                    item1 = points1[i];
                }
            }
            convexHull.Add(item1);
            points.Add(item1);
            foreach (Point point1 in points1)
            {
                if (points.Contains(point1) || !Compare.EQ(point1.Y, item1.Y) || !Compare.LT(point1.X, item1.X))
                {
                    continue;
                }
                points3.Add(point1);
                points.Add(point1);
            }
            convexHull.AddRange(
                from point in points3
                orderby point.X descending
                select point);
            Point item2 = convexHull[convexHull.Count - 1];
            points3.Clear();
            Point point2 = item2;
            while (!Compare.EQ(point2.Y, item.Y))
            {
                foreach (Point point3 in points1)
                {
                    if (points.Contains(point3) && !Geo.CompareTwoPoints2D(point3, item) || Geo.FindRelativeAngle(item2, point2) > Geo.FindRelativeAngle(item2, point3) || !Compare.LE(Geo.FindRelativeAngle(item2, point3), 180))
                    {
                        continue;
                    }
                    point2 = point3;
                }
                if (Compare.EQ(point2.Y, item.Y))
                {
                    continue;
                }
                item2 = point2;
                convexHull.Add(item2);
                points.Add(item2);
            }
            foreach (Point point4 in points1)
            {
                if (points.Contains(point4) || !Compare.EQ(point4.Y, item.Y) || !Compare.LE(point4.X, item.X))
                {
                    continue;
                }
                points3.Add(point4);
                points.Add(point4);
            }
            convexHull.AddRange(
                from point in points3
                orderby point.X
                select point);
            points3.Clear();
            item2 = item1;
            foreach (Point point5 in points1)
            {
                if (points.Contains(point5) || !Compare.EQ(point5.Y, item1.Y) || !Compare.GT(point5.X, item1.X))
                {
                    continue;
                }
                points3.Add(point5);
                points.Add(point5);
            }
            points2.AddRange(
                from point in points3
                orderby point.X
                select point);
            points3.Clear();
            point2 = item1;
            if (points2.Count > 0)
            {
                item2 = points2[points2.Count - 1];
                point2 = item2;
            }
            while (!Compare.EQ(point2.Y, item.Y))
            {
                point2 = item;
                foreach (Point point6 in points1)
                {
                    if (points.Contains(point6) && !Geo.CompareTwoPoints2D(point6, item) || Geo.FindRelativeAngle(item2, point2) < Geo.FindRelativeAngle(item2, point6) || !Compare.LE(Geo.FindRelativeAngle(item2, point6), 180))
                    {
                        continue;
                    }
                    point2 = point6;
                }
                if (Compare.EQ(point2.Y, item.Y))
                {
                    continue;
                }
                item2 = point2;
                points2.Add(item2);
                points.Add(item2);
            }
            foreach (Point point7 in points1)
            {
                if (points.Contains(point7) && !Geo.CompareTwoPoints2D(point7, item) || !Compare.EQ(point7.Y, item.Y) || !Compare.GE(point7.X, item.X))
                {
                    continue;
                }
                points3.Add(point7);
                points.Add(point7);
            }
            points2.AddRange(
                from point in points3
                orderby point.X descending
                select point);
            if (points2.Count > 0)
            {
                points2.Reverse();
                points2.RemoveAt(0);
                convexHull.AddRange(points2);
            }
        }

        public static void CopyPointPosition(Point pointToCopyTo, Point orginalPoint)
        {
            pointToCopyTo.X = orginalPoint.X;
            pointToCopyTo.Y = orginalPoint.Y;
            pointToCopyTo.Z = orginalPoint.Z;
        }

        public static List<LineSegment> CutLineByPolygon2D(LineSegment lineSegment, List<Point> polygon, bool returnInside)
        {
            List<LineSegment> lineSegments = new List<LineSegment>();
            List<Point> points = new List<Point>(polygon.Count);
            foreach (Point point in polygon)
            {
                Point point1 = new Point(point)
                {
                    Z = lineSegment.Point1.Z
                };
                points.Add(point1);
            }
            List<Point> points1 = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                int num = i + 1;
                if (num >= points.Count)
                {
                    num = 0;
                }
                LineSegment lineSegment1 = new LineSegment(new Point(points[i]), new Point(points[num]));
                List<Point> points2 = new List<Point>();
                Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, lineSegment1, ref points2);
                points1.AddRange(points2);
            }
            if (Geo.IsPointInsidePolygon2D(points, lineSegment.Point1, true) == returnInside)
            {
                points1.Add(new Point(lineSegment.Point1));
            }
            if (Geo.IsPointInsidePolygon2D(points, lineSegment.Point2, true) == returnInside)
            {
                points1.Add(new Point(lineSegment.Point2));
            }
            Geo.comparePoint = lineSegment.Point1;
            points1.Sort(new Comparison<Point>(Geo.ICompareTwoPoints2D));
            Geo.comparePoint = new Point(0, 0, 0);
            for (int j = 0; j < points1.Count; j++)
            {
                for (int k = 0; k < points1.Count; k++)
                {
                    if (j != k && Geo.CompareTwoPoints2D(points1[j], points1[k]))
                    {
                        points1.RemoveAt(k);
                        k--;
                    }
                }
            }
            for (int l = 0; l < points1.Count; l++)
            {
                int num1 = l + 1;
                if (num1 < points1.Count)
                {
                    lineSegments.Add(new LineSegment(points1[l], points1[num1]));
                }
            }
            for (int m = 0; m < lineSegments.Count; m++)
            {
                if (Geo.IsPointInsidePolygon2D(points, Geo.GetCenterPoint2D(lineSegments[m].Point1, lineSegments[m].Point2), true) != returnInside)
                {
                    lineSegments.RemoveAt(m);
                    m--;
                }
            }
            for (int n = 0; n < lineSegments.Count; n++)
            {
                int num2 = n + 1;
                if (num2 < lineSegments.Count && Geo.CompareTwoPoints2D(lineSegments[n].Point2, lineSegments[num2].Point1))
                {
                    lineSegments[n].Point2 = lineSegments[num2].Point2;
                    lineSegments.RemoveAt(num2);
                    n--;
                }
            }
            return lineSegments;
        }

        public static List<LineSegment> CutLineByPolygon2D(LineSegment lineSegment, Polygon polygon, bool returnInside)
        {
            return Geo.CutLineByPolygon2D(lineSegment, Geo.ConvertListPointsFromPolygon(polygon), returnInside);
        }

        private static double DX(Point p1, Point p2)
        {
            return p2.X - p1.X;
        }

        private static double DY(Point p1, Point p2)
        {
            return p2.Y - p1.Y;
        }

        private static double DZ(Point p1, Point p2)
        {
            return p2.Z - p1.Z;
        }

        private static double FindRelativeAngle(Point point1, Point point2)
        {
            double rADTODEG = 0;
            double x = point2.X - point1.X;
            double y = point2.Y - point1.Y;
            if (!Compare.EQ(x, 0) || !Compare.EQ(y, 0))
            {
                rADTODEG = Constants.RAD_TO_DEG * Math.Atan2(y, x);
                if (Compare.LT(rADTODEG, 0))
                {
                    rADTODEG = rADTODEG + 360;
                }
            }
            else
            {
                rADTODEG = 0;
            }
            return rADTODEG;
        }

        public static double GetAngle3D(Point center, Point point1, Point point2)
        {
            if (Geo.CompareTwoPoints3D(point1, point2))
            {
                return 0;
            }
            Vector vectorLineSegment = Geo.GetVectorLineSegment(point1, center);
            Vector vector = Geo.GetVectorLineSegment(point2, center);
            double x = (vectorLineSegment.X * vector.X + vectorLineSegment.Y * vector.Y + vectorLineSegment.Z * vector.Z) / (vectorLineSegment.GetLength() * vector.GetLength());
            return Math.Acos(x);
        }

        public static Point GetCenterPoint2D(Point linePoint1, Point linePoint2)
        {
            Point point = new Point(linePoint1);
            Vector vectorLineSegment = Geo.GetVectorLineSegment(linePoint2, linePoint1);
            Point x = point;
            x.X = x.X + vectorLineSegment.X * 0.5;
            Point y = point;
            y.Y = y.Y + vectorLineSegment.Y * 0.5;
            point.Z = 0;
            return point;
        }

        public static Point GetCenterPoint3D(Point linePoint1, Point linePoint2)
        {
            Point point = new Point(linePoint1);
            Vector vectorLineSegment = Geo.GetVectorLineSegment(linePoint2, linePoint1);
            Point x = point;
            x.X = x.X + vectorLineSegment.X * 0.5;
            Point y = point;
            y.Y = y.Y + vectorLineSegment.Y * 0.5;
            Point z = point;
            z.Z = z.Z + vectorLineSegment.Z * 0.5;
            return point;
        }

        public static double GetDistanceBeetveenTwoPoints2D(Point point1, Point point2)
        {
            return Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y));
        }

        public static double GetDistanceBeetveenTwoPoints3D(Point point1, Point point2)
        {
            return Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y) + (point2.Z - point1.Z) * (point2.Z - point1.Z));
        }

        public static double GetDistanceBetweenTwoLineSegments2D(LineSegment line1, LineSegment line2)
        {
            return Geo.GetDistanceBetweenTwoLineSegments2D(line1.Point1, line1.Point2, line2.Point1, line2.Point2);
        }

        public static double GetDistanceBetweenTwoLineSegments2D(Point line11, Point line12, Point line21, Point line22)
        {
            double distanceBeetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(line11, line21);
            double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints2D(line11, line22);
            if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
            {
                distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
            }
            distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints2D(line12, line21);
            if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
            {
                distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
            }
            distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints2D(line12, line22);
            if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
            {
                distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
            }
            Line line = new Line(line11, line12);
            Line line1 = new Line(line21, line22);
            Point point = Projection.PointToLine(line11, line1);
            if (Geo.IsPointInLineSegment2D(line21, line22, point))
            {
                distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point, line11);
                if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
                {
                    distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
                }
            }
            point = Projection.PointToLine(line12, line1);
            if (Geo.IsPointInLineSegment2D(line21, line22, point))
            {
                distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point, line12);
                if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
                {
                    distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
                }
            }
            point = Projection.PointToLine(line21, line);
            if (Geo.IsPointInLineSegment2D(line11, line12, point))
            {
                distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point, line21);
                if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
                {
                    distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
                }
            }
            point = Projection.PointToLine(line22, line);
            if (Geo.IsPointInLineSegment2D(line11, line12, point))
            {
                distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point, line22);
                if (Compare.LT(distanceBeetveenTwoPoints3D, distanceBeetveenTwoPoints2D))
                {
                    distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints3D;
                }
            }
            List<Point> points = new List<Point>();
            if (Intersect.IntersectLineSegmentToLineSegment2D(line11, line12, line21, line22, ref points))
            {
                distanceBeetveenTwoPoints2D = 0;
            }
            return distanceBeetveenTwoPoints2D;
        }

        public static double GetDistanceBetweenTwoLineSegments3D(LineSegment line1, LineSegment line2)
        {
            return Geo.GetDistanceBetweenTwoLineSegments3D(line1.Point1, line1.Point2, line2.Point1, line2.Point2);
        }

        public static double GetDistanceBetweenTwoLineSegments3D(Point line11, Point line12, Point line21, Point line22)
        {
            double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(line11, line21);
            double num = Geo.GetDistanceBeetveenTwoPoints3D(line11, line22);
            if (Compare.LT(num, distanceBeetveenTwoPoints3D))
            {
                distanceBeetveenTwoPoints3D = num;
            }
            num = Geo.GetDistanceBeetveenTwoPoints3D(line12, line21);
            if (Compare.LT(num, distanceBeetveenTwoPoints3D))
            {
                distanceBeetveenTwoPoints3D = num;
            }
            num = Geo.GetDistanceBeetveenTwoPoints3D(line12, line22);
            if (Compare.LT(num, distanceBeetveenTwoPoints3D))
            {
                distanceBeetveenTwoPoints3D = num;
            }
            Line line = new Line(line11, line12);
            Line line1 = new Line(line21, line22);
            Point point = Projection.PointToLine(line11, line1);
            if (Geo.IsPointInLineSegment2D(line21, line22, point))
            {
                num = Geo.GetDistanceBeetveenTwoPoints3D(point, line11);
                if (Compare.LT(num, distanceBeetveenTwoPoints3D))
                {
                    distanceBeetveenTwoPoints3D = num;
                }
            }
            point = Projection.PointToLine(line12, line1);
            if (Geo.IsPointInLineSegment2D(line21, line22, point))
            {
                num = Geo.GetDistanceBeetveenTwoPoints3D(point, line12);
                if (Compare.LT(num, distanceBeetveenTwoPoints3D))
                {
                    distanceBeetveenTwoPoints3D = num;
                }
            }
            point = Projection.PointToLine(line21, line);
            if (Geo.IsPointInLineSegment2D(line11, line12, point))
            {
                num = Geo.GetDistanceBeetveenTwoPoints3D(point, line21);
                if (Compare.LT(num, distanceBeetveenTwoPoints3D))
                {
                    distanceBeetveenTwoPoints3D = num;
                }
            }
            point = Projection.PointToLine(line22, line);
            if (Geo.IsPointInLineSegment2D(line11, line12, point))
            {
                num = Geo.GetDistanceBeetveenTwoPoints3D(point, line22);
                if (Compare.LT(num, distanceBeetveenTwoPoints3D))
                {
                    distanceBeetveenTwoPoints3D = num;
                }
            }
            List<Point> points = new List<Point>();
            if (Intersect.IntersectLineSegmentToLineSegment3D(line11, line12, line21, line22, ref points))
            {
                distanceBeetveenTwoPoints3D = 0;
            }
            return distanceBeetveenTwoPoints3D;
        }

        public static double GetDistancePointFromLine(Point point, Line line)
        {
            return Geo.GetDistanceBeetveenTwoPoints3D(Projection.PointToLine(point, line), point);
        }

        public static double GetDistancePointFromLine(Point point, Point linePoint1, Point linePoint2)
        {
            return Geo.GetDistancePointFromLine(point, new Line(linePoint1, linePoint2));
        }

        public static Vector GetNormalVectorInPlane(Vector vector, Vector vectorToDefinePlaneAndDirection)
        {
            return vector.Cross(vectorToDefinePlaneAndDirection).Cross(vector);
        }

        public static Point GetPointByAngleDistance2D(Point linePoint1, Point linePoint2, double angle, double distance)
        {
            Vector vectorLineSegment = Geo.GetVectorLineSegment(linePoint2, linePoint1);
            vectorLineSegment.Z = 0;
            Vector vector = new Vector(-vectorLineSegment.Y, vectorLineSegment.X, 0);
            SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
            setPlane.Begin(new Point(linePoint1), vectorLineSegment, vector);
            double num = Math.Cos(angle) * distance;
            double num1 = Math.Sin(angle) * distance;
            Point point = new Point(num, num1, 0);
            setPlane.AddPoints(new Point[] { point });
            setPlane.End();
            return point;
        }

        public static Vector GetVectorLineSegment(LineSegment lineSegment)
        {
            return Geo.GetVectorLineSegment(lineSegment.Point1, lineSegment.Point2);
        }

        public static Vector GetVectorLineSegment(Point point1, Point point2)
        {
            return new Vector(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);
        }

        public static int ICompareTwoPoints2D(Point point1, Point point2)
        {
            double distanceBeetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(point1, Geo.comparePoint);
            double num = Geo.GetDistanceBeetveenTwoPoints2D(point2, Geo.comparePoint);
            if (Math.Abs(distanceBeetveenTwoPoints2D) < 1 && Math.Abs(num) < 1)
            {
                distanceBeetveenTwoPoints2D = distanceBeetveenTwoPoints2D * 100;
                num = num * 100;
            }
            return Convert.ToInt32(distanceBeetveenTwoPoints2D - num);
        }

        public static int ICompareTwoPoints3D(Point point1, Point point2)
        {
            double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point1, Geo.comparePoint);
            double num = Geo.GetDistanceBeetveenTwoPoints3D(point2, Geo.comparePoint);
            return Convert.ToInt32(distanceBeetveenTwoPoints3D - num);
        }

        public static bool IsPointInLineSegment2D(LineSegment lineSegment, Point testPoint)
        {
            return Geo.IsPointInLineSegment2D(lineSegment.Point1, lineSegment.Point2, testPoint, 20 * Constants.CS_EPSILON);
        }

        public static bool IsPointInLineSegment2D(Point point1, Point point2, Point testPoint)
        {
            return Geo.IsPointInLineSegment2D(point1, point2, testPoint, 20 * Constants.CS_EPSILON);
        }

        public static bool IsPointInLineSegment2D(Point point1, Point point2, Point testPoint, double minimalDistance)
        {
            bool flag = false;
            if (Compare.LE(Geo.GetDistanceBeetveenTwoPoints2D(point1, testPoint) + Geo.GetDistanceBeetveenTwoPoints2D(point2, testPoint) - Geo.GetDistanceBeetveenTwoPoints2D(point1, point2), minimalDistance))
            {
                Point line = Projection.PointToLine(testPoint, new Line(point1, point2));
                flag = Compare.LE(Geo.GetDistanceBeetveenTwoPoints2D(line, testPoint), minimalDistance);
            }
            return flag;
        }

        public static bool IsPointInLineSegment3D(Point point1, Point point2, Point testPoint)
        {
            return Geo.IsPointInLineSegment3D(point1, point2, testPoint, 20 * Constants.CS_EPSILON);
        }

        public static bool IsPointInLineSegment3D(Point point1, Point point2, Point testPoint, double minimalDistance)
        {
            bool flag = false;
            if (Compare.LT(Geo.GetDistanceBeetveenTwoPoints3D(point1, testPoint) + Geo.GetDistanceBeetveenTwoPoints3D(point2, testPoint) - Geo.GetDistanceBeetveenTwoPoints3D(point1, point2), minimalDistance))
            {
                Point line = Projection.PointToLine(testPoint, new Line(point1, point2));
                flag = Compare.LE(Geo.GetDistanceBeetveenTwoPoints3D(line, testPoint), minimalDistance);
            }
            return flag;
        }

        public static bool IsPointInsideBoundingBox(Point point, Point boundingBoxMinimalPoint, Point boundingBoxMaximalPoint)
        {
            bool flag = false;
            if (point != null && boundingBoxMinimalPoint != null && boundingBoxMaximalPoint != null && Compare.IE(point.X, boundingBoxMinimalPoint.X, boundingBoxMaximalPoint.X) && Compare.IE(point.Y, boundingBoxMinimalPoint.Y, boundingBoxMaximalPoint.Y) && Compare.IE(point.Z, boundingBoxMinimalPoint.Z, boundingBoxMaximalPoint.Z))
            {
                flag = true;
            }
            return flag;
        }

        public static bool IsPointInsidePolygon2D(Polygon polygon, Point testPoint)
        {
            return Geo.IsPointInsidePolygon2D(Geo.ConvertListPointsFromPolygon(polygon), testPoint);
        }

        public static bool IsPointInsidePolygon2D(Polygon polygon, Point testPoint, bool withBorder)
        {
            return Geo.IsPointInsidePolygon2D(Geo.ConvertListPointsFromPolygon(polygon), testPoint, withBorder);
        }

        public static bool IsPointInsidePolygon2D(List<Point> polygonPoints, Point testPoint)
        {
            int num = 0;
            bool flag = false;
            for (int i = 0; i < polygonPoints.Count; i++)
            {
                num++;
                if (num == polygonPoints.Count)
                {
                    num = 0;
                }
                if (Compare.LT(polygonPoints[i].Y, testPoint.Y) && Compare.GE(polygonPoints[num].Y, testPoint.Y) || Compare.LT(polygonPoints[num].Y, testPoint.Y) && Compare.GE(polygonPoints[i].Y, testPoint.Y))
                {
                    double y = testPoint.Y - polygonPoints[i].Y;
                    double y1 = polygonPoints[num].Y - polygonPoints[i].Y;
                    double x = polygonPoints[num].X - polygonPoints[i].X;
                    if (Compare.LT(polygonPoints[i].X + y / y1 * x, testPoint.X))
                    {
                        flag = !flag;
                    }
                }
            }
            return flag;
        }

        public static bool IsPointInsidePolygon2D(List<Point> polygon, Point testPoint, bool withBorder)
        {
            int num = 0;
            for (int i = 0; i < polygon.Count; i++)
            {
                num++;
                if (num == polygon.Count)
                {
                    num = 0;
                }
                if (Geo.IsPointInLineSegment2D(polygon[i], polygon[num], testPoint))
                {
                    return withBorder;
                }
            }
            return Geo.IsPointInsidePolygon2D(polygon, testPoint);
        }

        private static bool IsPointsCollinear(Point point1, Point point2, Point point3)
        {
            double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point1, point2);
            double num = Geo.GetDistanceBeetveenTwoPoints3D(point2, point3);
            double distanceBeetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(point1, point3);
            if (Compare.EQ(distanceBeetveenTwoPoints3D + num, distanceBeetveenTwoPoints3D1) || Compare.EQ(num + distanceBeetveenTwoPoints3D1, distanceBeetveenTwoPoints3D))
            {
                return true;
            }
            return Compare.EQ(distanceBeetveenTwoPoints3D + distanceBeetveenTwoPoints3D1, num);
        }

        public static Point MovePoint(Point pointToMove, Vector direction, double distance)
        {
            Point point = new Point(pointToMove);
            Vector normal = direction.GetNormal();
            Point z = point;
            z.Z = z.Z + normal.Z * distance;
            Point y = point;
            y.Y = y.Y + normal.Y * distance;
            Point x = point;
            x.X = x.X + normal.X * distance;
            return point;
        }

        public static Point MovePointOnLine(Point pointToMove, Point linePoint, double distance)
        {
            Vector normal = Geo.GetVectorLineSegment(linePoint, pointToMove).GetNormal();
            return Geo.MovePoint(pointToMove, normal, distance);
        }

        public static void PointParallel(ref Point p1, ref Point p2, Point inputPoint1, Point inputPoint2, double actualOffset)
        {
            Point point;
            Point point1;
            if (inputPoint1 == null && inputPoint2 == null)
            {
                return;
            }
            if (p1 == null)
            {
                p1 = new Point();
            }
            if (p2 == null)
            {
                p2 = new Point();
            }
            point = (inputPoint1 != null ? inputPoint1 : new Point());
            point1 = (inputPoint2 != null ? inputPoint2 : new Point());
            if (actualOffset == 0)
            {
                p1 = new Point(point);
                p2 = new Point(point1);
                return;
            }
            Vector vector = new Vector(point1.X - point.X, point1.Y - point.Y, point1.Z - point.Z);
            Vector vector1 = new Vector(0, 0, -1000);
            vector1.Normalize();
            vector.Normalize();
            CoordinateSystem coordinateSystem = new CoordinateSystem()
            {
                Origin = point,
                AxisX = vector1.Cross(vector),
                AxisY = vector
            };
            SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
            setPlane.AddPoints(new Point[] { point });
            setPlane.AddPoints(new Point[] { point1 });
            setPlane.AddPoints(new Point[] { p1 });
            setPlane.AddPoints(new Point[] { p2 });
            setPlane.Begin(coordinateSystem);
            try
            {
                p1.X = point.X + actualOffset;
                p1.Y = point.Y;
                p1.Z = point.Z;
                p2.X = point1.X + actualOffset;
                p2.Y = point1.Y;
                p2.Z = point1.Z;
            }
            catch (Exception exception)
            {
                exception.ToString();
            }
            setPlane.End();
        }

        public static void SortPoints2D(List<Point> points, Point mainPoint)
        {
            Geo.comparePoint = mainPoint;
            points.Sort(new Comparison<Point>(Geo.ICompareTwoPoints2D));
            Geo.comparePoint = new Point(0, 0, 0);
        }

        public static void SortPoints3D(List<Point> points, Point mainPoint)
        {
            Geo.comparePoint = mainPoint;
            points.Sort(new Comparison<Point>(Geo.ICompareTwoPoints3D));
            Geo.comparePoint = new Point(0, 0, 0);
        }

        public static double VectorAngle(Point p1, Point p2, Point p3, Point p4)
        {
            double num = Geo.VectorDotProduct(p1, p2, p3, p4);
            return Math.Acos(num / (Geo.GetDistanceBeetveenTwoPoints3D(p1, p2) * Geo.GetDistanceBeetveenTwoPoints3D(p3, p4)));
        }

        public static double VectorDotProduct(Point point1, Point point2, Point point3, Point point4)
        {
            return Geo.DX(point2, point1) * Geo.DX(point4, point3) + Geo.DY(point2, point1) * Geo.DY(point4, point3) + Geo.DZ(point2, point1) * Geo.DZ(point4, point3);
        }
    }
}