﻿using System;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Threading;
using TowerDefenceBackend.ViewModel;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.DataAccess;
using TowerDefenceWPF.View;

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
        private DialogBox _dialogBox;

        private JsonDataAccess dataAccess = new();

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
            _model = new GameModel(dataAccess);
            _mapMakerModel = new MapMakerModel(dataAccess);
            _model.GameOver += _model_GameOver;
            //_model.NewGame();

            // Creating viewmodel
            _viewModel = new GameViewModel(_model);
            _viewModel.ExitCommand = new DelegateCommand(p => ExitFromGame());
            _viewModel.CloseGameCommand = new DelegateCommand(p => _view.Close());
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
            _viewModel.LoadGame += ViewModel_LoadGame;

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
            
            // Creating view
            //_view = new MainWindow();
            //_view.DataContext = _viewModel;
            //_view.Show();

        }

        private void _mapMakerViewModel_MessageSender(object? sender, string e)
        {
            MessageBox.Show(e);
        }

        private async void _mapMakerViewModel_LoadGame(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new(); // dialógablak
            openFileDialog.Title = "Pálya Betöltése";
            openFileDialog.Filter = "Json objektum|*.json|Összes fájl|*.*";
            openFileDialog.FileName = "TowerDefenceMentés.json";
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

        private async void _mapMakerViewModel_SaveGame(object? sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
            saveFileDialog.Title = "Pálya mentése";
            saveFileDialog.Filter = "Json objektum|*.json|Összes fájl|*.*";
            saveFileDialog.FileName = "TowerDefenceMentés";
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

        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            {
                OpenFileDialog openFileDialog = new (); // dialógablak
                openFileDialog.Title = "Játék Betöltése";
                openFileDialog.Filter = "Json objektum|*.json|Összes fájl|*.*";
                openFileDialog.FileName = "TowerDefenceMentés";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);
                }
            }
        }


        private void _model_GameOver(object? sender, GameModel.GameOverType e)
        {
            string msg;
            switch (e)
            {
                case GameModel.GameOverType.DRAW:
                    msg = "A játék döntetlen!";
                    break;
                case GameModel.GameOverType.REDWIN:
                    msg = "A piros játékos nyert!";
                    break;
                case GameModel.GameOverType.BLUEWIN:
                    msg = "A kék játékos nyert!";
                    break;
                default: throw new ArgumentException("what");
            }
            MessageBox.Show(msg, "Játék vége", MessageBoxButton.OK);
        }

        private void OpenNewWindow(int windowType)
        {
            if (windowType == 1)
            {
                if(_view==null)
                {
                    _view = new MainWindow();
                    _view.DataContext = _viewModel;
                    _model.NewGame();
                    _view.Show();
                    _view.Closing += CloseWindow;
                }
                else
                {
                    _model.NewGame();
                    _view.Show();
                }
                
            }
            else if(windowType == 2)
            {
                if (_view == null)
                {
                    ViewModel_LoadGame(this,EventArgs.Empty);
                    _view = new MainWindow();
                    _view.DataContext = _viewModel;
                    _view.Show();
                    _view.Closing += CloseWindow;
                }
                else
                {
                    ViewModel_LoadGame(this, EventArgs.Empty);
                    _view.Show();
                }
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
                if(_view == null)
                {
                    _view = new MainWindow();
                    _view.DataContext = _viewModel;
                }
                    
                _view.Show();
            }
            else if(windowType == 2)
            {
                if(_mapMaker == null)
                {
                    _mapMaker = new MapMaker();
                    _mapMaker.DataContext = _mapMakerViewModel;
                }
                _mapMaker.Show();
            }
            _dialogBox.Close();
            _mainMenu.Hide();
        }
        private void ExitFromGame()
        {
            /*_mainMenu = new MainMenu();
            _mainMenu.DataContext = this;*/
            _mainMenu.Show();
            _view.Hide();
        }

        private void ExitFromMapMaker()
        {
            /*_mainMenu = new MainMenu();
            _mainMenu.DataContext = this;*/
            _mainMenu.Show();
            _mapMaker.Hide();
        }

        private void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((_view is null && _mapMaker is null) || (_view is not null && _view.IsVisible) || (_mapMaker is not null && _mapMaker.IsVisible) || (_mainMenu is not null && _mainMenu.IsVisible))
            {
                MessageBoxResult result = MessageBox.Show("Biztos ki akarsz lépni a játékból?", "Tower Defence", MessageBoxButton.YesNo);

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
                return;
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
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