using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.ViewModel
{
    public class FieldViewModel : ViewModelBase
    {
        #region Variables

        private int blueBasic;
        private int blueTank;
        private int redBasic;
        private int redTank;
        private bool isUnits;
        private bool isTower;
        private bool isCastle;
        private bool isSelected;
        private PlayerType playerType;
        private Placement? placement;
        private string? placementType;
        private int isSelectedSize;

        #endregion

        #region Properties
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

        public int RedBasic
        {
            get { return redBasic; }
            set
            {
                redBasic = value;
                IsOccupied();
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

        public bool IsUnits
        {
            get { return isUnits; }
            set
            {
                isUnits = value;
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

        public bool IsCastle
        {
            get { return isCastle; }
            set
            {
                isCastle = value;
                OnPropertyChanged();
            }
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

        public PlayerType PlayerType
        {
            get { return playerType; }
            set
            {
                playerType = value;
                OnPropertyChanged();
            }
        }

        public Placement? Placement
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
                        Terrain? terrain = value as Terrain;
                        PlacementType = terrain?.Type.ToString(); //better way to reference Type from terrain?
                        break;
                }
            }
        }

        public string? PlacementType
        {
            get { return placementType; }
            set { placementType = value; OnPropertyChanged(); }
        }

        public int IsSelectedSize
        {
            get { return isSelectedSize; }
            set { isSelectedSize = value; OnPropertyChanged(); }
        }

        public int Number { get; set; }

        public (int x, int y) Coords { get; set; }

        #endregion

        #region Commands
        public DelegateCommand ClickCommand { get; set; } = null!;
        #endregion

        #region Private method(s)
        ///<summary>
        ///Sets IsUnits property according to field occupation.
        ///</summary>
        private void IsOccupied()
        {
            IsUnits = 0 < RedBasic || 0 < RedTank || 0 < BlueBasic || 0 < BlueTank;
        }
        #endregion
    }
}
