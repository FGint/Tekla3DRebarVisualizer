using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using Tekla.Structures;
using Tekla.Structures.Model;

namespace cs_net_lib
{
	public static class Misc
	{
		public static void GetAllReportProperties(ModelObject modelObject, ArrayList reportPropertyNames, ref Hashtable reportProperties)
		{
			modelObject.GetAllReportProperties(reportPropertyNames, reportPropertyNames, reportPropertyNames, ref reportProperties);
		}

		public static void GetTeklaAdvancedOption(string variable, ref string variableValue)
		{
			TeklaStructuresSettings.GetAdvancedOption(variable, ref variableValue);
		}

		public static int SetCultureInfo(Constants.Culture culture)
		{
			string str = "en-US";
			if (culture == Constants.Culture.CZECH)
			{
				str = "cs-CZ";
			}
			CultureInfo cultureInfo = new CultureInfo(str, false);
			Thread.CurrentThread.CurrentCulture = cultureInfo;
			return 1;
		}
	}
}