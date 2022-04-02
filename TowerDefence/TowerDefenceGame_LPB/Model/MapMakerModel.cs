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
        private ICollection<Barrack> rBarracks;
        private ICollection<Barrack> bBarracks;


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
            rBarracks = new HashSet<Barrack>();
            bBarracks = new HashSet<Barrack>();
            Castle rCastle = null, bCastle = null;
            rp = new Player(PlayerType.RED, rCastle, rBarracks);
            bp = new Player(PlayerType.BLUE, bCastle, bBarracks);
            SelectedPlayer = null;
            if (NewMapCreated != null)
                NewMapCreated(this, EventArgs.Empty);
        }

        public void ChangeTableSize(uint height, uint width)
        {
            Table newTable = new Table(height, width);
            allCoords.Clear();
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    if (Table.Size.x > i && Table.Size.y > j)
                        newTable[i,j] = Table[i,j];
                    else
                        newTable[i, j] = new Field(i, j);
                    allCoords.Add((i, j));
                }
            }
            Table = newTable;
            //nulling everything, re checking if item has been cut off of map
            rp.Castle = null;
            bp.Castle = null;
            rBarracks = new HashSet<Barrack>();
            bBarracks = new HashSet<Barrack>();
            //re checking
            foreach (Field field in Table)
            {
                if (field.Placement is Castle)
                    field.Owner.Castle = (Castle)field.Placement;
                if (field.Placement is Barrack)
                {
                    if(field.Owner == rp)
                        rBarracks.Add((Barrack)field.Placement);
                    else if(field.Owner == bp)
                        bBarracks.Add((Barrack)field.Placement);
                }
            }
            pathfinder = new AStar(Table);

        }

        public override ICollection<MenuOption> SelectField(Field field)
        {
            SelectedField = field;
            ICollection<MenuOption> options = new List<MenuOption>();
            if (SelectedField.Placement is null) //can expand to depend on selected player
            {
                options.Add(MenuOption.BuildCastle);
                options.Add(MenuOption.BuildBarrack);
                options.Add(MenuOption.BuildMountain);
                options.Add(MenuOption.BuildLake);

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
                case MenuOption.BuildMountain:
                    BuildTerrain(TerrainType.Mountain);
                    break;
                case MenuOption.BuildLake:
                    BuildTerrain(TerrainType.Lake);
                    break;
                case MenuOption.DestroyPlacement:
                    DestroyPlacement();
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
            if (!ValidatePath())
            {
                SelectedPlayer.Castle = null;
                SelectedField.Placement = null;
                throw new InvalidPlacementException(SelectedField, "Cannot block path between castle and barracks");
            }
        }

        private void BuildBarrack()
        {
            if (SelectedPlayer is null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack for neutral player");
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack on non-empty field");
            if (SelectedPlayer.Barracks.Count >= 2) //doesn't work currently with immutable set
                throw new InvalidPlacementException(SelectedField, "Cannot build more than two barracks per player");
            
            Barrack barrack = new Barrack(SelectedPlayer, SelectedField.Coords.x, SelectedField.Coords.y);
            switch (SelectedPlayer.Type) //temporary
            {
                case (PlayerType.BLUE):
                    if (bBarracks.Count >= 2)
                        throw new InvalidPlacementException(SelectedField,
                            "Cannot build more than two barracks per player");
                    bBarracks.Add(barrack);
                    break;
                case (PlayerType.RED):
                    if(rBarracks.Count >=2)
                        throw new InvalidPlacementException(SelectedField,
                            "Cannot build more than two barracks per player");
                    rBarracks.Add(barrack);
                    break;
            }
            //SelectedPlayer.Barracks.Add(barrack); //Cannot add because its immutable
            SelectedField.Placement = barrack;
            if (!ValidatePath())
            {
                if (SelectedPlayer == rp)
                    rBarracks.Remove(barrack);
                else if (SelectedPlayer == bp)
                    bBarracks.Remove(barrack);
                SelectedField.Placement = null;
                throw new InvalidPlacementException(SelectedField, "Cannot block path between castle and barracks");
            }
        }
        private void BuildTerrain(TerrainType type)
        {
            if (SelectedPlayer is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack for players");
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build terrain on non-empty field");
            SelectedField.Placement = new Terrain(SelectedField.Coords.x, SelectedField.Coords.y,type);
            if (!ValidatePath())
            {
                SelectedField.Placement = null;
                throw new InvalidPlacementException(SelectedField, "Cannot block path between castle and barracks");
            }
        }

        private bool ValidatePath()
        {
            IList<(uint x, uint y)> path = new List<(uint x, uint y)>();
            foreach (Barrack barrack in rBarracks)
            {
                if(bp.Castle is null)
                    return true;
                path = FindPath(barrack.Coords, bp.Castle.Coords);
                if (path.Count == 0 || path.Last() != bp.Castle.Coords )
                    return false;
            }
            foreach (Barrack barrack in bBarracks)
            {
                if (rp.Castle is null)
                    return true;
                path = FindPath(barrack.Coords, rp.Castle.Coords);
                if (path.Count == 0 || path.Last() != rp.Castle.Coords)
                    return false;
            }

            return true;
        }

        private void DestroyPlacement()
        {
            if (SelectedField.Placement is null)
                throw new InvalidPlacementException(SelectedField, "Cannot destroy empty field");
            switch (SelectedField.Placement)
            {
                case Castle:
                    SelectedField.Owner.Castle = null;
                    SelectedField.Placement = null;
                    break;
                case Barrack:
                    if (SelectedField.Owner == bp)
                        bBarracks.Remove((Barrack)SelectedField.Placement);
                    else if (SelectedField.Owner == rp)
                        rBarracks.Remove((Barrack) SelectedField.Placement);
                    SelectedField.Placement = null;
                    break;
                case Terrain:
                    SelectedField.Placement = null;
                    break;
            }
            
        }
    }
}
