using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using TowerDefenceGame_LPB.ViewModel;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.DataAccess;
using TowerDefenceGame_LPB.View;

namespace TowerDefenceGame_LPB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _view;
        private MapMaker _mapMaker;
        private MainMenu _mainMenu;
        private GameViewModel _viewModel;
        private MapMakerViewModel _mapMakerViewModel;
        private GameModel _model;
        private DialogBox _dialogBox;
        public DelegateCommand OpenNewWindowCommand { get; set; }
        public DelegateCommand OKCommand { get; set; }
        public DelegateCommand DialogCloseCommand { get; set; }
        public DelegateCommand CloseGameCommand { get; set; }

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Creating model
            _model = new GameModel(new DataAccess.JsonDataAccess());
            //_model.NewGame();

            // Creating viewmodel
            _viewModel = new GameViewModel(_model);
            _viewModel.ExitCommand = new DelegateCommand(p => ExitFromGame());
            _viewModel.CloseGameCommand = new DelegateCommand(p => _view.Close());
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            //Creating main menu
            _mainMenu = new MainMenu();
            OpenNewWindowCommand= new DelegateCommand(p => OpenNewWindow(Convert.ToInt32(p)));
            _mainMenu.DataContext = this;
            _mainMenu.Show();


            
            // Creating view
            //_view = new MainWindow();
            //_view.DataContext = _viewModel;
            //_view.Show();

        }
        private void OpenNewWindow(int windowType)
        {
            if (windowType == 1)
            {
                _view = new MainWindow();
                _view.DataContext = _viewModel;
                _model.NewGame();
                _view.Show();
                _view.Closing += CloseGame;
            }
            else if (windowType == 2)
            {
                _mapMaker = new MapMaker();
                _mapMaker.DataContext = _mapMakerViewModel;
                _mapMaker.Show();
            }
            _mainMenu.Close();
            /*
            _dialogBox = new DialogBox();
            _dialogBox.DataContext = this;
            OKCommand = new DelegateCommand(p => SetupNewWindow(windowType));
            DialogCloseCommand = new DelegateCommand(p => _dialogBox.Close());
            _dialogBox.Show();
            
            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Show();
            _mainMenu.Close();
            */
        }
        private void SetupNewWindow(int windowType)
        {
            //use values from dialogBox for gridSize
            //can use _dialogBox.Rows.Text and _dialogBox.Columns.Text with converting
            if(windowType == 1)
            {
                _view = new MainWindow();
                _view.DataContext = _viewModel;
                _view.Show();
            }
            else if(windowType == 2)
            {
                _mapMaker = new MapMaker();
                _mapMaker.DataContext = _mapMakerViewModel;
                _mapMaker.Show();
            }
            _dialogBox.Close();
            _mainMenu.Close();
        }
        private void ExitFromGame()
        {
            _mainMenu = new MainMenu();
            _mainMenu.DataContext = this;
            _mainMenu.Show(); ;
            _view.Close();
        }

        private void CloseGame(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztos ki akarsz lépni a játékból?", "Tower Defence",MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    System.Windows.Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    e.Cancel = true;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }

        private async void ViewModel_SaveGame(object sender, EventArgs e)
        {
            //try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Játék mentése";
                saveFileDialog.Filter = "Json objektum|*.json|Összes fájl|*.*";
                saveFileDialog.FileName = "TowerDefenceMentés";
                if (saveFileDialog.ShowDialog() == true)
                {
                    //try
                    {
                        // játéktábla mentése
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                   // catch (Exception)
                    {
                       // MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            //catch
            {
              //  MessageBox.Show("A fájl mentése sikertelen!", "Tower Defence", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
