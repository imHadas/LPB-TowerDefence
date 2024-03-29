﻿using System;
using System.Windows.Input;

namespace TowerDefenceBackend.ViewModel
{
    public class DelegateCommand : ICommand
    {
        #region Fields
        private readonly Action<Object> _execute;
        private readonly Func<Object, Boolean> _canExecute;
        #endregion

        #region Commands
        public DelegateCommand(Action<Object> execute) : this(null, execute) { }

        public DelegateCommand(Func<Object, Boolean> canExecute, Action<Object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region Event(s)
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Public methods
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(Object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled.");
            }
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
