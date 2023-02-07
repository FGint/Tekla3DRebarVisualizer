using System;

namespace cs_net_lib
{
	public static class Compare
	{
		public static bool EQ(double value1, double value2)
		{
			if (!Compare.GE(value1, value2))
			{
				return false;
			}
			return Compare.LE(value1, value2);
		}

		public static bool GE(double value1, double value2)
		{
			return !Compare.LT(value1, value2);
		}

		public static bool GT(double value1, double value2)
		{
			return value1 - value2 > Constants.CS_EPSILON;
		}

		public static bool IE(double value1, double value2, double value3)
		{
			if (!Compare.GE(value1, value2))
			{
				return false;
			}
			return Compare.LE(value1, value3);
		}

		public static bool IN(double value1, double value2, double value3)
		{
			if (!Compare.GE(value1, value2))
			{
				return false;
			}
			return Compare.LT(value1, value3);
		}

		public static bool IX(double value1, double value2, double value3)
		{
			if (!Compare.GT(value1, value2))
			{
				return false;
			}
			return Compare.LE(value1, value3);
		}

		public static bool LE(double value1, double value2)
		{
			return !Compare.GT(value1, value2);
		}

		public static bool LT(double value1, double value2)
		{
			return Compare.GT(value2, value1);
		}

		public static bool NE(double value1, double value2)
		{
			return !Compare.EQ(value1, value2);
		}

		public static bool NZ(double value)
		{
			return !Compare.ZR(value);
		}

		public static void Swap<TYPE>(ref TYPE objectX, ref TYPE objectY)
		{
			TYPE tYPE = objectX;
			objectX = objectY;
			objectY = tYPE;
		}

		public static bool ZR(double value)
		{
			return Compare.EQ(value, 0);
		}
	}
}