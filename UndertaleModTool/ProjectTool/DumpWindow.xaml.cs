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
using System.Windows.Shapes;
using UndertaleModLib.Scripting;

namespace UndertaleModTool.ProjectTool
{
    /// <summary>
    /// Interaction logic for DumpWindow.xaml
    /// </summary>
    public partial class DumpWindow : Window
    {
        Dump dump = new Dump();
        MainWindow w = Dump.GetMainWindow();

        public DumpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            w.SetUMTConsoleText("");

            Start(); // remove this
        }

        private void Start()
        {
            Close();

            // Configure
            dump.DoSprites = true;

            // Do it
            dump.Start();
        }
    }
}
