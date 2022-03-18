using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;
using TowerDefenceGame_LPB.DataAccess;

namespace TowerDefenceGame_LPB.Model
{
    public enum MenuOption
    {
        TrainBasic,
        TrainTank,
        BuildBasic,
        BuildSniper,
        BuildBomber,
        UpgradeTower,
        DestroyTower,
        // ShowUnits,

        // for map maker only
        DestroyPlacement,
        BuildCastle,
        BuildTerrain,
        SetOwnerWhite,
        SetOwnerRed,
        SetOwnerBlue
    }
    public abstract class ModelBase
    {
        #region Variables

        protected IDataAccess dataAccess;

        protected HashSet<(uint, uint)> allCoords = new HashSet<(uint, uint)>();

        internal IPathfinder pathfinder;

        #endregion

        #region Properties

        public Field? SelectedField { get; protected set; }

        public Table Table { get; set; }

        #endregion

        public abstract ICollection<MenuOption> SelectField(Field field);
        
        protected void SetupTable(uint width, uint height)  //height is the x coordinates (columns), width is the y (rows)
        {
            Table = new(height, width);
            allCoords.Clear();
            for (uint i = 0; i < height; i++) //go through every column (x coord)
            {
                for (uint j = 0; j < width; j++) //go through every row (y coord)
                {
                    Table[i, j] = new Field(i, j);
                    Table[i, j].Placement = new Placement((i, j));
                    allCoords.Add((i, j));
                }
            }
            pathfinder = new AStar(Table);
        }

        protected IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to)
        {
            return pathfinder.FindPath(from, to);
        }

        protected IList<(uint, uint)> FindPath(Field from, Field to)
        {
            return FindPath(from.Coords, to.Coords);
        }

        public abstract void SelectOption(MenuOption option);

        #region Events

        /*public EventHandler<Field> FieldChanged()
        {

        }*/

        #endregion

        public async void LoadMap()
        {

        }

    }
}
