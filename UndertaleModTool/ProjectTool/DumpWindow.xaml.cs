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
		MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
		bool endDumpOnClose = true;

        public DumpWindow()
        {
            InitializeComponent();
			DataContext = Dump.Options;
        }

        private async Task Start()
        {
            Dump.Current.BasePath = MainWindow.PromptChooseDirectory();
            if (Dump.Current.BasePath == null)
				return;

			endDumpOnClose = false;
			Close();
			MainWindow.StartProgressBarUpdater();

			try
			{
				await Task.Run(() => Dump.Current.Start());

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

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (endDumpOnClose)
				MainWindow.DumpEnd();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			_ = Start();
		}
	}
}
