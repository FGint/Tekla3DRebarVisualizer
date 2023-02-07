using System;
using System.Windows.Input;
using Fusion;

namespace Tekla3DRebarVisualizer
{

    public class ExampleApp : App
    {
        private MainWindowViewModel _mainVM;

        [STAThread]
        public static void Main()
        {
            App.Start(new ExampleApp());
        }

        [PublishedView("App.MainWindow")]
        public ViewModel CreateMainWindow(object parameter)
        {
            _mainVM = new MainWindowViewModel();
            return _mainVM;
        }
    }
}
