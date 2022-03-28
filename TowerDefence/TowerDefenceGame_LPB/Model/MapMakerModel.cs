using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.DataAccess;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.ViewModel;

namespace TowerDefenceGame_LPB.Model
{
    public class MapMakerModel : ModelBase
    {
        private Player rp;
        private Player bp;

        public Player RP
        {
            get { return rp; }
            private set { rp = value; }
        }

        public Player BP
        {
            get { return bp; }
            private set { bp = value; }
        }

        public Player SelectedPlayer { get; set; }
        public event EventHandler NewMapCreated;

        public MapMakerModel(IDataAccess dataAccess)
        {
            CreateNewMap();
        }

        public void CreateNewMap()
        {
            SetupTable(11, 11);
            ICollection<Barrack> rBarracks = new HashSet<Barrack>();
            ICollection<Barrack> bBarracks = new HashSet<Barrack>();
            Castle rCastle = null, bCastle = null;
            rp = new Player(PlayerType.RED, rCastle, rBarracks);
            bp = new Player(PlayerType.BLUE, bCastle, bBarracks);
            SelectedPlayer = null;
            if (NewMapCreated != null)
                NewMapCreated(this, EventArgs.Empty);
        }

        public override ICollection<MenuOption> SelectField(Field field)
        {
            SelectedField = field;
            ICollection<MenuOption> options = new List<MenuOption>();
            if (SelectedField.Placement is null)
            {
                options.Add(MenuOption.BuildCastle);
                options.Add(MenuOption.BuildBarrack);
                options.Add(MenuOption.BuildTerrain);

            }
            else if (SelectedField.Placement is not null)
            {
                options.Add(MenuOption.DestroyPlacement);
            }

            return options;
        }

        public override void SelectOption(MenuOption option)
        {
            switch (option)
            {
                case MenuOption.BuildCastle:
                    BuildCastle();
                    break;
                case MenuOption.BuildBarrack:
                    BuildBarrack();
                    break;
                case MenuOption.BuildTerrain:
                    break;
                case MenuOption.DestroyPlacement:
                    break;
            }
        }

        private void BuildCastle()
        {
            if (SelectedPlayer is null)
                throw new InvalidPlacementException(SelectedField, "Cannot build castle for neutral player");
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build castle on non-empty field");
            if (SelectedPlayer.Castle is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build more than one castle per player");

            SelectedPlayer.Castle = new Castle(SelectedPlayer, SelectedField.Coords.x, SelectedField.Coords.y);
            SelectedField.Placement = SelectedPlayer.Castle;
        }

        private void BuildBarrack()
        {
            if (SelectedPlayer is null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack for neutral player");
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack on non-empty field");
            if (SelectedPlayer.Barracks.Count >= 2)
                throw new InvalidPlacementException(SelectedField, "Cannot build more than two barracks per player");
            Barrack barrack = new Barrack(SelectedPlayer, SelectedField.Coords.x, SelectedField.Coords.y);
            //SelectedPlayer.Barracks.Add(barrack); //Cannot add because its immutable
            SelectedField.Placement = barrack;

        }
    }
}
