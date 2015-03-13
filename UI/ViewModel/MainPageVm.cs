using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Igniter;

namespace UI.ViewModel
{
    public class MainPageVm : ViewModelBase
    {
        private int _myProperty = -1;
        public int MyProperty
        {
            get { return _myProperty; }
            set { SetProperty(ref _myProperty, value); }
        }

        private int _myProperty2 = -1;
        public int MyProperty2
        {
            get { return _myProperty2; }
            set { SetProperty(ref _myProperty2, value); }
        }



        private DelegateCommand _randomizeCommand;
        public ICommand RandomizeCommand { get { return _randomizeCommand; } }

        private readonly Telerik.Windows.Controls.DelegateCommand _randomize2Command;
        public ICommand Randomize2Command { get { return _randomize2Command; } }

        public MainPageVm()
        {
            _randomizeCommand = new DelegateCommand(Randomize);
            _randomize2Command = new Telerik.Windows.Controls.DelegateCommand(Randomize);
        }


        private void Randomize()
        {
            MyProperty = (new Random()).Next(100);
            Revalidate();
        }
        private void Randomize(object o)
        {
            MyProperty = (new Random()).Next(100);
            MyProperty2 = (new Random()).Next(100);
            Revalidate();
        }

        private void Revalidate()
        {
            var isLowerPart = MyProperty < 50;
            _randomizeCommand.SetCanExecute(isLowerPart);
            //_randomize2Command.
        }

        private void Stub()
        {
            var x = this.GetType();
            var p = x.GetProperties();
            
        }
    }
}
