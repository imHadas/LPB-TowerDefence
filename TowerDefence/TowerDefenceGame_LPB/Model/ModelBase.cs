using System.Collections.Generic;
using TowerDefenceBackend.Persistence;
using TowerDefenceBackend.DataAccess;

namespace TowerDefenceBackend.Model
{
    /// <summary>
    /// All possible options for a field
    /// </summary>
    public enum MenuOption
    {
        TrainBasic,
        TrainTank,
        BuildBasic,
        BuildSniper,
        BuildBomber,
        UpgradeTower,
        DestroyTower,

        // for map maker only
        DestroyPlacement,
        BuildCastle,
        BuildBarrack,
        BuildMountain,
        BuildLake,
        SetOwnerWhite,
        SetOwnerRed,
        SetOwnerBlue
    }

    /// <summary>
    /// Base class for both models to derive from
    /// </summary>
    public abstract class ModelBase
    {
        #region Variables

        protected IDataAccess<GameSaveObject> gameDataAccess;

        internal IPathfinder pathfinder;

        #endregion

        #region Properties

        public Field? SelectedField { get; protected set; }

        public Table Table { get; set; }

        #endregion

        /// <summary>
        /// Abstract method for selecting a <c>Field</c> on the <c>Table</c>
        /// </summary>
        /// <param name="field"><c>Field</c> to select</param>
        /// <returns>Collection of available menu options</returns>
        public abstract ICollection<MenuOption> SelectField(Field field);

        /// <summary>
        /// Abstract method selecting an option
        /// </summary>
        /// <param name="option">Option to select</param>
        public abstract void SelectOption(MenuOption option);
        
        /// <summary>
        /// Sets <c>Table</c> to specified height and sets all fields to empty ones.
        /// Also updates pathfinder
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void SetupTable(uint width, uint height)  //height is the x coordinates (columns), width is the y (rows)
        {
            Table = new(height, width);
            for (uint i = 0; i < height; i++) //go through every column (x coord)
            {
                for (uint j = 0; j < width; j++) //go through every row (y coord)
                {
                    Table[i, j] = new Field(i, j);
                }
            }
            pathfinder = new AStar(Table);
        }

        /// <summary>
        /// Encapsulation of pathfinder's <c>FindPath</c> function
        /// </summary>
        /// <param name="from">Starting coordinate</param>
        /// <param name="to">Destination coordinate</param>
        /// <returns>List of coordinates containing the ordered path</returns>
        protected IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to)
        {
            return pathfinder.FindPath(from, to);
        }

        /// <summary>
        /// Shorthand for finding path between 2 <c>Field</c>s instead of coordinates
        /// </summary>
        /// <param name="from">Starting <c>Field</c></param>
        /// <param name="to">Destination <c>Field</c></param>
        /// <returns>List of coordinates containing the ordered path</returns>
        protected IList<(uint, uint)> FindPath(Field from, Field to)
        {
            return FindPath(from.Coords, to.Coords);
        }
    }
}
