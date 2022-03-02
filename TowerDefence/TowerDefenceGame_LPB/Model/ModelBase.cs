using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        DestroyPlacement,
        BuildCastle,
        BuildTerrain,
        SetOwnerWhite,
        SetOwnerRed,
        SetOwnerBlue
    }
    public abstract class ModelBase
    {
    }
}
