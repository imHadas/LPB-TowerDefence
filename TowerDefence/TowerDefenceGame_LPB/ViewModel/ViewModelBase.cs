using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TowerDefenceBackend.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Constructor(s)
        protected ViewModelBase() { }
        #endregion

        #region Event(s)
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Protected method(s)
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
