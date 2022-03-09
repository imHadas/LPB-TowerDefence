using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using TowerDefenceGame_LPB.ViewModel;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.DataAccess;

namespace TowerDefenceGame_LPB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _view;
        private TestViewModel _viewModel;
        private GameModel _model;


        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Creating model
            _model = new GameModel(new DataAccess.DataAccess());
            _model.NewGame();

            // Creating viewmodel
            _viewModel = new TestViewModel(_model);

            // Creating view
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();
            
        }
    }
}
