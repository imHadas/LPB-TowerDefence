using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TowerDefenceBackend.DataAccess;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.Model
{
    public class MapMakerModel : ModelBase
    {
        #region Fields

        private Player rp;
        private Player bp;

        #endregion

        #region Properties

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
        public Player? SelectedPlayer { get; private set; }

        #endregion

        #region Events

        public event EventHandler NewMapCreated;
        public event EventHandler GameLoaded;

        #endregion

        #region On-event methods

        private void OnGameLoaded()
        {
            GameLoaded?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor(s)

        public MapMakerModel(IDataAccess<GameSaveObject> dataAccess)
        {
            this.gameDataAccess = dataAccess;
            CreateNewMap();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates a new empty map with a size of 11x11.
        /// </summary>
        public void CreateNewMap()
        {
            SetupTable(11, 11);
            rp = new Player(PlayerType.RED);
            bp = new Player(PlayerType.BLUE);
            SelectedPlayer = null;
            if (NewMapCreated != null)
                NewMapCreated(this, EventArgs.Empty);
        }

        /// <summary>
        /// Change table size, to the height and width given in parameters.
        /// </summary>
        /// <param name="height">The height you want the map to have.</param>
        /// <param name="width">The width you want the map to have.</param>
        /// <exception cref="InvalidOperationException">Throw if the map size is too small or too large.</exception>
        public void ChangeTableSize(uint height, uint width)
        {
            if (width > 20 || width < 4 || height > 20 || height < 4)
                throw new InvalidOperationException("You can't set the size bigger than 20x20!");
            Table newTable = new Table(height, width);
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    if (Table.Size.x > i && Table.Size.y > j)
                        newTable[i, j] = Table[i, j];
                    else
                        newTable[i, j] = new Field(i, j);
                }
            }
            Table = newTable;
            //nulling everything, re checking if item has been cut off of map
            rp.Castle = null;
            bp.Castle = null;
            rp.Barracks = new HashSet<Barrack>();
            bp.Barracks = new HashSet<Barrack>();
            //re checking
            foreach (Field field in Table)
            {
                if (field.Placement is Castle)
                    field.Owner.Castle = (Castle)field.Placement;
                if (field.Placement is Barrack)
                {
                    field.Owner.Barracks.Add((Barrack) field.Placement);
                }
            }

            pathfinder = new AStar(Table);

        }

        /// <summary>
        /// Method for setting the SelectedField to the field given in parameter.
        /// Adding menu options to a collection depending on what is on the field that we selected.
        /// </summary>
        /// <param name="field">Field that is getting selected.</param>
        /// <returns>Returns the collection we added options to.</returns>
        public override ICollection<MenuOption> SelectField(Field field)
        {
            SelectedField = field;
            ICollection<MenuOption> options = new List<MenuOption>();
            if (SelectedField.Placement is null) //can expand to depend on selected player
            {
                if (SelectedPlayer is null)
                {
                    options.Add(MenuOption.BuildMountain);
                    options.Add(MenuOption.BuildLake);
                }
                else
                {
                    options.Add(MenuOption.BuildCastle);
                    options.Add(MenuOption.BuildBarrack);
                }
            }
            else if (SelectedField.Placement is not null)
            {
                options.Add(MenuOption.DestroyPlacement);
            }

            return options;
        }

        /// <summary>
        /// Method for building or destroying depending on the option given in paramter.
        /// </summary>
        /// <param name="option">The option, which was chosen from the menu options available.</param>
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

        /// <summary>
        /// Method for selecting the player in the parameter.
        /// </summary>
        /// <param name="player">The player we want to select.</param>
        public void SelectPlayer(Player? player)
        {
            SelectedPlayer = player;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Method for building the castle of the selected player on the selected field.
        /// </summary>
        /// <exception cref="InvalidPlacementException">
        /// Throws if the player is not selected, or something is already placed on the selected field, or there is already a castle for this player on the table.
        /// </exception>
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

        /// <summary>
        /// Method for building a barrack for the selected player on the selected field.
        /// </summary>
        /// <exception cref="InvalidPlacementException">
        /// Throws if the player is not selected, or something is already placed on the selected field, or the player already has 2 barracks on the table, or the path between a castle and enemy barrack becomes blocked.
        /// </exception>
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
                    if (bp.Barracks.Count >= 2)
                        throw new InvalidPlacementException(SelectedField,
                            "Cannot build more than two barracks per player");
                    bp.Barracks.Add(barrack);
                    break;
                case (PlayerType.RED):
                    if (rp.Barracks.Count >= 2)
                        throw new InvalidPlacementException(SelectedField,
                            "Cannot build more than two barracks per player");
                    rp.Barracks.Add(barrack);
                    break;
            }
            //SelectedPlayer.Barracks.Add(barrack); //Cannot add because its immutable
            SelectedField.Placement = barrack;
            if (!ValidatePath())
            {
                if (SelectedPlayer == rp)
                    rp.Barracks.Remove(barrack);
                else if (SelectedPlayer == bp)
                    bp.Barracks.Remove(barrack);
                SelectedField.Placement = null;
                throw new InvalidPlacementException(SelectedField, "Cannot block path between castle and barracks");
            }
        }

        /// <summary>
        /// Method for building a terrain element on the selected field
        /// </summary>
        /// <param name="type">Type of terrain</param>
        /// <exception cref="InvalidPlacementException">Throws if player is selected or something is already placed on the selected field</exception>
        private void BuildTerrain(TerrainType type)
        {
            if (SelectedPlayer is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build barrack for players");
            if (SelectedField.Placement is not null)
                throw new InvalidPlacementException(SelectedField, "Cannot build terrain on non-empty field");
            SelectedField.Placement = new Terrain(SelectedField.Coords.x, SelectedField.Coords.y, type);
            if (!ValidatePath())
            {
                SelectedField.Placement = null;
                throw new InvalidPlacementException(SelectedField, "Cannot block path between castle and barracks");
            }
        }

        /// <summary>
        /// Method for validating the available path between each players barracks and the enemy castle
        /// </summary>
        /// <returns>If the path is valid</returns>
        private bool ValidatePath()
        {
            IList<(uint x, uint y)> path = new List<(uint x, uint y)>();
            foreach (Barrack barrack in rp.Barracks)
            {
                if (bp.Castle is null)
                    continue;
                path = FindPath(barrack.Coords, bp.Castle.Coords);
                if (path.Count == 0 || path.Last() != bp.Castle.Coords)
                    return false;
            }
            foreach (Barrack barrack in bp.Barracks)
            {
                if (rp.Castle is null)
                    continue;
                path = FindPath(barrack.Coords, rp.Castle.Coords);
                if (path.Count == 0 || path.Last() != rp.Castle.Coords)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Method for destroying a placement on the selected field
        /// </summary>
        /// <exception cref="InvalidPlacementException">Throws when the field is already empty</exception>
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
                        bp.Barracks.Remove((Barrack)SelectedField.Placement);
                    else if (SelectedField.Owner == rp)
                        rp.Barracks.Remove((Barrack)SelectedField.Placement);
                    SelectedField.Placement = null;
                    break;
                case Terrain:
                    SelectedField.Placement = null;
                    break;
            }

        }

        #endregion

        #region Persistence methods

        public async Task SaveGameAsync(string path)
        {
            if (gameDataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            if (!ValidatePath()) throw new Exception("Path is blocked between barracks and castles");
            if (rp.Castle == null || bp.Castle == null || rp.Barracks.Count != 2 || bp.Barracks.Count != 2)
                throw new Exception("Map contains invalid amount of castles and/or barracks");


            Table.PhaseCounter = 1;

            await gameDataAccess.SaveAsync(path, new(Table, bp, rp));
        }

        public async Task LoadGameAsync(string path)
        {
            if (gameDataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            GameSaveObject save = await gameDataAccess.LoadAsync(path);
            (Table, bp, rp) = (save.Table, save.BluePlayer, save.RedPlayer);
            pathfinder = new AStar(Table);
            OnGameLoaded();
        }

        #endregion
    }
}
