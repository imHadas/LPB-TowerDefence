﻿using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        private int blueBasic;
        private int blueTank;
        private int redBasic;
        private int redTank;
        private bool isUnits;
        private bool isTower; // wanna find another way for this
        private bool isCastle; // guess who's back
        /*
        private bool isBarrack;
        private bool isSniperTower;
        private bool isBomberTower;
        */
        private bool isSelected;
        private PlayerType playerType;
        private Placement placement;
        private string placementType;
        private int isSelectedSize;

        public int IsSelectedSize
        {
            get { return isSelectedSize; }
            set { isSelectedSize = value; OnPropertyChanged(); }
        }


        public string PlacementType
        {
            get { return placementType; }
            set { placementType = value; OnPropertyChanged();}
        }


        public int Number { get; set; }
        public (int x, int y) Coords { get; set; }

        public Placement Placement
        {
            get { return placement; }
            set 
            {
                placement = value;
                IsTower = false;
                IsCastle = false;
                //OnPropertyChanged(); //kinda unnecessary
                if (placement is null)
                {
                    PlacementType = "";
                    return;
                }

                switch (placement)
                {
                    case Barrack:
                        PlacementType = "Barrack";
                        break;
                    case Castle:
                        PlacementType = "Castle";
                        IsCastle = true;
                        break;
                    case BasicTower:
                        PlacementType = "BasicTower";
                        IsTower = true;
                        break;
                    case BomberTower:
                        PlacementType = "BomberTower";
                        IsTower = true;
                        break;
                    case SniperTower:
                        PlacementType = "SniperTower";
                        IsTower = true;
                        break;
                    case Terrain:
                        Terrain terrain = value as Terrain;
                        PlacementType = terrain.Type.ToString(); //better way to reference Type from terrain?
                        break;
                }
                
                /*
                IsBarrack = placement.GetType() == typeof(Barrack) ? true : false;
                IsCastle = placement.GetType() == typeof(Castle) ? true : false;
                IsBasicTower = placement.GetType() == typeof(BasicTower) ? true : false;
                IsBomberTower = placement.GetType() == typeof(BomberTower) ? true : false;
                IsSniperTower = placement.GetType() == typeof(SniperTower) ? true : false;
                 */
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
        
        public bool IsTower
        {
            get { return isTower; }
            set 
            {
                isTower = value;
                OnPropertyChanged();
            }
        }
        /*
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
        */
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

        public bool IsSelected 
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
