using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UI.ViewModel;

namespace UI
{
    public partial class MainPage : UserControl
    {
        public MainPageVm Vm { get; private set; }

        public MainPage(MainPageVm vm)
        {
            Vm = vm;
            System.Diagnostics.Debug.WriteLine("Init");
            InitializeComponent();
        }
    }
}
