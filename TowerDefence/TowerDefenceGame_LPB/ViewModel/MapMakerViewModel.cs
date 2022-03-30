using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TowerDefenceGame_LPB.Model;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.ViewModel
{
    internal class MapMakerViewModel : MainViewModel
    {
        private MapMakerModel model;
        private Player selectedPlayer;
        private int selectedField;

        public DelegateCommand SelectPlayerCommand { get; set; }
        public DelegateCommand SetGameSizeCommand { get; set; }

        public uint setGridSizeX { get; set; }
        public uint setGridSizeY { get; set; }

        public Player SelectedPlayer
        {
            get { return selectedPlayer; }
            set
            {
                selectedPlayer = value;
                model.SelectedPlayer = selectedPlayer; OnPropertyChanged();ButtonClick(selectedField);
            }
        }
        public int SelectedField
        {
            get { return selectedField; }
            set
            {
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Black;
                Fields[selectedField].IsSelectedSize = 1;
                selectedField = value;
                model.SelectField(model.Table[(uint)Fields[selectedField].Coords.x, (uint)Fields[selectedField].Coords.y]);
                Fields[selectedField].IsSelected = System.Windows.Media.Brushes.Red;
                Fields[selectedField].IsSelectedSize = 3;
            }
        }
        public MapMakerViewModel(MapMakerModel model)
        {
            this.model = model;
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            setGridSizeX = (uint)GridSizeX;
            setGridSizeY = (uint)GridSizeY;
            OptionFields = new ObservableCollection<OptionField>();
            model.NewMapCreated += (sender, args) => RefreshTable();
            SelectPlayerCommand = new DelegateCommand(param => SelectPlayer((string)param));
            SetGameSizeCommand = new DelegateCommand(p => SetGameSize());
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
                        IsSelectedSize = 1,
                        ClickCommand = new DelegateCommand(param => ButtonClick(Convert.ToInt32(param)))
                    });
                }
            }
        }
        private void RefreshTable()
        {
            foreach (FieldViewModel field in Fields)
            {
                field.Placement = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement;
                field.PlayerType = model.Table[(uint)field.Coords.x, (uint)field.Coords.y].Placement?.Owner?.Type ?? PlayerType.NEUTRAL;
            }
        }
        public override void ButtonClick(int index)
        {
            SelectedField = index;
            FieldViewModel field = Fields[index];
            if (field.Placement is null)
            {
                OptionFields.Clear();
                if (selectedPlayer is null)
                {
                    OptionFields.Add(new OptionField { Type = "BuildMountain", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                    OptionFields.Add(new OptionField{Type = "BuildLake",OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param))});
                }
                else
                {
                    OptionFields.Add(new OptionField { Type = "BuildCastle", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                    OptionFields.Add(new OptionField { Type = "BuildBarrack", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
                }
                
            }
            else if (field.Placement is not null)
            {
                OptionFields.Clear();
                OptionFields.Add(new OptionField { Type = "DestroyPlacement", OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }
        }

        public void SetGameSize()
        {
            SelectedField = 0;
            model.ChangeTableSize(setGridSizeX,setGridSizeY);
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged("Fields");
            ButtonClick(selectedField);
        }
        public void OptionsButtonClick(string option)
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
                ButtonClick(selectedField);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
    }
}
