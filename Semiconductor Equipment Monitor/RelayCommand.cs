using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Semiconductor_Equipment_Monitor
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action _execute;//Action委托
        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;
        

        public void Execute(object parameter)
        {
           _execute();
        }
    }
}
