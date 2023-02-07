using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace cs_net_lib
{
	public class Input
	{
		public Input()
		{
		}

		public static void Def(ref string inputValue, string defaultValue)
		{
			if (inputValue == Constants.DEF_STR)
			{
				inputValue = defaultValue;
			}
		}

		public static void Def(ref int inputValue, int defaultValue)
		{
			if ((double)inputValue == Constants.DEF)
			{
				inputValue = defaultValue;
			}
		}

		public static void Def(ref double inputValue, double defaultValue)
		{
			if (inputValue == Constants.DEF)
			{
				inputValue = defaultValue;
			}
		}

		public static int ParseDistanceList(string list, ref List<double> double_distance_list, CultureInfo culture_format)
		{
			if (list.Length == 0)
			{
				return 1;
			}
			double num = 0;
			double num1 = 0;
			int num2 = 0;
			string[] strArrays = list.Split(new char[] { ' ' });
			double_distance_list.Add(0);
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				char[] chrArray = new char[] { '*', 'x', 'X' };
				if (strArrays[i].IndexOfAny(chrArray) != -1)
				{
					string[] strArrays1 = strArrays[i].Split(chrArray);
					try
					{
						num2 = int.Parse(strArrays1[0], culture_format);
						num = (double)float.Parse(strArrays1[1], culture_format);
					}
					catch (Exception exception)
					{
						num2 = 0;
					}
				}
				else
				{
					try
					{
						num = (double)float.Parse(strArrays[i], culture_format);
						num2 = 1;
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						num2 = 0;
						exception1.ToString();
					}
				}
				for (int j = 0; j < num2; j++)
				{
					num1 = num1 + num;
					double_distance_list.Add(num1);
				}
			}
			return double_distance_list.Count;
		}

		public static int ParseDistanceList(string list, ref List<double> double_distance_list)
		{
			return Input.ParseDistanceList(list, ref double_distance_list, new CultureInfo("en-US"));
		}

		public static int ParseFloatList(string list, ref List<double> double_list, CultureInfo culture_format)
		{
			if (list.Length == 0)
			{
				return 1;
			}
			double num = 0;
			int num1 = 0;
			string[] strArrays = list.Split(new char[] { ' ' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				char[] chrArray = new char[] { '*', 'x', 'X' };
				if (strArrays[i].IndexOfAny(chrArray) != -1)
				{
					string[] strArrays1 = strArrays[i].Split(chrArray);
					try
					{
						num1 = int.Parse(strArrays1[0], culture_format);
						num = (double)float.Parse(strArrays1[1], culture_format);
					}
					catch (Exception exception)
					{
						num1 = 0;
					}
				}
				else
				{
					try
					{
						num = (double)float.Parse(strArrays[i], culture_format);
						num1 = 1;
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						num1 = 0;
						exception1.ToString();
					}
				}
				for (int j = 0; j < num1; j++)
				{
					double_list.Add(num);
				}
			}
			return double_list.Count;
		}

		public static int ParseFloatList(string list, ref List<double> double_list)
		{
			return Input.ParseFloatList(list, ref double_list, new CultureInfo("en-US"));
		}

		public static void ParseIntList(string classList, ref List<int> listOfClasses)
		{
			classList = Input.RemoveMoreSpaces(ref classList);
			bool flag = false;
			if (classList.Length != 0)
			{
				string[] strArrays = classList.Split(new char[] { ' ' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					try
					{
						listOfClasses.Add(int.Parse(strArrays[i]));
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						flag = true;
						exception.ToString();
					}
				}
				if (flag)
				{
					MessageBox.Show("Class list is in an incorrect format.");
				}
			}
		}

		public static void ParseStringList(string classList, ref List<string> listOfClasses)
		{
			Input.RemoveMoreSpaces(ref classList);
			string[] strArrays = classList.Split(new char[0]);
			for (int i = 0; i != (int)strArrays.Length; i++)
			{
				if (strArrays[i] != "")
				{
					listOfClasses.Add(strArrays[i]);
				}
			}
		}

		private static string RemoveMoreSpaces(ref string str)
		{
			int num = 0;
			str = str.Trim();
			while (num < str.Length - 1)
			{
				if (str.Substring(num, 2) != "  ")
				{
					num++;
				}
				else
				{
					str = str.Remove(num, 1);
				}
			}
			return str;
		}
	}
}