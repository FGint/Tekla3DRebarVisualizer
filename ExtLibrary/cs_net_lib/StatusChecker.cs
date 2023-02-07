using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tekla.Structures;

namespace cs_net_lib
{
	public static class StatusChecker
	{
		private static StatusChecker.AuthorizationStatus AuthStatus;

		static StatusChecker()
		{
		}

		public static void IsConcreteDetailingEnabled()
		{
			if (StatusChecker.AuthStatus == StatusChecker.AuthorizationStatus.Check)
			{
				StatusChecker.AuthStatus = (ModuleManager.ConcreteDetailing ? StatusChecker.AuthorizationStatus.Authorized : StatusChecker.AuthorizationStatus.NotAuthorized);
			}
			if (StatusChecker.AuthStatus == StatusChecker.AuthorizationStatus.NotAuthorized)
			{
				WarningDialog warningDialog = new WarningDialog("albl_Cast_unit_type", "albl_change_cast_unit_type_warning");
				if (warningDialog.ShowDialog() == DialogResult.OK && warningDialog.DoNotShow)
				{
					StatusChecker.AuthStatus = StatusChecker.AuthorizationStatus.NotAuthorizedOk;
				}
				warningDialog.Dispose();
			}
		}

		public enum AuthorizationStatus
		{
			Check,
			Authorized,
			NotAuthorized,
			NotAuthorizedOk
		}
	}
}