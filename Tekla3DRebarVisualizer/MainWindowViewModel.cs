using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs_net_lib;
using ExtLibrary;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using SWS = System.Windows.Shapes;
using TSM = Tekla.Structures.Model;

namespace Tekla3DRebarVisualizer
{
    public class MainWindowViewModel : Fusion.WindowViewModel
    {

        private SingleRebar _demorebar = new SingleRebar();
        public SingleRebar DemoRebar
        {
            get { return _demorebar; }
            set { this.SetValue(ref this._demorebar, value); }
        }

        private bool _demomode = false;
        public bool DemoMode
        {
            get { return _demomode; }
            set { this.SetValue(ref this._demomode, value); }
        }

        private ObservableCollection<SWS.Shape> _rebarCanvasLines;
        public ObservableCollection<SWS.Shape> RebarCanvasLines
        {
            get { return _rebarCanvasLines; }
            set { this.SetValue(ref this._rebarCanvasLines, value); }
        }

        private int _matrixRotateByX = Properties.Settings.Default.MatrixRotateByX;
        public int MatrixRotateByX
        {
            get { return _matrixRotateByX; }
            set
            {
                this.SetValue(ref this._matrixRotateByX, value);
                this.SetValue(ref this._matrixRotateByXLabel, value.ToString(), "MatrixRotateByXLabel");
                CreatePulloutPicture.Draw(RebarCanvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, false, DemoMode, DemoRebar);
                Properties.Settings.Default.MatrixRotateByX = value;
            }
        }

        private int _matrixRotateByY = Properties.Settings.Default.MatrixRotateByY;
        public int MatrixRotateByY
        {
            get { return _matrixRotateByY; }
            set
            {
                this.SetValue(ref this._matrixRotateByY, value);
                this.SetValue(ref this._matrixRotateByYLabel, value.ToString(), "MatrixRotateByYLabel");
                CreatePulloutPicture.Draw(RebarCanvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, false, DemoMode, DemoRebar);
                Properties.Settings.Default.MatrixRotateByY = value;
            }
        }

        private int _matrixRotateByZ = Properties.Settings.Default.MatrixRotateByZ;
        public int MatrixRotateByZ
        {
            get { return _matrixRotateByZ; }
            set
            {
                this.SetValue(ref this._matrixRotateByZ, value);
                this.SetValue(ref this._matrixRotateByZLabel, value.ToString(), "MatrixRotateByZLabel");
                CreatePulloutPicture.Draw(RebarCanvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, false, DemoMode, DemoRebar);
                Properties.Settings.Default.MatrixRotateByZ = value;
            }
        }

        private string _matrixRotateByXLabel = Properties.Settings.Default.MatrixRotateByX.ToString();
        public string MatrixRotateByXLabel
        {
            get { return _matrixRotateByXLabel; }
            set { this.SetValue(ref this._matrixRotateByXLabel, value); }
        }

        private string _matrixRotateByYLabel = Properties.Settings.Default.MatrixRotateByY.ToString();
        public string MatrixRotateByYLabel
        {
            get { return _matrixRotateByYLabel; }
            set { this.SetValue(ref this._matrixRotateByYLabel, value); }
        }

        private string _matrixRotateByZLabel = Properties.Settings.Default.MatrixRotateByZ.ToString();
        public string MatrixRotateByZLabel
        {
            get { return _matrixRotateByZLabel; }
            set { this.SetValue(ref this._matrixRotateByZLabel, value); }
        }

        private double _pulloutScale = Properties.Settings.Default.PulloutScale;
        public double PulloutScale
        {
            get { return _pulloutScale; }
            set { this.SetValue(ref this._pulloutScale, value); }
        }

        private string _pulloutLineProp = Properties.Settings.Default.PulloutLineProp;
        public string PulloutLineProp
        {
            get { return _pulloutLineProp; }
            set { this.SetValue(ref this._pulloutLineProp, value); }
        }

        private string _pulloutTextProp = Properties.Settings.Default.PulloutTextProp;
        public string PulloutTextProp
        {
            get { return _pulloutTextProp; }
            set { this.SetValue(ref this._pulloutTextProp, value); }
        }


        [Fusion.CommandHandler]
        public void GetRebarPulloutPicture()
        {
            string message = "";
            CreatePulloutPicture.GenerateBrushesList();
            RebarCanvasLines = new ObservableCollection<SWS.Shape>();
            message = CreatePulloutPicture.CreatePicture(MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, RebarCanvasLines);

            if (!string.IsNullOrEmpty(message))
            {
                this.ShowWarning(message);
            }
        }

        [Fusion.CommandHandler]
        public void InsertRebarPicture()
        {
            string message = "";
            message = CreatePulloutPicture.InsertIntoDrawing(MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, RebarCanvasLines, PulloutScale, PulloutLineProp, PulloutTextProp);

            if (!string.IsNullOrEmpty(message))
            {
                this.ShowWarning(message);
            }
        }

        [Fusion.CommandHandler]
        public void PrepareDemoData()
        {
            //Demo data
            DemoRebar = new SingleRebar();
            DemoMode = true;
            DemoRebar.Polygon = new Polygon
            {
                Points = new System.Collections.ArrayList() {
                new Point(8750, 12560, -500),
                new Point(8750, 12100, -500),
                new Point(8750, 12000, 0),
                new Point(9250, 12000, 0),
                new Point(9250, 12100, -500),
                new Point(9250, 12550, -500)}
            };
            //

            CreatePulloutPicture.GenerateBrushesList();
            RebarCanvasLines = new ObservableCollection<SWS.Shape>();
            CreatePulloutPicture.Draw(RebarCanvasLines, MatrixRotateByX, MatrixRotateByY, MatrixRotateByZ, false, true, DemoRebar);

            //Initial rotation
            MatrixRotateByX = -100;
            MatrixRotateByY = 0;
            MatrixRotateByZ = 50;
            //
        }
    }
}
