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
		MainWindow _mainWindow = Application.Current.MainWindow as MainWindow; // Fucking gross fuck this fucking shit.
		bool _doEndOnClosed = true;

        public DumpWindow()
        {
            InitializeComponent();
			DataContext = Dump.Options;
        }

		private async Task Start()
		{
			Dump.BasePath = _mainWindow.PromptChooseDirectory();
			if (Dump.BasePath == null)
				return;

			_doEndOnClosed = false;
			Close();

			try
			{
				_mainWindow.StartProgressBarUpdater();
				await Task.Run(Dump.Current.Start);
				await _mainWindow.StopProgressBarUpdater();
				_mainWindow.HideProgressBar();
				_mainWindow.SetUMTConsoleText("");

				if (Dump.YesNoQuestion("Done. Open folder?"))
					Dump.OpenInExplorer();
			}
			catch (Exception ex)
			{
				_mainWindow.ScriptError($"{ex.Message}\n\n---\n\n{ex.ToString()}");
				_mainWindow.SetUMTConsoleText(ex.ToString());
				await _mainWindow.StopProgressBarUpdater();
				_mainWindow.HideProgressBar();
			}
			finally
			{
				_mainWindow.DumpEnd();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (_doEndOnClosed)
				_mainWindow.DumpEnd();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			_ = Start();
		}
	}
}
