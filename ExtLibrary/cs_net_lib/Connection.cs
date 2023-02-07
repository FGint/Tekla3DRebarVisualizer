using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Model;

namespace cs_net_lib
{
	public class Connection
	{
		public Connection()
		{
		}

		private static void AddPartToList(Part partToAdd, List<Part> parts, List<Part> partsToBeChecked)
		{
			bool flag = false;
			int num = 0;
			while (num < parts.Count)
			{
				if (parts[num].Identifier.ID != partToAdd.Identifier.ID)
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
				parts.Add(partToAdd);
				partsToBeChecked.Add(partToAdd);
			}
		}

		public static bool ConnectPartToPart(Part mainPart, ModelObject secondaryPart, Constants.ConnectionType connectionType, Weld weldData)
		{
			if (secondaryPart == null || mainPart == null)
			{
				return false;
			}
			cs_net_lib.Connection.RemoveOldConnection(mainPart, secondaryPart);
			if (connectionType == Constants.ConnectionType.CAST_UNIT)
			{
				Assembly assembly = mainPart.GetAssembly();
				if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Component"))
				{
					assembly.Add(secondaryPart as Component);
				}
				else if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.TSM.Part"))
				{
					assembly.Add(secondaryPart as Part);
				}
				else if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.PolyBeam"))
				{
					assembly.Add(secondaryPart as Part);
				}
				else if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.ContourPlate"))
				{
					assembly.Add(secondaryPart as Part);
				}
				else if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Beam"))
				{
					assembly.Add(secondaryPart as Part);
				}
				return assembly.Modify();
			}
			if (connectionType != Constants.ConnectionType.SUBASSEMBLY)
			{
				if (connectionType != Constants.ConnectionType.WELD)
				{
					return true;
				}
				return cs_net_lib.Connection.CreateWeld(mainPart, secondaryPart, weldData);
			}
			Assembly assembly1 = mainPart.GetAssembly();
			if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.TSM.Part") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.PolyBeam") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.ContourPlate") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Beam"))
			{
				assembly1.Add((secondaryPart as Part).GetAssembly());
				return assembly1.Modify();
			}
			if (!secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Component"))
			{
				return false;
			}
			ModelObjectEnumerator children = secondaryPart.GetChildren();
			while (children.MoveNext())
			{
				Part current = children.Current as Part;
				if (current == null)
				{
					continue;
				}
				assembly1.Add(current.GetAssembly());
			}
			return assembly1.Modify();
		}

		public static bool CreateWeld(ModelObject mainPart, ModelObject secondaryPart, Weld weld)
		{
			bool flag = false;
			try
			{
				if (weld != null)
				{
					weld.MainObject = mainPart;
					weld.SecondaryObject = secondaryPart;
					flag = weld.Insert();
				}
			}
			catch
			{
			}
			return flag;
		}

		private static void GetAssemblyParts(Assembly assembly, List<Part> subParts, List<Part> partsToBeChecked)
		{
			ArrayList secondaries = assembly.GetSecondaries();
			secondaries.Add(assembly.GetMainPart());
			for (int i = 0; i < secondaries.Count; i++)
			{
				if (secondaries[i] is Part)
				{
					cs_net_lib.Connection.AddPartToList(secondaries[i] as Part, subParts, partsToBeChecked);
				}
				ArrayList subAssemblies = assembly.GetSubAssemblies();
				for (int j = 0; j < subAssemblies.Count; j++)
				{
					if (subAssemblies[j] is Assembly)
					{
						cs_net_lib.Connection.GetAssemblyParts(subAssemblies[j] as Assembly, subParts, partsToBeChecked);
					}
				}
			}
		}

		private static void GetBoltedParts(Part part, List<Part> boltedParts, List<Part> partsToBeChecked)
		{
			ModelObjectEnumerator bolts = part.GetBolts();
			while (bolts.MoveNext())
			{
				BoltGroup current = bolts.Current as BoltGroup;
				if (current == null)
				{
					continue;
				}
				Part partToBoltTo = current.PartToBoltTo;
				if (partToBoltTo != null)
				{
					cs_net_lib.Connection.AddPartToList(partToBoltTo, boltedParts, partsToBeChecked);
				}
				Part partToBeBolted = current.PartToBeBolted;
				if (partToBeBolted != null)
				{
					cs_net_lib.Connection.AddPartToList(partToBeBolted, boltedParts, partsToBeChecked);
				}
				ArrayList otherPartsToBolt = current.GetOtherPartsToBolt();
				for (int i = 0; i < otherPartsToBolt.Count; i++)
				{
					Part item = otherPartsToBolt[i] as Part;
					if (item != null)
					{
						cs_net_lib.Connection.AddPartToList(item, boltedParts, partsToBeChecked);
					}
				}
			}
		}

		public static List<Part> GetConnectedParts(Beam mainPart)
		{
			List<Part> parts = new List<Part>();
			List<Part> parts1 = new List<Part>()
			{
				mainPart
			};
			while (parts1.Count != 0)
			{
				Part item = parts1[parts1.Count - 1];
				parts1.RemoveAt(parts1.Count - 1);
				cs_net_lib.Connection.GetAssemblyParts(item.GetAssembly(), parts, parts1);
				cs_net_lib.Connection.GetWeldedParts(item, parts, parts1);
				cs_net_lib.Connection.GetBoltedParts(item, parts, parts1);
				cs_net_lib.Connection.GetOtherParts(item, parts, parts1);
			}
			return parts;
		}

		private static void GetOtherParts(Part part, List<Part> otherParts, List<Part> partsToBeChecked)
		{
			ModelObjectEnumerator children = part.GetChildren();
			while (children.MoveNext())
			{
				Part current = children.Current as Part;
				if (current == null)
				{
					continue;
				}
				cs_net_lib.Connection.AddPartToList(current, otherParts, partsToBeChecked);
			}
		}

		private static void GetWeldedParts(Part part, List<Part> weldedParts, List<Part> partsToBeChecked)
		{
			ModelObjectEnumerator welds = part.GetWelds();
			while (welds.MoveNext())
			{
				Weld current = welds.Current as Weld;
				if (current == null)
				{
					continue;
				}
				Part mainObject = current.MainObject as Part;
				if (mainObject != null)
				{
					cs_net_lib.Connection.AddPartToList(mainObject, weldedParts, partsToBeChecked);
				}
				Part secondaryObject = current.SecondaryObject as Part;
				if (secondaryObject == null)
				{
					continue;
				}
				cs_net_lib.Connection.AddPartToList(secondaryObject, weldedParts, partsToBeChecked);
			}
		}

		public static List<Weld> GetWeldList(ref List<cs_net_lib.Connection.weldStructureData> weldStructList)
		{
			List<Weld> welds = new List<Weld>();
			foreach (cs_net_lib.Connection.weldStructureData weldStructureDatum in weldStructList)
			{
				cs_net_lib.Connection.SetDefaultValues(weldStructureDatum);
				Weld weld = new Weld()
				{
					AngleAbove = weldStructureDatum.angle,
					AngleBelow = weldStructureDatum.angle2,
					AroundWeld = weldStructureDatum.around == 0,
					ContourAbove = (BaseWeld.WeldContourEnum)weldStructureDatum.ftype,
					ContourBelow = (BaseWeld.WeldContourEnum)((int)weldStructureDatum.ftype2),
					FinishAbove = (BaseWeld.WeldFinishEnum)weldStructureDatum.finish,
					FinishBelow = (BaseWeld.WeldFinishEnum)weldStructureDatum.finish2,
					LengthAbove = weldStructureDatum.length,
					LengthBelow = weldStructureDatum.length2,
					PitchAbove = weldStructureDatum.cc,
					PitchBelow = weldStructureDatum.cc2,
					ReferenceText = weldStructureDatum.text,
					ShopWeld = weldStructureDatum.wtype == 1,
					SizeAbove = weldStructureDatum.size,
					SizeBelow = weldStructureDatum.size2,
					TypeAbove = (BaseWeld.WeldTypeEnum)weldStructureDatum.type,
					TypeBelow = (BaseWeld.WeldTypeEnum)weldStructureDatum.type2,
					EffectiveThroatAbove = weldStructureDatum.effective_throat_above,
					EffectiveThroatBelow = weldStructureDatum.effective_throat_below,
					RootOpeningAbove = weldStructureDatum.root_opening_above,
					RootOpeningBelow = weldStructureDatum.root_opening_below,
					RootFaceAbove = weldStructureDatum.root_face_thick_above,
					RootFaceBelow = weldStructureDatum.root_face_thick_below
				};
				welds.Add(weld);
			}
			return welds;
		}

		private static void RemoveOldConnection(Part mainPart, ModelObject secondaryPart)
		{
			Assembly assembly = mainPart.GetAssembly();
			if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Component"))
			{
				ArrayList arrayLists = new ArrayList();
				arrayLists = assembly.GetSubAssemblies();
				ModelObjectEnumerator children = secondaryPart.GetChildren();
				children.MoveNext();
				Assembly assembly1 = (children.Current as Part).GetAssembly();
				for (int i = 0; i < arrayLists.Count; i++)
				{
					if ((arrayLists[i] as Assembly).Identifier.ID == assembly1.Identifier.ID)
					{
						assembly.Remove(assembly1);
					}
				}
			}
			else if (secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.TSM.Part") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.PolyBeam") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.ContourPlate") || secondaryPart.GetType().FullName.Equals("Tekla.Structures.Model.Beam"))
			{
				ArrayList secondaries = new ArrayList();
				secondaries = assembly.GetSecondaries();
				for (int j = 0; j < secondaries.Count; j++)
				{
					if ((secondaries[j] as Part).Identifier.ID == secondaryPart.Identifier.ID)
					{
						assembly.Remove(secondaryPart);
					}
				}
			}
			assembly.Modify();
		}

		private static void SetDefaultValues(cs_net_lib.Connection.weldStructureData data)
		{
			Input.Def(ref data.angle, 0);
			Input.Def(ref data.angle2, 0);
			Input.Def(ref data.around, 0);
			Input.Def(ref data.cc, 0);
			Input.Def(ref data.cc2, 0);
			Input.Def(ref data.finish, 0);
			Input.Def(ref data.finish2, 0);
			Input.Def(ref data.ftype, 0);
			Input.Def(ref data.ftype2, 0);
			Input.Def(ref data.length, 0);
			Input.Def(ref data.length2, 0);
			Input.Def(ref data.size, 0);
			Input.Def(ref data.size2, 0);
			Input.Def(ref data.text, "");
			Input.Def(ref data.type, 10);
			Input.Def(ref data.type2, 0);
			Input.Def(ref data.wtype, 1);
			Input.Def(ref data.effective_throat_above, 0);
			Input.Def(ref data.effective_throat_below, 0);
			Input.Def(ref data.root_opening_above, 0);
			Input.Def(ref data.root_opening_below, 0);
			Input.Def(ref data.root_face_thick_above, 0);
			Input.Def(ref data.root_face_thick_below, 0);
		}

		public static Weld SetWeldingAttributes(double angleAbove, double angleBelow, bool aroundWeld, BaseWeld.WeldContourEnum contourAbove, BaseWeld.WeldContourEnum contourBelow, double effectiveThroatAbove, double effectiveThroatBelow, BaseWeld.WeldFinishEnum finnishAbove, BaseWeld.WeldFinishEnum finnishBelow, double lengthAbove, double lengthBelow, double pitchAbove, double pitchBelow, string referenceText, double rootFaceAbove, double rootFaceBelow, double rootOpeningAbove, double rootOpeningBelow, bool shopWeld, double sizeAbove, double sizeBelow, BaseWeld.WeldTypeEnum typeAbove, BaseWeld.WeldTypeEnum typeBelow)
		{
			Weld weld = new Weld()
			{
				AngleAbove = angleAbove,
				AngleBelow = angleBelow,
				AroundWeld = aroundWeld,
				ContourAbove = contourAbove,
				ContourBelow = contourBelow,
				FinishAbove = finnishAbove,
				FinishBelow = finnishBelow,
				LengthAbove = lengthAbove,
				LengthBelow = lengthBelow,
				PitchAbove = pitchAbove,
				PitchBelow = pitchBelow,
				ReferenceText = referenceText,
				ShopWeld = shopWeld,
				SizeAbove = sizeAbove,
				SizeBelow = sizeBelow,
				TypeAbove = typeAbove,
				TypeBelow = typeBelow,
				EffectiveThroatAbove = effectiveThroatAbove,
				EffectiveThroatBelow = effectiveThroatBelow,
				RootFaceAbove = rootFaceAbove,
				RootFaceBelow = rootFaceBelow,
				RootOpeningAbove = rootOpeningAbove,
				RootOpeningBelow = rootOpeningBelow
			};
			return weld;
		}

		public struct weldStructureData
		{
			public double cc2;

			public double length2;

			public int finish2;

			public double ftype2;

			public double angle2;

			public int type2;

			public double size2;

			public string text;

			public int wtype;

			public int around;

			public double cc;

			public double length;

			public int finish;

			public int ftype;

			public double angle;

			public int type;

			public double size;

			public double effective_throat_above;

			public double effective_throat_below;

			public double root_opening_above;

			public double root_opening_below;

			public double root_face_thick_above;

			public double root_face_thick_below;
		}
	}
}