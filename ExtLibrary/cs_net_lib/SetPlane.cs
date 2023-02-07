using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
	public class SetPlane
	{
		private readonly List<ModelObject> dynamicObjects;

		private readonly List<Point> dynamicPoints;

		private readonly List<Polygon> dynamicPolygons;

		private readonly List<Matrix> listOfMatrices;

		private readonly List<TransformationPlane> listOfOriginalPlanes;

		private readonly Tekla.Structures.Model.Model model;

		public SetPlane(Tekla.Structures.Model.Model actualModel)
		{
			this.dynamicPoints = new List<Point>();
			this.dynamicPolygons = new List<Polygon>();
			this.dynamicObjects = new List<ModelObject>();
			this.listOfMatrices = new List<Matrix>();
			this.listOfOriginalPlanes = new List<TransformationPlane>();
			this.model = actualModel;
		}

		public void AddArrayList(ArrayList list)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is ArrayList)
					{
						this.AddList(list[i] as ArrayList);
					}
					else if (list[i] is Point)
					{
						Point item = (Point)list[i];
						if (item != null)
						{
							this.AddPoints(new Point[] { item });
						}
					}
					else if (list[i] is Polygon)
					{
						Polygon polygon = (Polygon)list[i];
						if (polygon != null)
						{
							this.AddPolygons(new Polygon[] { polygon });
						}
					}
					else if (list[i] is ModelObject)
					{
						ModelObject[] modelObjectArray = new ModelObject[] { list[i] as ModelObject };
						this.AddModelObjects(modelObjectArray);
					}
				}
			}
		}

		public void AddList(IList list)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is IList)
					{
						this.AddList(list[i] as IList);
					}
					else if (list[i] is Point)
					{
						Point item = (Point)list[i];
						if (item != null)
						{
							this.AddPoints(new Point[] { item });
						}
					}
					else if (list[i] is Polygon)
					{
						Polygon polygon = (Polygon)list[i];
						if (polygon != null)
						{
							this.AddPolygons(new Polygon[] { polygon });
						}
					}
					else if (list[i] is ModelObject)
					{
						ModelObject[] modelObjectArray = new ModelObject[] { list[i] as ModelObject };
						this.AddModelObjects(modelObjectArray);
					}
				}
			}
		}

		public void AddModelObjects(params ModelObject[] dynamicObjectsToAdd)
		{
			this.dynamicObjects.AddRange(dynamicObjectsToAdd);
		}

		public void AddPoints(params Point[] dynamicPointsToAdd)
		{
			this.dynamicPoints.AddRange(dynamicPointsToAdd);
		}

		public void AddPolygons(params Polygon[] dynamicPolygonsToAdd)
		{
			this.dynamicPolygons.AddRange(dynamicPolygonsToAdd);
		}

		public void Begin(CoordinateSystem newSystem)
		{
			this.Begin(newSystem.Origin, newSystem.AxisX, newSystem.AxisY);
		}

		public void Begin(Point newOrigin, Point pointInNewX, Point pointInNewY)
		{
			this.Begin(newOrigin, new Vector(pointInNewX - newOrigin), new Vector(pointInNewY - newOrigin));
		}

		public void Begin(Point newOrigin, Vector newVectorX, Vector newVectorY)
		{
			try
			{
				Vector normalVectorInPlane = Geo.GetNormalVectorInPlane(newVectorX, newVectorY);
				CoordinateSystem coordinateSystem = new CoordinateSystem(newOrigin, newVectorX, normalVectorInPlane);
				TransformationPlane transformationPlane = new TransformationPlane(coordinateSystem);
				TransformationPlane currentTransformationPlane = this.model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
				this.listOfOriginalPlanes.Add(currentTransformationPlane);
				Matrix matrix = MatrixFactory.ToCoordinateSystem(coordinateSystem);
				this.listOfMatrices.Add(matrix);
				this.TransformAll(matrix);
				this.model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);
				this.TransformModelObjects();
			}
			catch (Exception exception)
			{
				exception.ToString();
			}
		}

		public void Begin()
		{
			Matrix transformationMatrixToLocal = this.model.GetWorkPlaneHandler().GetCurrentTransformationPlane().TransformationMatrixToLocal;
			Point point = transformationMatrixToLocal.Transform(new Point(0, 0, 0));
			Point point1 = transformationMatrixToLocal.Transform(new Point(1000, 0, 0));
			Point point2 = transformationMatrixToLocal.Transform(new Point(0, 1000, 0));
			this.Begin(point, point1, point2);
		}

		public void BeginZ(Point newOrigin, Vector newVectorX, Vector newVectorZ)
		{
			Vector vector = newVectorX.Cross(newVectorZ);
			vector = new Vector(-vector.X, -vector.Y, -vector.Z);
			this.Begin(newOrigin, newVectorX, vector);
		}

		public void BeginZ(CoordinateSystem newSystem)
		{
			this.BeginZ(newSystem.Origin, newSystem.AxisX, newSystem.AxisY);
		}

		public void BeginZ(Point newOrigin, Point pointInNewX, Point pointInNewZ)
		{
			this.BeginZ(newOrigin, new Vector(pointInNewX - newOrigin), new Vector(pointInNewZ - newOrigin));
		}

		public void End()
		{
			int count = this.listOfMatrices.Count - 1;
			int num = this.listOfOriginalPlanes.Count - 1;
			if (count > -1 && num > -1)
			{
				Matrix item = this.listOfMatrices[count];
				item.Transpose();
				this.TransformAll(item);
				TransformationPlane transformationPlane = this.listOfOriginalPlanes[num];
				this.model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlane);
				this.TransformModelObjects();
				this.listOfMatrices.RemoveAt(count);
				this.listOfOriginalPlanes.RemoveAt(num);
			}
		}

		public Point Point(double valueX, double valueY, double valueZ)
		{
			Point point = new Point(valueX, valueY, valueZ);
			this.dynamicPoints.Add(point);
			return point;
		}

		public Polygon Polygon()
		{
			Polygon polygon = new Polygon();
			this.dynamicPolygons.Add(polygon);
			return polygon;
		}

		public void RemoveAllModelObjects()
		{
			this.dynamicObjects.Clear();
		}

		public void RemoveAllPoints()
		{
			this.dynamicPoints.Clear();
		}

		public void RemoveAllPolygons()
		{
			this.dynamicPolygons.Clear();
		}

		public void RemoveArrayList(ArrayList list)
		{
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is ArrayList)
					{
						this.RemoveArrayList(list[i] as ArrayList);
					}
					else if (list[i] is Point)
					{
						Point item = (Point)list[i];
						if (item != null)
						{
							this.RemovePoints(new Point[] { item });
						}
					}
					else if (list[i] is Polygon)
					{
						Polygon polygon = (Polygon)list[i];
						if (polygon != null)
						{
							this.RemovePolygons(new Polygon[] { polygon });
						}
					}
					else if (list[i] is ModelObject)
					{
						ModelObject[] modelObjectArray = new ModelObject[] { list[i] as ModelObject };
						this.RemoveModelObjects(modelObjectArray);
					}
				}
			}
		}

		public void RemoveModelObjects(params ModelObject[] dynamicObjectsToRemove)
		{
			for (int i = 0; i < (int)dynamicObjectsToRemove.Length; i++)
			{
				this.dynamicObjects.Remove(dynamicObjectsToRemove[i]);
			}
		}

		public void RemovePoints(params Point[] dynamicPointsToRemove)
		{
			for (int i = 0; i < (int)dynamicPointsToRemove.Length; i++)
			{
				this.dynamicPoints.Remove(dynamicPointsToRemove[i]);
			}
		}

		public void RemovePolygons(params Polygon[] dynamicPolygonsToRemove)
		{
			for (int i = 0; i < (int)dynamicPolygonsToRemove.Length; i++)
			{
				this.dynamicPolygons.Remove(dynamicPolygonsToRemove[i]);
			}
		}

		public void TransformAll(Matrix transformationMatrix)
		{
			if (this.dynamicPoints != null)
			{
				this.TransformPoints(transformationMatrix);
			}
			if (this.dynamicPolygons != null)
			{
				this.TransformPolygons(transformationMatrix);
			}
		}

		public void TransformModelObjects()
		{
			if (this.dynamicObjects != null)
			{
				for (int i = 0; i < this.dynamicObjects.Count; i++)
				{
					this.dynamicObjects[i].Select();
				}
			}
		}

		public void TransformPoints(Matrix transformationMatrix)
		{
			for (int i = 0; i < this.dynamicPoints.Count; i++)
			{
				Point point = transformationMatrix.Transform(this.dynamicPoints[i]);
				Geo.CopyPointPosition(this.dynamicPoints[i], point);
			}
		}

		public void TransformPoints(Matrix transformationMatrix, ArrayList pointsToMoveList)
		{
			try
			{
				for (int i = 0; i < pointsToMoveList.Count; i++)
				{
					Point point = transformationMatrix.Transform((Point)pointsToMoveList[i]);
					Geo.CopyPointPosition((Point)pointsToMoveList[i], point);
				}
			}
			catch (Exception exception)
			{
				exception.ToString();
			}
		}

		public void TransformPolygons(Matrix transformationMatrix)
		{
			for (int i = 0; i < this.dynamicPolygons.Count; i++)
			{
				this.TransformPoints(transformationMatrix, this.dynamicPolygons[i].Points);
			}
		}
	}
}