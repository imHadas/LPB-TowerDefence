﻿using System;
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
        private FieldViewModel selectedField;

        private ICollection<MenuOption> menuOptions;
        public event EventHandler SaveGame;
        public event EventHandler LoadGame;

        public DelegateCommand SelectPlayerCommand { get; set; }
        public DelegateCommand SetGameSizeCommand { get; set; }
        public DelegateCommand SetStartingMoneyCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand CloseMapMakerCommand { get; set; }

        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }

        public uint SetGridSizeX { get; set; }
        public uint SetGridSizeY { get; set; }
        public uint SetBlueMoney { get; set; }
        public uint SetRedMoney { get; set; }

        public uint BlueMoney
        {
            get { return model.BP.Money; }
            set { model.BP.Money = value; OnPropertyChanged();}
        }

        public uint RedMoney
        {
            get { return model.RP.Money; }
            set { model.RP.Money = value; OnPropertyChanged(); }
        }

        public Player SelectedPlayer
        {
            get { return selectedPlayer; }
            set
            {
                selectedPlayer = value;
                model.SelectedPlayer = selectedPlayer; OnPropertyChanged();ButtonClick();
            }
        }
        public FieldViewModel SelectedField
        {
            get { return selectedField; }
            set
            {
                if (selectedField is not null)
                {
                    selectedField.IsSelected = System.Windows.Media.Brushes.Black;
                    selectedField.IsSelectedSize = 1;
                }
                selectedField = value;
                if(selectedField is not null)
                {
                    selectedField.IsSelected = System.Windows.Media.Brushes.Red;
                    selectedField.IsSelectedSize = 3;
                }
                ButtonClick();
            }
        }
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
            model.NewMapCreated += (sender, args) => RefreshTable();
            model.GameLoaded += Model_GameLoaded;
            SelectPlayerCommand = new DelegateCommand(param => SelectPlayer((string)param));
            SetGameSizeCommand = new DelegateCommand(p => SetGameSize());
            SetStartingMoneyCommand = new DelegateCommand(p => SetStartingMoney());

            SaveGameCommand = new(p => OnSaveGame());
            LoadGameCommand = new(p => OnLoadGame());

            GenerateTable();
            RefreshTable();
        }

        private void Model_GameLoaded(object? sender, EventArgs e)
        {
            GenerateTable();
            RefreshTable();
            OnPropertyChanged(nameof(Fields));
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
                        ClickCommand = new DelegateCommand(param => SelectedField = Fields[(int)param])
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
        public override void ButtonClick()
        {
            menuOptions = model.SelectField(model.Table[(uint)selectedField.Coords.x, (uint)selectedField.Coords.y]);
            OptionFields.Clear();
            foreach (MenuOption menuOption in menuOptions)
            {
                OptionFields.Add(new OptionField { Type = menuOption.ToString(), OptionsClickCommand = new DelegateCommand(param => OptionsButtonClick((string)param)) });
            }
        }

        public void SetGameSize()
        {
            SelectedField = null;
            model.ChangeTableSize(SetGridSizeX,SetGridSizeY);
            GridSizeX = model.Table.Size.x;
            GridSizeY = model.Table.Size.y;
            GenerateTable();
            RefreshTable();
            OnPropertyChanged("Fields");
            ButtonClick();
        }

        public void SetStartingMoney()
        {
            BlueMoney = SetBlueMoney;
            RedMoney = SetRedMoney;
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
                ButtonClick();
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

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
