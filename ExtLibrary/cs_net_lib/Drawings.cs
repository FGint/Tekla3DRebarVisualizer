using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Tekla.Structures.Datatype;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
	public static class Drawings
	{
		private const string NumberRebarsMark = "bm90ZV9kaWFsLmluX2ZpbGUgInN0YW5kYXJkIg0Kbm90ZV9kaWFsLm91dF9maWxlICJudW1iZXJS\r\nZWJhcnMiDQpub3RlX2RpYWwuY3JlYXRlIDANCm5vdGVfZGlhbC5mb3JfbWFpbiAxDQpub3RlX2Rp\r\nYWwuZm9yX3NlYyAxDQpub3RlX2RpYWwuZm9yX3doaWNoX3BhcnRzIDENCm5vdGVfZGlhbC5pbl9h\r\nbGxfdmlld3MgMA0Kbm90ZV9kaWFsLk91dE9mUGxhbmUgMQ0Kbm90ZV9kaWFsLnRleHRfdHlwZSAx\r\nMDAwMg0Kbm90ZV9kaWFsLnRleHRfY29sb3VyIDE1NQ0Kbm90ZV9kaWFsLmZyYW1lX3R5cGUgMg0K\r\nbm90ZV9kaWFsLmxpbmVfY29sb3VyIDE1Mw0Kbm90ZV9kaWFsLkFuZ2xlIDAuMDAwMDAwDQpub3Rl\r\nX2RpYWwuQXJyb3dUeXBlIDANCm5vdGVfZGlhbC5BcnJvd0hlaWdodCAyLjAwMDAwMA0Kbm90ZV9k\r\naWFsLkFycm93TGVuZ3RoIDIuNTAwMDAwDQpub3RlX2RpYWwuaGVpZ2h0IDIuNTAwMDAwDQpub3Rl\r\nX2RpYWwuZGVmYXVsdF9zaXplIDAuMDAwMDAwDQpub3RlX2RpYWwubWFya19uc2ZzIDANCm5vdGVf\r\nZGlhbC5jb250ZW50XzEgMQ0Kbm90ZV9kaWFsLmZsYWdzIDANCm5vdGVfZGlhbC5tYXJrX3VzZV9w\r\nYXJ0X2VuZCAwDQpub3RlX2RpYWwuQ29udGVudFN0cmluZzEgIjw/eG1sIHZlcnNpb249IjEuMCIg\r\nZW5jb2Rpbmc9IlVURi04IiBzdGFuZGFsb25lPSJubyI/PgE8IURPQ1RZUEUgTWFyaz48TWFyaz4B\r\nPE1hcmtQYXJ0IGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1lPSJQQVJUX1BPUyIgdmFsdWU9IiIg\r\nc3ViVHlwZT0iMjMiPgE8Rm9ybWF0IGZvbnQ9IkFyaWFsIE5hcnJvdyIgY29sb3I9IjE1NSIgaGVp\r\nZ2h0PSIyLjUwMCIgaGlkZGVuPSIwIi8+ATwvRWxlbWVudD4BPC9NYXJrUGFydD4BPE1hcmtQYXJ0\r\nIGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1lPSJQUk9GSUxFIiB2YWx1ZT0iIiBzdWJUeXBlPSIy\r\nNCI+ATxGb3JtYXQgZm9udD0iQXJpYWwgTmFycm93IiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAw\r\nIiBoaWRkZW49IjAiLz4BPC9FbGVtZW50PgE8L01hcmtQYXJ0PgE8L01hcms+ASINCm5vdGVfZGlh\r\nbC5Db250ZW50U3RyaW5nMiAiPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0\r\nYW5kYWxvbmU9Im5vIj8+ATwhRE9DVFlQRSBNYXJrPjxNYXJrPgE8TWFya1BhcnQgZ3JvdXBpZD0i\r\nMCI+ATxTaXplRWxlbWVudCB2YWx1ZT0iIiBzdWJUeXBlPSI0NyI+ATxGb3JtYXQgZm9udD0iQXJp\r\nYWwgTmFycm93IiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIiBoaWRkZW49IjAiLz4BPC9TaXpl\r\nRWxlbWVudD4BPC9NYXJrUGFydD4BPC9NYXJrPgEiDQpub3RlX2RpYWwuQ29udGVudFN0cmluZzMg\r\nIjw/eG1sIHZlcnNpb249IjEuMCIgZW5jb2Rpbmc9IlVURi04IiBzdGFuZGFsb25lPSJubyI/PgE8\r\nIURPQ1RZUEUgTWFyaz48TWFyaz4BPE1hcmtQYXJ0IGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1l\r\nPSJOQU1FIiB2YWx1ZT0iIiBzdWJUeXBlPSIzNSI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9y\r\nPSIxNTUiIGhlaWdodD0iMi41MDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+\r\nATwvTWFyaz4BIg0Kbm90ZV9kaWFsLkNvbnRlbnRTdHJpbmc0ICI8P3htbCB2ZXJzaW9uPSIxLjAi\r\nIGVuY29kaW5nPSJVVEYtOCIgc3RhbmRhbG9uZT0ibm8iPz4BPCFET0NUWVBFIE1hcms+PE1hcms+\r\nATxNYXJrUGFydCBncm91cGlkPSIwIj4BPEVsZW1lbnQgbmFtZT0iIiB2YWx1ZT0iIiBzdWJUeXBl\r\nPSI0MSI+ATxGb3JtYXQgZm9udD0iQXJpYWwgTmFycm93IiBjb2xvcj0iMTU4IiBoZWlnaHQ9IjIu\r\nNTAwIiBoaWRkZW49IjAiLz4BPC9FbGVtZW50PgE8L01hcmtQYXJ0PgE8L01hcms+ASINCm5vdGVf\r\nZGlhbC5DaGFtZmVyQ29udGVudFN0cmluZyAiPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0i\r\nVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+ATwhRE9DVFlQRSBNYXJrPjxNYXJrPgE8TWFya1BhcnQg\r\nZ3JvdXBpZD0iMCI+ATxFbGVtZW50IG5hbWU9IlhfRElSRUNUSU9OX1dJRFRIIiB2YWx1ZT0iIiBz\r\ndWJUeXBlPSI4NyI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdodD0iMi41\r\nMDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+ATxNYXJrUGFydCBncm91cGlk\r\nPSIwIj4BPFVzZXJUZXh0PgF4ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdo\r\ndD0iMi41MDAiIGhpZGRlbj0iMCIvPgE8L1VzZXJUZXh0PgE8L01hcmtQYXJ0PgE8TWFya1BhcnQg\r\nZ3JvdXBpZD0iMCI+ATxFbGVtZW50IG5hbWU9IllfRElSRUNUSU9OX1dJRFRIIiB2YWx1ZT0iIiBz\r\ndWJUeXBlPSI4OCI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdodD0iMi41\r\nMDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+ATwvTWFyaz4BIg0Kbm90ZV9k\r\naWFsLlBvdXJDb250ZW50U3RyaW5nICI8P3htbCB2ZXJzaW9uPSIxLjAiIGVuY29kaW5nPSJVVEYt\r\nOCIgc3RhbmRhbG9uZT0ibm8iPz48IURPQ1RZUEUgTWFyaz48TWFyaz48TWFya1BhcnQgZ3JvdXBp\r\nZD0iMCI+PEVsZW1lbnQgbmFtZT0iTUFURVJJQUwiIHZhbHVlPSIiIHN1YlR5cGU9IjEwIj48Rm9y\r\nbWF0IGZvbnQ9IkFyaWFsIiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIi8+PC9FbGVtZW50Pjwv\r\nTWFya1BhcnQ+PC9NYXJrPiINCm5vdGVfZGlhbC5Qb3VyQnJlYWtDb250ZW50U3RyaW5nICI8P3ht\r\nbCB2ZXJzaW9uPSIxLjAiIGVuY29kaW5nPSJVVEYtOCIgc3RhbmRhbG9uZT0ibm8iPz48IURPQ1RZ\r\nUEUgTWFyaz48TWFyaz48TWFya1BhcnQgZ3JvdXBpZD0iMCI+PFVzZXJUZXh0PlBPVVIgQlJFQUs8\r\nRm9ybWF0IGZvbnQ9IkFyaWFsIiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIi8+PC9Vc2VyVGV4\r\ndD48L01hcmtQYXJ0PjwvTWFyaz4iDQpub3RlX2RpYWwubGVhZGVyX2xpbmVfdHlwZSAwDQpub3Rl\r\nX2RpYWwubWVyZ2VfZGlyIDENCm5vdGVfZGlhbC5NZXJnZU1hcmtzIDANCm5vdGVfZGlhbC5DdXJy\r\nZW50Q29udGVudFRhYmxlTnVtYmVyIDINCm5vdGVfZGlhbC5Gb3JTdWJhc3NNYWluIDENCm5vdGVf\r\nZGlhbC5Gb3JTdWJhc3NTZWNvbmRhcnkgMQ0Kbm90ZV9kaWFsLlVzZUhpZGRlbkxpbmVzIDENCm5v\r\ndGVfZGlhbC5Sb3RhdGlvbiAwDQpub3RlX2RpYWwuY3JlYXRlX2VuIDENCm5vdGVfZGlhbC5mb3Jf\r\nbWFpbl9lbiAxDQpub3RlX2RpYWwuZm9yX3NlY19lbiAxDQpub3RlX2RpYWwuZm9yX3doaWNoX3Bh\r\ncnRzX2VuIDENCm5vdGVfZGlhbC5pbl9hbGxfdmlld3NfZW4gMQ0Kbm90ZV9kaWFsLk91dE9mUGxh\r\nbmVfZW4gMQ0Kbm90ZV9kaWFsLnRleHRfdHlwZV9lbiAxDQpub3RlX2RpYWwudGV4dF9jb2xvdXJf\r\nZW4gMQ0Kbm90ZV9kaWFsLmZyYW1lX2VuIDENCm5vdGVfZGlhbC5saW5lX2NvbG91cl9lbiAxDQpu\r\nb3RlX2RpYWwuQW5nbGVfZW4gMQ0Kbm90ZV9kaWFsLkFycm93VHlwZV9lbiAxDQpub3RlX2RpYWwu\r\nQXJyb3dIZWlnaHRfZW4gMQ0Kbm90ZV9kaWFsLkFycm93TGVuZ3RoX2VuIDENCm5vdGVfZGlhbC5o\r\nZWlnaHRfZW4gMQ0Kbm90ZV9kaWFsLmNvbnRlbnRfZW4gMQ0Kbm90ZV9kaWFsLnBsYWNlX2VuIDEN\r\nCm5vdGVfZGlhbC5kZWZhdWx0X3NpemVfZW4gMQ0Kbm90ZV9kaWFsLm1hcmtfbnNmc19lbiAxDQpu\r\nb3RlX2RpYWwudXNlX3BhcnRfZW5kX2VuIDENCm5vdGVfZGlhbC5sZWFkZXJfbGluZV90eXBlX2Vu\r\nIDENCm5vdGVfZGlhbC5tZXJnZV9kaXJfZW4gMQ0Kbm90ZV9kaWFsLk1lcmdlTWFya3NfZW4gMQ0K\r\nbm90ZV9kaWFsLkZvclN1YmFzc01haW5fZW4gMQ0Kbm90ZV9kaWFsLkZvclN1YmFzc1NlY29uZGFy\r\neV9lbiAxDQpub3RlX2RpYWwuVXNlSGlkZGVuTGluZXNfZW4gMQ0Kbm90ZV9kaWFsLlJvdGF0aW9u\r\nX2VuIDENCm5vdGVfZGlhbC50eHBsLnRleHRfcGxhY2luZ19tb2RlIDENCm5vdGVfZGlhbC50eHBs\r\nLnF1YXJ0ZXJfMSAxDQpub3RlX2RpYWwudHhwbC5xdWFydGVyXzIgMg0Kbm90ZV9kaWFsLnR4cGwu\r\ncXVhcnRlcl8zIDQNCm5vdGVfZGlhbC50eHBsLnF1YXJ0ZXJfNCA4DQpub3RlX2RpYWwudHhwbC50\r\nZXh0X2ZyZWVwbF9taW4gMi4wMDAwMDANCm5vdGVfZGlhbC50eHBsLnRleHRfZnJlZXBsX21hcmdp\r\nbiAxLjAwMDAwMA0Kbm90ZV9kaWFsLnR4cGwudGV4dF9wbGFjZV9lbiAxDQpub3RlX2RpYWwudHhw\r\nbC50ZXh0X3F1YXJ0ZXJfZW4gMQ0Kbm90ZV9kaWFsLnR4cGwudGV4dF9mcmVlcGxfbWluX2VuIDEN\r\nCm5vdGVfZGlhbC50eHBsLnRleHRfZnJlZXBsX21hcmdpbl9lbiAxDQpub3RlX2RpYWwuRm9udEF0\r\ndHIuZm9udF9uYW1lICJBcmlhbCBOYXJyb3ciDQpub3RlX2RpYWwuRm9udEF0dHIudGV4dF9jb2xv\r\ndXIgMTU4DQpub3RlX2RpYWwuRm9udEF0dHIuaGVpZ2h0IDIuNTAwMDAwDQpub3RlX2RpYWwuRm9u\r\ndEF0dHIuZm9udF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkZvbnRBdHRyLnRleHRfY29sb3VyX2VuYWJs\r\nZSAxDQpub3RlX2RpYWwuRm9udEF0dHIuaGVpZ2h0X2VuYWJsZSAxDQpub3RlX2RpYWwuRnJhbWVB\r\ndHRyLmZyYW1lX3R5cGUgMQ0Kbm90ZV9kaWFsLkZyYW1lQXR0ci5saW5lX2NvbG91ciAxNTUNCm5v\r\ndGVfZGlhbC5GcmFtZUF0dHIuZnJhbWVfZW5hYmxlIDENCm5vdGVfZGlhbC5GcmFtZUF0dHIubGlu\r\nZV9jb2xvdXJfZW5hYmxlIDENCm5vdGVfZGlhbC5Vbml0QXR0ci5Vbml0IDANCm5vdGVfZGlhbC5V\r\nbml0QXR0ci5QcmVjaXNpb24gMA0Kbm90ZV9kaWFsLlVuaXRBdHRyLkZvcm1hdCAwDQpub3RlX2Rp\r\nYWwuTG9jYXRpb25BdHRyaWJ1dGVzLkRlc2lyZWRMb2NhdGlvbk9uUGFydF9lbmFibGUgMQ0Kbm90\r\nZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5EZXNpcmVkTG9jYXRpb25PblBhcnQgMQ0Kbm90ZV9k\r\naWFsLkxvY2F0aW9uQXR0cmlidXRlcy5Mb2NhdGlvbk9uUGFydCAxDQpub3RlX2RpYWwuTG9jYXRp\r\nb25BdHRyaWJ1dGVzLk9yaWVudGF0aW9uX2VuYWJsZSAxDQpub3RlX2RpYWwuTG9jYXRpb25BdHRy\r\naWJ1dGVzLkFuZ2xlUmVsYXRpdmVUbyAyDQpub3RlX2RpYWwuTG9jYXRpb25BdHRyaWJ1dGVzLkFs\r\naWdubWVudF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5BbGlnbm1lbnQg\r\nMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5QbGFjaW5nRmFsbGJhY2tfZW5hYmxlIDEN\r\nCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMuUGxhY2luZ0ZhbGxiYWNrIDY0DQpub3RlX2Rp\r\nYWwuTG9jYXRpb25BdHRyaWJ1dGVzLk9mZnNldF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9u\r\nQXR0cmlidXRlcy5PZmZzZXQgMC4wMDAwMDANCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMu\r\nTGVhZGVyTGluZURlc2lyZWRfZW5hYmxlIDENCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMu\r\nTGVhZGVyTGluZURlc2lyZWQgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5MZWFkZXJM\r\naW5lSW5Vc2UgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5NaW5pbXVtTWFya1NjYWxl\r\nIDAuMDAwMDAwDQpub3RlX2RpYWwuUmViYXJHcm91cE1hcmtBdHRyaWJ1dGVzLkxpbmVUeXBlIDAN\r\nCm5vdGVfZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuVHlwZSAwDQpub3RlX2RpYWwuUmVi\r\nYXJHcm91cE1hcmtBdHRyaWJ1dGVzLlRhZ2dlZFJlYmFyRGltTWFya0xvY2F0aW9uIDMNCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuQ2xvc2VSZWJhckRpbU1hcmtUb0ZhdGhlciAw\r\nDQpub3RlX2RpYWwuUmViYXJHcm91cE1hcmtBdHRyaWJ1dGVzLkxpbmVUeXBlX2VuIDENCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuVHlwZV9lbiAxDQpub3RlX2RpYWwuUmViYXJH\r\ncm91cE1hcmtBdHRyaWJ1dGVzLlRhZ2dlZFJlYmFyRGltTWFya0xvY2F0aW9uX2VuIDENCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuQ2xvc2VSZWJhckRpbU1hcmtUb0ZhdGhlcl9l\r\nbiAxDQo=";

		private static void ClearFile(this FileInfo file)
		{
			if (file.Exists)
			{
				file.IsReadOnly = false;
				file.Delete();
			}
		}

		public static Symbol DrawSymbol(View view, Point point, DrawingColors color = (DrawingColors)162, string symbolFile = "COG", int symbolIndex = 6, double height = 3, double angle = 0)
		{
			Symbol symbol = null;
			if (symbolFile != null && symbolFile.Length > 0)
			{
				Point point1 = new Point(point);
				Point x = point1;
				x.X = x.X - (double)((int)(height / 2));
				Point y = point1;
				y.Y = y.Y - (double)((int)(height / 2));
				symbol = new Symbol(view, point1, new SymbolInfo(symbolFile, symbolIndex))
				{
					InsertionPoint = point1
				};
				symbol.Attributes.Frame.Type = FrameTypes.None;
				symbol.Attributes.Height = height;
				symbol.Attributes.Color = color;
				symbol.Attributes.PreferredPlacing = PreferredSymbolPlacingTypes.PointPlacingType();
				symbol.Attributes.Angle = angle;
				symbol.Insert();
			}
			return symbol;
		}

		public static int GetCodeValue(this Enum value)
		{
			int dEF = (int)cs_net_lib.Constants.DEF;
			FieldInfo field = value.GetType().GetField(value.ToString());
			Drawings.DrawingMark.Code[] customAttributes = field.GetCustomAttributes(typeof(Drawings.DrawingMark.Code), false) as Drawings.DrawingMark.Code[];
			if ((int)customAttributes.Length > 0)
			{
				dEF = customAttributes[0].Value;
			}
			return dEF;
		}

		private static List<Point> GetCustomizedRebarPoints(Reinforcement reinforcement, int startRebarIndex, int endRebarIndex, double customPosition, Point startPoint, Point endPoint, bool createNormalVector)
		{
			List<Point> points = new List<Point>();
			if (reinforcement != null)
			{
				ArrayList rebarGeometries = reinforcement.GetRebarGeometries(false);
				if (startRebarIndex >= 0 && startRebarIndex < rebarGeometries.Count && endRebarIndex >= 0 && endRebarIndex < rebarGeometries.Count && startRebarIndex <= endRebarIndex)
				{
					ArrayList arrayLists = (rebarGeometries[startRebarIndex] as RebarGeometry).Shape.Points;
					ArrayList points1 = (rebarGeometries[endRebarIndex] as RebarGeometry).Shape.Points;
					Point item = arrayLists[0] as Point;
					Point point = arrayLists[arrayLists.Count - 1] as Point;
					if (item.Equals(point) && arrayLists.Count > 2)
					{
						point = arrayLists[arrayLists.Count - 2] as Point;
					}
					Tekla.Structures.Geometry3d.Line line = new Tekla.Structures.Geometry3d.Line(item, point);
					Point point1 = new Point(points1[0] as Point);
					double num = Tekla.Structures.Geometry3d.Distance.PointToLine(point1, line);
					double num1 = double.MinValue;
					int num2 = startRebarIndex;
					for (int i = startRebarIndex; i < endRebarIndex; i++)
					{
						RebarGeometry rebarGeometry = rebarGeometries[i] as RebarGeometry;
						double point2 = Tekla.Structures.Geometry3d.Distance.PointToPoint(rebarGeometry.Shape.Points[0] as Point, rebarGeometry.Shape.Points[rebarGeometry.Shape.Points.Count - 1] as Point);
						if (point2 > num1)
						{
							num2 = i;
							num1 = point2;
						}
					}
					RebarGeometry item1 = rebarGeometries[num2] as RebarGeometry;
					double line1 = Tekla.Structures.Geometry3d.Distance.PointToLine(item1.Shape.Points[0] as Point, line);
					double num3 = num * customPosition - line1;
					Tekla.Structures.Geometry3d.Line line2 = new Tekla.Structures.Geometry3d.Line(startPoint, endPoint);
					Vector vector = null;
					vector = (!createNormalVector ? line2.Direction : new Vector(-line2.Direction.Y, line2.Direction.X, line2.Direction.Z));
					vector.Normalize(num3);
					for (int j = 0; j < item1.Shape.Points.Count; j++)
					{
						Point point3 = new Point(item1.Shape.Points[j] as Point);
						point3.Translate(vector.X, vector.Y, vector.Z);
						points.Add(point3);
					}
				}
			}
			return points;
		}

		public static List<double> GetRebarSegmentsLengths(Reinforcement rebar)
		{
			List<double> nums = new List<double>();
			ArrayList arrayLists = new ArrayList();
			Hashtable hashtables = new Hashtable();
			arrayLists.Add("DIM_A");
			arrayLists.Add("DIM_B");
			arrayLists.Add("DIM_C");
			arrayLists.Add("DIM_D");
			arrayLists.Add("DIM_E");
			arrayLists.Add("DIM_F");
			arrayLists.Add("DIM_G");
			arrayLists.Add("DIM_H1");
			arrayLists.Add("DIM_H2");
			rebar.GetDoubleReportProperties(arrayLists, ref hashtables);
			List<double> nums1 = new List<double>();
			foreach (string arrayList in arrayLists)
			{
				if (!hashtables.ContainsKey(arrayList))
				{
					continue;
				}
				nums1.Add((double)hashtables[arrayList]);
			}
			if (nums1.Count > 0)
			{
				ArrayList rebarGeometries = rebar.GetRebarGeometries(true);
				if (rebarGeometries.Count > 0)
				{
					RebarGeometry item = rebarGeometries[0] as RebarGeometry;
					for (int i = 1; i < item.Shape.Points.Count; i++)
					{
						double distanceBeetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(item.Shape.Points[i - 1] as Point, item.Shape.Points[i] as Point);
						double num = double.MaxValue;
						nums.Add(distanceBeetveenTwoPoints3D);
						foreach (double num1 in nums1)
						{
							double num2 = Math.Abs(distanceBeetveenTwoPoints3D - num1);
							if (!Compare.LT(num2, num))
							{
								continue;
							}
							num = num2;
							nums[nums.Count - 1] = num1;
						}
					}
				}
			}
			return nums;
		}

		public static string GetTranslationValue(this Enum value)
		{
			string empty = string.Empty;
			FieldInfo field = value.GetType().GetField(value.ToString());
			Drawings.DrawingMark.Translation[] customAttributes = field.GetCustomAttributes(typeof(Drawings.DrawingMark.Translation), false) as Drawings.DrawingMark.Translation[];
			if ((int)customAttributes.Length > 0)
			{
				empty = customAttributes[0].Value;
			}
			return empty;
		}

		public static object GetVisibleRebarPositionsFromReinforcementGroup(ReinforcementGroup drawingGroup, Vector axis)
		{
			List<int> nums = new List<int>();
			List<Point> customizedRebarPoints = null;
			bool flag = false;
			Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
			RebarGroup exactSpacingsDistribution = null;
			if (drawingGroup != null)
			{
				drawingGroup.Select();
				exactSpacingsDistribution = model.SelectModelObject(drawingGroup.ModelIdentifier) as RebarGroup;
				if (exactSpacingsDistribution != null)
				{
					exactSpacingsDistribution = cs_net_lib.Model.ConvertToExactSpacingsDistribution(exactSpacingsDistribution);
					Vector vector = new Vector(exactSpacingsDistribution.EndPoint - exactSpacingsDistribution.StartPoint);
					if (Compare.GE(axis.GetNormal().GetAngleBetween(vector.GetNormal()) * cs_net_lib.Constants.RAD_TO_DEG, 90))
					{
						flag = true;
					}
					if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.All)
					{
						for (int i = 0; i < exactSpacingsDistribution.Spacings.Count + 1; i++)
						{
							nums.Add(i);
						}
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.First)
					{
						if (!flag)
						{
							nums.Add(0);
						}
						else
						{
							nums.Add(exactSpacingsDistribution.Spacings.Count);
						}
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.Last)
					{
						if (!flag)
						{
							nums.Add(exactSpacingsDistribution.Spacings.Count);
						}
						else
						{
							nums.Add(0);
						}
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.FirstAndLast)
					{
						nums.Add(0);
						if (exactSpacingsDistribution.Spacings.Count > 1)
						{
							nums.Add(exactSpacingsDistribution.Spacings.Count);
						}
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.OneInTheMiddle)
					{
						int count = (int)((double)exactSpacingsDistribution.Spacings.Count * 0.35);
						if (flag)
						{
							count = exactSpacingsDistribution.Spacings.Count - count;
						}
						nums.Add(count);
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.TwoInTheMiddle)
					{
						int num = (int)((double)exactSpacingsDistribution.Spacings.Count * 0.35);
						if (!flag)
						{
							nums.Add(num);
							if (exactSpacingsDistribution.Spacings.Count > 1)
							{
								nums.Add(num + 1);
							}
						}
						else
						{
							num = exactSpacingsDistribution.Spacings.Count - num;
							nums.Add(num - 1);
							if (exactSpacingsDistribution.Spacings.Count > 1)
							{
								nums.Add(num);
							}
						}
					}
					else if (drawingGroup.Attributes.ReinforcementVisibility == ReinforcementBase.ReinforcementVisibilityTypes.Customized)
					{
						ArrayList rebarGeometries = exactSpacingsDistribution.GetRebarGeometries(false);
						customizedRebarPoints = Drawings.GetCustomizedRebarPoints(exactSpacingsDistribution, 0, rebarGeometries.Count - 1, drawingGroup.ReinforcementCustomPosition, exactSpacingsDistribution.StartPoint, exactSpacingsDistribution.EndPoint, false);
					}
				}
			}
			if (customizedRebarPoints != null)
			{
				return customizedRebarPoints;
			}
			return nums;
		}

		public static int NumberOfRebars(this ReinforcementBase reinforcement)
		{
			int num = 0;
			Tekla.Structures.Model.Model model = new Tekla.Structures.Model.Model();
			string str = Path.Combine(model.GetInfo().ModelPath, "attributes", "cs_temporary_NumberRebars.note");
			byte[] numArray = Convert.FromBase64String("bm90ZV9kaWFsLmluX2ZpbGUgInN0YW5kYXJkIg0Kbm90ZV9kaWFsLm91dF9maWxlICJudW1iZXJS\r\nZWJhcnMiDQpub3RlX2RpYWwuY3JlYXRlIDANCm5vdGVfZGlhbC5mb3JfbWFpbiAxDQpub3RlX2Rp\r\nYWwuZm9yX3NlYyAxDQpub3RlX2RpYWwuZm9yX3doaWNoX3BhcnRzIDENCm5vdGVfZGlhbC5pbl9h\r\nbGxfdmlld3MgMA0Kbm90ZV9kaWFsLk91dE9mUGxhbmUgMQ0Kbm90ZV9kaWFsLnRleHRfdHlwZSAx\r\nMDAwMg0Kbm90ZV9kaWFsLnRleHRfY29sb3VyIDE1NQ0Kbm90ZV9kaWFsLmZyYW1lX3R5cGUgMg0K\r\nbm90ZV9kaWFsLmxpbmVfY29sb3VyIDE1Mw0Kbm90ZV9kaWFsLkFuZ2xlIDAuMDAwMDAwDQpub3Rl\r\nX2RpYWwuQXJyb3dUeXBlIDANCm5vdGVfZGlhbC5BcnJvd0hlaWdodCAyLjAwMDAwMA0Kbm90ZV9k\r\naWFsLkFycm93TGVuZ3RoIDIuNTAwMDAwDQpub3RlX2RpYWwuaGVpZ2h0IDIuNTAwMDAwDQpub3Rl\r\nX2RpYWwuZGVmYXVsdF9zaXplIDAuMDAwMDAwDQpub3RlX2RpYWwubWFya19uc2ZzIDANCm5vdGVf\r\nZGlhbC5jb250ZW50XzEgMQ0Kbm90ZV9kaWFsLmZsYWdzIDANCm5vdGVfZGlhbC5tYXJrX3VzZV9w\r\nYXJ0X2VuZCAwDQpub3RlX2RpYWwuQ29udGVudFN0cmluZzEgIjw/eG1sIHZlcnNpb249IjEuMCIg\r\nZW5jb2Rpbmc9IlVURi04IiBzdGFuZGFsb25lPSJubyI/PgE8IURPQ1RZUEUgTWFyaz48TWFyaz4B\r\nPE1hcmtQYXJ0IGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1lPSJQQVJUX1BPUyIgdmFsdWU9IiIg\r\nc3ViVHlwZT0iMjMiPgE8Rm9ybWF0IGZvbnQ9IkFyaWFsIE5hcnJvdyIgY29sb3I9IjE1NSIgaGVp\r\nZ2h0PSIyLjUwMCIgaGlkZGVuPSIwIi8+ATwvRWxlbWVudD4BPC9NYXJrUGFydD4BPE1hcmtQYXJ0\r\nIGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1lPSJQUk9GSUxFIiB2YWx1ZT0iIiBzdWJUeXBlPSIy\r\nNCI+ATxGb3JtYXQgZm9udD0iQXJpYWwgTmFycm93IiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAw\r\nIiBoaWRkZW49IjAiLz4BPC9FbGVtZW50PgE8L01hcmtQYXJ0PgE8L01hcms+ASINCm5vdGVfZGlh\r\nbC5Db250ZW50U3RyaW5nMiAiPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0\r\nYW5kYWxvbmU9Im5vIj8+ATwhRE9DVFlQRSBNYXJrPjxNYXJrPgE8TWFya1BhcnQgZ3JvdXBpZD0i\r\nMCI+ATxTaXplRWxlbWVudCB2YWx1ZT0iIiBzdWJUeXBlPSI0NyI+ATxGb3JtYXQgZm9udD0iQXJp\r\nYWwgTmFycm93IiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIiBoaWRkZW49IjAiLz4BPC9TaXpl\r\nRWxlbWVudD4BPC9NYXJrUGFydD4BPC9NYXJrPgEiDQpub3RlX2RpYWwuQ29udGVudFN0cmluZzMg\r\nIjw/eG1sIHZlcnNpb249IjEuMCIgZW5jb2Rpbmc9IlVURi04IiBzdGFuZGFsb25lPSJubyI/PgE8\r\nIURPQ1RZUEUgTWFyaz48TWFyaz4BPE1hcmtQYXJ0IGdyb3VwaWQ9IjAiPgE8RWxlbWVudCBuYW1l\r\nPSJOQU1FIiB2YWx1ZT0iIiBzdWJUeXBlPSIzNSI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9y\r\nPSIxNTUiIGhlaWdodD0iMi41MDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+\r\nATwvTWFyaz4BIg0Kbm90ZV9kaWFsLkNvbnRlbnRTdHJpbmc0ICI8P3htbCB2ZXJzaW9uPSIxLjAi\r\nIGVuY29kaW5nPSJVVEYtOCIgc3RhbmRhbG9uZT0ibm8iPz4BPCFET0NUWVBFIE1hcms+PE1hcms+\r\nATxNYXJrUGFydCBncm91cGlkPSIwIj4BPEVsZW1lbnQgbmFtZT0iIiB2YWx1ZT0iIiBzdWJUeXBl\r\nPSI0MSI+ATxGb3JtYXQgZm9udD0iQXJpYWwgTmFycm93IiBjb2xvcj0iMTU4IiBoZWlnaHQ9IjIu\r\nNTAwIiBoaWRkZW49IjAiLz4BPC9FbGVtZW50PgE8L01hcmtQYXJ0PgE8L01hcms+ASINCm5vdGVf\r\nZGlhbC5DaGFtZmVyQ29udGVudFN0cmluZyAiPD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0i\r\nVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+ATwhRE9DVFlQRSBNYXJrPjxNYXJrPgE8TWFya1BhcnQg\r\nZ3JvdXBpZD0iMCI+ATxFbGVtZW50IG5hbWU9IlhfRElSRUNUSU9OX1dJRFRIIiB2YWx1ZT0iIiBz\r\ndWJUeXBlPSI4NyI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdodD0iMi41\r\nMDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+ATxNYXJrUGFydCBncm91cGlk\r\nPSIwIj4BPFVzZXJUZXh0PgF4ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdo\r\ndD0iMi41MDAiIGhpZGRlbj0iMCIvPgE8L1VzZXJUZXh0PgE8L01hcmtQYXJ0PgE8TWFya1BhcnQg\r\nZ3JvdXBpZD0iMCI+ATxFbGVtZW50IG5hbWU9IllfRElSRUNUSU9OX1dJRFRIIiB2YWx1ZT0iIiBz\r\ndWJUeXBlPSI4OCI+ATxGb3JtYXQgZm9udD0iQXJpYWwiIGNvbG9yPSIxNTUiIGhlaWdodD0iMi41\r\nMDAiIGhpZGRlbj0iMCIvPgE8L0VsZW1lbnQ+ATwvTWFya1BhcnQ+ATwvTWFyaz4BIg0Kbm90ZV9k\r\naWFsLlBvdXJDb250ZW50U3RyaW5nICI8P3htbCB2ZXJzaW9uPSIxLjAiIGVuY29kaW5nPSJVVEYt\r\nOCIgc3RhbmRhbG9uZT0ibm8iPz48IURPQ1RZUEUgTWFyaz48TWFyaz48TWFya1BhcnQgZ3JvdXBp\r\nZD0iMCI+PEVsZW1lbnQgbmFtZT0iTUFURVJJQUwiIHZhbHVlPSIiIHN1YlR5cGU9IjEwIj48Rm9y\r\nbWF0IGZvbnQ9IkFyaWFsIiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIi8+PC9FbGVtZW50Pjwv\r\nTWFya1BhcnQ+PC9NYXJrPiINCm5vdGVfZGlhbC5Qb3VyQnJlYWtDb250ZW50U3RyaW5nICI8P3ht\r\nbCB2ZXJzaW9uPSIxLjAiIGVuY29kaW5nPSJVVEYtOCIgc3RhbmRhbG9uZT0ibm8iPz48IURPQ1RZ\r\nUEUgTWFyaz48TWFyaz48TWFya1BhcnQgZ3JvdXBpZD0iMCI+PFVzZXJUZXh0PlBPVVIgQlJFQUs8\r\nRm9ybWF0IGZvbnQ9IkFyaWFsIiBjb2xvcj0iMTU1IiBoZWlnaHQ9IjIuNTAwIi8+PC9Vc2VyVGV4\r\ndD48L01hcmtQYXJ0PjwvTWFyaz4iDQpub3RlX2RpYWwubGVhZGVyX2xpbmVfdHlwZSAwDQpub3Rl\r\nX2RpYWwubWVyZ2VfZGlyIDENCm5vdGVfZGlhbC5NZXJnZU1hcmtzIDANCm5vdGVfZGlhbC5DdXJy\r\nZW50Q29udGVudFRhYmxlTnVtYmVyIDINCm5vdGVfZGlhbC5Gb3JTdWJhc3NNYWluIDENCm5vdGVf\r\nZGlhbC5Gb3JTdWJhc3NTZWNvbmRhcnkgMQ0Kbm90ZV9kaWFsLlVzZUhpZGRlbkxpbmVzIDENCm5v\r\ndGVfZGlhbC5Sb3RhdGlvbiAwDQpub3RlX2RpYWwuY3JlYXRlX2VuIDENCm5vdGVfZGlhbC5mb3Jf\r\nbWFpbl9lbiAxDQpub3RlX2RpYWwuZm9yX3NlY19lbiAxDQpub3RlX2RpYWwuZm9yX3doaWNoX3Bh\r\ncnRzX2VuIDENCm5vdGVfZGlhbC5pbl9hbGxfdmlld3NfZW4gMQ0Kbm90ZV9kaWFsLk91dE9mUGxh\r\nbmVfZW4gMQ0Kbm90ZV9kaWFsLnRleHRfdHlwZV9lbiAxDQpub3RlX2RpYWwudGV4dF9jb2xvdXJf\r\nZW4gMQ0Kbm90ZV9kaWFsLmZyYW1lX2VuIDENCm5vdGVfZGlhbC5saW5lX2NvbG91cl9lbiAxDQpu\r\nb3RlX2RpYWwuQW5nbGVfZW4gMQ0Kbm90ZV9kaWFsLkFycm93VHlwZV9lbiAxDQpub3RlX2RpYWwu\r\nQXJyb3dIZWlnaHRfZW4gMQ0Kbm90ZV9kaWFsLkFycm93TGVuZ3RoX2VuIDENCm5vdGVfZGlhbC5o\r\nZWlnaHRfZW4gMQ0Kbm90ZV9kaWFsLmNvbnRlbnRfZW4gMQ0Kbm90ZV9kaWFsLnBsYWNlX2VuIDEN\r\nCm5vdGVfZGlhbC5kZWZhdWx0X3NpemVfZW4gMQ0Kbm90ZV9kaWFsLm1hcmtfbnNmc19lbiAxDQpu\r\nb3RlX2RpYWwudXNlX3BhcnRfZW5kX2VuIDENCm5vdGVfZGlhbC5sZWFkZXJfbGluZV90eXBlX2Vu\r\nIDENCm5vdGVfZGlhbC5tZXJnZV9kaXJfZW4gMQ0Kbm90ZV9kaWFsLk1lcmdlTWFya3NfZW4gMQ0K\r\nbm90ZV9kaWFsLkZvclN1YmFzc01haW5fZW4gMQ0Kbm90ZV9kaWFsLkZvclN1YmFzc1NlY29uZGFy\r\neV9lbiAxDQpub3RlX2RpYWwuVXNlSGlkZGVuTGluZXNfZW4gMQ0Kbm90ZV9kaWFsLlJvdGF0aW9u\r\nX2VuIDENCm5vdGVfZGlhbC50eHBsLnRleHRfcGxhY2luZ19tb2RlIDENCm5vdGVfZGlhbC50eHBs\r\nLnF1YXJ0ZXJfMSAxDQpub3RlX2RpYWwudHhwbC5xdWFydGVyXzIgMg0Kbm90ZV9kaWFsLnR4cGwu\r\ncXVhcnRlcl8zIDQNCm5vdGVfZGlhbC50eHBsLnF1YXJ0ZXJfNCA4DQpub3RlX2RpYWwudHhwbC50\r\nZXh0X2ZyZWVwbF9taW4gMi4wMDAwMDANCm5vdGVfZGlhbC50eHBsLnRleHRfZnJlZXBsX21hcmdp\r\nbiAxLjAwMDAwMA0Kbm90ZV9kaWFsLnR4cGwudGV4dF9wbGFjZV9lbiAxDQpub3RlX2RpYWwudHhw\r\nbC50ZXh0X3F1YXJ0ZXJfZW4gMQ0Kbm90ZV9kaWFsLnR4cGwudGV4dF9mcmVlcGxfbWluX2VuIDEN\r\nCm5vdGVfZGlhbC50eHBsLnRleHRfZnJlZXBsX21hcmdpbl9lbiAxDQpub3RlX2RpYWwuRm9udEF0\r\ndHIuZm9udF9uYW1lICJBcmlhbCBOYXJyb3ciDQpub3RlX2RpYWwuRm9udEF0dHIudGV4dF9jb2xv\r\ndXIgMTU4DQpub3RlX2RpYWwuRm9udEF0dHIuaGVpZ2h0IDIuNTAwMDAwDQpub3RlX2RpYWwuRm9u\r\ndEF0dHIuZm9udF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkZvbnRBdHRyLnRleHRfY29sb3VyX2VuYWJs\r\nZSAxDQpub3RlX2RpYWwuRm9udEF0dHIuaGVpZ2h0X2VuYWJsZSAxDQpub3RlX2RpYWwuRnJhbWVB\r\ndHRyLmZyYW1lX3R5cGUgMQ0Kbm90ZV9kaWFsLkZyYW1lQXR0ci5saW5lX2NvbG91ciAxNTUNCm5v\r\ndGVfZGlhbC5GcmFtZUF0dHIuZnJhbWVfZW5hYmxlIDENCm5vdGVfZGlhbC5GcmFtZUF0dHIubGlu\r\nZV9jb2xvdXJfZW5hYmxlIDENCm5vdGVfZGlhbC5Vbml0QXR0ci5Vbml0IDANCm5vdGVfZGlhbC5V\r\nbml0QXR0ci5QcmVjaXNpb24gMA0Kbm90ZV9kaWFsLlVuaXRBdHRyLkZvcm1hdCAwDQpub3RlX2Rp\r\nYWwuTG9jYXRpb25BdHRyaWJ1dGVzLkRlc2lyZWRMb2NhdGlvbk9uUGFydF9lbmFibGUgMQ0Kbm90\r\nZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5EZXNpcmVkTG9jYXRpb25PblBhcnQgMQ0Kbm90ZV9k\r\naWFsLkxvY2F0aW9uQXR0cmlidXRlcy5Mb2NhdGlvbk9uUGFydCAxDQpub3RlX2RpYWwuTG9jYXRp\r\nb25BdHRyaWJ1dGVzLk9yaWVudGF0aW9uX2VuYWJsZSAxDQpub3RlX2RpYWwuTG9jYXRpb25BdHRy\r\naWJ1dGVzLkFuZ2xlUmVsYXRpdmVUbyAyDQpub3RlX2RpYWwuTG9jYXRpb25BdHRyaWJ1dGVzLkFs\r\naWdubWVudF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5BbGlnbm1lbnQg\r\nMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5QbGFjaW5nRmFsbGJhY2tfZW5hYmxlIDEN\r\nCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMuUGxhY2luZ0ZhbGxiYWNrIDY0DQpub3RlX2Rp\r\nYWwuTG9jYXRpb25BdHRyaWJ1dGVzLk9mZnNldF9lbmFibGUgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9u\r\nQXR0cmlidXRlcy5PZmZzZXQgMC4wMDAwMDANCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMu\r\nTGVhZGVyTGluZURlc2lyZWRfZW5hYmxlIDENCm5vdGVfZGlhbC5Mb2NhdGlvbkF0dHJpYnV0ZXMu\r\nTGVhZGVyTGluZURlc2lyZWQgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5MZWFkZXJM\r\naW5lSW5Vc2UgMQ0Kbm90ZV9kaWFsLkxvY2F0aW9uQXR0cmlidXRlcy5NaW5pbXVtTWFya1NjYWxl\r\nIDAuMDAwMDAwDQpub3RlX2RpYWwuUmViYXJHcm91cE1hcmtBdHRyaWJ1dGVzLkxpbmVUeXBlIDAN\r\nCm5vdGVfZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuVHlwZSAwDQpub3RlX2RpYWwuUmVi\r\nYXJHcm91cE1hcmtBdHRyaWJ1dGVzLlRhZ2dlZFJlYmFyRGltTWFya0xvY2F0aW9uIDMNCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuQ2xvc2VSZWJhckRpbU1hcmtUb0ZhdGhlciAw\r\nDQpub3RlX2RpYWwuUmViYXJHcm91cE1hcmtBdHRyaWJ1dGVzLkxpbmVUeXBlX2VuIDENCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuVHlwZV9lbiAxDQpub3RlX2RpYWwuUmViYXJH\r\ncm91cE1hcmtBdHRyaWJ1dGVzLlRhZ2dlZFJlYmFyRGltTWFya0xvY2F0aW9uX2VuIDENCm5vdGVf\r\nZGlhbC5SZWJhckdyb3VwTWFya0F0dHJpYnV0ZXMuQ2xvc2VSZWJhckRpbU1hcmtUb0ZhdGhlcl9l\r\nbiAxDQo=");
			FileInfo fileInfo = new FileInfo(str);
			fileInfo.ClearFile();
			File.WriteAllText(str, Encoding.UTF8.GetString(numArray, 0, (int)numArray.Length));
			Mark mark = new Mark(reinforcement);
			mark.Attributes.LoadAttributes("cs_temporary_NumberRebars");
			mark.InsertionPoint = new Point();
			mark.Insert();
			mark.Select();
			string empty = string.Empty;
			foreach (ElementBase content in mark.Attributes.Content)
			{
				empty = content.GetUnformattedString();
			}
			mark.Delete();
			fileInfo.Refresh();
			fileInfo.ClearFile();
			try
			{
				num = int.Parse(empty);
			}
			catch
			{
				num = 0;
			}
			return num;
		}

		public class DrawingMark : List<Drawings.DrawingMark.TextLine>
		{
			public const string MarkItemsSeparator = ";;;";

			public const string TextAttibuteSeparator = ".";

			private readonly string rebarPositionStandard;

			private readonly ReinforcementBase reinforcementDrawing;

			private readonly Reinforcement reinforcementModel;

			private readonly string textStandard;

			private readonly View view;

			private Drawings.DrawingMark.TextItem pullOut;

			private double rowDistance;

			public double HeightWithoutPull
			{
				get;
				private set;
			}

			public string RebarPosition
			{
				get;
				private set;
			}

			public Drawings.DrawingMark.Size TotalSize
			{
				get;
				private set;
			}

			public DrawingMark(ReinforcementBase reinforcement, string textAttributies, string textStandard, string rebarPositionStandard, int rebarPositionType, double rowDistance = 0) : this(reinforcement, textAttributies, textStandard, rebarPositionStandard, rebarPositionType, null, 1, rowDistance)
			{
			}

			public DrawingMark(ReinforcementBase reinforcement, string textAttributies, string textStandard, string rebarPositionStandard, int rebarPositionType, List<KeyValuePair<Drawings.DrawingMark.AttributeTypes, string>> overriddenTextAttributies, int multiplicityNumber, double rowDistance = 0)
			{
				this.reinforcementDrawing = reinforcement;
				this.reinforcementModel = (new Tekla.Structures.Model.Model()).SelectModelObject(this.reinforcementDrawing.ModelIdentifier) as Reinforcement;
				this.view = this.reinforcementDrawing.GetView() as View;
				this.textStandard = textStandard;
				this.rebarPositionStandard = rebarPositionStandard;
				this.RebarPosition = string.Empty;
				if (rowDistance >= 0)
				{
					this.rowDistance = rowDistance;
				}
				this.GetMarkText(textAttributies, overriddenTextAttributies, multiplicityNumber);
				string empty = string.Empty;
				if (!this.reinforcementModel.GetReportProperty("REBAR_POS", ref empty))
				{
					this.RebarPosition = this.reinforcementModel.NumberingSeries.Prefix;
					Drawings.DrawingMark drawingMark = this;
					drawingMark.RebarPosition = string.Concat(drawingMark.RebarPosition, this.reinforcementModel.NumberingSeries.StartNumber);
				}
				else
				{
					this.RebarPosition = empty;
				}
				if (rebarPositionType == 1)
				{
					base[0].RebarPosition = new Drawings.DrawingMark.TextItem(this.RebarPosition, this.reinforcementDrawing);
					base[0].RebarPositionType = rebarPositionType;
				}
				else if (rebarPositionType == 2)
				{
					base[base.Count - 1].RebarPosition = new Drawings.DrawingMark.TextItem(this.RebarPosition, this.reinforcementDrawing);
					base[base.Count - 1].RebarPositionType = rebarPositionType;
				}
				this.GetSize();
			}

			private static bool ComparePointsLeftToRight(Point point1, Point point2)
			{
				if (Compare.GE(point1.X, point2.X) && Compare.GE(point1.Y, point2.Y))
				{
					return true;
				}
				if (!Compare.GT(point1.X, point2.X))
				{
					return false;
				}
				return Compare.LT(point1.Y, point2.Y);
			}

			private void ControlRebarOrdering(ref List<double> lengths)
			{
				double num = 5;
				bool flag = false;
				RebarGeometry item = this.reinforcementModel.GetRebarGeometries(true)[0] as RebarGeometry;
				if (item != null)
				{
					ArrayList points = item.Shape.Points;
					for (int i = 0; i < points.Count; i++)
					{
						if (i + 1 < points.Count)
						{
							Point point = new Point(points[i] as Point);
							Point point1 = new Point(points[i + 1] as Point);
							double num1 = Tekla.Structures.Geometry3d.Distance.PointToPoint(point, point1);
							if (lengths.Count > i)
							{
								double item1 = lengths[i];
								if (Compare.GT(item1, num1 + num) || Compare.LT(item1, num1 - num))
								{
									flag = true;
									i = points.Count;
								}
							}
						}
					}
				}
				if (lengths.Count == 0)
				{
					double num2 = 0;
					if (this.reinforcementModel.GetUserProperty("LENGTH", ref num2))
					{
						lengths.Add(num2);
					}
				}
				if (flag)
				{
					lengths.Reverse();
				}
			}

			public void Draw(Tekla.Structures.Drawing.Line line, Point point)
			{
				Point point1 = new Point(point);
				Vector lineVector = Drawings.DrawingMark.GetLineVector(line);
				double height = 0;
				if (base.Count > 0)
				{
					height = base[0].Size.Height / 2;
				}
				Point point2 = new Point(point);
				Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
				if (this.pullOut != null)
				{
					size = this.pullOut.GetSize(this.view, new Text.TextAttributes(this.textStandard));
					Drawings.DrawingMark.Size size1 = new Drawings.DrawingMark.Size();
					foreach (Drawings.DrawingMark.TextLine textLine in this)
					{
						Drawings.DrawingMark.Size beforeSize = textLine.GetBeforeSize();
						if (!Compare.GT(beforeSize.Width, size1.Width))
						{
							continue;
						}
						size1 = beforeSize;
					}
					Point point3 = Geo.MovePoint(new Point(point2), Drawings.DrawingMark.GetParallelVector2D(lineVector), this.TotalSize.Height / 2);
					point3 = Geo.MovePoint(point3, lineVector, size1.Width);
					this.pullOut.Draw(this.view, new Text.TextAttributes(this.textStandard), line, ref point3);
					size = size + size1;
				}
				if (Compare.GT(this.TotalSize.Height, this.HeightWithoutPull))
				{
					height = (this.TotalSize.Height - this.HeightWithoutPull) / 2 + height;
				}
				foreach (Drawings.DrawingMark.TextLine textLine1 in this)
				{
					point1 = Geo.MovePoint(new Point(point2), Drawings.DrawingMark.GetParallelVector2D(lineVector), height);
					textLine1.Draw(line, point1, size);
					height = height + (textLine1.Size.Height + this.rowDistance);
				}
			}

			private static Vector GetLineVector(Tekla.Structures.Drawing.Line line)
			{
				Vector vector = new Vector(line.StartPoint - line.EndPoint)
				{
					Z = 0
				};
				Vector vector1 = new Vector(vector.Y, -vector.X, 0);
				vector1.Normalize();
				vector = (!Drawings.DrawingMark.ComparePointsLeftToRight(line.StartPoint, line.EndPoint) ? new Vector(line.EndPoint - line.StartPoint) : new Vector(line.StartPoint - line.EndPoint));
				vector.Z = 0;
				vector.Normalize();
				return vector;
			}

			private void GetMarkText(string attributes)
			{
				this.GetMarkText(attributes, null, 1);
			}

			private void GetMarkText(string attributes, List<KeyValuePair<Drawings.DrawingMark.AttributeTypes, string>> overriddenTextAttributies, int multiplicityNumber)
			{
				Drawings.DrawingMark.TextLine textLine = new Drawings.DrawingMark.TextLine(this.reinforcementDrawing, this.textStandard, this.rebarPositionStandard, this.view, false, this.rowDistance);
				Drawings.DrawingMark.TextItem item = textLine[0];
				int num = 0;
				ContentAttributesReader contentAttributesReader = new ContentAttributesReader();
				if (Compare.LE(this.rowDistance, 0))
				{
					this.rowDistance = textLine.RowDistance;
				}
				string empty = string.Empty;
				string str = string.Empty;
				if (attributes != string.Empty)
				{
					int num1 = Drawings.DrawingMark.ReturnCurrentUnitType();
					string[] strArrays = new string[] { ";;;" };
					string[] strArrays1 = attributes.Split(strArrays, StringSplitOptions.None);
					string empty1 = string.Empty;
					if (multiplicityNumber > 1)
					{
						Drawings.DrawingMark.TextItem textItem = item;
						textItem.TextValue = string.Concat(textItem.TextValue, multiplicityNumber.ToString(), "x", empty1);
					}
					for (int i = 0; i < (int)strArrays1.Length; i++)
					{
						if (strArrays1[i] != string.Empty)
						{
							string value = string.Empty;
							Drawings.DrawingMark.TextAttribute textAttribute = new Drawings.DrawingMark.TextAttribute(strArrays1[i], num1);
							string str1 = string.Empty;
							switch (textAttribute.AttributeType)
							{
								case Drawings.DrawingMark.AttributeTypes.Name:
								{
									Drawings.DrawingMark.TextItem textItem1 = item;
									textItem1.TextValue = string.Concat(textItem1.TextValue, empty1, this.reinforcementModel.Name);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Grade:
								{
									Drawings.DrawingMark.TextItem textItem2 = item;
									textItem2.TextValue = string.Concat(textItem2.TextValue, empty1, this.reinforcementModel.Grade);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Diameter:
								{
									string empty2 = string.Empty;
									this.reinforcementModel.GetReportProperty("SIZE", ref empty2);
									Drawings.DrawingMark.TextItem textItem3 = item;
									textItem3.TextValue = string.Concat(textItem3.TextValue, empty1, empty2);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Class:
								{
									Drawings.DrawingMark.TextItem textItem4 = item;
									textItem4.TextValue = string.Concat(textItem4.TextValue, empty1, this.reinforcementModel.Class);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Length:
								{
									if (textAttribute.TypeLength == 0)
									{
										this.GetRebarLengthValues(textAttribute, out empty, out str);
									}
									else if (textAttribute.TypeLength != 1)
									{
										double num2 = 0;
										if (this.reinforcementModel.GetReportProperty("LENGTH", ref num2) && Compare.GT(num2, 0))
										{
											empty = (textAttribute.Units == -1 ? num2.ToString() : Drawings.DrawingMark.ReturnDimensionValue(this.view, num2, textAttribute));
										}
									}
									else
									{
										this.GetRebarLengthAxis(textAttribute, out empty, out str);
									}
									if (empty == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem5 = item;
									textItem5.TextValue = string.Concat(textItem5.TextValue, empty1, empty);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Number:
								{
									if (overriddenTextAttributies != null)
									{
										KeyValuePair<Drawings.DrawingMark.AttributeTypes, string> keyValuePair = overriddenTextAttributies.First<KeyValuePair<Drawings.DrawingMark.AttributeTypes, string>>((KeyValuePair<Drawings.DrawingMark.AttributeTypes, string> c) => c.Key == Drawings.DrawingMark.AttributeTypes.Number);
										value = keyValuePair.Value;
									}
									if (!string.IsNullOrEmpty(value))
									{
										Drawings.DrawingMark.TextItem textItem6 = item;
										textItem6.TextValue = string.Concat(textItem6.TextValue, empty1, value);
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else if (!textAttribute.Code.Contains("0"))
									{
										Drawings.DrawingMark.TextItem textItem7 = item;
										textItem7.TextValue = string.Concat(textItem7.TextValue, empty1, this.reinforcementModel.GetNumberOfRebars());
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else
									{
										Drawings.DrawingMark.TextItem textItem8 = item;
										textItem8.TextValue = string.Concat(textItem8.TextValue, empty1);
										Drawings.DrawingMark.TextItem textItem9 = item;
										textItem9.TextValue = string.Concat(textItem9.TextValue, this.reinforcementDrawing.NumberOfRebars());
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
								}
								case Drawings.DrawingMark.AttributeTypes.Position:
								{
									string str2 = string.Empty;
									if (!this.reinforcementModel.GetReportProperty("REBAR_POS", ref str2))
									{
										Drawings.DrawingMark.TextItem textItem10 = item;
										object textValue = textItem10.TextValue;
										object[] prefix = new object[] { textValue, empty1, this.reinforcementModel.NumberingSeries.Prefix, this.reinforcementModel.NumberingSeries.StartNumber };
										textItem10.TextValue = string.Concat(prefix);
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else
									{
										Drawings.DrawingMark.TextItem textItem11 = item;
										textItem11.TextValue = string.Concat(textItem11.TextValue, empty1, str2);
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
								}
								case Drawings.DrawingMark.AttributeTypes.Shape:
								{
									string empty3 = string.Empty;
									if (!this.reinforcementModel.GetReportProperty("SHAPE", ref empty3))
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem12 = item;
									textItem12.TextValue = string.Concat(textItem12.TextValue, empty1, empty3);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Weight:
								{
									double num3 = 0;
									if (!this.reinforcementModel.GetReportProperty("WEIGHT", ref num3))
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem13 = item;
									textItem13.TextValue = string.Concat(textItem13.TextValue, empty1, num3);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.CC:
								{
									if (overriddenTextAttributies != null)
									{
										KeyValuePair<Drawings.DrawingMark.AttributeTypes, string> keyValuePair1 = overriddenTextAttributies.First<KeyValuePair<Drawings.DrawingMark.AttributeTypes, string>>((KeyValuePair<Drawings.DrawingMark.AttributeTypes, string> c) => c.Key == Drawings.DrawingMark.AttributeTypes.CC);
										value = keyValuePair1.Value;
									}
									if (string.IsNullOrEmpty(value))
									{
										this.reinforcementModel.GetReportProperty("CC", ref str1);
										if (str1 == string.Empty)
										{
											goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
										}
										Drawings.DrawingMark.TextItem textItem14 = item;
										textItem14.TextValue = string.Concat(textItem14.TextValue, empty1, Drawings.DrawingMark.ReturnCcText(this.view, str1, textAttribute));
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else
									{
										Drawings.DrawingMark.TextItem textItem15 = item;
										textItem15.TextValue = string.Concat(textItem15.TextValue, empty1, value);
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
								}
								case Drawings.DrawingMark.AttributeTypes.CCMin:
								{
									this.reinforcementModel.GetReportProperty("CC_MIN", ref str1);
									if (str1 == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem16 = item;
									textItem16.TextValue = string.Concat(textItem16.TextValue, empty1, Drawings.DrawingMark.ReturnCcText(this.view, str1, textAttribute));
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.CCMax:
								{
									this.reinforcementModel.GetReportProperty("CC_MAX", ref str1);
									if (str1 == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem17 = item;
									textItem17.TextValue = string.Concat(textItem17.TextValue, empty1, Drawings.DrawingMark.ReturnCcText(this.view, str1, textAttribute));
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.CCExact:
								{
									this.reinforcementModel.GetReportProperty("CC_EXACT", ref str1);
									if (str1 == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem18 = item;
									textItem18.TextValue = string.Concat(textItem18.TextValue, empty1, Drawings.DrawingMark.ReturnCcText(this.view, str1, textAttribute));
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.CCTarget:
								{
									this.reinforcementModel.GetReportProperty("CC_TARGET", ref str1);
									if (str1 == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem19 = item;
									textItem19.TextValue = string.Concat(textItem19.TextValue, empty1, Drawings.DrawingMark.ReturnCcText(this.view, str1, textAttribute));
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.CCSeparator:
								{
									if (!(this.reinforcementModel is RebarGroup) || this.reinforcementModel.GetNumberOfRebars() <= 1)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem20 = item;
									textItem20.TextValue = string.Concat(textItem20.TextValue, empty1, textAttribute.Code[0]);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Text:
								{
									Drawings.DrawingMark.TextItem textItem21 = item;
									textItem21.TextValue = string.Concat(textItem21.TextValue, empty1, textAttribute.Code[0]);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.UDA:
								{
									bool flag = false;
									string item1 = string.Empty;
									Hashtable hashtables = new Hashtable();
									if (this.reinforcementModel.GetStringUserProperties(ref hashtables) && hashtables.Contains(textAttribute.Code[0]))
									{
										flag = true;
										item1 = hashtables[textAttribute.Code[0]] as string;
									}
									if (!flag && this.reinforcementModel.GetDoubleUserProperties(ref hashtables) && hashtables.Contains(textAttribute.Code[0]))
									{
										flag = true;
										double item2 = (double)hashtables[textAttribute.Code[0]];
										item1 = item2.ToString();
									}
									if (!flag && this.reinforcementModel.GetIntegerUserProperties(ref hashtables) && hashtables.Contains(textAttribute.Code[0]))
									{
										flag = true;
										int item3 = (int)hashtables[textAttribute.Code[0]];
										item1 = item3.ToString();
									}
									if (!flag)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem22 = item;
									textItem22.TextValue = string.Concat(textItem22.TextValue, empty1, item1);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.Symbol:
								{
									if (item.TextValue != string.Empty)
									{
										item = new Drawings.DrawingMark.TextItem(this.reinforcementDrawing);
										textLine.Add(item, this.pullOut != null);
									}
									item.Symbol = new SymbolInfo();
									item.Angle = (new Text.TextAttributes(this.textStandard)).Angle;
									if (textAttribute.Code.Count<string>() <= 2)
									{
										item.Symbol.SymbolFile = "concrete";
										item.Symbol.SymbolIndex = 34;
									}
									else
									{
										item.Symbol.SymbolFile = textAttribute.Code[0];
										int num4 = 0;
										item.Symbol.SymbolIndex = 0;
										if (int.TryParse(textAttribute.Code[1], out num4))
										{
											item.Symbol.SymbolIndex = num4;
										}
									}
									item = new Drawings.DrawingMark.TextItem(this.reinforcementDrawing);
									textLine.Add(item, this.pullOut != null);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.LengthItemized1:
								{
									if (textAttribute.TypeLength != 0)
									{
										this.GetRebarLengthAxis(textAttribute, out empty, out str);
									}
									else
									{
										this.GetRebarLengthValues(textAttribute, out empty, out str);
									}
									if (str == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem23 = item;
									string textValue1 = textItem23.TextValue;
									string[] strArrays2 = new string[] { textValue1, empty1, "(", str, ")" };
									textItem23.TextValue = string.Concat(strArrays2);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.LengthItemized2:
								{
									if (textAttribute.TypeLength != 0)
									{
										this.GetRebarLengthAxis(textAttribute, out empty, out str);
									}
									else
									{
										this.GetRebarLengthValues(textAttribute, out empty, out str);
									}
									if (str == string.Empty)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									Drawings.DrawingMark.TextItem textItem24 = item;
									textItem24.TextValue = string.Concat(textItem24.TextValue, empty1, str);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.NewLine:
								{
									base.Add(textLine);
									textLine = new Drawings.DrawingMark.TextLine(this.reinforcementDrawing, this.textStandard, this.rebarPositionStandard, this.view, this.pullOut != null, this.rowDistance);
									item = textLine[0];
									num++;
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.PulloutPicture:
								{
									if (this.pullOut != null)
									{
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									if (item.IsEmpty())
									{
										textLine.RemoveAt(textLine.Count - 1);
									}
									this.pullOut = this.SetPulloutPicture(textAttribute);
									item.Angle = (new Text.TextAttributes(this.textStandard)).Angle;
									item = new Drawings.DrawingMark.TextItem(this.reinforcementDrawing);
									textLine.Add(item, this.pullOut != null);
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.ExtraGap:
								{
									Drawings.DrawingMark.TextItem textItem25 = item;
									textItem25.TextValue = string.Concat(textItem25.TextValue, " ");
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
								case Drawings.DrawingMark.AttributeTypes.RemoveGap:
								{
									if (item.TextValue == string.Empty || textAttribute.AttributeType == Drawings.DrawingMark.AttributeTypes.RemoveGap)
									{
										empty1 = string.Empty;
										break;
									}
									else
									{
										empty1 = " ";
										break;
									}
								}
								case Drawings.DrawingMark.AttributeTypes.Template:
								{
									ContentAttributesReader.TargetTypes targetType = contentAttributesReader.GetTargetType(textAttribute.Code[0]);
									if (targetType == ContentAttributesReader.TargetTypes.Date || targetType == ContentAttributesReader.TargetTypes.Integer)
									{
										int num5 = 0;
										if (!this.reinforcementModel.GetReportProperty(textAttribute.Code[0], ref num5))
										{
											goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
										}
										Drawings.DrawingMark.TextItem textItem26 = item;
										textItem26.TextValue = string.Concat(textItem26.TextValue, empty1, num5.ToString());
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else if (targetType != ContentAttributesReader.TargetTypes.Double)
									{
										if (targetType != ContentAttributesReader.TargetTypes.String)
										{
											goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
										}
										string str3 = string.Empty;
										if (!this.reinforcementModel.GetReportProperty(textAttribute.Code[0], ref str3))
										{
											goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
										}
										Drawings.DrawingMark.TextItem textItem27 = item;
										textItem27.TextValue = string.Concat(textItem27.TextValue, empty1, str3);
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
									else
									{
										double num6 = 0;
										if (!this.reinforcementModel.GetReportProperty(textAttribute.Code[0], ref num6))
										{
											goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
										}
										Drawings.DrawingMark.TextItem textItem28 = item;
										textItem28.TextValue = string.Concat(textItem28.TextValue, empty1, num6.ToString());
										goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
									}
								}
								default:
								{
									goto case Drawings.DrawingMark.AttributeTypes.RemoveGap;
								}
							}
						}
					}
				}
				base.Add(textLine);
			}

			private static Vector GetParallelVector2D(Vector vector)
			{
				return new Vector(vector.Y, -vector.X, vector.Z);
			}

			private void GetRebarLengthAxis(Drawings.DrawingMark.TextAttribute textAttribute, out string rebarLength, out string rebarString)
			{
				rebarString = string.Empty;
				rebarLength = string.Empty;
				RebarGeometry item = this.reinforcementModel.GetRebarGeometries(true)[0] as RebarGeometry;
				if (item != null)
				{
					double num = 0;
					ArrayList points = item.Shape.Points;
					for (int i = 0; i < points.Count; i++)
					{
						if (i + 1 < points.Count)
						{
							if (i > 0)
							{
								rebarString = string.Concat(rebarString, "+");
							}
							Point point = new Point(points[i] as Point);
							Point point1 = new Point(points[i + 1] as Point);
							double num1 = Tekla.Structures.Geometry3d.Distance.PointToPoint(point, point1);
							num = num + num1;
							if (textAttribute.Units == -1)
							{
								rebarLength = num1.ToString();
							}
							else
							{
								rebarLength = Drawings.DrawingMark.ReturnDimensionValue(this.view, num1, textAttribute);
							}
							rebarString = string.Concat(rebarString, rebarLength);
						}
						else if (textAttribute.Units == -1)
						{
							rebarLength = num.ToString();
						}
						else
						{
							rebarLength = Drawings.DrawingMark.ReturnDimensionValue(this.view, num, textAttribute);
						}
					}
				}
			}

			private void GetRebarLengthValues(Drawings.DrawingMark.TextAttribute textAttribute, out string rebarLength, out string rebarString)
			{
				rebarString = string.Empty;
				rebarLength = string.Empty;
				List<double> rebarSegmentsLengths = Drawings.GetRebarSegmentsLengths(this.reinforcementModel);
				if (rebarSegmentsLengths != null && rebarSegmentsLengths.Count > 0)
				{
					double item = 0;
					this.ControlRebarOrdering(ref rebarSegmentsLengths);
					for (int i = 0; i < rebarSegmentsLengths.Count; i++)
					{
						if (i > 0)
						{
							rebarString = string.Concat(rebarString, "+");
						}
						item = item + rebarSegmentsLengths[i];
						if (textAttribute.Units == -1)
						{
							rebarLength = rebarSegmentsLengths[i].ToString();
						}
						else
						{
							rebarLength = Drawings.DrawingMark.ReturnDimensionValue(this.view, rebarSegmentsLengths[i], textAttribute);
						}
						rebarString = string.Concat(rebarString, rebarLength);
					}
					if (Compare.GT(item, 0))
					{
						if (textAttribute.Units != -1)
						{
							rebarLength = Drawings.DrawingMark.ReturnDimensionValue(this.view, item, textAttribute);
							return;
						}
						rebarLength = item.ToString();
					}
				}
			}

			public Drawings.DrawingMark.Size GetSize()
			{
				this.TotalSize = new Drawings.DrawingMark.Size();
				Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
				double num = 0;
				double num1 = 0;
				if (this.pullOut != null)
				{
					size = this.pullOut.GetSize(this.view, new Text.TextAttributes(this.textStandard));
				}
				foreach (Drawings.DrawingMark.TextLine textLine in this)
				{
					Drawings.DrawingMark.Size width = textLine.GetSize();
					width.Width = size.Width;
					double width1 = textLine.GetBeforeSize().Width;
					double width2 = textLine.GetAfterSize().Width;
					if (Compare.GT(width1, num))
					{
						num = width1;
					}
					if (Compare.GT(width2, num1))
					{
						num1 = width2;
					}
					Drawings.DrawingMark.Size size1 = width;
					size1.Width = size1.Width + (num + num1);
					if (Compare.GT(width.Width, this.TotalSize.Width))
					{
						this.TotalSize.Width = width.Width;
					}
					Drawings.DrawingMark.Size totalSize = this.TotalSize;
					totalSize.Height = totalSize.Height + width.Height;
				}
				if (base.Count > 1)
				{
					Drawings.DrawingMark.Size height = this.TotalSize;
					height.Height = height.Height + (double)(base.Count - 1) * this.rowDistance;
				}
				this.HeightWithoutPull = this.TotalSize.Height;
				if (Compare.GT(size.Height, this.TotalSize.Height))
				{
					this.TotalSize.Height = size.Height;
				}
				return this.TotalSize;
			}

			private double GetSymbolAngle(Tekla.Structures.Drawing.Line line)
			{
				double num = 0;
				Point point = new Point(line.EndPoint.X, line.StartPoint.Y, 0);
				if (!Compare.NE(line.EndPoint.Y, line.StartPoint.Y))
				{
					num = 0;
				}
				else if (!Compare.NE(line.EndPoint.X, line.StartPoint.X))
				{
					num = 90;
				}
				else
				{
					num = Math.Asin(Tekla.Structures.Geometry3d.Distance.PointToPoint(line.EndPoint, point) / Tekla.Structures.Geometry3d.Distance.PointToPoint(line.StartPoint, line.EndPoint));
					num = num * 180 / 3.14159265358979;
					if (Compare.GT(line.StartPoint.X, line.EndPoint.X))
					{
						num = 360 - num;
					}
					if (Compare.GT(line.StartPoint.Y, line.EndPoint.Y))
					{
						num = num * -1;
					}
				}
				return num;
			}

			private static string ReturnCcText(View view, string cc, Drawings.DrawingMark.TextAttribute textAtribute)
			{
				string empty = string.Empty;
				string str = " + ";
				char chr = ' ';
				double num = 0;
				if (cc.Contains<char>('+'))
				{
					string[] strArrays = cc.Split(new char[] { '+' });
					for (int i = 0; i < (int)strArrays.Length; i++)
					{
						if (i == 0)
						{
							strArrays[i] = strArrays[i].Substring(0, strArrays[i].Length - 1);
						}
						else if (i != (int)strArrays.Length - 1)
						{
							strArrays[i] = strArrays[i].Substring(1, strArrays[i].Length - 2);
						}
						else
						{
							strArrays[i] = strArrays[i].Substring(1, strArrays[i].Length - 1);
							str = string.Empty;
						}
						if (strArrays[i].Contains<char>('*') || strArrays[i].Contains<char>('x') || strArrays[i].Contains<char>('X'))
						{
							if (strArrays[i].Contains<char>('*'))
							{
								chr = '*';
							}
							else if (strArrays[i].Contains<char>('x'))
							{
								chr = 'x';
							}
							else if (strArrays[i].Contains<char>('X'))
							{
								chr = 'X';
							}
							empty = string.Concat(empty, strArrays[i].Substring(0, strArrays[i].IndexOf(chr) + 1));
							if (!double.TryParse(strArrays[i].Substring(strArrays[i].IndexOf(chr) + 1), out num))
							{
								return cc;
							}
							empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute), str);
						}
						else if (strArrays[i].Contains<char>('/') || strArrays[i].Contains<char>('-'))
						{
							if (strArrays[i].Contains<char>('/'))
							{
								chr = '/';
							}
							else if (strArrays[i].Contains<char>('-'))
							{
								chr = '-';
							}
							string[] strArrays1 = cc.Split(new char[] { chr });
							for (int j = 0; j < (int)strArrays1.Length - 1; j++)
							{
								if (!double.TryParse(strArrays1[j], out num))
								{
									return cc;
								}
								empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute), chr);
							}
							if (!double.TryParse(strArrays1[(int)strArrays1.Length - 1], out num))
							{
								return cc;
							}
							empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute), str);
						}
					}
				}
				else if (cc.Contains<char>('*') || cc.Contains<char>('x') || cc.Contains<char>('X'))
				{
					if (cc.Contains<char>('*'))
					{
						chr = '*';
					}
					else if (cc.Contains<char>('x'))
					{
						chr = 'x';
					}
					else if (cc.Contains<char>('X'))
					{
						chr = 'X';
					}
					empty = string.Concat(empty, cc.Substring(0, cc.IndexOf(chr) + 1));
					if (!double.TryParse(cc.Substring(cc.IndexOf(chr) + 1), out num))
					{
						return cc;
					}
					empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute));
				}
				else if (cc.Contains<char>('/') || cc.Contains<char>('-'))
				{
					if (cc.Contains<char>('/'))
					{
						chr = '/';
					}
					else if (cc.Contains<char>('-'))
					{
						chr = '-';
					}
					string[] strArrays2 = cc.Split(new char[] { chr });
					for (int k = 0; k < (int)strArrays2.Length - 1; k++)
					{
						if (!double.TryParse(strArrays2[k], out num))
						{
							return cc;
						}
						empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute), chr);
					}
					if (!double.TryParse(strArrays2[(int)strArrays2.Length - 1], out num))
					{
						return cc;
					}
					empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute));
				}
				else
				{
					if (!double.TryParse(cc, out num))
					{
						return cc;
					}
					empty = string.Concat(empty, Drawings.DrawingMark.ReturnDimensionValue(view, num, textAtribute));
				}
				return empty;
			}

			private static int ReturnCurrentUnitType()
			{
				int num = 0;
				if (Tekla.Structures.Datatype.Distance.CurrentUnitType == Tekla.Structures.Datatype.Distance.UnitType.Millimeter)
				{
					num = 1;
				}
				else if (Tekla.Structures.Datatype.Distance.CurrentUnitType == Tekla.Structures.Datatype.Distance.UnitType.Centimeter)
				{
					num = 2;
				}
				else if (Tekla.Structures.Datatype.Distance.CurrentUnitType == Tekla.Structures.Datatype.Distance.UnitType.Meter)
				{
					num = 3;
				}
				else if (Tekla.Structures.Datatype.Distance.CurrentUnitType == Tekla.Structures.Datatype.Distance.UnitType.Foot)
				{
					num = 4;
				}
				else if (Tekla.Structures.Datatype.Distance.CurrentUnitType == Tekla.Structures.Datatype.Distance.UnitType.Inch)
				{
					num = 5;
				}
				return num;
			}

			private static string ReturnDimensionValue(View view, double distance, Drawings.DrawingMark.TextAttribute textAttribute)
			{
				bool flag = false;
				StraightDimension current = null;
				DimensionSetBaseAttributes.DimensionValueFormats dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.NoDecimals;
				DimensionSetBaseAttributes.DimensionValuePrecisions dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.Whole;
				DimensionSetBaseAttributes.DimensionValueUnits dimensionValueUnit = DimensionSetBaseAttributes.DimensionValueUnits.Automatic;
				switch (textAttribute.Format)
				{
					case 1:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.OneOptionalDecimal;
						break;
					}
					case 2:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.OneDecimal;
						break;
					}
					case 3:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.TwoOptionalDecimals;
						break;
					}
					case 4:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.TwoDecimals;
						break;
					}
					case 5:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.ThreeOptionalDecimals;
						break;
					}
					case 6:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.ThreeDecimals;
						break;
					}
					case 7:
					{
						dimensionValueFormat = DimensionSetBaseAttributes.DimensionValueFormats.RationalPart;
						break;
					}
				}
				switch (textAttribute.Precision)
				{
					case 1:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerTwo;
						break;
					}
					case 2:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerThree;
						break;
					}
					case 3:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerFour;
						break;
					}
					case 4:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerEight;
						break;
					}
					case 5:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerSixteen;
						break;
					}
					case 6:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerThirtytwo;
						break;
					}
					case 7:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerTen;
						break;
					}
					case 8:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerHundred;
						break;
					}
					case 9:
					{
						dimensionValuePrecision = DimensionSetBaseAttributes.DimensionValuePrecisions.OnePerThousand;
						break;
					}
				}
				switch (textAttribute.Units)
				{
					case 1:
					{
						dimensionValueUnit = DimensionSetBaseAttributes.DimensionValueUnits.Millimeter;
						break;
					}
					case 2:
					{
						dimensionValueUnit = DimensionSetBaseAttributes.DimensionValueUnits.Centimeter;
						break;
					}
					case 3:
					{
						dimensionValueUnit = DimensionSetBaseAttributes.DimensionValueUnits.Meter;
						break;
					}
					case 4:
					case 5:
					{
						dimensionValueUnit = DimensionSetBaseAttributes.DimensionValueUnits.Inch;
						break;
					}
				}
				DimensionSetBaseAttributes.DimensionFormatAttributes dimensionFormatAttribute = new DimensionSetBaseAttributes.DimensionFormatAttributes(dimensionValuePrecision, dimensionValueFormat, dimensionValueUnit);
				StraightDimensionSet.StraightDimensionSetAttributes straightDimensionSetAttribute = new StraightDimensionSet.StraightDimensionSetAttributes((Tekla.Structures.Drawing.ModelObject)null)
				{
					Format = dimensionFormatAttribute
				};
				StraightDimensionSetHandler straightDimensionSetHandler = new StraightDimensionSetHandler();
				PointList pointList = new PointList();
				pointList.Add(new Point(0, 0, 0));
				pointList.Add(new Point(distance, 0, 0));
				StraightDimensionSet straightDimensionSet = straightDimensionSetHandler.CreateDimensionSet(view, pointList, new Vector(0, 0.1, 0), 0, straightDimensionSetAttribute);
				if (Compare.LT(distance, 0))
				{
					flag = true;
				}
				DrawingObjectEnumerator objects = straightDimensionSet.GetObjects();
				bool flag1 = true;
				while (objects.MoveNext() && flag1)
				{
					if (!(objects.Current is StraightDimension))
					{
						continue;
					}
					current = (StraightDimension)objects.Current;
					flag1 = false;
				}
				string unformattedString = current.Value.GetUnformattedString();
				if (unformattedString.Length <= 4)
				{
					unformattedString = "0";
				}
				else
				{
					unformattedString = unformattedString.Substring(2, unformattedString.Length - 4);
					if (flag)
					{
						unformattedString = string.Concat("-", unformattedString);
					}
				}
				if (textAttribute.Units == 5)
				{
					unformattedString = Drawings.DrawingMark.ReturnInches(unformattedString);
				}
				if (textAttribute.Units == 4 && !unformattedString.Contains<char>('\"') && (unformattedString.Contains<char>('-') || unformattedString.Contains<char>('\\') || !unformattedString.Contains<char>('\'')))
				{
					unformattedString = string.Concat(unformattedString, "\"");
				}
				straightDimensionSet.Delete();
				if (current != null)
				{
					current.Delete();
				}
				return unformattedString;
			}

			private static string ReturnInches(string value)
			{
				string empty = string.Empty;
				int num = 0;
				int num1 = 0;
				char chr = '\"';
				if (value != string.Empty)
				{
					string[] strArrays = value.Split(new char[] { '\\' });
					string[] strArrays1 = strArrays[0].Split(new char[] { '\'' });
					if (int.TryParse(strArrays1[0], out num) && strArrays[0].Contains<char>('\''))
					{
						num = num * 12;
						if ((int)strArrays1.Length > 1 && strArrays1[1].Contains<char>('-'))
						{
							empty = strArrays1[1].Substring(strArrays1[1].IndexOf('-') + 1);
						}
						if ((int)strArrays1.Length == 1)
						{
							empty = strArrays1[0];
						}
						if (empty.Contains<char>(chr))
						{
							int length = empty.Length;
							empty = empty.Substring(0, length - 1);
						}
						string[] strArrays2 = empty.Split(new char[] { ',' });
						if (int.TryParse(strArrays2[0], out num1))
						{
							num = num + num1;
						}
						empty = ((int)strArrays2.Length <= 1 ? num.ToString() : string.Concat(num, ",", strArrays2[1]));
					}
					if ((int)strArrays1.Length == 1 && !strArrays[0].Contains<char>('\''))
					{
						empty = strArrays1[0];
					}
					strArrays[0] = empty;
					empty = string.Join(" ", strArrays);
					if (!empty.Contains<char>('\"'))
					{
						empty = string.Concat(empty, "\"");
					}
				}
				return empty;
			}

			private Drawings.DrawingMark.TextItem SetPulloutPicture(Drawings.DrawingMark.TextAttribute textAttribute)
			{
				ReinforcementPulloutElement reinforcementPulloutElement = new ReinforcementPulloutElement();
				Drawings.DrawingMark.TextItem textItem = new Drawings.DrawingMark.TextItem(reinforcementPulloutElement, this.reinforcementDrawing);
				reinforcementPulloutElement.AutomaticScaling = true;
				reinforcementPulloutElement.ScaleX = 0;
				reinforcementPulloutElement.ScaleY = 0;
				reinforcementPulloutElement.Dimensioning = textAttribute.PullOutDimensioning != 0;
				reinforcementPulloutElement.Exaggeration = textAttribute.PullOutExaggeration != 0;
				reinforcementPulloutElement.BendingRadius = textAttribute.PullOutBendingRadius != 0;
				reinforcementPulloutElement.BendingAngle = textAttribute.PullOutBendingAngle != 0;
				if (textAttribute.PullOutAutomaticScaling > 0)
				{
					reinforcementPulloutElement.AutomaticScaling = false;
					reinforcementPulloutElement.ScaleX = textAttribute.PullOutScaleX;
					if (textAttribute.PullOutAutomaticScaling == 2)
					{
						reinforcementPulloutElement.ScaleY = textAttribute.PullOutScaleY;
					}
				}
				if (textAttribute.PullOutRotationAxis == 0)
				{
					reinforcementPulloutElement.RotationAxis = ReinforcementPulloutElement.Rotation.Automatic;
				}
				else if (textAttribute.PullOutRotationAxis != 1)
				{
					reinforcementPulloutElement.RotationAxis = ReinforcementPulloutElement.Rotation.Global;
				}
				else
				{
					reinforcementPulloutElement.RotationAxis = ReinforcementPulloutElement.Rotation.Plane;
				}
				if (textAttribute.PullOutEndSymbolType == 0)
				{
					reinforcementPulloutElement.EndSymbolType = ReinforcementPulloutElement.EndSymbols.None;
				}
				else if (textAttribute.PullOutEndSymbolType != 1)
				{
					reinforcementPulloutElement.EndSymbolType = ReinforcementPulloutElement.EndSymbols.Both;
				}
				else
				{
					reinforcementPulloutElement.EndSymbolType = ReinforcementPulloutElement.EndSymbols.Single;
				}
				return textItem;
			}

			public void SetSymbolAndPullOutAngle(Tekla.Structures.Drawing.Line line)
			{
				double symbolAngle = this.GetSymbolAngle(line);
				foreach (Drawings.DrawingMark.TextLine textLine in this)
				{
					textLine.SetAngle(symbolAngle);
				}
				if (this.pullOut != null)
				{
					this.pullOut.Angle = symbolAngle;
				}
				this.GetSize();
			}

			public enum AttributeTypes
			{
				[Code(0)]
				[Translation("albl_Name")]
				Name,
				[Code(1)]
				[Translation("albl_Grade")]
				Grade,
				[Code(2)]
				[Translation("albl_Diameter")]
				Diameter,
				[Code(3)]
				[Translation("albl_Class")]
				Class,
				[Code(4)]
				[Translation("albl_Length")]
				Length,
				[Code(5)]
				[Translation("albl_Number")]
				Number,
				[Code(6)]
				[Translation("albl_Position")]
				Position,
				[Code(7)]
				[Translation("albl_Shape")]
				Shape,
				[Code(8)]
				[Translation("albl_Weight")]
				Weight,
				[Code(9)]
				[Translation("albl_cc")]
				CC,
				[Code(10)]
				[Translation("albl_cc_min")]
				CCMin,
				[Code(11)]
				[Translation("albl_cc_max")]
				CCMax,
				[Code(12)]
				[Translation("albl_cc_exact")]
				CCExact,
				[Code(13)]
				[Translation("albl_cc_target")]
				CCTarget,
				[Code(14)]
				[Translation("albl_cc_separator")]
				CCSeparator,
				[Code(15)]
				[Translation("albl_Text")]
				Text,
				[Code(16)]
				[Translation("albl_UDA")]
				UDA,
				[Code(17)]
				[Translation("albl_Symbol")]
				Symbol,
				[Code(18)]
				[Translation("albl_Length_itemized_")]
				LengthItemized1,
				[Code(19)]
				[Translation("albl_Length_itemized")]
				LengthItemized2,
				[Code(20)]
				[Translation("<--'")]
				NewLine,
				[Code(21)]
				[Translation("albl_Pullout_picture")]
				PulloutPicture,
				[Code(22)]
				[Translation("< >")]
				ExtraGap,
				[Code(23)]
				[Translation("<--")]
				RemoveGap,
				[Code(24)]
				[Translation("albl_Template_attribute")]
				Template
			}

			public class Code : Attribute
			{
				public int Value
				{
					get;
					private set;
				}

				public Code(int value)
				{
					this.Value = value;
				}
			}

			public class Size
			{
				public double Height
				{
					get;
					set;
				}

				public double Width
				{
					get;
					set;
				}

				public Size()
				{
					this.Height = 0;
					this.Width = 0;
				}

				public static Drawings.DrawingMark.Size operator +(Drawings.DrawingMark.Size size1, Drawings.DrawingMark.Size size2)
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size()
					{
						Height = (Compare.GT(size1.Height, size2.Height) ? size1.Height : size2.Height),
						Width = size1.Width + size2.Width
					};
					return size;
				}

				public override string ToString()
				{
					return string.Concat(this.Height, "*", this.Width);
				}
			}

			private class TextAttribute
			{
				private readonly int format;

				private readonly int precision;

				private readonly int pullOutAutomaticScaling;

				private readonly int pullOutBendingAngle;

				private readonly int pullOutBendingRadius;

				private readonly int pullOutDimensioning;

				private readonly int pullOutEndSymbolType;

				private readonly int pullOutExaggeration;

				private readonly int pullOutRotationAxis;

				private readonly double pullOutScaleX;

				private readonly double pullOutScaleY;

				private readonly int typeLength;

				private readonly int units;

				public Drawings.DrawingMark.AttributeTypes AttributeType
				{
					get;
					private set;
				}

				public List<string> Code
				{
					get;
					private set;
				}

				public int Format
				{
					get
					{
						return this.format;
					}
				}

				public int Precision
				{
					get
					{
						return this.precision;
					}
				}

				public int PullOutAutomaticScaling
				{
					get
					{
						return this.pullOutAutomaticScaling;
					}
				}

				public int PullOutBendingAngle
				{
					get
					{
						return this.pullOutBendingAngle;
					}
				}

				public int PullOutBendingRadius
				{
					get
					{
						return this.pullOutBendingRadius;
					}
				}

				public int PullOutDimensioning
				{
					get
					{
						return this.pullOutDimensioning;
					}
				}

				public int PullOutEndSymbolType
				{
					get
					{
						return this.pullOutEndSymbolType;
					}
				}

				public int PullOutExaggeration
				{
					get
					{
						return this.pullOutExaggeration;
					}
				}

				public int PullOutRotationAxis
				{
					get
					{
						return this.pullOutRotationAxis;
					}
				}

				public double PullOutScaleX
				{
					get
					{
						return this.pullOutScaleX;
					}
				}

				public double PullOutScaleY
				{
					get
					{
						return this.pullOutScaleY;
					}
				}

				public int TypeLength
				{
					get
					{
						return this.typeLength;
					}
				}

				public int Units
				{
					get
					{
						return this.units;
					}
				}

				public TextAttribute(string code, int currentUnitType)
				{
					string[] strArrays = new string[] { "." };
					string[] strArrays1 = code.Split(strArrays, StringSplitOptions.RemoveEmptyEntries);
					this.AttributeType = (Drawings.DrawingMark.AttributeTypes)int.Parse(strArrays1[0]);
					this.Code = new List<string>();
					this.format = 0;
					this.precision = 0;
					this.typeLength = 0;
					this.units = 0;
					if (this.AttributeType == Drawings.DrawingMark.AttributeTypes.Template)
					{
						string empty = string.Empty;
						for (int i = 1; i < (int)strArrays1.Length; i++)
						{
							empty = (i != (int)strArrays1.Length - 1 ? string.Concat(empty, strArrays1[i], ".") : string.Concat(empty, strArrays1[i]));
						}
						this.Code.Add(empty);
						return;
					}
					if (this.AttributeType == Drawings.DrawingMark.AttributeTypes.PulloutPicture)
					{
						string[] strArrays2 = new string[] { "." };
						strArrays1 = code.Split(strArrays2, StringSplitOptions.None);
						for (int j = 1; j < (int)strArrays1.Length; j++)
						{
							this.Code.Add(strArrays1[j]);
							switch (j)
							{
								case 1:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutAutomaticScaling))
									{
										break;
									}
									this.pullOutAutomaticScaling = 0;
									break;
								}
								case 2:
								{
									if (double.TryParse(strArrays1[j], out this.pullOutScaleX))
									{
										break;
									}
									this.pullOutScaleX = 0;
									break;
								}
								case 3:
								{
									if (double.TryParse(strArrays1[j], out this.pullOutScaleY))
									{
										break;
									}
									this.pullOutScaleY = 0;
									break;
								}
								case 4:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutRotationAxis))
									{
										break;
									}
									this.pullOutRotationAxis = 1;
									break;
								}
								case 5:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutEndSymbolType))
									{
										break;
									}
									this.pullOutEndSymbolType = 0;
									break;
								}
								case 6:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutDimensioning))
									{
										break;
									}
									this.pullOutDimensioning = 1;
									break;
								}
								case 7:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutExaggeration))
									{
										break;
									}
									this.pullOutExaggeration = 1;
									break;
								}
								case 8:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutBendingRadius))
									{
										break;
									}
									this.pullOutBendingRadius = 1;
									break;
								}
								case 9:
								{
									if (int.TryParse(strArrays1[j], out this.pullOutBendingAngle))
									{
										break;
									}
									this.pullOutBendingAngle = 1;
									break;
								}
							}
						}
					}
					else
					{
						for (int k = 1; k < (int)strArrays1.Length; k++)
						{
							this.Code.Add(strArrays1[k]);
							if (k == 1 && !int.TryParse(strArrays1[k], out this.units))
							{
								this.units = 0;
							}
							else if (k == 2 && !int.TryParse(strArrays1[k], out this.format))
							{
								this.format = 0;
							}
							else if (k == 3 && !int.TryParse(strArrays1[k], out this.precision))
							{
								this.precision = 0;
							}
							else if (k == 4 && !int.TryParse(strArrays1[k], out this.typeLength))
							{
								this.typeLength = 0;
							}
						}
						if (this.Units == 0)
						{
							this.units = currentUnitType;
							return;
						}
					}
				}
			}

			public class TextItem
			{
				private readonly ReinforcementBase parentReinforcement;

				public double Angle
				{
					get;
					set;
				}

				public ReinforcementPulloutElement PulloutElement
				{
					get;
					set;
				}

				public SymbolInfo Symbol
				{
					get;
					set;
				}

				public string TextValue
				{
					get;
					set;
				}

				public TextItem(ReinforcementBase reinforcement)
				{
					this.TextValue = string.Empty;
					this.Symbol = null;
					this.PulloutElement = null;
					this.Angle = 0;
					this.parentReinforcement = reinforcement;
				}

				public TextItem(string text, ReinforcementBase reinforcement) : this(reinforcement)
				{
					this.TextValue = text;
				}

				public TextItem(ReinforcementPulloutElement pulloutElement, ReinforcementBase reinforcement) : this(reinforcement)
				{
					this.PulloutElement = pulloutElement;
				}

				public void Draw(View view, Text.TextAttributes textStandard, Tekla.Structures.Drawing.Line line, ref Point pointLocation)
				{
					Drawings.DrawingMark.Size size = this.GetSize(view, textStandard);
					Vector lineVector = Drawings.DrawingMark.GetLineVector(line);
					pointLocation = Geo.MovePoint(pointLocation, lineVector, size.Width / 2);
					AlongLinePlacing alongLinePlacing = new AlongLinePlacing(line.StartPoint, line.EndPoint);
					textStandard.PreferredPlacing = PreferredPlacingTypes.AlongLinePlacingType();
					if (this.Symbol != null)
					{
						if (this.Angle == 0)
						{
							this.Angle = textStandard.Angle;
						}
						Drawings.DrawSymbol(view, pointLocation, textStandard.Font.Color, this.Symbol.SymbolFile, this.Symbol.SymbolIndex, textStandard.Font.Height, this.Angle);
					}
					else if (this.PulloutElement != null)
					{
						this.DrawPulloutPicture(pointLocation);
					}
					else if (this.TextValue != string.Empty)
					{
						Text text = new Text(view, pointLocation, this.TextValue, alongLinePlacing, textStandard);
						text.Insert();
					}
					pointLocation = Geo.MovePoint(pointLocation, lineVector, size.Width / 2);
				}

				private void DrawPulloutPicture(Point point)
				{
					Mark mark = new Mark(this.parentReinforcement)
					{
						InsertionPoint = point,
						Attributes = new Mark.MarkAttributes(this.parentReinforcement)
					};
					mark.Attributes.Content.Clear();
					mark.Attributes.Content.Add(this.PulloutElement);
					mark.Attributes.PlacingAttributes.IsFixed = true;
					mark.Attributes.PreferredPlacing = new PointPlacingType();
					mark.Attributes.Frame.Type = FrameTypes.None;
					mark.Insert();
				}

				private Drawings.DrawingMark.Size GetPullOutPictureSize(ReinforcementPulloutElement pulloutElement, ReinforcementBase reinforcement)
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					Mark mark = new Mark(reinforcement)
					{
						InsertionPoint = new Point(),
						Attributes = new Mark.MarkAttributes(reinforcement)
					};
					mark.Attributes.Content.Clear();
					mark.Attributes.Content.Add(pulloutElement);
					mark.Attributes.PlacingAttributes.IsFixed = true;
					mark.Attributes.PreferredPlacing = new PointPlacingType();
					mark.Attributes.Frame.Type = FrameTypes.None;
					mark.Insert();
					size.Width = mark.GetObjectAlignedBoundingBox().Width;
					size.Height = mark.GetObjectAlignedBoundingBox().Height;
					double num = 0;
					double height = 0;
					double num1 = Math.Abs(this.Angle);
					double width = size.Width;
					double width1 = size.Width;
					double height1 = size.Height;
					if (Compare.GT(num1, 90))
					{
						num1 = 90 - num1 % 90;
					}
					if (Compare.EQ(num1, 90))
					{
						width = size.Width;
						size.Width = size.Height;
						size.Height = width;
					}
					else if (Compare.NZ(num1))
					{
						width1 = width / Math.Cos(num1 * 3.14159265358979 / 180);
						num = Math.Sqrt(width1 * width1 - width * width);
						height = size.Height - num;
						width1 = width1 + Math.Cos(num1 * 3.14159265358979 / 180) * height;
						height1 = size.Height / Math.Sin(num1 * 3.14159265358979 / 180);
						num = Math.Sqrt(height1 * height1 - size.Height * size.Height);
						height = width - num;
						height1 = height1 + Math.Cos(num1 * 3.14159265358979 / 180) * height;
						size.Width = width1;
						size.Height = height1;
					}
					mark.Delete();
					return size;
				}

				public Drawings.DrawingMark.Size GetSize(View view, Text.TextAttributes textStandard)
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					if (this.Symbol != null)
					{
						size = this.GetSymbolSize(this.Symbol, view, textStandard);
						Drawings.DrawingMark.Size width = size;
						width.Width = width.Width / 2;
					}
					else if (this.TextValue != string.Empty)
					{
						size = Drawings.DrawingMark.TextItem.GetTextSize(this.TextValue, view, textStandard, true);
					}
					else if (this.PulloutElement == null)
					{
						size.Height = textStandard.Font.Height * view.Attributes.Scale;
					}
					else
					{
						size = this.GetPullOutPictureSize(this.PulloutElement, this.parentReinforcement);
					}
					return size;
				}

				private Drawings.DrawingMark.Size GetSymbolSize(SymbolInfo symbolInfo, View view, Text.TextAttributes textStandard)
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					Symbol.SymbolAttributes symbolAttribute = new Symbol.SymbolAttributes()
					{
						Height = textStandard.Font.Height,
						Color = textStandard.Font.Color,
						Angle = this.Angle,
						PreferredPlacing = PreferredSymbolPlacingTypes.PointPlacingType()
					};
					Symbol symbol = new Symbol(view, new Point(0, 0, 0), symbolInfo, symbolAttribute);
					symbol.Insert();
					size.Width = symbol.GetObjectAlignedBoundingBox().Width;
					size.Height = textStandard.Font.Height * view.Attributes.Scale;
					symbol.Delete();
					return size;
				}

				private static Drawings.DrawingMark.Size GetTextSize(string text, View view, Text.TextAttributes textStandard, bool isRound)
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					if (text != null && text.Length > 0)
					{
						AlongLinePlacing alongLinePlacing = new AlongLinePlacing(new Point(0, 0, 0), new Point(1000, 0, 0));
						Text text1 = new Text(view, new Point(0, 0, 0), text, alongLinePlacing, textStandard);
						text1.Insert();
						size.Width = text1.GetObjectAlignedBoundingBox().Width;
						size.Height = textStandard.Font.Height * view.Attributes.Scale;
						text1.Delete();
						if (isRound && textStandard.Frame.Type != FrameTypes.None)
						{
							Drawings.DrawingMark.Size width = size;
							width.Width = width.Width + textStandard.Font.Height * view.Attributes.Scale * 2;
						}
					}
					return size;
				}

				public bool IsEmpty()
				{
					if (this.Symbol != null || !(this.TextValue == string.Empty))
					{
						return false;
					}
					return this.PulloutElement == null;
				}

				public override string ToString()
				{
					if (this.Symbol == null)
					{
						if (this.PulloutElement != null)
						{
							return "[PullOutPicture]";
						}
						return this.TextValue;
					}
					object[] symbolFile = new object[] { "[", this.Symbol.SymbolFile, " ", this.Symbol.SymbolIndex, "]" };
					return string.Concat(symbolFile);
				}
			}

			public class TextLine
			{
				private readonly List<Drawings.DrawingMark.TextItem> afterPullOutTexts;

				private readonly List<Drawings.DrawingMark.TextItem> beforePullOutTexts;

				private readonly Text.TextAttributes rebarPositionStandard;

				private readonly double rowDistance;

				private readonly Text.TextAttributes textStandard;

				private readonly View view;

				private Drawings.DrawingMark.TextItem rebarPosition;

				public int Count
				{
					get
					{
						return this.afterPullOutTexts.Count + this.beforePullOutTexts.Count;
					}
				}

				public Drawings.DrawingMark.TextItem this[int index]
				{
					get
					{
						if (index < this.beforePullOutTexts.Count)
						{
							return this.beforePullOutTexts[index];
						}
						return this.afterPullOutTexts[index - this.beforePullOutTexts.Count];
					}
					set
					{
						if (index < this.beforePullOutTexts.Count)
						{
							this.beforePullOutTexts[index] = value;
							return;
						}
						this.afterPullOutTexts[index - this.beforePullOutTexts.Count] = value;
					}
				}

				public Drawings.DrawingMark.TextItem RebarPosition
				{
					get
					{
						return this.rebarPosition;
					}
					set
					{
						this.rebarPosition = value;
						this.Size = this.GetSize();
					}
				}

				public int RebarPositionType
				{
					get;
					set;
				}

				public double RowDistance
				{
					get
					{
						return this.rowDistance;
					}
				}

				public Drawings.DrawingMark.Size Size
				{
					get;
					private set;
				}

				public TextLine(ReinforcementBase reinforcement, string textStandard, string rebarPositionStandard, View view, bool afterPullOut, double rowDistance = 0)
				{
					this.afterPullOutTexts = new List<Drawings.DrawingMark.TextItem>();
					this.beforePullOutTexts = new List<Drawings.DrawingMark.TextItem>();
					this.textStandard = Drawings.DrawingMark.TextLine.SetTextAttributes(textStandard);
					this.rebarPositionStandard = Drawings.DrawingMark.TextLine.SetTextAttributes(rebarPositionStandard);
					this.view = view;
					this.rebarPosition = null;
					this.RebarPositionType = 0;
					if (!Compare.LE(rowDistance, 0))
					{
						this.rowDistance = rowDistance;
					}
					else
					{
						this.rowDistance = this.textStandard.Font.Height * this.view.Attributes.Scale * 1.5;
					}
					this.Add(new Drawings.DrawingMark.TextItem(reinforcement), afterPullOut);
					this.GetSize();
				}

				public void Add(Drawings.DrawingMark.TextItem text, bool afterPullOut)
				{
					if (afterPullOut)
					{
						this.afterPullOutTexts.Add(text);
						return;
					}
					this.beforePullOutTexts.Add(text);
				}

				public void Draw(Tekla.Structures.Drawing.Line line, Point point, Drawings.DrawingMark.Size pullOutSize)
				{
					Point point1 = new Point(point);
					if (this.rebarPosition != null && this.RebarPositionType == 1)
					{
						this.rebarPosition.Draw(this.view, this.rebarPositionStandard, line, ref point1);
					}
					foreach (Drawings.DrawingMark.TextItem beforePullOutText in this.beforePullOutTexts)
					{
						beforePullOutText.Draw(this.view, this.textStandard, line, ref point1);
					}
					if (Compare.NZ(pullOutSize.Width))
					{
						double width = pullOutSize.Width - this.GetBeforeSize().Width;
						Vector lineVector = Drawings.DrawingMark.GetLineVector(line);
						point1 = Geo.MovePoint(point1, lineVector, width);
					}
					foreach (Drawings.DrawingMark.TextItem afterPullOutText in this.afterPullOutTexts)
					{
						afterPullOutText.Draw(this.view, this.textStandard, line, ref point1);
					}
					if (this.rebarPosition != null && this.RebarPositionType == 2)
					{
						this.rebarPosition.Draw(this.view, this.rebarPositionStandard, line, ref point1);
					}
				}

				public Drawings.DrawingMark.Size GetAfterSize()
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					foreach (Drawings.DrawingMark.TextItem afterPullOutText in this.afterPullOutTexts)
					{
						size = size + afterPullOutText.GetSize(this.view, this.textStandard);
					}
					if (this.rebarPosition != null && this.RebarPositionType == 2)
					{
						size = size + this.rebarPosition.GetSize(this.view, this.rebarPositionStandard);
					}
					return size;
				}

				public Drawings.DrawingMark.Size GetBeforeSize()
				{
					Drawings.DrawingMark.Size size = new Drawings.DrawingMark.Size();
					foreach (Drawings.DrawingMark.TextItem beforePullOutText in this.beforePullOutTexts)
					{
						size = size + beforePullOutText.GetSize(this.view, this.textStandard);
					}
					if (this.rebarPosition != null && this.RebarPositionType == 1)
					{
						size = size + this.rebarPosition.GetSize(this.view, this.rebarPositionStandard);
					}
					return size;
				}

				public Drawings.DrawingMark.Size GetSize()
				{
					this.Size = new Drawings.DrawingMark.Size();
					foreach (Drawings.DrawingMark.TextItem beforePullOutText in this.beforePullOutTexts)
					{
						if (beforePullOutText.PulloutElement != null)
						{
							continue;
						}
						Drawings.DrawingMark.TextLine size = this;
						size.Size = size.Size + beforePullOutText.GetSize(this.view, this.textStandard);
					}
					foreach (Drawings.DrawingMark.TextItem afterPullOutText in this.afterPullOutTexts)
					{
						if (afterPullOutText.PulloutElement != null)
						{
							continue;
						}
						Drawings.DrawingMark.TextLine textLine = this;
						textLine.Size = textLine.Size + afterPullOutText.GetSize(this.view, this.textStandard);
					}
					if (this.rebarPosition != null)
					{
						Drawings.DrawingMark.TextLine size1 = this;
						size1.Size = size1.Size + this.rebarPosition.GetSize(this.view, this.rebarPositionStandard);
					}
					if (this.Size.Height == 0 && this.Count == 0)
					{
						this.Size.Height = this.textStandard.Font.Height * this.view.Attributes.Scale * 1.5;
					}
					return this.Size;
				}

				public void RemoveAt(int index)
				{
					if (index < this.beforePullOutTexts.Count)
					{
						this.beforePullOutTexts.RemoveAt(index);
						return;
					}
					this.afterPullOutTexts.RemoveAt(index - this.beforePullOutTexts.Count);
				}

				public void SetAngle(double angle)
				{
					foreach (Drawings.DrawingMark.TextItem beforePullOutText in this.beforePullOutTexts)
					{
						if (beforePullOutText.Symbol == null && beforePullOutText.PulloutElement == null)
						{
							continue;
						}
						beforePullOutText.Angle = angle;
					}
					foreach (Drawings.DrawingMark.TextItem afterPullOutText in this.afterPullOutTexts)
					{
						if (afterPullOutText.Symbol == null && afterPullOutText.PulloutElement == null)
						{
							continue;
						}
						afterPullOutText.Angle = angle;
					}
				}

				private static Text.TextAttributes SetTextAttributes(string name)
				{
					Text.TextAttributes textAttribute = new Text.TextAttributes();
					if (name == string.Empty)
					{
						textAttribute.PreferredPlacing = PreferredTextPlacingTypes.PointPlacingType();
						textAttribute.PlacingAttributes.PlacingDistance.MinimalDistance = 5;
						textAttribute.PlacingAttributes.PlacingDistance.SearchMargin = 1;
						textAttribute.Font.Color = DrawingColors.Red;
						textAttribute.Font.Name = "Arial";
						textAttribute.Font.Height = 3;
						textAttribute.Frame.Type = FrameTypes.None;
					}
					else
					{
						textAttribute.LoadAttributes(name);
					}
					textAttribute.PlacingAttributes.IsFixed = true;
					textAttribute.PlacingAttributes.PlacingQuarter.TopLeft = true;
					return textAttribute;
				}

				public override string ToString()
				{
					string empty = string.Empty;
					foreach (Drawings.DrawingMark.TextItem beforePullOutText in this.beforePullOutTexts)
					{
						empty = string.Concat(empty, beforePullOutText.ToString(), " ");
					}
					foreach (Drawings.DrawingMark.TextItem afterPullOutText in this.afterPullOutTexts)
					{
						empty = string.Concat(empty, afterPullOutText.ToString(), " ");
					}
					return empty;
				}
			}

			public class Translation : Attribute
			{
				public string Value
				{
					get;
					private set;
				}

				public Translation(string value)
				{
					this.Value = value;
				}
			}
		}
	}
}