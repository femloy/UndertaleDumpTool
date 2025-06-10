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
        Dump Dump;

        public DumpWindow(Dump dump)
        {
            InitializeComponent();
            Dump = dump;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
			// TEMPORARY
			Dump.Options.data_filename = Dump.MainWindow.FilePath;
			_ = Start();
        }

        private async Task Start()
        {
            Dump.BasePath = Dump.MainWindow.PromptChooseDirectory();
            if (Dump.BasePath == null)
            {
                Close();
				Dump.Log("Canceled");
                return;
            }

			Close();
			await Dump.Start();
		}
    }
}
