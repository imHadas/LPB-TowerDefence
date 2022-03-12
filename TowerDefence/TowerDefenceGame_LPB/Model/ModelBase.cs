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
        ShowUnits,

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
        protected Field? selectedField;

        #endregion

        #region Properties

        public Table Table { get; set; }
        public Field SelectedField { get; set; }

        #endregion

        public abstract ICollection<MenuOption>? SelectField(Field field);
        
        private void SetupTable()
        {
            return;
        }

        private (int,int)[] FindPath(Field start, Field end)
        {
            return new (int, int)[2];
        }

        public virtual void SelectOption(MenuOption menus)
        {
            return;
        }

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
