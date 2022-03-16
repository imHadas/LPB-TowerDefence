using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class TestField : ViewModelBase
    {
        private int blueBasic;
        private int blueTank;
        private int redBasic;
        private int redTank;
        private bool isUnits;
        private bool isCastle;
        private bool isBarrack;
        private bool isBasicTower;
        private bool isSniperTower;
        private bool isBomberTower;
        private Brush isSelected;
        private PlayerType playerType;
        private Placement placement;

        public int Number { get; set; }
        public (int x, int y) Coords { get; set; }

        public Placement Placement
        {
            get { return placement; }
            set 
            {
                placement = value;
                //OnPropertyChanged(); //kinda unnecessary
                IsBarrack = placement.GetType().ToString() == "TowerDefenceGame_LPB.Persistence.Barrack" ? true : false;
                IsCastle = placement.GetType().ToString() == "TowerDefenceGame_LPB.Persistence.Caslte" ? true : false;
                IsBasicTower = placement.GetType().ToString() == "TowerDefenceGame_LPB.Persistence.BasicTower" ? true : false;
                IsBomberTower = placement.GetType().ToString() == "TowerDefenceGame_LPB.Persistence.BomberTower" ? true : false;
                IsSniperTower = placement.GetType().ToString() == "TowerDefenceGame_LPB.Persistence.SniperTower" ? true : false;
            }
        }

        public PlayerType PlayerType 
        { 
            get { return playerType; }
            set 
            { 
                playerType = value;
                OnPropertyChanged();
            } 
        }

        public bool IsBasicTower
        {
            get { return isBasicTower; }
            set 
            {
                isBasicTower = value;
                OnPropertyChanged();
            }
        }
        public bool IsSniperTower
        {
            get { return isSniperTower; }
            set
            {
                isSniperTower = value;
                OnPropertyChanged();
            }
        }
        public bool IsBomberTower
        {
            get { return isBomberTower; }
            set
            {
                isBomberTower = value;
                OnPropertyChanged();
            }
        }


        public bool IsBarrack
        {
            get { return isBarrack; }
            set 
            { 
                isBarrack = value;
                OnPropertyChanged();
            }
        }


        public bool IsCastle
        {
            get { return isCastle; }
            set 
            {
                isCastle = value;
                OnPropertyChanged();
            }
        }


        public bool IsUnits
        {
            get { return isUnits; }
            set 
            { 
                isUnits = value;
                OnPropertyChanged();
            }
        }

        public int RedTank
        {
            get { return redTank; }
            set 
            { 
                redTank = value;
                IsOccupied();
                OnPropertyChanged();
            }
        }

        public int RedBasic
        {
            get { return redBasic; }
            set {
                redBasic = value;
                IsOccupied();
                OnPropertyChanged();
            }
        }

        public int BlueTank
        {
            get { return blueTank; }
            set
            {
                blueTank = value;
                IsOccupied();
                OnPropertyChanged();
            }
        }
        public int BlueBasic
        {
            get { return blueBasic; }
            set 
            { 
                blueBasic = value;
                IsOccupied();
                OnPropertyChanged();
            }
        }

        private void IsOccupied()
        {
            if (RedBasic == 0 && RedTank == 0 && BlueBasic == 0 && BlueTank == 0)
                IsUnits = false;
            else
                IsUnits = true;
        }

        public Brush IsSelected 
        { 
            get { return isSelected; } 
            set 
            { 
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand ClickCommand { get; set; }
    }
}
