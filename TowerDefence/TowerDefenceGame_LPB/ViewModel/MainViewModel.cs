using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceGame_LPB.Model;

namespace TowerDefenceGame_LPB.ViewModel
{
    public abstract class MainViewModel : ViewModelBase
    {
        public int GridSizeX { get; set; }
        public int GridSizeY { get; set; }
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<OptionField> OptionFields { get; set; }
        public abstract void ButtonClick(int index);
    }
}
