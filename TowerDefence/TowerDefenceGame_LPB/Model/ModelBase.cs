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

        #endregion

        #region Properties

        public Field? SelectedField { get; protected set; }

        public Table Table { get; set; }

        #endregion

        public abstract ICollection<MenuOption> SelectField(Field field);
        
        protected void SetupTable()
        {
            for (uint i = 0; i < Table.Size.x; i++)
            {
                for (uint j = 0; j < Table.Size.y; j++)
                {
                    Table[i, j] = new Field(i, j);
                    Table[i, j].Placement = new Placement((i, j));
                }
            }
        }

        protected IList<(uint, uint)> FindPath((uint, uint) from, (uint, uint) to)
        {
            throw new NotImplementedException();
        }

        protected IList<(uint, uint)> FindPath(Field from, Field to)
        {
            throw new NotImplementedException();
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
