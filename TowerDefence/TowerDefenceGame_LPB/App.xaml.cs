using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using TowerDefenceGame_LPB.ViewModel;

namespace TowerDefenceGame_LPB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _view;
        private TestViewModel _viewModel;
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            _viewModel = new TestViewModel();
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();
        }
    }
}
