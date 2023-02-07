using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Solid;

namespace cs_net_lib
{
	public static class GetProjectedShape
	{
		public static bool GetShape(Identifier partId, ref List<Polygon> resultShapePolygons)
		{
            
			ModelObject modelObject = (new Tekla.Structures.Model.Model()).SelectModelObject(partId);
			bool shape = false;
			if (modelObject != null && modelObject is Part)
			{
				shape = GetProjectedShape.GetShape(modelObject as Part, ref resultShapePolygons);
			}
			return shape;
		}

		public static bool GetShape(Identifier partId, CoordinateSystem coordinateSystem, ref List<Polygon> resultShapePolygons)
		{
			ModelObject modelObject = (new Tekla.Structures.Model.Model()).SelectModelObject(partId);
			bool shape = false;
			if (modelObject != null && modelObject is Part)
			{
				shape = GetProjectedShape.GetShape(modelObject as Part, coordinateSystem, ref resultShapePolygons);
			}
			return shape;
		}

		public static bool GetShape(Part part, CoordinateSystem coordinateSystem, ref List<Polygon> resultShapePolygons)
		{
			SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
			setPlane.Begin(coordinateSystem);
			Tekla.Structures.Model.Solid solid = part.GetSolid();
			bool shape = GetProjectedShape.GetShape(GetProjectedShape.LineDetermining.GetMainPolygonsFromSolid(solid), ref resultShapePolygons);
			foreach (Polygon resultShapePolygon in resultShapePolygons)
			{
				setPlane.AddArrayList(resultShapePolygon.Points);
			}
			setPlane.End();
			return shape;
		}

		public static bool GetShape(Part part, ref List<Polygon> resultShapePolygons)
		{
			List<Polygon> mainPolygonsFromSolid = GetProjectedShape.LineDetermining.GetMainPolygonsFromSolid(part.GetSolid());
			return GetProjectedShape.GetShape(mainPolygonsFromSolid, ref resultShapePolygons);
		}

		private static bool GetShape(List<Polygon> polygons, ref List<Polygon> resultShapePolygons)
		{
			resultShapePolygons = new List<Polygon>();
			Polygon polygon = new Polygon();
			Point point = new Point();
			bool flag = true;
			List<LineSegment> lineSegmetsFromPolygons = GetProjectedShape.LineDetermining.GetLineSegmetsFromPolygons(polygons);
			lineSegmetsFromPolygons = GetProjectedShape.LineDetermining.RemoveUselessLines(lineSegmetsFromPolygons);
			do
			{
				List<Point> points = GetProjectedShape.ShapeDetermining.PointsFromConvexHull(lineSegmetsFromPolygons);
				List<LineSegment> lineSegments = GetProjectedShape.ShapeDetermining.LinesInConvexHull(lineSegmetsFromPolygons, points, ref point);
				if (lineSegments.Count == 0)
				{
					List<LineSegment> lineSegments1 = GetProjectedShape.LineDetermining.FindLinesFromPoint(points[0], lineSegmetsFromPolygons);
					LineSegment lineSegment = GetProjectedShape.ShapeDetermining.FindStartLine(points[0], points[1], lineSegments1);
					point = GetProjectedShape.ShapeDetermining.FindLastPoint(lineSegment, points[0]);
					lineSegments.Add(lineSegment);
				}
				try
				{
					List<LineSegment> lineSegments2 = GetProjectedShape.ShapeDetermining.ModelBasicShape(lineSegmetsFromPolygons, lineSegments, point);
					polygon.Points = GetProjectedShape.ShapeDetermining.LinesToPointsArrayList(point, lineSegments2);
					GetProjectedShape.LineDetermining.DeleteLineFromCreatedPolygon(lineSegmetsFromPolygons, polygon);
					PolygonOperation.RemoveUnnecessaryPolygonPoints(polygon);
					resultShapePolygons.Add(polygon);
					polygon = new Polygon();
				}
				catch (GetProjectedShape.ShapeNotFoundException shapeNotFoundException)
				{
					flag = false;
				}
			}
			while (lineSegmetsFromPolygons.Count != 0 && flag);
			return flag;
		}

		private class IntersectLineSegments
		{
			public List<double> Distances
			{
				get;
				set;
			}

			public List<Point> IntersectPoints
			{
				get;
				set;
			}

			public List<LineSegment> LineSegments
			{
				get;
				set;
			}

			public IntersectLineSegments()
			{
				this.IntersectPoints = new List<Point>();
				this.LineSegments = new List<LineSegment>();
				this.Distances = new List<double>();
			}
		}

		private static class LineDetermining
		{
			public static void DeleteLineFromCreatedPolygon(List<LineSegment> lines, Polygon resultShapePolygon)
			{
				for (int i = lines.Count - 1; i > -1; i--)
				{
					if (Geo.IsPointInsidePolygon2D(resultShapePolygon, lines[i].Point1, true))
					{
						lines.RemoveAt(i);
					}
				}
			}

			public static List<LineSegment> FindLinesFromPoint(Point controlPoint, List<LineSegment> lines)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				foreach (LineSegment line in lines)
				{
					if (!Geo.CompareTwoPoints2D(controlPoint, line.Point1) && !Geo.CompareTwoPoints2D(controlPoint, line.Point2))
					{
						continue;
					}
					lineSegments.Add(line);
				}
				return lineSegments;
			}

			public static List<LineSegment> GetLineSegmetsFromPolygons(List<Polygon> polygons)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				foreach (Polygon polygon in polygons)
				{
					ArrayList points = polygon.Points;
					for (int i = 0; i < points.Count; i++)
					{
						if (i >= points.Count - 1)
						{
							lineSegments.Add(new LineSegment((Point)points[i], (Point)points[0]));
						}
						else
						{
							lineSegments.Add(new LineSegment((Point)points[i], (Point)points[i + 1]));
						}
					}
				}
				return lineSegments;
			}

			public static List<Polygon> GetMainPolygonsFromSolid(Tekla.Structures.Model.Solid solid)
			{
				GeometricPlane geometricPlane = new GeometricPlane();
				List<Polygon> polygons = new List<Polygon>();
				Polygon polygon = new Polygon();
				ArrayList arrayLists = new ArrayList();
				FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
				Point current = null;
				int num = 0;
				int num1 = 0;
				while (faceEnumerator.MoveNext())
				{
					Face face = faceEnumerator.Current;
					if (face != null)
					{
						arrayLists.Add(face.Normal);
						LoopEnumerator loopEnumerator = face.GetLoopEnumerator();
						while (loopEnumerator.MoveNext())
						{
							Loop loop = loopEnumerator.Current;
							if (loop == null)
							{
								continue;
							}
							if (num1 == 0)
							{
								VertexEnumerator vertexEnumerator = loop.GetVertexEnumerator();
								while (vertexEnumerator.MoveNext())
								{
									current = vertexEnumerator.Current;
									if (current == null)
									{
										continue;
									}
									current = Projection.PointToPlane(new Point(current), geometricPlane);
									polygon.Points.Add(current);
								}
								polygons.Add(polygon);
								polygon = new Polygon();
							}
							num1++;
							num++;
						}
					}
					num1 = 0;
				}
				return polygons;
			}

			public static List<Polygon> GetMainPolygonsFromSolid(Tekla.Structures.Model.Solid solid, GeometricPlane geoPlane)
			{
				List<Polygon> polygons = new List<Polygon>();
				Polygon polygon = new Polygon();
				ArrayList arrayLists = new ArrayList();
				FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
				Point current = null;
				int num = 0;
				int num1 = 0;
				while (faceEnumerator.MoveNext())
				{
					Face face = faceEnumerator.Current;
					if (face != null)
					{
						arrayLists.Add(face.Normal);
						LoopEnumerator loopEnumerator = face.GetLoopEnumerator();
						while (loopEnumerator.MoveNext())
						{
							Loop loop = loopEnumerator.Current;
							if (loop == null)
							{
								continue;
							}
							if (num1 == 0)
							{
								VertexEnumerator vertexEnumerator = loop.GetVertexEnumerator();
								while (vertexEnumerator.MoveNext())
								{
									current = vertexEnumerator.Current;
									if (current == null)
									{
										continue;
									}
									current = Projection.PointToPlane(new Point(current), geoPlane);
									polygon.Points.Add(current);
								}
								polygons.Add(polygon);
								polygon = new Polygon();
							}
							num1++;
							num++;
						}
					}
					num1 = 0;
				}
				return polygons;
			}

			public static List<LineSegment> RemoveUselessLines(List<LineSegment> lines)
			{
				for (int i = 0; i < lines.Count; i++)
				{
					for (int j = lines.Count - 1; j > i; j--)
					{
						if (Geo.CompareTwoLinesSegment2D(lines[i], lines[j]))
						{
							lines.RemoveAt(j);
						}
					}
				}
				for (int k = lines.Count - 1; k > -1; k--)
				{
					if (Geo.CompareTwoPoints2D(lines[k].Point1, lines[k].Point2))
					{
						lines.RemoveAt(k);
					}
				}
				return lines;
			}
		}

		private static class ShapeDetermining
		{
			public static double FindAngle(LineSegment lineMain, Point mainPoint, LineSegment line)
			{
				double num;
				Point point = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, lineMain);
				Vector vector = new Vector(mainPoint - point);
				Vector vector1 = new Vector(vector.Y, -vector.X, 0);
				point = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, line);
				Vector vector2 = new Vector(point - mainPoint);
				double angleBetween2Vectors = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1, vector2);
				double angleBetween2Vectors1 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector, vector2);
				if (!Compare.LE(angleBetween2Vectors, 90))
				{
					if (Compare.EQ(angleBetween2Vectors1, 180))
					{
						angleBetween2Vectors1 = angleBetween2Vectors1 * -1;
					}
					num = angleBetween2Vectors1;
				}
				else
				{
					num = angleBetween2Vectors1 * -1;
				}
				return num;
			}

			public static List<double> FindAngles(LineSegment lineMain, Point mainPoint, List<LineSegment> lines)
			{
				List<double> nums = new List<double>();
				Point point = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, lineMain);
				Vector vector = new Vector(mainPoint - point);
				Vector vector1 = new Vector(vector.Y, -vector.X, 0);
				foreach (LineSegment line in lines)
				{
					point = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, line);
					Vector vector2 = new Vector(point - mainPoint);
					double angleBetween2Vectors = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1, vector2);
					double num = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector, vector2);
					if (!Compare.LE(angleBetween2Vectors, 90))
					{
						if (Compare.EQ(num, 180))
						{
							num = num * -1;
						}
						nums.Add(num);
					}
					else
					{
						nums.Add(num * -1);
					}
				}
				return nums;
			}

			private static LineSegment FindBestLine(List<LineSegment> bestLines)
			{
				double num = 0;
				int num1 = -1;
				for (int i = 0; i < bestLines.Count; i++)
				{
					double distanceBeetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(bestLines[i].Point1, bestLines[i].Point2);
					if (Compare.GT(distanceBeetveenTwoPoints2D, num))
					{
						num = distanceBeetveenTwoPoints2D;
						num1 = i;
					}
				}
				return bestLines[num1];
			}

			private static bool FindBestLineAfterIntersect(List<double> angles, GetProjectedShape.IntersectLineSegments intersectLineSegmetsInMin, ref LineSegment nextLineOfShape)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				nextLineOfShape = new LineSegment();
				double item = -1;
				int num = -1;
				for (int i = 0; i < angles.Count; i++)
				{
					if (Compare.GT(angles[i], item))
					{
						item = angles[i];
						num = i;
					}
				}
				if (Compare.LT(item, 0))
				{
					return false;
				}
				for (int j = num; j < intersectLineSegmetsInMin.LineSegments.Count; j++)
				{
					if (Compare.EQ(angles[j], item))
					{
						lineSegments.Add(intersectLineSegmetsInMin.LineSegments[j]);
					}
				}
				nextLineOfShape = GetProjectedShape.ShapeDetermining.FindBestLine(lineSegments);
				return true;
			}

			private static List<LineSegment> FindBestLines(List<LineSegment> lines, List<double> angles)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				double item = -181;
				int num = -1;
				for (int i = 0; i < angles.Count; i++)
				{
					if (Compare.GT(angles[i], item))
					{
						item = angles[i];
						num = i;
					}
				}
				if (Compare.EQ(item, -181))
				{
					throw new GetProjectedShape.ShapeNotFoundException("Cannot find angles in FindBestLine");
				}
				for (int j = num; j < lines.Count; j++)
				{
					if (Compare.EQ(angles[j], item))
					{
						lineSegments.Add(lines[j]);
					}
				}
				return lineSegments;
			}

			private static GetProjectedShape.IntersectLineSegments FindIntersectLineSegmetsInMin(GetProjectedShape.IntersectLineSegments intersectLineSegmets, double minDistance)
			{
				GetProjectedShape.IntersectLineSegments intersectLineSegment = new GetProjectedShape.IntersectLineSegments();
				for (int i = 0; i < intersectLineSegmets.Distances.Count; i++)
				{
					if (Compare.EQ(intersectLineSegmets.Distances[i], minDistance))
					{
						intersectLineSegment.Distances.Add(intersectLineSegmets.Distances[i]);
						intersectLineSegment.IntersectPoints.Add(intersectLineSegmets.IntersectPoints[i]);
						intersectLineSegment.LineSegments.Add(intersectLineSegmets.LineSegments[i]);
						intersectLineSegmets.Distances.RemoveAt(i);
						intersectLineSegmets.IntersectPoints.RemoveAt(i);
						intersectLineSegmets.LineSegments.RemoveAt(i);
						i--;
					}
				}
				return intersectLineSegment;
			}

			public static Point FindLastPoint(LineSegment lastLineOfShape, Point lastPoint)
			{
				if (!Geo.CompareTwoPoints2D(lastPoint, lastLineOfShape.Point1))
				{
					return lastLineOfShape.Point1;
				}
				return lastLineOfShape.Point2;
			}

			private static double FindMinDistance(GetProjectedShape.IntersectLineSegments intersectLineSegmets)
			{
				double item = intersectLineSegmets.Distances[0];
				foreach (double distance in intersectLineSegmets.Distances)
				{
					if (!Compare.LT(distance, item))
					{
						continue;
					}
					item = distance;
				}
				return item;
			}

			private static bool FindNextLineInConvex(List<LineSegment> linesInConvexHull, Point lastPoint, ref LineSegment nextLine)
			{
				bool flag;
				List<LineSegment>.Enumerator enumerator = linesInConvexHull.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						LineSegment current = enumerator.Current;
						if (!Geo.CompareTwoPoints2D(lastPoint, current.Point1) && !Geo.CompareTwoPoints2D(lastPoint, current.Point2))
						{
							continue;
						}
						nextLine = current;
						linesInConvexHull.Remove(current);
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}

			private static List<LineSegment> FindNextLines(Point lastPoint, List<LineSegment> lines)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				for (int i = 0; i < lines.Count; i++)
				{
					if (Geo.CompareTwoPoints2D(lastPoint, lines[i].Point1) || Geo.CompareTwoPoints2D(lastPoint, lines[i].Point2))
					{
						lineSegments.Add(lines[i]);
					}
				}
				if (lineSegments.Count == 0)
				{
					object[] x = new object[] { "Cannot find path on point x: ", lastPoint.X, " y: ", lastPoint.Y };
					throw new GetProjectedShape.ShapeNotFoundException(string.Concat(x));
				}
				return lineSegments;
			}

			private static Point FindOtherPoint(Point mainPoint, LineSegment line)
			{
				if (!Geo.CompareTwoPoints2D(line.Point1, mainPoint))
				{
					return line.Point1;
				}
				return line.Point2;
			}

			private static GetProjectedShape.IntersectLineSegments FindPossibleIntersectLines(List<LineSegment> lines, LineSegment possibleLineOfShape, Point lastPoint, ref GetProjectedShape.IntersectLineSegments intersectLineSegmetsInOneVectorWithPossibleLine)
			{
				Point point;
				List<Point> points = new List<Point>();
				GetProjectedShape.IntersectLineSegments intersectLineSegment = new GetProjectedShape.IntersectLineSegments();
				intersectLineSegmetsInOneVectorWithPossibleLine = new GetProjectedShape.IntersectLineSegments();
				Point point1 = GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, possibleLineOfShape);
				LineSegment lineSegment = new LineSegment(lastPoint, point1);
				foreach (LineSegment line in lines)
				{
					if (!Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, line, ref points))
					{
						continue;
					}
					if (points.Count > 1)
					{
						double angleBetween2Vectors = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(lineSegment.Point2 - lineSegment.Point1), new Vector(line.Point2 - line.Point1));
						if (!GetProjectedShape.ShapeDetermining.IsIntersectLineSegmetsNotParallel(angleBetween2Vectors))
						{
							if (Geo.CompareTwoPoints2D(points[1], point1) && !Geo.CompareTwoPoints2D(points[1], line.Point1) && !Geo.CompareTwoPoints2D(points[1], line.Point2))
							{
								double distanceBeetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(points[0], lineSegment.Point2);
								point = (!Compare.ZR(angleBetween2Vectors) ? line.Point1 : line.Point2);
								if (Compare.LT(Geo.GetDistanceBeetveenTwoPoints2D(points[0], point), distanceBeetveenTwoPoints2D))
								{
									points[1] = point;
								}
							}
						}
						else if (Geo.CompareTwoPoints2D(points[1], lineSegment.Point1) || Geo.CompareTwoPoints2D(points[1], lineSegment.Point2) || Geo.CompareTwoPoints2D(points[1], line.Point1) || Geo.CompareTwoPoints2D(points[1], line.Point2))
						{
							points.Remove(points[0]);
						}
						else if (Geo.CompareTwoPoints2D(points[0], lineSegment.Point1) || Geo.CompareTwoPoints2D(points[0], lineSegment.Point2) || Geo.CompareTwoPoints2D(points[0], line.Point1) || Geo.CompareTwoPoints2D(points[0], line.Point2))
						{
							points.Remove(points[1]);
						}
						if (points.Count != 1 && Geo.CompareTwoPoints2D(points[1], point1) && !Geo.CompareTwoPoints2D(points[1], line.Point1) && !Geo.CompareTwoPoints2D(points[1], line.Point2))
						{
							intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.Add(point1);
							intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.Add(new LineSegment(point1, line.Point1));
							intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.Add(point1);
							intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.Add(new LineSegment(point1, line.Point2));
						}
					}
					if (points.Count != 1 || !GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, lastPoint) || !GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, point1) || !GetProjectedShape.ShapeDetermining.IsItGoodIntersect(lineSegment, points[0], line))
					{
						continue;
					}
					if (Geo.CompareTwoPoints2D(points[0], line.Point1) || Geo.CompareTwoPoints2D(points[0], line.Point2))
					{
						intersectLineSegment.LineSegments.Add(line);
						intersectLineSegment.IntersectPoints.Add(points[0]);
						intersectLineSegment.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, points[0]));
					}
					else
					{
						intersectLineSegment.LineSegments.Add(new LineSegment(points[0], line.Point1));
						intersectLineSegment.IntersectPoints.Add(points[0]);
						intersectLineSegment.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, points[0]));
						intersectLineSegment.LineSegments.Add(new LineSegment(points[0], line.Point2));
						intersectLineSegment.IntersectPoints.Add(points[0]);
						intersectLineSegment.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, points[0]));
					}
				}
				return intersectLineSegment;
			}

			private static List<LineSegment> FindPossibleIntersectLinesOnEndOfLine(List<LineSegment> lines, LineSegment lastLineOfShape, Point lastPoint)
			{
				List<Point> points = new List<Point>();
				List<LineSegment> lineSegments = new List<LineSegment>();
				LineSegment lineSegment = new LineSegment(GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, lastLineOfShape), lastPoint);
				foreach (LineSegment line in lines)
				{
					if (!Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, line, ref points))
					{
						if ((!Geo.CompareTwoPoints2D(lastPoint, line.Point1) || Geo.CompareTwoPoints2D(lastPoint, line.Point2)) && (!Geo.CompareTwoPoints2D(lastPoint, line.Point2) || Geo.CompareTwoPoints2D(lastPoint, line.Point1)))
						{
							continue;
						}
						lineSegments.Add(line);
					}
					else if (GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, lastPoint) || points.Count != 1)
					{
						if (points.Count <= 1 || !Geo.CompareTwoPoints2D(points[1], lastPoint) || Geo.CompareTwoPoints2D(points[1], line.Point1) || Geo.CompareTwoPoints2D(points[1], line.Point2))
						{
							continue;
						}
						lineSegments.Add(new LineSegment(lastPoint, line.Point1));
						lineSegments.Add(new LineSegment(lastPoint, line.Point2));
					}
					else
					{
						lineSegments.Add(line);
					}
				}
				return lineSegments;
			}

			public static LineSegment FindStartLine(Point pointConvexHull0, Point pointConvexHull1, List<LineSegment> possibleStartLines)
			{
				LineSegment lineSegment = new LineSegment(pointConvexHull1, pointConvexHull0);
				List<double> nums = GetProjectedShape.ShapeDetermining.FindAngles(lineSegment, pointConvexHull0, possibleStartLines);
				return GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(possibleStartLines, nums));
			}

			private static double GetAngleBetween2Vectors(Vector vector1, Vector vector2)
			{
				double angleBetween = vector1.GetAngleBetween(vector2) * Constants.RAD_TO_DEG;
				if (Compare.GT(vector1.GetNormal().Cross(vector2.GetNormal()).GetNormal().Y, 0))
				{
					angleBetween = 360 - angleBetween;
				}
				return angleBetween;
			}

			private static bool IsIntersectLineSegmetsNotParallel(double angle)
			{
				if (!Compare.NZ(angle))
				{
					return false;
				}
				return Compare.NE(angle, 180);
			}

			private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect)
			{
				bool flag = true;
				if (Compare.GE(GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(mainLine.Point2 - mainLine.Point1), new Vector(intersect - mainLine.Point1)), 89.96))
				{
					flag = false;
				}
				return flag;
			}

			private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect, Point lastPoint)
			{
				bool flag = true;
				if (Compare.GE(GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(lastPoint - GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, mainLine)), new Vector(intersect - GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, mainLine))), 90))
				{
					flag = false;
				}
				return flag;
			}

			private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect, LineSegment nextPossibleLine)
			{
				bool flag = true;
				Vector vector = new Vector(mainLine.Point2 - mainLine.Point1);
				Vector vector1 = new Vector(intersect - mainLine.Point1);
				double angleBetween2Vectors = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector, vector1);
				Point point = GetProjectedShape.ShapeDetermining.FindOtherPoint(intersect, nextPossibleLine);
				Vector vector2 = new Vector(vector.Y, -vector.X, 0);
				if (Compare.GT(angleBetween2Vectors, 0))
				{
					vector1 = new Vector(point - intersect);
					double num = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector2, vector1);
					double angleBetween2Vectors1 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector, vector1);
					angleBetween2Vectors = (!Compare.LE(num, 90) ? angleBetween2Vectors1 : angleBetween2Vectors1 * -1);
					if (Compare.LE(angleBetween2Vectors, 0) || Compare.GE(angleBetween2Vectors, 90))
					{
						flag = false;
					}
				}
				return flag;
			}

			private static bool IsItLineForIntersect(LineSegment line, Point comparisonPoint)
			{
				if (Geo.CompareTwoPoints2D(comparisonPoint, line.Point1))
				{
					return false;
				}
				return !Geo.CompareTwoPoints2D(comparisonPoint, line.Point2);
			}

			public static List<LineSegment> LinesInConvexHull(List<LineSegment> lines, List<Point> pointsOfConvexHull, ref Point firstLastPoint)
			{
				List<LineSegment> lineSegments = new List<LineSegment>();
				List<LineSegment> lineSegments1 = new List<LineSegment>();
				bool flag = false;
				for (int i = 0; i < pointsOfConvexHull.Count; i++)
				{
					if (i == pointsOfConvexHull.Count - 1)
					{
						lineSegments1 = lines.FindAll((LineSegment lineCondition) => {
							if (Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[i]) && Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[0]))
							{
								return true;
							}
							if (!Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[i]))
							{
								return false;
							}
							return Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[0]);
						});
						if (!flag && lineSegments1.Count != 0)
						{
							flag = true;
							firstLastPoint = pointsOfConvexHull[0];
						}
					}
					else
					{
						lineSegments1 = lines.FindAll((LineSegment lineCondition) => {
							if (Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[i]) && Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[i + 1]))
							{
								return true;
							}
							if (!Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[i]))
							{
								return false;
							}
							return Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[i + 1]);
						});
						if (!flag && lineSegments1.Count != 0)
						{
							flag = true;
							firstLastPoint = pointsOfConvexHull[i + 1];
						}
					}
					foreach (LineSegment lineSegment in lineSegments1)
					{
						lineSegments.Add(lineSegment);
					}
				}
				return lineSegments;
			}

			public static List<Point> LinesToPoints(List<LineSegment> linesOfShape, Point firstLastPoint)
			{
				List<Point> points = new List<Point>();
				if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
				{
					Point point = new Point(linesOfShape[0].Point1);
					linesOfShape[0].Point1 = linesOfShape[0].Point2;
					linesOfShape[0].Point2 = point;
				}
				foreach (LineSegment lineSegment in linesOfShape)
				{
					points.Add(lineSegment.Point1);
					points.Add(lineSegment.Point2);
				}
				GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints(points);
				return points;
			}

			public static ArrayList LinesToPointsArrayList(Point firstLastPoint, List<LineSegment> linesOfShape)
			{
				ArrayList arrayLists = new ArrayList();
				if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
				{
					Point point = new Point(linesOfShape[0].Point1);
					linesOfShape[0].Point1 = linesOfShape[0].Point2;
					linesOfShape[0].Point2 = point;
				}
				foreach (LineSegment lineSegment in linesOfShape)
				{
					arrayLists.Add(lineSegment.Point1);
					arrayLists.Add(lineSegment.Point2);
				}
				GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints(arrayLists);
				return arrayLists;
			}

			public static List<LineSegment> ModelBasicShape(List<LineSegment> lines, List<LineSegment> linesInConvexHull, Point firstLastPoint)
			{
				List<double> nums;
				List<LineSegment> lineSegments = new List<LineSegment>();
				LineSegment lineSegment = new LineSegment();
				Point point = new Point(firstLastPoint);
				LineSegment lineSegment1 = new LineSegment();
				List<Point> points = new List<Point>();
				List<LineSegment> lineSegments1 = new List<LineSegment>();
				GetProjectedShape.IntersectLineSegments intersectLineSegment = new GetProjectedShape.IntersectLineSegments();
				int num = 2;
				bool flag = false;
				int num1 = 0;
				lineSegments.Add(linesInConvexHull[0]);
				LineSegment item = linesInConvexHull[0];
				linesInConvexHull.RemoveAt(0);
				Point point1 = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, item);
				LineSegment lineSegment2 = new LineSegment(point1, firstLastPoint);
				do
				{
					if (GetProjectedShape.ShapeDetermining.FindNextLineInConvex(linesInConvexHull, point, ref item))
					{
						if (!Compare.EQ(-180, GetProjectedShape.ShapeDetermining.FindAngle(lineSegments[lineSegments.Count - 1], point, item)))
						{
							lineSegments.Add(item);
							point = GetProjectedShape.ShapeDetermining.FindLastPoint(item, point);
							num = 2;
						}
						else
						{
							item = lineSegments[lineSegments.Count - 1];
						}
					}
					else if (!Intersect.IntersectLineSegmentToLineSegment2D(lineSegment2, item, ref points) || num1 <= 2)
					{
						switch (num)
						{
							case 0:
							{
								lineSegments1 = GetProjectedShape.ShapeDetermining.FindNextLines(point, lines);
								nums = GetProjectedShape.ShapeDetermining.FindAngles(item, point, lineSegments1);
								lineSegment = GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(lineSegments1, nums));
								break;
							}
							case 1:
							{
								lineSegment = lineSegment1;
								break;
							}
							case 2:
							{
								lineSegments1 = GetProjectedShape.ShapeDetermining.FindPossibleIntersectLinesOnEndOfLine(lines, item, point);
								nums = GetProjectedShape.ShapeDetermining.FindAngles(item, point, lineSegments1);
								lineSegment = GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(lineSegments1, nums));
								break;
							}
							default:
							{
								goto case 2;
							}
						}
						GetProjectedShape.IntersectLineSegments intersectLineSegment1 = GetProjectedShape.ShapeDetermining.FindPossibleIntersectLines(lines, lineSegment, point, ref intersectLineSegment);
						if (intersectLineSegment1.LineSegments.Count > 0)
						{
							do
							{
								GetProjectedShape.IntersectLineSegments intersectLineSegment2 = GetProjectedShape.ShapeDetermining.FindIntersectLineSegmetsInMin(intersectLineSegment1, GetProjectedShape.ShapeDetermining.FindMinDistance(intersectLineSegment1));
								nums = GetProjectedShape.ShapeDetermining.FindAngles(new LineSegment(point, intersectLineSegment2.IntersectPoints[0]), intersectLineSegment2.IntersectPoints[0], intersectLineSegment2.LineSegments);
								for (int i = nums.Count - 1; i > -1; i--)
								{
									if (Compare.ZR(nums[i]))
									{
										intersectLineSegment2.IntersectPoints.RemoveAt(i);
										intersectLineSegment2.LineSegments.RemoveAt(i);
										intersectLineSegment2.Distances.Remove((double)i);
										nums.RemoveAt(i);
									}
								}
								if (!GetProjectedShape.ShapeDetermining.FindBestLineAfterIntersect(nums, intersectLineSegment2, ref lineSegment1))
								{
									continue;
								}
								if (!GetProjectedShape.ShapeDetermining.IsItGoodIntersect(lineSegment, intersectLineSegment2.IntersectPoints[0], point))
								{
									intersectLineSegment2.IntersectPoints[0] = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, lineSegment);
								}
								num = 1;
								item = new LineSegment(point, intersectLineSegment2.IntersectPoints[0]);
								lineSegments.Add(item);
								point = intersectLineSegment2.IntersectPoints[0];
								flag = true;
							}
							while (intersectLineSegment1.LineSegments.Count > 0 && !flag);
						}
						if (!flag && intersectLineSegment.LineSegments.Count > 0)
						{
							LineSegment lineSegment3 = new LineSegment(point, intersectLineSegment.IntersectPoints[0]);
							nums = GetProjectedShape.ShapeDetermining.FindAngles(lineSegment3, intersectLineSegment.IntersectPoints[0], intersectLineSegment.LineSegments);
							for (int j = nums.Count - 1; j > -1; j--)
							{
								if (Compare.NZ(nums[j]))
								{
									intersectLineSegment.IntersectPoints.RemoveAt(j);
									intersectLineSegment.LineSegments.RemoveAt(j);
									nums.RemoveAt(j);
								}
							}
							if (GetProjectedShape.ShapeDetermining.FindBestLineAfterIntersect(nums, intersectLineSegment, ref lineSegment1))
							{
								num = 1;
								Point point2 = GetProjectedShape.ShapeDetermining.FindOtherPoint(intersectLineSegment.IntersectPoints[0], lineSegment1);
								lineSegment1 = new LineSegment(point, point2);
								num1--;
								flag = true;
							}
						}
						if (!flag)
						{
							num = 0;
							item = lineSegment;
							lineSegments.Add(item);
							point = GetProjectedShape.ShapeDetermining.FindLastPoint(item, point);
						}
						flag = false;
					}
					else
					{
						point = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, item);
						lineSegments[lineSegments.Count - 1].Point2 = point1;
						lineSegments[lineSegments.Count - 1].Point1 = point;
						point = point1;
					}
					num1++;
					if (num1 <= 2 * lines.Count)
					{
						continue;
					}
					throw new GetProjectedShape.ShapeNotFoundException("endless cycle");
				}
				while (!Geo.CompareTwoPoints2D(point, point1) && (!Geo.CompareTwoPoints2D(point, firstLastPoint) || num1 <= 2));
				return lineSegments;
			}

			public static List<Point> PointsFromConvexHull(List<LineSegment> lines)
			{
				List<Point> points = new List<Point>();
				foreach (LineSegment line in lines)
				{
					points.Add(line.Point1);
					points.Add(line.Point2);
				}
				GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints(points);
				Geo.ConvexHull(ref points, false);
				return points;
			}

			private static void RemoveDuplicitPoints(IList points)
			{
				for (int i = 0; i < points.Count; i++)
				{
					for (int j = points.Count - 1; j > i; j--)
					{
						if (Geo.CompareTwoPoints2D((Point)points[i], (Point)points[j]))
						{
							points.RemoveAt(j);
						}
					}
				}
			}
		}

		private class ShapeNotFoundException : Exception
		{
			public ShapeNotFoundException(string message) : base(message)
			{
			}
		}
	}
}