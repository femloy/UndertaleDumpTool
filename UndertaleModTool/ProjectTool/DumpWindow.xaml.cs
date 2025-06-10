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
        Dump dump;

        public DumpWindow(Dump dump)
        {
            InitializeComponent();
            this.dump = dump;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TEMPORARY
            Start();
        }

        private void Start()
        {
            dump.BasePath = MainWindow.Get().PromptChooseDirectory();
            if (dump.BasePath == null)
            {
                Close();
                return;
            }

            Close();
            dump.Start();
        }
    }
}
