using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.Model;
using System.Threading.Tasks;

namespace TowerDefenceBackend.ViewModel
{
    public class GameViewModel : MainViewModel
    {
        #region Variables
        private string turnText;
        private string nextTurnText;
        private bool advanceEnable;
        private int round;
        private uint money;

        private FieldViewModel? selectedField;
        private ICollection<MenuOption> menuOptions;
        private GameModel model;
        private Tower selectedTower; //I want to get rid of this
        private Castle selectedCastle; // it's just bloat at this point
        #endregion

        #region Properties
        public Castle SelectedCastle
        {
            get { return selectedCastle; }
            set { selectedCastle = value; OnPropertyChanged(); }
        }

        public Tower SelectedTower
        {
            get { return selectedTower; }
            set { selectedTower = value; OnPropertyChanged(); }
        }

        public string TurnText { get { return turnText; } set { turnText = value; OnPropertyChanged(); } }
        public string NextTurnText { get { return nextTurnText; } set { nextTurnText = value; OnPropertyChanged(); } }
        public bool AdvanceEnable { get { return advanceEnable; } set { advanceEnable = value; OnPropertyChanged(); } }
        public bool SaveEnabled { get { return model.SaveEnabled; } }
        public int Round { get { return round; } set { round = value; OnPropertyChanged(); } }
        public uint Money { get { return money; } set { money = value; OnPropertyChanged(); } }
        public ObservableCollection<Unit> UnitFields { get; set; }
        public FieldViewModel? SelectedField
        {
            get { return selectedField; }
            set
            {
                if (selectedField is not null)
                {
                    selectedField.IsSelected = false;
                    selectedField.IsSelectedSize = 1;
                }
                selectedField = value;
                if (selectedField is not null)
                {
                    selectedField.IsSelected = true;
                    selectedField.IsSelectedSize = 3;
                    ButtonClick();
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region Events
        public event EventHandler SaveGame;
        public event EventHandler LoadGame;
        public event EventHandler LoadMap;
        #endregion

        #region On-Event Methods

        private void OnLoadMap()
        {
            LoadMap?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void Model_GameOver(object? sender, GameModel.GameOverType e)
        {
            AdvanceEnable = false;
        }

        private void OnSaveGame()
        {
            if (SaveGame != null)
                SaveGame(this, EventArgs.Empty);
        }

        private async Task OnTowerFired(Field field)
        {
            int number = (int)field.Coords.x * GridSizeY + (int)field.Coords.y;
            Fields[number].IsFiredOn = true;
            await Task.Delay(300);
            Fields[number].IsFiredOn = false;
        }
        #endregion

        #region Commands
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand CloseGameCommand { get; set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand AdvanceCommand { get; private set; }
        public DelegateCommand LoadMapCommand { get; private set; }
        #endregion

        #region Constructor
        public GameViewModel(GameModel model)
        {
            this.model = model;
            AdvanceCommand = new DelegateCommand(async p => await AdvanceGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            LoadMapCommand = new(param => OnLoadMap());
            model.TowerFired += new EventHandler<Field>(async (object? o, Field f) => await OnTowerFired(f));
            model.AttackEnded += new EventHandler(async (object? o, EventArgs e) => await AdvanceGame());
            model.UnitMoved += new EventHandler((object? o, EventArgs e) => RefreshTable());
            model.GameLoaded += Model_GameLoaded;
            model.UnitMoved += new EventHandler((object? o, EventArgs e) => ButtonClick());
            model.GameOver += Model_GameOver;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method for resetting the game board for a new game
        /// </summary>
        public void NewGame()
        {
            GridSizeX = model.Table.Size.Item1;
            GridSizeY = model.Table.Size.Item2;
            SetupText();
            OptionFields = new ObservableCollection<OptionField>();
            UnitFields = new ObservableCollection<Unit>();
            GenerateTable();
            RefreshTable();
            SelectedField = new FieldViewModel
            {
                IsCastle = false,
                IsTower = false,
            };
            SelectedTower = null;
            SelectedCastle = null;
            OptionFields.Clear();
        }
        /// <summary>
        /// Method for setting up the sidebar menu when a button is clicked on the board
        /// </summary>
        public override void ButtonClick()
        {
            if (selectedField is null) return;
            menuOptions = model.SelectField(model.Table[(uint)selectedField.Coords.x, (uint)selectedField.Coords.y]);
            SelectedTower = null;
            SelectedCastle = null;
            OptionFields.Clear();
            UnitFields.Clear();
            if (selectedField.Placement is Castle)
                SelectedCastle = (Castle)selectedField.Placement;
            if (selectedField.IsUnits)
            {
                foreach (Unit _unit in model.SelectedField.Units)
                    UnitFields.Add(_unit);
            }

            if (selectedField.Placement is BasicTower or SniperTower or BomberTower)
                SelectedTower = (Tower)SelectedField.Placement;
            if (SelectedField.Placement is Barrack)
            {
                Barrack selectedBarrack = (Barrack)SelectedField.Placement;
                foreach (Unit unit in selectedBarrack.UnitQueue)
                    UnitFields.Add(unit);
            }
            foreach (MenuOption menuOption in menuOptions)
            {
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, Type = menuOption.ToString(), OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }

        }
        /// <summary>
        /// Method for executing the sidebar menu option
        /// </summary>
        /// <param name="option">Selected option</param>
        public override void OptionsButtonClick(string option)
        {
            switch (option)
            {
                case "TrainBasic":
                    model.SelectOption(MenuOption.TrainBasic);
                    break;
                case "TrainTank":
                    model.SelectOption(MenuOption.TrainTank);
                    break;
                case "BuildBasic":
                    model.SelectOption(MenuOption.BuildBasic);
                    break;
                case "BuildBomber":
                    model.SelectOption(MenuOption.BuildBomber);
                    break;
                case "BuildSniper":
                    model.SelectOption(MenuOption.BuildSniper);
                    break;
                case "DestroyTower":
                    model.SelectOption(MenuOption.DestroyTower);
                    break;
                case "UpgradeTower":
                    model.SelectOption(MenuOption.UpgradeTower);
                    break;
            }
            Money = model.CurrentPlayer.Money;
            RefreshTable();
            ButtonClick();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Method for generating an empty game board
        /// </summary>
        private void GenerateTable()
        {
            Fields = new ObservableCollection<FieldViewModel>();
            for (int i = 0; i < GridSizeX; i++)
            {
                for (int j = 0; j < GridSizeY; j++)
                {
                    Fields.Add(new FieldViewModel
                    {
                        Coords = (i, j),
                        Number = i * GridSizeY + j,
                        BlueBasic = 0,
                        BlueTank = 0,
                        RedBasic = 0,
                        RedTank = 0,
                        PlayerType = model.Table[(uint)i, (uint)j].Placement?.Owner?.Type ?? PlayerType.NEUTRAL,
                        Placement = model.Table[(uint)i, (uint)j].Placement,
                        IsSelected = false,
                        IsSelectedSize = 1,
                        IsCastle = false,
                        IsTower = false,
                        ClickCommand = new DelegateCommand(param => SelectedField = Fields[(int)param])
                    });
                }
            }
        }
        /// <summary>
        /// Method for refreshing the game board, setting the owner and placement of the fields and adding all the units to them
        /// </summary>
        private void RefreshTable()
        {
            foreach (FieldViewModel field in Fields)
            {
                field.Placement = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement;
                field.PlayerType = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement?.Owner?.Type ?? PlayerType.NEUTRAL;
                field.BlueBasic = 0;
                field.BlueTank = 0;
                field.RedBasic = 0;
                field.RedTank = 0;
                if (model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Units.Count == 0)
                    continue;
                foreach (Unit unit in model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Units)
                {
                    switch (unit.Owner?.Type)
                    {
                        case PlayerType.BLUE:
                            if (unit is BasicUnit)
                                field.BlueBasic++;
                            else if (unit is TankUnit)
                                field.BlueTank++;
                            break;
                        case PlayerType.RED:
                            if (unit is BasicUnit)
                                field.RedBasic++;
                            else if (unit is TankUnit)
                                field.RedTank++;
                            break;
                    }
                }
            }
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(OptionFields));
            OnPropertyChanged(nameof(UnitFields));
        }
        /// <summary>
        /// Method for advancing the game through the game model
        /// </summary>
        /// <returns></returns>
        private async Task AdvanceGame()
        {
            var t = model.Advance();
            SetupText();
            await t;
            RefreshTable();
            ButtonClick();
        }
        /// <summary>
        /// Method for setting up all the text on the screen
        /// </summary>
        private void SetupText()
        {
            Money = model.CurrentPlayer.Money;
            Round = (int)model.Round;
            if (model.Phase % 3 == 0)
            {
                AdvanceEnable = false;
                NextTurnText = "Kék Építés";
                TurnText = "Támadás";
            }
            if (model.Phase % 3 == 2)
            {
                AdvanceEnable = true;
                NextTurnText = "Támadás";
                TurnText = "Piros Építés";
            }
            if (model.Phase % 3 == 1)
            {
                AdvanceEnable = true;
                NextTurnText = "Piros Építés";
                TurnText = "Kék Építés";
            }
        }

        private void Model_GameLoaded(object? sender, EventArgs e)
        {
            GridSizeX = model.Table.Size.Item1;
            GridSizeY = model.Table.Size.Item2;
            GenerateTable();
            RefreshTable();
            SetupText();
            OnPropertyChanged(nameof(Fields));
        }
        #endregion
    }
}
