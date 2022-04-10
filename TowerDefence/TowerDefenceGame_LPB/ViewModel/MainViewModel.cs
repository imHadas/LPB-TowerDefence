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
        private int gridSizeX;
        private int gridSizeY;

        public int GridSizeX
        {
            get { return gridSizeX;}
            set { gridSizeX = value;OnPropertyChanged(); }
        }
        public int GridSizeY
        {
            get { return gridSizeY;}
            set { gridSizeY = value;OnPropertyChanged(); }
        }
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<OptionField> OptionFields { get; set; }
        public abstract void ButtonClick();

    }
}
