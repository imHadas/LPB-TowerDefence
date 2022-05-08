using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefenceBackend.Model;

namespace TowerDefenceBackend.ViewModel
{
    public abstract class MainViewModel : ViewModelBase
    {
        #region Variables
        private int gridSizeX;
        private int gridSizeY;
        #endregion

        #region Properties
        public int GridSizeX
        {
            get { return gridSizeX; }
            set { gridSizeX = value; OnPropertyChanged(); }
        }
        public int GridSizeY
        {
            get { return gridSizeY; }
            set { gridSizeY = value; OnPropertyChanged(); }
        }
        public ObservableCollection<FieldViewModel> Fields { get; set; }
        public ObservableCollection<OptionField> OptionFields { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Abstract method for setting up the sidebar menu when a button is clicked on the board
        /// </summary>
        public abstract void ButtonClick();
        /// <summary>
        /// Abstract method for executing the sidebar menu option
        /// </summary>
        /// <param name="option">Selected option</param>
        public abstract void OptionsButtonClick(string option);
        #endregion

    }
}
