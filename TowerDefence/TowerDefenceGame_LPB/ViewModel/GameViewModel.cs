using System;
using System.Collections.ObjectModel;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.Model;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private int selectedField;
        private GameModel model;
        public int GridSize { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public int SelectedField 
        { 
            get { return selectedField; } 
            set 
            {
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Black;
                selectedField = value;
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Red;
            }
        }
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<OptionField> OptionFields { get; set; }
        public GameViewModel(GameModel model)
        {
            this.model = model;
            GridSize = 11;
            OptionFields = new ObservableCollection<OptionField>();
            GenerateTable();
            RefreshTable();
        }
        private void GenerateTable()
        {
            Fields = new ObservableCollection<FieldViewModel>();
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Fields.Add(new FieldViewModel
                    {
                        Coords = (i, j),
                        Number = i * GridSize + j,
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
            }
        }
        public void ButtonClick(int index)
        {
            SelectedField = index;
            FieldViewModel testField = Fields[index];
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
                case "BuildBasic":
                    model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new BasicTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "BuildBomber":
                    model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new BomberTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "BuildSniper":
                    model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new SniperTower(model.CurrentPlayer, ((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
                case "DestroyTower":
                    model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y].Placement = new Placement(((uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y));
                    break;
            }
            RefreshTable();
            ButtonClick(selectedField);
        }
    }
}