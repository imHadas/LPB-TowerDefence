using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Persistence;

namespace TowerDefenceGame_LPB.ViewModel
{
    public class OptionField : ViewModelBase
    {
        public string Type { get; set; }
        public DelegateCommand OptionsClickCommand { get; set; }
        public PlayerType Player { get; set; }
        /*
        //Training Options
        public bool TrainBasic { get; set; }
        public bool TrainTank { get; set; }
        //Build Options
        public bool BuildBasic { get; set; }
        public bool BuildBomber { get; set; }
        public bool BuildSniper { get; set; }
        //Tower Options
        public bool UpgradeTower { get; set; }
        public bool DestroyTower { get; set; }
        */
    }

    

}
