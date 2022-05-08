using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceBackend.Persistence;

namespace TowerDefenceBackend.ViewModel
{
    public class OptionField : ViewModelBase
    {
        #region Properties
        public string Type { get; set; }

        public PlayerType Player { get; set; }
        #endregion

        #region Command(s)
        public DelegateCommand OptionsClickCommand { get; set; }
        #endregion
    }
}
