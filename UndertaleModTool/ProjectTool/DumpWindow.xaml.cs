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
		MainWindow MainWindow = Application.Current.MainWindow as MainWindow;

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
            Dump.BasePath = MainWindow.PromptChooseDirectory();
            if (Dump.BasePath == null)
            {
                Close();
				MainWindow.DumpEnd();
				Dump.Log("Canceled");
				return;
            }

			Close();
			MainWindow.StartProgressBarUpdater();

			try
			{
				await Task.Run(() => Dump.Start());

				MainWindow.SetUMTConsoleText("");

				bool openInExplorer = Dump.YesNoQuestion("Done. Open folder?");
				if (openInExplorer)
					Dump.OpenInExplorer();
			}
			catch (Exception ex)
			{
				MainWindow.ScriptError($"{ex.Message}\n\n---\n\n{ex.ToString()}");
				MainWindow.SetUMTConsoleText(ex.ToString());
			}

			await MainWindow.StopProgressBarUpdater();
			MainWindow.HideProgressBar();

			MainWindow.DumpEnd();
		}
    }
}
