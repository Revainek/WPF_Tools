using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_ExternalAutoCADController.AutoCAD;

namespace WPF_ExternalAutoCADController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AutoCADOperationClass _cadOp = new AutoCADOperationClass();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SimpleAsThat_btnClick(object sender, RoutedEventArgs e)
        {
            _cadOp.GetAcad();
        }
    }
}
