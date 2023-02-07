using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD = Tekla.Structures.Drawing;
using TSDUI = Tekla.Structures.Drawing.UI;

namespace ExtLibrary
{
    public static class TeklaExtensions
    {
        public static string ShowSuccess(this Fusion.ViewModel viewModel, string message)
        {
            return viewModel.Host.UI.ShowMessageDialog(message, "Success!", "Geometry.Information");
        }

        public static string ShowWarning(this Fusion.ViewModel viewModel, string message)
        {
            return viewModel.Host.UI.ShowMessageDialog(message, "Warning!", "Geometry.Error");
        }
    }
}
