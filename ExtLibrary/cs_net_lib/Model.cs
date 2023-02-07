using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace cs_net_lib
{
    public static class Model
    {
        private const double DivisionConstantForHalfLength = 2;

        private const int MinimalNumberOfPointsInLine = 2;

        private static double CenterLineLength(RebarGeometry geometry)
        {
            double num = 0;
            Vector vector = null;
            for (int i = 0; i < geometry.Shape.Points.Count - 1; i++)
            {
                Vector vectorLineSegment = Geo.GetVectorLineSegment(geometry.Shape.Points[i + 1] as Point, geometry.Shape.Points[i] as Point);
                num = num + Math.Sqrt(vectorLineSegment.Dot(vectorLineSegment));
                Vector normal = vectorLineSegment.GetNormal();
                if (vector != null)
                {
                    double num1 = Math.Acos(Math.Max(-1, Math.Min(1, vector.Dot(normal))));
                    double bendingRadius = cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter / 2;
                    num = num + (num1 * bendingRadius - 2 * Math.Tan(num1 / 2) * bendingRadius);
                }
                vector = normal;
            }
            return num;
        }

        private static List<double> CenterLineLengths(RebarGeometry geometry)
        {
            List<double> nums = new List<double>();
            for (int i = 0; i < geometry.Shape.Points.Count - 1; i++)
            {
                Vector legthVector = cs_net_lib.Model.GetLegthVector(geometry, i);
                if (legthVector != null)
                {
                    nums.Add(Math.Sqrt(legthVector.Dot(legthVector)));
                    legthVector.Normalize();
                    Vector vector = cs_net_lib.Model.GetLegthVector(geometry, i - 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        List<double> item = nums;
                        List<double> nums1 = item;
                        int count = nums.Count - 1;
                        int num = count;
                        item[count] = nums1[num] - cs_net_lib.Model.RadiusDelta(vector, legthVector, cs_net_lib.Model.GetBendingRadius(geometry, i - 1) + geometry.Diameter / 2);
                    }
                    vector = cs_net_lib.Model.GetLegthVector(geometry, i + 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        List<double> item1 = nums;
                        List<double> nums2 = item1;
                        int count1 = nums.Count - 1;
                        int num1 = count1;
                        item1[count1] = nums2[num1] - cs_net_lib.Model.RadiusDelta(legthVector, vector, cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter / 2);
                    }
                }
            }
            return nums;
        }

        public static RebarGroup ConvertToExactSpacingsDistribution(RebarGroup originalGroup)
        {
            RebarGroup rebarGroup = null;
            double num = 400;
            if (originalGroup != null)
            {
                ModelObject modelObject = (new Tekla.Structures.Model.Model()).SelectModelObject(originalGroup.Identifier);
                if (modelObject != null && modelObject.GetType().Equals(typeof(RebarGroup)))
                {
                    rebarGroup = modelObject as RebarGroup;
                }
            }
            if (rebarGroup != null)
            {
                Vector vector = new Vector(rebarGroup.EndPoint - rebarGroup.StartPoint);
                vector.Normalize();
                if (rebarGroup.SpacingType != BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS)
                {
                    List<double> nums = new List<double>();
                    ArrayList rebarGeometries = rebarGroup.GetRebarGeometries(false);
                    if (rebarGeometries.Count > 0)
                    {
                        rebarGroup.FromPlaneOffset = 0;
                        rebarGroup.ExcludeType = BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;
                        if (rebarGroup.StirrupType == RebarGroup.RebarGroupStirrupTypeEnum.STIRRUP_TYPE_SPIRAL)
                        {
                            Point point = null;
                            foreach (Point point1 in ((RebarGeometry)rebarGeometries[0]).Shape.Points)
                            {
                                if (point == null)
                                {
                                    point = point1;
                                    RebarGroup startPoint = rebarGroup;
                                    startPoint.StartPoint = startPoint.StartPoint + (vector.Dot(new Vector(point - rebarGroup.StartPoint)) * vector);
                                }
                                else
                                {
                                    Vector vector1 = new Vector(point1 - point);
                                    Vector vector2 = new Vector(vector1 - (vector.Dot(vector1) * vector));
                                    if (!Compare.LT(vector2.Dot(vector2), num))
                                    {
                                        continue;
                                    }
                                    nums.Add(vector.Dot(vector1));
                                    point = point1;
                                }
                            }
                            RebarGroup endPoint = rebarGroup;
                            endPoint.EndPoint = endPoint.EndPoint + (vector.Dot(new Vector(point - rebarGroup.EndPoint)) * vector);
                        }
                        else
                        {
                            Point point2 = null;
                            for (int i = 0; i < rebarGeometries.Count; i++)
                            {
                                Point item = (Point)((RebarGeometry)rebarGeometries[i]).Shape.Points[0];
                                if (point2 == null)
                                {
                                    RebarGroup startPoint1 = rebarGroup;
                                    startPoint1.StartPoint = startPoint1.StartPoint + (vector.Dot(new Vector(item - rebarGroup.StartPoint)) * vector);
                                }
                                else
                                {
                                    Vector vector3 = new Vector(item - point2);
                                    nums.Add(vector.Dot(vector3));
                                }
                                point2 = item;
                            }
                            RebarGroup endPoint1 = rebarGroup;
                            endPoint1.EndPoint = endPoint1.EndPoint + (vector.Dot(new Vector(point2 - rebarGroup.EndPoint)) * vector);
                        }
                        rebarGroup.Spacings.Clear();
                        foreach (double num1 in nums)
                        {
                            rebarGroup.Spacings.Add(num1);
                        }
                        rebarGroup.SpacingType = BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS;
                    }
                }
                else
                {
                    if (rebarGroup.ExcludeType == BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_FIRST || rebarGroup.ExcludeType == BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_BOTH)
                    {
                        double item1 = (double)rebarGroup.Spacings[0];
                        RebarGroup rebarGroup1 = rebarGroup;
                        rebarGroup1.StartPoint = rebarGroup1.StartPoint + (item1 * vector);
                        rebarGroup.Spacings.RemoveAt(0);
                    }
                    if (rebarGroup.ExcludeType == BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_LAST || rebarGroup.ExcludeType == BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_BOTH)
                    {
                        double item2 = (double)rebarGroup.Spacings[rebarGroup.Spacings.Count - 1];
                        RebarGroup endPoint2 = rebarGroup;
                        endPoint2.EndPoint = endPoint2.EndPoint - (item2 * vector);
                        rebarGroup.Spacings.RemoveAt(rebarGroup.Spacings.Count - 1);
                    }
                }
            }
            return rebarGroup;
        }

        public static Beam CopyBeam(Beam original)
        {
            Beam beam = new Beam()
            {
                StartPoint = new Point(original.StartPoint),
                EndPoint = new Point(original.EndPoint),
                AssemblyNumber = cs_net_lib.Model.CopyNumberingSeries(original.AssemblyNumber),
                PartNumber = cs_net_lib.Model.CopyNumberingSeries(original.PartNumber),
                CastUnitType = original.CastUnitType,
                Class = original.Class,
                DeformingData = cs_net_lib.Model.CopyDeformingData(original.DeformingData),
                EndPointOffset = cs_net_lib.Model.CopyOffset(original.EndPointOffset),
                StartPointOffset = cs_net_lib.Model.CopyOffset(original.StartPointOffset),
                Finish = original.Finish
            };
            beam.Material.MaterialString = original.Material.MaterialString;
            beam.Name = original.Name;
            beam.Position = cs_net_lib.Model.CopyPosition(original.Position);
            beam.Profile.ProfileString = original.Profile.ProfileString;
            return beam;
        }

        public static DeformingData CopyDeformingData(DeformingData original)
        {
            DeformingData deformingDatum = new DeformingData()
            {
                Angle = original.Angle,
                Angle2 = original.Angle2,
                Cambering = original.Cambering,
                Shortening = original.Shortening
            };
            return deformingDatum;
        }

        public static NumberingSeries CopyNumberingSeries(NumberingSeries original)
        {
            NumberingSeries numberingSeries = new NumberingSeries()
            {
                Prefix = original.Prefix,
                StartNumber = original.StartNumber
            };
            return numberingSeries;
        }

        public static Offset CopyOffset(Offset original)
        {
            Offset offset = new Offset()
            {
                Dx = original.Dx,
                Dy = original.Dy,
                Dz = original.Dz
            };
            return offset;
        }

        public static void CopyPartData(Part copy, Part original)
        {
            copy.AssemblyNumber = cs_net_lib.Model.CopyNumberingSeries(original.AssemblyNumber);
            copy.PartNumber = cs_net_lib.Model.CopyNumberingSeries(original.PartNumber);
            copy.CastUnitType = original.CastUnitType;
            copy.Class = original.Class;
            copy.DeformingData = cs_net_lib.Model.CopyDeformingData(original.DeformingData);
            copy.Finish = original.Finish;
            copy.Material.MaterialString = original.Material.MaterialString;
            copy.Name = original.Name;
            copy.Position = cs_net_lib.Model.CopyPosition(original.Position);
            copy.Profile.ProfileString = original.Profile.ProfileString;
        }

        public static Position CopyPosition(Position original)
        {
            Position position = new Position()
            {
                Depth = original.Depth,
                DepthOffset = original.DepthOffset,
                Plane = original.Plane,
                PlaneOffset = original.PlaneOffset,
                Rotation = original.Rotation,
                RotationOffset = original.RotationOffset
            };
            return position;
        }

        public static BooleanPart CutPart(Part partToCut, Part cuttingPart, BooleanPart.BooleanTypeEnum type = (BooleanPart.BooleanTypeEnum)2)
        {
            BooleanPart booleanPart = null;
            if (partToCut == null || cuttingPart == null)
            {
                return booleanPart;
            }
            cuttingPart.Class = "BlOpCl";
            cuttingPart.Insert();
            if (cuttingPart == null)
            {
                return booleanPart;
            }
            booleanPart = new BooleanPart()
            {
                Father = partToCut
            };
            booleanPart.SetOperativePart(cuttingPart);
            booleanPart.Type = type;
            bool test1 = booleanPart.Insert();
            bool test2 = cuttingPart.Delete();
            return booleanPart;
        }

        public static BooleanPart CutPartByExistingPart(Part partToCut, Part cuttingPart, bool copyMode = false)
        {
            BooleanPart booleanPart = null;
            if (partToCut == null || cuttingPart == null)
            {
                return booleanPart;
            }
            if (copyMode)
            {
                Part newCuttingPart = Tekla.Structures.Model.Operations.Operation.CopyObject(cuttingPart, new Vector(0, 0, 0)) as Part;
                newCuttingPart.Class = "BlOpCl";
                newCuttingPart.Modify();
                newCuttingPart.Select();

                booleanPart = new BooleanPart()
                {
                    Father = partToCut
                };
                booleanPart.SetOperativePart(newCuttingPart);
                booleanPart.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
                booleanPart.Insert();
                newCuttingPart.Delete();

                return booleanPart;
            }
            else
            {
                string str = cuttingPart.Identifier.GUID.ToString();
                cuttingPart.Class = "BlOpCl";
                cuttingPart.Insert();
                booleanPart = new BooleanPart()
                {
                    Father = partToCut
                };
                booleanPart.SetOperativePart(cuttingPart);
                booleanPart.Type = BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
                booleanPart.Insert();
                cuttingPart.Delete();
                cuttingPart.Identifier = (new Tekla.Structures.Model.Model()).GetIdentifierByGUID(str);
                cuttingPart.Select();
                return booleanPart;
            }
        }

        public static Line GetBeamLine(Beam leftBlockLine)
        {
            return new Line(leftBlockLine.StartPoint, leftBlockLine.EndPoint);
        }

        private static double GetBendingRadius(RebarGeometry geometry, int index)
        {
            double item = 0;
            if (index > -1 && index < geometry.BendingRadiuses.Count)
            {
                item = (double)geometry.BendingRadiuses[index];
            }
            else if (geometry.BendingRadiuses.Count > 0)
            {
                item = (double)geometry.BendingRadiuses[0];
            }
            return item;
        }

        private static Vector GetLegthVector(RebarGeometry geometry, int index)
        {
            if (index < 0 || index >= geometry.Shape.Points.Count - 1)
            {
                return null;
            }
            return new Vector((geometry.Shape.Points[index + 1] as Point) - (geometry.Shape.Points[index] as Point));
        }

        public static double GetRebarLength(RebarGeometry geometry, bool roundUp = true)
        {
            double num = 0;
            if (geometry == null || geometry.Shape.Points.Count < 2)
            {
                return num;
            }
            bool flag = false;
            bool flag1 = false;
            double num1 = 0.1;
            bool flag2 = true;
            cs_net_lib.Model.GetSummaryReinforcementLengthOptions(ref flag, ref flag1, ref num1, ref flag2);
            if (!flag)
            {
                num = cs_net_lib.Model.CenterLineLength(geometry);
            }
            else if (flag1)
            {
                num = cs_net_lib.Model.SumOfSegLength(geometry);
            }
            if (roundUp)
            {
                num = (!flag2 ? num1 - num % num1 + num : num - num % num1);
            }
            return num;
        }

        public static List<double> GetRebarLengths(RebarGeometry geometry)
        {
            List<double> nums = new List<double>();
            if (geometry == null || geometry.Shape.Points.Count < 2)
            {
                return nums;
            }
            bool flag = false;
            bool flag1 = false;
            double num = 0.1;
            bool flag2 = true;
            cs_net_lib.Model.GetSummaryReinforcementLengthOptions(ref flag, ref flag1, ref num, ref flag2);
            if (!flag)
            {
                nums = cs_net_lib.Model.CenterLineLengths(geometry);
            }
            else if (flag1)
            {
                nums = cs_net_lib.Model.SumOfSegLengths(geometry);
            }
            for (int i = 0; i < nums.Count; i++)
            {
                nums[i] = Math.Round(nums[i], 0);
            }
            return nums;
        }

        private static void GetSummaryReinforcementLengthOptions(ref bool useUserDefinedRebarLengthAndWeight, ref bool useUserDefinedRebarShapeRules, ref double scheduleTotalLengthRoundingAccuracy, ref bool scheduleTotalLengthRoundingDirection)
        {
            string empty = string.Empty;
            Misc.GetTeklaAdvancedOption("XS_USE_USER_DEFINED_REBAR_LENGTH_AND_WEIGHT", ref empty);
            useUserDefinedRebarLengthAndWeight = empty.ToUpper() == "TRUE";
            empty = string.Empty;
            Misc.GetTeklaAdvancedOption("XS_USE_USER_DEFINED_REBARSHAPERULES", ref empty);
            useUserDefinedRebarShapeRules = empty.ToUpper() == "TRUE";
            string str = string.Empty;
            Misc.GetTeklaAdvancedOption("XS_SYSTEM".ToUpper(), ref str);
            char[] chrArray = new char[] { ';' };
            string[] strArrays = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str1 = Path.Combine(strArrays[i], "rebar_config.inp");
                if (File.Exists(str1))
                {
                    using (StreamReader streamReader = new StreamReader(str1))
                    {
                        for (string j = streamReader.ReadLine(); j != null; j = streamReader.ReadLine())
                        {
                            if (j.Contains("ScheduleTotalLengthRoundingAccuracy"))
                            {
                                try
                                {
                                    j = j.Trim();
                                    int num = j.IndexOf('=');
                                    if (num > 0)
                                    {
                                        j = j.Remove(0, num + 1);
                                    }
                                    num = j.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                                    if (num > 0)
                                    {
                                        j = j.Remove(num, j.Length - 1 - num);
                                    }
                                    scheduleTotalLengthRoundingAccuracy = double.Parse(j.Replace(',', '.'), CultureInfo.InvariantCulture);
                                }
                                catch (Exception exception)
                                {
                                    exception.ToString();
                                }
                            }
                            else if (j.Contains("ScheduleTotalLengthRoundingDirection"))
                            {
                                scheduleTotalLengthRoundingDirection = j.Contains("DOWN");
                            }
                        }
                    }
                }
            }
        }

        public static int PartCut(Part partToCut, Part cuttingPart)
        {
            if (cs_net_lib.Model.CutPart(partToCut, cuttingPart, BooleanPart.BooleanTypeEnum.BOOLEAN_CUT) != null)
            {
                return 1;
            }
            return 0;
        }

        public static int PartCutByExistingPart(Part partToCut, Part cuttingPart)
        {
            if (cs_net_lib.Model.CutPartByExistingPart(partToCut, cuttingPart) != null)
            {
                return 1;
            }
            return 0;
        }

        private static double RadiusDelta(Vector vector1, Vector vector2, double radius)
        {
            Vector vector = vector1.Cross(vector2);
            vector.Normalize();
            Vector vector3 = vector.Cross(vector1);
            Vector vector4 = vector.Cross(vector2);
            Vector vector5 = new Vector(vector1 + vector2);
            Vector vector6 = new Vector(vector3 - vector4);
            return radius * vector6.Dot(vector5) / vector5.Dot(vector5);
        }

        private static double SumOfSegLength(RebarGeometry geometry)
        {
            double bendingRadius = 0;
            for (int i = 0; i < geometry.Shape.Points.Count - 1; i++)
            {
                Vector legthVector = cs_net_lib.Model.GetLegthVector(geometry, i);
                if (legthVector != null)
                {
                    bendingRadius = bendingRadius + Math.Sqrt(legthVector.Dot(legthVector));
                    legthVector.Normalize();
                    Vector vector = cs_net_lib.Model.GetLegthVector(geometry, i - 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        if (legthVector.Dot(vector) >= 0)
                        {
                            bendingRadius = bendingRadius + cs_net_lib.Model.RadiusDelta(vector, legthVector, geometry.Diameter / 2);
                        }
                        else
                        {
                            bendingRadius = bendingRadius + (cs_net_lib.Model.GetBendingRadius(geometry, i - 1) + geometry.Diameter);
                            bendingRadius = bendingRadius - cs_net_lib.Model.RadiusDelta(vector, legthVector, cs_net_lib.Model.GetBendingRadius(geometry, i - 1) + geometry.Diameter / 2);
                        }
                    }
                    vector = cs_net_lib.Model.GetLegthVector(geometry, i + 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        if (legthVector.Dot(vector) >= 0)
                        {
                            bendingRadius = bendingRadius + cs_net_lib.Model.RadiusDelta(legthVector, vector, geometry.Diameter / 2);
                        }
                        else
                        {
                            bendingRadius = bendingRadius + (cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter);
                            bendingRadius = bendingRadius - cs_net_lib.Model.RadiusDelta(vector, legthVector, cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter / 2);
                        }
                    }
                }
            }
            return bendingRadius;
        }

        private static List<double> SumOfSegLengths(RebarGeometry geometry)
        {
            List<double> nums = new List<double>();
            for (int i = 0; i < geometry.Shape.Points.Count - 1; i++)
            {
                Vector legthVector = cs_net_lib.Model.GetLegthVector(geometry, i);
                if (legthVector != null)
                {
                    nums.Add(Math.Sqrt(legthVector.Dot(legthVector)));
                    legthVector.Normalize();
                    Vector vector = cs_net_lib.Model.GetLegthVector(geometry, i - 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        if (legthVector.Dot(vector) >= 0)
                        {
                            List<double> item = nums;
                            List<double> nums1 = item;
                            int count = nums.Count - 1;
                            int num = count;
                            item[count] = nums1[num] + cs_net_lib.Model.RadiusDelta(vector, legthVector, geometry.Diameter / 2);
                        }
                        else
                        {
                            List<double> item1 = nums;
                            List<double> nums2 = item1;
                            int count1 = nums.Count - 1;
                            int num1 = count1;
                            item1[count1] = nums2[num1] + (cs_net_lib.Model.GetBendingRadius(geometry, i - 1) + geometry.Diameter);
                            List<double> item2 = nums;
                            List<double> nums3 = item2;
                            int count2 = nums.Count - 1;
                            int num2 = count2;
                            item2[count2] = nums3[num2] - cs_net_lib.Model.RadiusDelta(vector, legthVector, cs_net_lib.Model.GetBendingRadius(geometry, i - 1) + geometry.Diameter / 2);
                        }
                    }
                    vector = cs_net_lib.Model.GetLegthVector(geometry, i + 1);
                    if (vector != null)
                    {
                        vector.Normalize();
                        if (legthVector.Dot(vector) >= 0)
                        {
                            List<double> item3 = nums;
                            List<double> nums4 = item3;
                            int count3 = nums.Count - 1;
                            int num3 = count3;
                            item3[count3] = nums4[num3] + cs_net_lib.Model.RadiusDelta(legthVector, vector, geometry.Diameter / 2);
                        }
                        else
                        {
                            List<double> item4 = nums;
                            List<double> nums5 = item4;
                            int count4 = nums.Count - 1;
                            int num4 = count4;
                            item4[count4] = nums5[num4] + (cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter);
                            List<double> item5 = nums;
                            List<double> nums6 = item5;
                            int count5 = nums.Count - 1;
                            int num5 = count5;
                            item5[count5] = nums6[num5] - cs_net_lib.Model.RadiusDelta(vector, legthVector, cs_net_lib.Model.GetBendingRadius(geometry, i) + geometry.Diameter / 2);
                        }
                    }
                }
            }
            return nums;
        }
    }
}