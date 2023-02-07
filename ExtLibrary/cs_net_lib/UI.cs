using System;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;

namespace cs_net_lib
{
	public static class UI
	{
		public static Color white;

		public static Color black;

		public static Color blue;

		public static Color red;

		public static Color green;

		public static Color purple;

		static UI()
		{
			UI.white = new Color(1, 1, 1);
			UI.black = new Color(0, 0, 0);
			UI.blue = new Color(0, 0, 1);
			UI.red = new Color(1, 0, 0);
			UI.green = new Color(0, 1, 0);
			UI.purple = new Color(1, 0, 1);
		}

		public static void DrawLine(Point p1, Point p2, string comment1, string comment2)
		{
			UI.DrawPoint(p1, comment1);
			UI.DrawPoint(p2, comment2);
			UI.DrawLine(p1, p2, UI.red);
		}

		public static void DrawLine(Point p1, Point p2, Color textColor)
		{
			(new GraphicsDrawer()).DrawLineSegment(p1, p2, textColor);
		}

		public static void DrawLine(Point p1, Point p2)
		{
			UI.DrawLine(p1, p2, UI.red);
		}

		public static void DrawPlane(CoordinateSystem planeCoord, Color lineColor, Color textColor, string comment)
		{
			Vector vector = planeCoord.AxisX.Cross(planeCoord.AxisY);
			planeCoord.AxisX.Normalize(1500);
			planeCoord.AxisY.Normalize(1500);
			vector.Normalize(1500);
			GraphicsDrawer graphicsDrawer = new GraphicsDrawer();
			Point point = new Point(planeCoord.Origin + planeCoord.AxisX);
			Point point1 = new Point(planeCoord.Origin + planeCoord.AxisY);
			Point point2 = new Point(planeCoord.Origin + vector);
			graphicsDrawer.DrawLineSegment(planeCoord.Origin, point, lineColor);
			graphicsDrawer.DrawLineSegment(planeCoord.Origin, point1, lineColor);
			graphicsDrawer.DrawLineSegment(planeCoord.Origin, point2, lineColor);
			graphicsDrawer.DrawText(point, "X", textColor);
			graphicsDrawer.DrawText(point1, "Y", textColor);
			graphicsDrawer.DrawText(point2, "Z", textColor);
			graphicsDrawer.DrawText(planeCoord.Origin, comment, textColor);
		}

		public static void DrawPlane(Color lineColor, Color textColor)
		{
			UI.DrawPlane(new CoordinateSystem(), lineColor, textColor, "");
		}

		public static void DrawPlane(string comment)
		{
			UI.DrawPlane(new CoordinateSystem(), UI.red, UI.blue, comment);
		}

		public static void DrawPlane(CoordinateSystem planeCoord)
		{
			UI.DrawPlane(planeCoord, UI.red, UI.blue, "");
		}

		public static void DrawPlane()
		{
			UI.DrawPlane(new CoordinateSystem(), UI.red, UI.blue, "");
		}

		public static void DrawPoint(Point point, string text)
		{
			UI.DrawPoint(point, text, UI.red);
		}

		public static void DrawPoint(Point point, string text, Color textColor)
		{
			GraphicsDrawer graphicsDrawer = new GraphicsDrawer();
			graphicsDrawer.DrawText(point, string.Concat("...", text), textColor);
		}
	}
}