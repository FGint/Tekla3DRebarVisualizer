using System;

namespace cs_net_lib
{
	public static class Constants
	{
		public const string MATERIAL_STEEL = "STEEL";

		public const string MATERIAL_CONCRETE = "CONCRETE";

		public const string MATERIAL_MISCELANEOUS = "MISCELLANEOUS";

		public const string MATERIAL_TIMBER = "TIMBER";

		public const double DRAW_PLANE_POINTS_DISTANCE = 1500;

		public const string PLACE_OF_POINT = "...";

		public const string ENGLISH_CULTURE = "en-US";

		public const string CZECH_CULTURE = "cs-CZ";

		public const string INP_ABOUT_DIALOG = "         {\n              attribute (_, \"16.12.2010\", label, , , none, , , 258, 20)     \n              attribute (_, j_date,     label, , , none, , , 208, 20)     \n              attribute (_, TS 18.0, label, , , none, , , 138, 20)        \n              attribute (_, j_version,  label, , , none, , ,  63, 20)     \n              \n              picture(\"cs_net_j001_construsoft_logo\", 350, 320, 5, 50)  \n         }\n";

		public readonly static double DEF;

		public readonly static double CS_EPSILON;

		public readonly static string DEF_STR;

		public readonly static int CLASS_CONCRETE;

		public readonly static double PI;

		public readonly static double RAD_TO_DEG;

		public readonly static double DEG_TO_RAD;

		static Constants()
		{
			Constants.DEF = -2147483648;
			Constants.CS_EPSILON = 0.01;
			Constants.DEF_STR = "";
			Constants.CLASS_CONCRETE = 6;
			Constants.PI = 3.14159265358979;
			Constants.RAD_TO_DEG = 180 / Constants.PI;
			Constants.DEG_TO_RAD = 1 / Constants.RAD_TO_DEG;
		}

		public enum ConnectionType
		{
			CREATE_NO_CONNECTION,
			WELD,
			CAST_UNIT,
			SUBASSEMBLY
		}

		public enum Culture
		{
			ENGLISH,
			CZECH
		}

		public enum Rotation
		{
			FRONT,
			TOP,
			BACK,
			BELOW
		}
	}
}