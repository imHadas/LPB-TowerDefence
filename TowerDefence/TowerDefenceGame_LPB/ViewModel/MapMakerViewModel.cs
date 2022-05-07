using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TowerDefenceBackend.Model;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.ViewModel
{
    public class MapMakerViewModel : MainViewModel
    {
        #region Variables
        private MapMakerModel model;
        private FieldViewModel selectedField;

        private ICollection<MenuOption> menuOptions;
        #endregion

        #region Properties
        public uint SetGridSizeX { get; set; }
        public uint SetGridSizeY { get; set; }
        public uint SetBlueMoney { get; set; }
        public uint SetRedMoney { get; set; }

        public uint BlueMoney
        {
            get { return model.BP.Money; }
            set { model.BP.Money = value; OnPropertyChanged(); }
        }

        public uint RedMoney
        {
            get { return model.RP.Money; }
            set { model.RP.Money = value; OnPropertyChanged(); }
        }

        public Player SelectedPlayer
        {
            get { return model.SelectedPlayer; }
            set
            {
                model.SelectPlayer(value); OnPropertyChanged(); ButtonClick();
            }
        }
        public FieldViewModel SelectedField
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
                }
                ButtonClick();
            }
        }
        #endregion

        #region Events
        public event EventHandler SaveGame;
        public event EventHandler LoadGame;
        public event EventHandler<string> MessageSender;
        #endregion

        #region On-Event Methods

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }
        private void OnSendMessage(string msg)
        {
            MessageSender?.Invoke(this, msg);
        }

        #endregion

        #region Commands
        public DelegateCommand SelectPlayerCommand { get; set; }
        public DelegateCommand SetGameSizeCommand { get; set; }
        public DelegateCommand SetStartingMoneyCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand CloseMapMakerCommand { get; set; }

        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }
        #endregion

        #region Constructor
        public MapMakerViewModel(MapMakerModel model)
        {
            this.model = model;
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            SetGridSizeX = (uint)GridSizeX;
            SetGridSizeY = (uint)GridSizeY;
            SetBlueMoney = model.BP.Money;
            SetRedMoney = model.RP.Money;
            OptionFields = new ObservableCollection<OptionField>();
            model.NewMapCreated += (sender, args) => { GenerateTable(); RefreshTable(); };
            model.GameLoaded += Model_GameLoaded;
            SelectPlayerCommand = new DelegateCommand(param => SelectPlayer((string)param));
            SetGameSizeCommand = new DelegateCommand(p => SetGameSize());
            SetStartingMoneyCommand = new DelegateCommand(p => SetStartingMoney());
            menuOptions = new List<MenuOption>();
            SaveGameCommand = new(p => OnSaveGame());
            LoadGameCommand = new(p => OnLoadGame());

            GenerateTable();
            RefreshTable();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method for setting up the sidebar menu when a button is clicked on the board
        /// </summary>
        public override void ButtonClick()
        {
            if (selectedField is not null)
                menuOptions = model.SelectField(model.Table[(uint)selectedField.Coords.x, (uint)selectedField.Coords.y]);
            else
                menuOptions?.Clear();
            OptionFields.Clear();
            foreach (MenuOption menuOption in menuOptions)
            {
                OptionFields.Add(new OptionField { Type = menuOption.ToString(), OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }
        }
        /// <summary>
        /// Method for executing the sidebar menu option
        /// </summary>
        /// <param name="option">Selected option</param>
        public override void OptionsButtonClick(string option)
        {
            try
            {
                switch (option)
                {
                    case "BuildCastle":
                        model.SelectOption(MenuOption.BuildCastle);
                        break;
                    case "BuildBarrack":
                        model.SelectOption(MenuOption.BuildBarrack);
                        break;
                    case "BuildMountain":
                        model.SelectOption(MenuOption.BuildMountain);
                        break;
                    case "BuildLake":
                        model.SelectOption(MenuOption.BuildLake);
                        break;
                    case "DestroyPlacement":
                        model.SelectOption(MenuOption.DestroyPlacement);
                        break;

                }

                RefreshTable();
                ButtonClick();
            }
            catch (Exception ex)
            {
                OnSendMessage(ex.Message);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Method for setting the new size of the game board
        /// </summary>
        private void SetGameSize()
        {
            try
            {
                model.ChangeTableSize(SetGridSizeX, SetGridSizeY);
            }
            catch (InvalidOperationException ex)
            {
                OnSendMessage(ex.Message);
                return;
            }
            SelectedField = null;
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged("Fields");
            ButtonClick();
        }
        /// <summary>
        /// Method for setting the starting money for each player in the game model
        /// </summary>
        private void SetStartingMoney()
        {
            BlueMoney = SetBlueMoney;
            RedMoney = SetRedMoney;
        }
        /// <summary>
        /// Method for generating an empty game board
        /// </summary>
        private void GenerateTable()
        {
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            SetGridSizeX = (uint)GridSizeX;
            SetGridSizeY = (uint)GridSizeY;
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
                        ClickCommand = new DelegateCommand(param => SelectedField = Fields[(int)param])
                    });
                }
            }
        }

        /// <summary>
        /// Method for refreshing the game board, setting the owner and placement of the fields
        /// </summary>
        private void RefreshTable()
        {
            foreach (FieldViewModel field in Fields)
            {
                field.Placement = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement;
                field.PlayerType = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement?.Owner?.Type ?? PlayerType.NEUTRAL;
            }
        }

        /// <summary>
        /// Method for setting the player the user wants to place down a new <c>Placement</c>
        /// </summary>
        /// <param name="type">The player type</param>
        public void SelectPlayer(string type)
        {
            switch (type)
            {
                case "Blue":
                    SelectedPlayer = model.BP;
                    break;
                case "Red":
                    SelectedPlayer = model.RP;
                    break;
                case "Neutral":
                    SelectedPlayer = null;
                    break;
            }
        }

        private void Model_GameLoaded(object? sender, EventArgs e)
        {
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            SetGridSizeX = (uint)GridSizeX;
            SetGridSizeY = (uint)GridSizeY;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(SetGridSizeX));
            OnPropertyChanged(nameof(SetGridSizeY));
        }

        #endregion
    }
}
