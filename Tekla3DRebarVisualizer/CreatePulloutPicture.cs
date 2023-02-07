using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using SWM = System.Windows.Media;
using SWS = System.Windows.Shapes;
using TSD = Tekla.Structures.Drawing;
using ExtLibrary;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.UI;

namespace Tekla3DRebarVisualizer
{
    public static class CreatePulloutPicture
    {
        private static double Scale;
        private static string LineProperties;
        private static string TextProperties;
        private static List<string> BrushesNames = new List<string>();
        private static SingleRebar CurrentSingleRebar = null;
        private static RebarGroup CurrentRebarGroup = null;
        private static RotationMatrices RotationMatrices = null;
        private static double TotalLength;
        private static List<TSD.Line> TeklaLines;
        private static Point PickedPoint = null;
        private static TSD.ViewBase ViewBase = null;

        public static string CreatePicture(double MatrixRotateByX, double MatrixRotateByY, double MatrixRotateByZ, ObservableCollection<SWS.Shape> canvasLines)
        {
            Model model = new Model();

            WorkPlaneHandler wph = model.GetWorkPlaneHandler();
            TransformationPlane originalPlane = wph.GetCurrentTransformationPlane();

            wph.SetCurrentTransformationPlane(new TransformationPlane());

            TSD.DrawingHandler drawingHandler = new TSD.DrawingHandler();

            if (!CheckIfDrawingIsOpened())
                return "Drawing is not opened";

            TSD.Drawing activeDrawing = drawingHandler.GetActiveDrawing();

            TSD.UI.Picker picker = drawingHandler.GetPicker();

            TSD.DrawingObject drawingObject = null;
            try
            {
                picker.PickObject("Choose rebar:", out drawingObject, out ViewBase);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            if (drawingObject == null)
                return "CHoose rebar which you wish to draw";


            TeklaLines = new List<TSD.Line>();
            if (drawingObject is TSD.ReinforcementSingle)
            {
                TSD.ReinforcementSingle currentObject = drawingObject as TSD.ReinforcementSingle;

                Tekla.Structures.Model.ModelObject pickedObject = model.SelectModelObject(currentObject.ModelIdentifier);
                if (pickedObject is SingleRebar)
                {
                    CurrentSingleRebar = pickedObject as SingleRebar;
                    ArrayList points = new ArrayList();
                    foreach (Point item in CurrentSingleRebar.Polygon.Points)
                    {
                        points.Add($"{item.X} + {item.Y} + {item.Z}");
                    }
                    Draw(canvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ);
                }
            }
            else if (drawingObject is TSD.ReinforcementGroup)
            {
                TSD.ReinforcementGroup currentObject = drawingObject as TSD.ReinforcementGroup;

                Tekla.Structures.Model.ModelObject pickedObject = model.SelectModelObject(currentObject.ModelIdentifier);
                if (pickedObject is RebarGroup)
                {
                    CurrentRebarGroup = pickedObject as RebarGroup;
                    CreateSingleRebarData(CurrentRebarGroup);
                    Draw(canvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ);
                }
            }

            wph.SetCurrentTransformationPlane(originalPlane);

            return "";
        }

        public static void CreateSingleRebarData(RebarGroup rebarGroup)
        {
            CurrentSingleRebar = new SingleRebar();
            CurrentSingleRebar.Polygon = rebarGroup.Polygons[0] as Tekla.Structures.Model.Polygon;
            CurrentSingleRebar.Size = rebarGroup.Size;
            CurrentSingleRebar.StartPointOffsetValue = rebarGroup.StartPointOffsetValue;
            CurrentSingleRebar.EndPointOffsetValue = rebarGroup.EndPointOffsetValue;
            CurrentSingleRebar.RadiusValues = new System.Collections.ArrayList { rebarGroup.RadiusValues[0] };
        }

        public static string InsertIntoDrawing(double MatrixRotateByX, double MatrixRotateByY, double MatrixRotateByZ, ObservableCollection<SWS.Shape> canvasLines, double pulloutScale, string pulloutLineProp, string pulloutTextProp)
        {
            Scale = pulloutScale;
            LineProperties = pulloutLineProp;
            TextProperties = pulloutTextProp;

            Model model = new Model();

            WorkPlaneHandler wph = model.GetWorkPlaneHandler();
            TransformationPlane originalPlane = wph.GetCurrentTransformationPlane();

            wph.SetCurrentTransformationPlane(new TransformationPlane());

            TSD.DrawingHandler drawingHandler = new TSD.DrawingHandler();

            if (!CheckIfDrawingIsOpened())
                return "Drawing is not opened";

            TSD.Drawing activeDrawing = drawingHandler.GetActiveDrawing();

            TSD.UI.Picker picker = drawingHandler.GetPicker();

            try
            {
                picker.PickPoint("Pick insertion point:", out PickedPoint, out ViewBase);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            if (PickedPoint == null)
                return "Pick insertion point correctly";

            Draw(canvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, true);

            activeDrawing.CommitChanges();
            wph.SetCurrentTransformationPlane(originalPlane);

            return "";
        }

        public static void Draw(ObservableCollection<SWS.Shape> canvasLines, double MatrixRotateByX, double MatrixRotateByY, double MatrixRotateByZ, bool insertIntoDrawing = false, bool demoMode = false, SingleRebar demoBar = null)
        {
            if (demoMode)
            {
                CurrentSingleRebar = demoBar;
                TeklaLines = new List<TSD.Line>();
            }
            else if (CurrentSingleRebar == null)
                return;

            TotalLength = 0;
            TeklaLines.Clear();
            canvasLines.Clear();
            RotationMatrices = new RotationMatrices(MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ);

            List<TSD.Line> modelLines = new List<TSD.Line>();
            List<SWS.Line> swsLines = new List<SWS.Line>();
            TSD.Line tsdModelLine = null;
            bool doOnce = true;
            Vector translationVector = new Vector();
            Vector translationVectorBrush = new Vector();

            SWM.BrushConverter converter = new SWM.BrushConverter();

            //
            for (int i = 0; i < CurrentSingleRebar.Polygon.Points.Count - 1; i++)
            {
                Point thisPoint = new Point(CurrentSingleRebar.Polygon.Points[i] as Point);
                Point nextPoint = new Point(CurrentSingleRebar.Polygon.Points[i + 1] as Point);
                Point thisPointScaled = new Point(thisPoint);
                Point nextPointScaled = new Point(nextPoint);

                if (!demoMode)
                {
                    tsdModelLine = new TSD.Line(ViewBase, thisPoint, nextPoint);
                    modelLines.Add(tsdModelLine);
                }

                TransformTo(ref thisPoint, RotationMatrices.RotateAboutZ);
                TransformTo(ref nextPoint, RotationMatrices.RotateAboutZ);

                TransformTo(ref thisPoint, RotationMatrices.RotateAboutY);
                TransformTo(ref nextPoint, RotationMatrices.RotateAboutY);

                TransformTo(ref thisPoint, RotationMatrices.RotateAboutX);
                TransformTo(ref nextPoint, RotationMatrices.RotateAboutX);

                thisPointScaled = new Point(thisPoint.X * Scale, thisPoint.Y * Scale, thisPoint.Z * Scale);
                nextPointScaled = new Point(nextPoint.X * Scale, nextPoint.Y * Scale, nextPoint.Z * Scale);

                if (doOnce)
                {
                    Point pointForBrush = new Point(0, 0, 0);
                    if (insertIntoDrawing)
                        translationVector = new Vector(PickedPoint.X - thisPointScaled.X, PickedPoint.Y - thisPointScaled.Y, PickedPoint.Z - thisPointScaled.Z);
                    translationVectorBrush = new Vector(pointForBrush.X - thisPoint.X, pointForBrush.Y - thisPoint.Y, pointForBrush.Z - thisPoint.Z);

                    doOnce = false;
                }

                Point thisPointBrush = new Point(thisPoint);
                Point nextPointBrush = new Point(nextPoint);

                thisPointBrush.Translate(translationVectorBrush.X, translationVectorBrush.Y, translationVectorBrush.Z);
                nextPointBrush.Translate(translationVectorBrush.X, translationVectorBrush.Y, translationVectorBrush.Z);

                if (insertIntoDrawing)
                {
                    thisPointScaled.Translate(translationVector.X, translationVector.Y, translationVector.Z);
                    nextPointScaled.Translate(translationVector.X, translationVector.Y, translationVector.Z);

                    AddLinesAndTexts(thisPointScaled, nextPointScaled, tsdModelLine, i);
                    if (i == CurrentSingleRebar.Polygon.Points.Count - 2)
                        CreateAnglesTexts(TeklaLines, modelLines);
                }

                SWM.Brush brush = converter.ConvertFromString(BrushesNames[i + 20]) as SWM.Brush;
                SWS.Line line = new SWS.Line();
                line.Stroke = brush;
                line.X1 = thisPointBrush.X;
                line.X2 = nextPointBrush.X;
                line.Y1 = -thisPointBrush.Y;
                line.Y2 = -nextPointBrush.Y;
                line.StrokeThickness = 5;
                swsLines.Add(line);
            }

            if (insertIntoDrawing)
            {
                //BoundingBoxOfLines boundingBoxOfLinesDrawing = new BoundingBoxOfLines(TeklaLines);
                AddRebarPosInfo(TeklaLines);
            }

            BoundingBoxOfLines boundingBoxOfLines = new BoundingBoxOfLines(swsLines);

            foreach (SWS.Line swsLine in swsLines)
            {
                swsLine.X1 += Math.Abs(boundingBoxOfLines.minX);
                swsLine.X2 += Math.Abs(boundingBoxOfLines.minX);
                swsLine.Y1 -= Math.Abs(boundingBoxOfLines.maxY);
                swsLine.Y2 -= Math.Abs(boundingBoxOfLines.maxY);

                double scale = 1;

                if (boundingBoxOfLines.Height > boundingBoxOfLines.Width)
                {
                    if (boundingBoxOfLines.Height != 0)
                        scale = 150 / boundingBoxOfLines.Height;
                }
                else
                {
                    if (boundingBoxOfLines.Width != 0)
                        scale = 150 / boundingBoxOfLines.Width;
                }

                swsLine.X1 *= scale;
                swsLine.X2 *= scale;
                swsLine.Y1 *= scale;
                swsLine.Y2 *= scale;

                canvasLines.Add(swsLine);
            }
        }

        private static void AddRebarPosInfo(List<TSD.Line> teklalines)
        {
            //Point middlePoint = new Point((boundingboxoflinesdrawing.minX + boundingboxoflinesdrawing.maxX)/2, boundingboxoflinesdrawing.minY - boundingboxoflinesdrawing.Height/10);
            Point insertPoint = new Point(teklalines[0].StartPoint);

            int number = 0;
            string position = "";

            CurrentSingleRebar.GetReportProperty("NUMBER", ref number);
            CurrentSingleRebar.GetReportProperty("REBAR_POS", ref position);
            if (CurrentRebarGroup != null)
            {
                CurrentRebarGroup.GetReportProperty("NUMBER", ref number);
                CurrentRebarGroup.GetReportProperty("REBAR_POS", ref position);
            }
            string rebarInfo = $"{number} Ø{CurrentSingleRebar.Size} {position}, L={Math.Round(TotalLength, 0)}, R={CurrentSingleRebar.RadiusValues[0]}";

            TSD.Text text = new TSD.Text(ViewBase, insertPoint, $"{rebarInfo}");
            TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes(TextProperties);
            textAttributes.Font.Height += 1;
            text.Attributes = textAttributes;
            text.Insert();
        }

        private static void AddLinesAndTexts(Point thisPointScale, Point nextPointScale, TSD.Line modelLine, int i)
        {

            TSD.Line tsdLine = new TSD.Line(ViewBase, thisPointScale, nextPointScale);
            double distance = Math.Round(cs_net_lib.Geo.GetDistanceBeetveenTwoPoints3D(modelLine.StartPoint, modelLine.EndPoint), 0);
            if (i == 0 && CurrentSingleRebar.StartPointOffsetValue != 0)
                distance = Math.Round(CurrentSingleRebar.StartPointOffsetValue, 0);
            else if (i == CurrentSingleRebar.Polygon.Points.Count - 2 && CurrentSingleRebar.StartPointOffsetValue != 0)
                distance = Math.Round(CurrentSingleRebar.EndPointOffsetValue, 0);

            Point middlePoint = cs_net_lib.Geo.GetCenterPoint3D(thisPointScale, nextPointScale);
            TSD.Text text = new TSD.Text(ViewBase, middlePoint, $"{distance}");
            TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes(LineProperties);
            textAttributes.Font.Color = TSD.DrawingColors.Black;
            text.Attributes = textAttributes;
            TeklaLines.Add(tsdLine);
            TotalLength += distance;
            //
            tsdLine.Insert();
            text.Insert();
            //
        }

        private static void CreateAnglesTexts(List<TSD.Line> InsertedTeklaLines, List<TSD.Line> realModelLines)
        {
            for (int i = 0; i < InsertedTeklaLines.Count - 1; i++)
            {
                TSD.Line firstLine = InsertedTeklaLines[i];
                TSD.Line nextLine = InsertedTeklaLines[i + 1];

                double firstLineDist = cs_net_lib.Geo.GetDistanceBeetveenTwoPoints3D(firstLine.StartPoint, firstLine.EndPoint);
                double nextLineDist = cs_net_lib.Geo.GetDistanceBeetveenTwoPoints3D(firstLine.StartPoint, firstLine.EndPoint);
                double firstLineOffset = Math.Round(firstLineDist, 0) / 20;
                double nextLineOffset = Math.Round(nextLineDist, 0) / 20;

                Vector firstVector = new Vector(firstLine.StartPoint.X - firstLine.EndPoint.X, firstLine.StartPoint.Y - firstLine.EndPoint.Y, firstLine.StartPoint.Z - firstLine.EndPoint.Z);
                Vector nextVector = new Vector(nextLine.EndPoint.X - nextLine.StartPoint.X, nextLine.EndPoint.Y - nextLine.StartPoint.Y, nextLine.EndPoint.Z - nextLine.StartPoint.Z);
                firstVector = firstVector.GetNormal();
                nextVector = nextVector.GetNormal();

                Vector bisectorVector = new Vector(firstVector + nextVector);
                bisectorVector = bisectorVector.GetNormal();

                #region old Arc
                //GET ARC INFO
                //Vector firstPerp = firstVector.PerpendicularClockwise2D();
                //Vector nextPerp = nextVector.PerpendicularClockwise2D();

                //double firstDot = firstPerp.Dot(nextVector);
                //if (firstDot < 0)
                //    firstPerp.GetOpposite();

                //double secondDot = nextPerp.Dot(firstVector);
                //if (secondDot < 0)
                //    nextPerp.GetOpposite();

                //double radius = 20;
                //Point offsFirstLineStPt = new Point(firstLine.StartPoint.X + (firstPerp.X * radius), firstLine.StartPoint.Y + (firstPerp.Y * radius), 0);
                //Point offsFirstLineEndPt = new Point(firstLine.EndPoint.X + (firstPerp.X * radius), firstLine.EndPoint.Y + (firstPerp.Y * radius), 0);

                //Point offsNextLineStPt = new Point(nextLine.StartPoint.X + (nextPerp.X * radius), nextLine.StartPoint.Y + (nextPerp.Y * radius), 0);
                //Point offsNextLineEndPt = new Point(nextLine.EndPoint.X + (nextPerp.X * radius), nextLine.EndPoint.Y + (nextPerp.Y * radius), 0);

                //Point interPoint = new Point();
                //FindIntersection(offsFirstLineStPt, offsFirstLineEndPt, offsNextLineStPt, offsNextLineEndPt, out interPoint);

                //Point centralPoint = new Point(interPoint.X + (bisectorVector.X * radius * -1), interPoint.Y + (bisectorVector.Y * radius * -1), 0);
                //Point firstPoint = new Point(interPoint.X + (firstPerp.X * radius * -1), interPoint.Y + (firstPerp.Y * radius * -1), 0);
                //Point thirdPoint = new Point(interPoint.X + (nextPerp.X * radius * -1), interPoint.Y + (nextPerp.Y * radius * -1), 0);
                //
                #endregion

                TSD.Line firstModelLine = realModelLines[i];
                TSD.Line nextModelLine = realModelLines[i + 1];
                Vector firstModelVector = new Vector(firstModelLine.StartPoint.X - firstModelLine.EndPoint.X, firstModelLine.StartPoint.Y - firstModelLine.EndPoint.Y, firstModelLine.StartPoint.Z - firstModelLine.EndPoint.Z);
                Vector nextModelVector = new Vector(nextModelLine.EndPoint.X - nextModelLine.StartPoint.X, nextModelLine.EndPoint.Y - nextModelLine.StartPoint.Y, nextModelLine.EndPoint.Z - nextModelLine.StartPoint.Z);
                firstModelVector = firstModelVector.GetNormal();
                nextModelVector = nextModelVector.GetNormal();

                double angleValue = firstModelVector.GetAngleBetween(nextModelVector);
                angleValue = Math.Round((180 * angleValue) / Math.PI, 0);

                bool isInverted = false;
                bool reverseAngle = false;
                Point[] points = GetRoundedCornerPoints(firstLine.EndPoint, nextLine.EndPoint, firstLine.StartPoint, (double)CurrentSingleRebar.RadiusValues[0] * 0.5, out isInverted, out reverseAngle);

                TSD.PointList pointList = new TSD.PointList();

                foreach (Point item in points)
                {
                    pointList.Add(item);
                }
                TSD.Polyline polyline = new TSD.Polyline(ViewBase, pointList);
                TSD.Polyline.PolylineAttributes polylineAttributes = new TSD.Polyline.PolylineAttributes(LineProperties);
                polyline.Attributes = polylineAttributes;
                polyline.Insert();

                Point arcPt3 = new Point(nextLine.StartPoint);
                if (!isInverted)
                {
                    firstLine.EndPoint = pointList[pointList.Count - 1];
                    nextLine.StartPoint = pointList[0];
                }
                else
                {
                    firstLine.EndPoint = pointList[0];
                    nextLine.StartPoint = pointList[pointList.Count - 1];
                }
                firstLine.Modify();
                nextLine.Modify();

                Point arcPt1 = new Point(firstLine.EndPoint);
                arcPt1 = new Point(arcPt1.X + (firstVector.X * firstLineOffset), arcPt1.Y + (firstVector.Y * firstLineOffset), arcPt1.Z + (firstVector.Z * firstLineOffset));
                Point arcPt2 = new Point(nextLine.StartPoint);
                arcPt2 = new Point(arcPt2.X + (nextVector.X * nextLineOffset), arcPt2.Y + (nextVector.Y * nextLineOffset), arcPt2.Z + (nextVector.Z * nextLineOffset));

                arcPt3 = new Point(arcPt3.X + (bisectorVector.X * (firstLineOffset + nextLineOffset) / 2), arcPt3.Y + (bisectorVector.Y * (firstLineOffset + nextLineOffset) / 2), arcPt3.Z + (bisectorVector.Z * (firstLineOffset + nextLineOffset) / 2));

                if (angleValue > 0)
                {
                    TSD.Arc arcAngle;
                    if (!isInverted)
                        arcAngle = new TSD.Arc(ViewBase, arcPt2, arcPt1, arcPt3);
                    else
                        arcAngle = new TSD.Arc(ViewBase, arcPt1, arcPt2, arcPt3);

                    if (reverseAngle && isInverted)
                        arcAngle = new TSD.Arc(ViewBase, arcPt2, arcPt1, arcPt3);
                    else if (reverseAngle && !isInverted)
                        arcAngle = new TSD.Arc(ViewBase, arcPt1, arcPt2, arcPt3);

                    TSD.Arc.ArcAttributes arcAngleAttributes = new TSD.Arc.ArcAttributes("standard");
                    arcAngleAttributes.Line.Color = TSD.DrawingColors.Cyan;
                    arcAngle.Attributes = arcAngleAttributes;
                    arcAngle.Insert();

                    TSD.Text text = new TSD.Text(ViewBase, arcPt3, $"{angleValue}°");
                    TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes(TextProperties);
                    textAttributes.Font.Color = TSD.DrawingColors.Black;
                    text.Attributes = textAttributes;
                    text.Insert();
                }
            }
        }

        public static bool CheckIfDrawingIsOpened(bool showMessage = true)
        {
            try
            {
                TSD.DrawingHandler drawingHandler = new TSD.DrawingHandler();
                TSD.Drawing drawing = drawingHandler.GetActiveDrawing();
                string name = drawing.Name;
                DrawingObjectSelector drawingObjectSelectorTest = drawingHandler.GetDrawingObjectSelector();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static Vector PerpendicularClockwise2D(this Vector vector2)
        {
            return new Vector(vector2.Y, -vector2.X, 0);
        }

        public static Vector PerpendicularCounterClockwise2D(this Vector vector2)
        {
            return new Vector(-vector2.Y, vector2.X, 0);
        }

        public static void GenerateBrushesList()
        {
            Type t = typeof(System.Windows.Media.Brushes);
            PropertyInfo[] colors = t.GetProperties();
            foreach (PropertyInfo pColor in colors)
                BrushesNames.Add(pColor.Name);
        }

        private static void TransformTo(ref Point p, Matrix mat)
        {
            p = new Point
            {
                X = p.X * mat[0, 0] + p.Y * mat[0, 1] + p.Z * mat[0, 2],
                Y = p.X * mat[1, 0] + p.Y * mat[1, 1] + p.Z * mat[1, 2],
                Z = p.X * mat[2, 0] + p.Y * mat[2, 1] + p.Z * mat[2, 2]
            };
        }

        private static void TransformFrom(ref Point p, Matrix mat)
        {
            p = new Point
            {
                X = p.X * mat[0, 0] + p.Y * mat[1, 0] + p.Z * mat[2, 0],
                Y = p.X * mat[0, 1] + p.Y * mat[1, 1] + p.Z * mat[2, 1],
                Z = p.X * mat[0, 2] + p.Y * mat[1, 2] + p.Z * mat[2, 2]
            };
        }

        public static Vector GetOpposite(this Vector vector)
        {
            vector.X = 0.0 - vector.X;
            vector.Y = 0.0 - vector.Y;
            vector.Z = 0.0 - vector.Z;
            return vector;
        }

        // Find the point of intersection between
        // the lines p1 --> p2 and p3 --> p4.
        private static void FindIntersection(
            Point p1, Point p2, Point p3, Point p4,
            out Point intersection)
        {
            bool lines_intersect, segments_intersect;
            Point close_p1 = new Point();
            Point close_p2 = new Point();

            // Get the segments' parameters.
            double dx12 = p2.X - p1.X;
            double dy12 = p2.Y - p1.Y;
            double dx34 = p4.X - p3.X;
            double dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Point(float.NaN, float.NaN);
                close_p1 = new Point(float.NaN, float.NaN);
                close_p2 = new Point(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            double t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Point(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Point(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        private static Point[] GetRoundedCornerPoints(Point angularPoint, Point p1, Point p2, double radius, out bool isInverted, out bool reverseAngle)
        {
            isInverted = false;
            reverseAngle = false;
            //Vector 1
            double dx1 = angularPoint.X - p1.X;
            double dy1 = angularPoint.Y - p1.Y;

            //Vector 2
            double dx2 = angularPoint.X - p2.X;
            double dy2 = angularPoint.Y - p2.Y;

            //Angle between vector 1 and vector 2 divided by 2
            double angle = (Math.Atan2(dy1, dx1) - Math.Atan2(dy2, dx2)) / 2;

            // The length of segment between angular point and the
            // points of intersection with the circle of a given radius
            double tan = Math.Abs(Math.Tan(angle));
            double segment = radius / tan;

            //Check the segment
            double length1 = GetLength(dx1, dy1);
            double length2 = GetLength(dx2, dy2);

            double length = Math.Min(length1, length2);

            if (segment > length)
            {
                segment = length;
                radius = (float)(length * tan);
            }

            // Points of intersection are calculated by the proportion between 
            // the coordinates of the vector, length of vector and the length of the segment.
            var p1Cross = GetProportionPoint(angularPoint, segment, length1, dx1, dy1);
            var p2Cross = GetProportionPoint(angularPoint, segment, length2, dx2, dy2);

            // Calculation of the coordinates of the circle 
            // center by the addition of angular vectors.
            double dx = angularPoint.X * 2 - p1Cross.X - p2Cross.X;
            double dy = angularPoint.Y * 2 - p1Cross.Y - p2Cross.Y;

            double L = GetLength(dx, dy);
            double d = GetLength(segment, radius);

            var circlePoint = GetProportionPoint(angularPoint, d, L, dx, dy);

            //StartAngle and EndAngle of arc
            var startAngle = Math.Atan2(p1Cross.Y - circlePoint.Y, p1Cross.X - circlePoint.X);
            var endAngle = Math.Atan2(p2Cross.Y - circlePoint.Y, p2Cross.X - circlePoint.X);

            //Sweep angle
            var sweepAngle = endAngle - startAngle;

            //Some additional checks
            if (sweepAngle < 0)
            {
                startAngle = endAngle;
                sweepAngle = -sweepAngle;
                isInverted = true;
            }

            if (sweepAngle > Math.PI)
            {
                sweepAngle = -(2 * Math.PI - sweepAngle);
                reverseAngle = true;
            }

            //Draw result using graphics
            int degreeFactor = 10;
            int pointsCount = (int)Math.Abs(sweepAngle * degreeFactor);
            int sign = Math.Sign(sweepAngle);

            Point[] points = new Point[pointsCount];

            for (int i = 0; i < pointsCount; i++)
            {
                var pointX =
                   (float)(circlePoint.X
                           + Math.Cos(startAngle + sign * (double)i / degreeFactor)
                           * radius);

                var pointY =
                   (float)(circlePoint.Y
                           + Math.Sin(startAngle + sign * (double)i / degreeFactor)
                           * radius);

                points[i] = new Point(pointX, pointY);
            }

            return points;
        }

        private static double GetLength(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static Point GetProportionPoint(Point point, double segment,
                                          double length, double dx, double dy)
        {
            double factor = segment / length;

            return new Point((float)(point.X - dx * factor),
                              (float)(point.Y - dy * factor));
        }
    }

    public class RotationMatrices
    {
        public Matrix RotateAboutX { get; set; }
        public Matrix RotateAboutY { get; set; }
        public Matrix RotateAboutZ { get; set; }
        public Matrix GeneralRotation { get; set; }

        public RotationMatrices(double rotX, double rotY, double rotZ)
        {
            rotX = (Math.PI * rotX) / 180;
            rotY = (Math.PI * rotY) / 180;
            rotZ = (Math.PI * rotZ) / 180;

            RotateAboutX = new Matrix
            {
                [0, 0] = 1,
                [0, 1] = 0,
                [0, 2] = 0,
                [1, 0] = 0,
                [1, 1] = Math.Cos(rotX),
                [1, 2] = -Math.Sin(rotX),
                [2, 0] = 0,
                [2, 1] = Math.Sin(rotX),
                [2, 2] = Math.Cos(rotX)
            };

            RotateAboutY = new Matrix
            {
                [0, 0] = Math.Cos(rotY),
                [0, 1] = 0,
                [0, 2] = Math.Sin(rotY),
                [1, 0] = 0,
                [1, 1] = 1,
                [1, 2] = 0,
                [2, 0] = -Math.Sin(rotY),
                [2, 1] = 0,
                [2, 2] = Math.Cos(rotY)
            };

            RotateAboutZ = new Matrix
            {
                [0, 0] = Math.Cos(rotZ),
                [0, 1] = -Math.Sin(rotZ),
                [0, 2] = 0,
                [1, 0] = Math.Sin(rotZ),
                [1, 1] = Math.Cos(rotZ),
                [1, 2] = 0,
                [2, 0] = 0,
                [2, 1] = 0,
                [2, 2] = 1
            };

            GeneralRotation = RotateAboutZ * RotateAboutY * RotateAboutX;
        }
    }
}

