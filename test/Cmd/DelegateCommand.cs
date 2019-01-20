using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace test.Cmd
{
    public abstract class DelegateCommand : ICommand
    {

        public event EventHandler CanExecuteChanged;

        protected ViewModel _viewModel;

        public DelegateCommand( ViewModel viewModel )
        {
            _viewModel = viewModel;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
        public virtual void RaiseCanExecuteChanged(object obj, EventArgs eventArgs)
        {
            RaiseCanExecuteChanged();
        }
        public virtual void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
