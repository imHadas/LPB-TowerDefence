using System;
using System.Collections.ObjectModel;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private string turnText;
        private string nextTurnText;
        private bool advanceEnable;
        private int round;
        private uint money;

        private int selectedField;
        private GameModel model;
        
        public int GridSizeX { get; set; }
        public int GridSizeY { get; set; }
        public string TurnText { get { return turnText; } set { turnText = value; OnPropertyChanged(); } }
        public string NextTurnText { get { return nextTurnText; } set { nextTurnText = value; OnPropertyChanged(); } }
        public bool AdvanceEnable { get { return advanceEnable; } set { advanceEnable = value; OnPropertyChanged(); } }
        public int Round { get { return round; } set { round = value; OnPropertyChanged(); } }
        public uint Money{get { return money; } set { money = value; OnPropertyChanged(); }}
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand CloseGameCommand { get; set; }
        public DelegateCommand AdvanceCommand { get; set; }
        public int SelectedField 
        { 
            get { return selectedField; } 
            set 
            {
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Black;
                selectedField = value;
                model.SelectField(model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y]);
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Red;
            }
        }
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<OptionField> OptionFields { get; set; }
        public GameViewModel(GameModel model)
        {
            this.model = model;
            GridSizeX = model.Table.Size.Item1;
            GridSizeY = model.Table.Size.Item2;
            AdvanceCommand = new DelegateCommand(p => AdvanceGame());
            model.NewGameCreated += new EventHandler((object o, EventArgs e) => RefreshTable());
            model.AttackEnded += new EventHandler((object o, EventArgs e) => AdvanceGame());
            model.UnitMoved += new EventHandler((object o, EventArgs e) => RefreshTable());
            SetupText();
            OptionFields = new ObservableCollection<OptionField>();
            GenerateTable();
            RefreshTable();
        }

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
                        IsSelected =  System.Windows.Media.Brushes.Black,
                        ClickCommand = new DelegateCommand(param => ButtonClick(Convert.ToInt32(param)))
                    });
                }
            }
        }
        private void RefreshTable()
        {
            foreach(FieldViewModel field in Fields)
            {
                //add units
                field.Placement = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement;
                field.PlayerType = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement?.Owner?.Type ?? PlayerType.NEUTRAL;
                field.BlueBasic = 0;
                field.BlueTank = 0;
                field.RedBasic = 0;
                field.RedTank = 0;
                if (model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Units.Count == 0)
                    continue;
                foreach(Unit unit in model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Units)
                {
                    switch(unit.Owner?.Type)
                    {
                        case PlayerType.BLUE:
                            if (unit.GetType() == typeof(BasicUnit))
                                field.BlueBasic++;
                            else if(unit.GetType() == typeof(TankUnit))
                                field.BlueTank++;
                            break;
                        case PlayerType.RED:
                            if (unit.GetType() == typeof(BasicUnit))
                                field.RedBasic++;
                            else if (unit.GetType() == typeof(TankUnit))
                                field.RedTank++;
                            break;
                    }
                }
            }
        }
        private void AdvanceGame()
        {
            model.Advance();
            SetupText();
            ButtonClick(SelectedField);
        }
        private void SetupText()
        {
            Money = model.CurrentPlayer.Money;
            Round = model.Round;
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
                NextTurnText="Piros Építés";
                TurnText = "Kék Építés";
            }
        }
        public void ButtonClick(int index)
        {
            SelectedField = index;
            FieldViewModel testField = Fields[index];
            if (testField.PlayerType == model.OtherPlayer.Type)
            {
                OptionFields.Clear();
                return;
            }
                
            if (testField.IsBarrack || testField.IsCastle)
            {
                OptionFields.Clear();
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, TrainBasic = true, Type = "TrainBasic", OptionsClickCommand = new DelegateCommand(param=>OptionsButtonClick((string)param)) });
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, TrainTank = true, Type = "TrainTank", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }
            else if (testField.IsUnits)
            {
                OptionFields.Clear();
            }
            else if (testField.IsBasicTower || testField.IsSniperTower || testField.IsBomberTower)
            {
                OptionFields.Clear();
                //Check if path blocked
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, UpgradeTower = true, Type = "UpgradeTower", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, DestroyTower = true, Type = "DestroyTower", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }
           
            else
            {
                OptionFields.Clear();
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, BuildBasic = true, Type="BuildBasic", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, BuildBomber = true, Type = "BuildBomber", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                OptionFields.Add(new OptionField { Player = model.CurrentPlayer.Type, BuildSniper = true, Type = "BuildSniper", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }

        }
        public void OptionsButtonClick(string option)
        {
            //Modellben levo SelectOption?
            switch(option)
            {
                case "TrainBasic":
                    model.SelectOption(MenuOption.TrainBasic);
                    break;
                case "TrainTank":
                    model.SelectOption(MenuOption.TrainTank);
                    break;
                case "BuildBasic":
                    model.SelectOption(MenuOption.BuildBasic);
                    //model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new BasicTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "BuildBomber":
                    model.SelectOption(MenuOption.BuildBomber);
                    //model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new BomberTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "BuildSniper":
                    model.SelectOption(MenuOption.BuildSniper);
                    //model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new SniperTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "DestroyTower":
                    model.SelectOption(MenuOption.DestroyTower);
                    //model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new Placement(((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
            }
            Money = model.CurrentPlayer.Money;
            RefreshTable();
            ButtonClick(selectedField);
        }
    }
}
