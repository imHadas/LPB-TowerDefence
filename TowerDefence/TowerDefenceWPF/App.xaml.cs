using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using TowerDefenceBackend.ViewModel;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.DataAccess;
using TowerDefenceWPF.View;
using System.Threading.Tasks;
using System.IO;

namespace TowerDefenceWPF
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
        private MapMakerModel _mapMakerModel;

        private JsonDataAccess dataAccess = new();

        public DelegateCommand OpenNewWindowCommand { get; set; }

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Creating model
            _model = new GameModel(dataAccess);
            _mapMakerModel = new MapMakerModel(dataAccess);
            _model.GameOver += _model_GameOver;

            // Creating viewmodel
            _viewModel = new GameViewModel(_model);
            _viewModel.ExitCommand = new DelegateCommand(p => ExitFromGame());
            _viewModel.CloseGameCommand = new DelegateCommand(p => _view.Close());
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.LoadGame += ViewModel_LoadGame;
            _viewModel.LoadMap += ViewModel_LoadMap;

            _mapMakerViewModel = new MapMakerViewModel(_mapMakerModel);
            _mapMakerViewModel.CloseMapMakerCommand = new DelegateCommand(p => _mapMaker.Close());
            _mapMakerViewModel.ExitCommand = new DelegateCommand(p => ExitFromMapMaker());
            _mapMakerViewModel.SaveGame += _mapMakerViewModel_SaveGame;
            _mapMakerViewModel.LoadGame += _mapMakerViewModel_LoadGame;
            _mapMakerViewModel.MessageSender += _mapMakerViewModel_MessageSender;

            //Creating main menu
            _mainMenu = new MainMenu();
            OpenNewWindowCommand= new DelegateCommand(p => OpenNewWindow(Convert.ToInt32(p)));
            _mainMenu.DataContext = this;
            _mainMenu.Closing += CloseWindow;
            _mainMenu.Show();            

        }

        /// <summary>
        /// Display error messages sent by the mapmaker's viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mapMakerViewModel_MessageSender(object? sender, string e)
        {
            MessageBox.Show(e);
        }

        /// <summary>
        /// Handle mapmaker's map load request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _mapMakerViewModel_LoadGame(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new(); // dialógablak
            openFileDialog.Title = "Pálya Betöltése";
            openFileDialog.Filter = "JSON formátumú pálya|*.tdm|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefencePálya.tdm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\maps";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _mapMakerModel.LoadGameAsync(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Handle mapmaker's map save request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void _mapMakerViewModel_SaveGame(object? sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
            saveFileDialog.Title = "Pálya mentése";
            saveFileDialog.Filter = "JSON formátumú pálya|*.tdm|Összes fájl|*.*";
            saveFileDialog.FileName = "TowerDefencePálya.tdm";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\maps";
            if (saveFileDialog.ShowDialog() == true)
                try 
                { 
                    await _mapMakerModel.SaveGameAsync(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK);
                }
        }

        /// <summary>
        /// Handle game's game load request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new (); // dialógablak
            openFileDialog.Title = "Játék Betöltése";
            openFileDialog.Filter = "JSON formátumú mentés|*.tds|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefenceMentés.tds";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\saves";
            if (openFileDialog.ShowDialog() == true)
            {
                await _model.LoadGameAsync(openFileDialog.FileName);
            }
        }


        /// <summary>
        /// Handle game's map load request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewModel_LoadMap(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new(); // dialógablak
            openFileDialog.Title = "Pálya Betöltése";
            openFileDialog.Filter = "JSON formátumú pálya|*.tdm|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefencePálya.tdm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\maps";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Handle game's game save request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\saves"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\saves");
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
            saveFileDialog.Title = "Játék mentése";
            saveFileDialog.Filter = "JSON formátumú mentés|*.tds|Összes fájl|*.*";
            saveFileDialog.FileName = "TowerDefenceMentés.tds";
            saveFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\saves";
            if (saveFileDialog.ShowDialog() == true)
            {
                await _model.SaveGameAsync(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Starting a new game from the main menu (loading a map)
        /// </summary>
        /// <returns>Whether the user selected a map or not</returns>
        private async Task<bool> StartNewGame()
        {
            //_model.NewGame();
            OpenFileDialog openFileDialog = new(); // dialógablak
            openFileDialog.Title = "Pálya Betöltése";
            openFileDialog.Filter = "JSON formátumú pálya|*.tdm|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefencePálya.tdm";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\maps";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Hiba", MessageBoxButton.OK);
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Loading game save from the main menu
        /// </summary>
        /// <returns>Whether the user selected a save or not</returns>
        private async Task<bool> LoadFromMain()
        {
            if(!Directory.Exists(Environment.CurrentDirectory + "\\saves"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\saves");
            OpenFileDialog openFileDialog = new(); // dialógablak
            openFileDialog.Title = "Játék Betöltése";
            openFileDialog.Filter = "JSON formátumú mentés|*.tds|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefenceMentés.tds";
            openFileDialog.InitialDirectory = Environment.CurrentDirectory + "\\saves";
            if (openFileDialog.ShowDialog() == true)
            {
                await _model.LoadGameAsync(openFileDialog.FileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handle game over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Type of game over state</param>
        /// <exception cref="ArgumentException">Thrown if game over type is unrecognized</exception>
        private void _model_GameOver(object? sender, GameModel.GameOverType e)
        {
            string msg = e switch
            {
                GameModel.GameOverType.DRAW => "A játék döntetlen!",
                GameModel.GameOverType.REDWIN => "A piros játékos nyert!",
                GameModel.GameOverType.BLUEWIN => "A kék játékos nyert!",
                _ => throw new ArgumentException("Unrecognized game over type"),
            };
            MessageBox.Show(msg, "Játék vége", MessageBoxButton.OK);
        }

        /// <summary>
        /// Determining selected main menu button
        /// </summary>
        /// <param name="windowType"></param>
        private async void OpenNewWindow(int windowType)
        {
            if (windowType == 1)
            {
                if (!await StartNewGame()) return;
                if (_view==null)
                {
                    _view = new MainWindow();
                    _view.DataContext = _viewModel;
                    _view.Closing += CloseWindow;
                }
                _viewModel.NewGame();
                _view.Show();

            }
            else if(windowType == 2)
            {
                if (!await LoadFromMain()) return;
                if (_view == null)
                {
                    _view = new MainWindow();
                    _view.DataContext = _viewModel;
                    _view.Closing += CloseWindow;
                }
                _view.Show();
            }
            else if (windowType == 3)
            {
                _mapMaker = new MapMaker();
                _mapMaker.DataContext = _mapMakerViewModel;
                _mapMakerModel.CreateNewMap();
                _mapMaker.Show();
                _mapMaker.Closing += CloseWindow;
            }
            _mainMenu.Hide();
        }

        /// <summary>
        /// Handle exiting from the game
        /// </summary>
        private void ExitFromGame()
        {
            _mainMenu.Show();
            _view.Hide();
        }

        /// <summary>
        /// Handle exiting from the map maker
        /// </summary>
        private void ExitFromMapMaker()
        {
            _mainMenu.Show();
            _mapMaker.Hide();
        }

        /// <summary>
        /// Handle a window closing.
        /// Main function is to to display "are you sure?" message.
        /// Also makes sure the app shuts down properly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_view is null && _mapMaker is null) || (_view is not null && _view.IsVisible) || (_mapMaker is not null && _mapMaker.IsVisible) || (_mainMenu is not null && _mainMenu.IsVisible))
            {
                MessageBoxResult result = MessageBox.Show("Biztos ki akarsz lépni a játékból?", "Tower Defence", MessageBoxButton.YesNo);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Current.Shutdown();
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
                return;
            }
            else
            {
                Current.Shutdown();
            }
        }
    }
}
