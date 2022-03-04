using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private bool isTower;
        private PlayerType playerType;

        public int Number { get; set; }
        public (int x, int y) Coords { get; set; }

        public PlayerType PlayerType 
        { 
            get { return playerType; }
            set 
            { 
                playerType = value;
                OnPropertyChanged();
            } 
        }

        public bool IsTower
        {
            get { return isTower; }
            set 
            {
                isTower = value;
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
            }
        }

        public int RedBasic
        {
            get { return redBasic; }
            set {
                redBasic = value;
                IsOccupied();
            }
        }

        public int BlueTank
        {
            get { return blueTank; }
            set
            {
                blueTank = value;
                IsOccupied();
            }
        }
        public int BlueBasic
        {
            get { return blueBasic; }
            set 
            { 
                blueBasic = value;
                IsOccupied();
            }
        }

        private void IsOccupied()
        {
            if (RedBasic == 0 && RedTank == 0 && BlueBasic == 0 && BlueTank == 0)
                IsUnits = false;
            else
                IsUnits = true;
        }

        public DelegateCommand ClickCommand { get; set; }
    }
}
