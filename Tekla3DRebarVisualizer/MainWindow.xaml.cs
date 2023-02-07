using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TSM = Tekla.Structures.Model;
using ExtLibrary;

namespace Tekla3DRebarVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TSM.Model model = new TSM.Model();
            if (!model.GetConnectionStatus())
                PrepareDemoMode();
        }

        public void PrepareDemoMode()
        {
            this.GetRebar.IsEnabled = false;
            this.InsertRebar.IsEnabled = false;
            this.DemoData.Visibility = Visibility.Visible;
            MessageBox.Show("Tekla Structures is not opened, program will be opened in demo mode");
        }
    }
}
