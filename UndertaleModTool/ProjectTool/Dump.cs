using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using UndertaleModTool.ProjectTool.Resources;
using UndertaleModTool.ProjectTool.Resources.GMOptions;
using UndertaleModTool.ProjectTool.Resources.Options;

namespace UndertaleModTool.ProjectTool
{
    public partial class Dump
    {
        public string BasePath;
		private DumpOptions _options = new();
        TextureWorker _worker = new();
		private GMProject _project = new();


        public async Task Start()
        {
			MainWindow.StartProgressBarUpdater();

			if (Options.asset_project)
				_project = new GMProject(Data).Save();

			if (Options.asset_options)
			{
				new GMMainOptions(Data).Save();
				new GMWindowsOptions(Data).Save();

				if (Options.options_other_platforms)
				{
					// TODO
				}
			}

			if (Options.asset_texturegroups)
				TpageAlign.Init();

			if (Options.asset_sprites)
			{
				MainWindow.SetProgressBar(null, "Sprites", 0, Data.Sprites.Count);

				await Task.Run(() => Parallel.ForEach(Data.Sprites, (sprite) =>
				{
					Log($"{sprite.Name.Content}");		// Progress bars are still as unreliable and undocumented as ever
					//new GMSprite(sprite).Save();
					MainWindow.IncrementProgressParallel();
				}));
			}

			await MainWindow.StopProgressBarUpdater();	// You know I wrote this to look better than the old one...
			MainWindow.HideProgressBar();

			bool openInExplorer = YesNoQuestion("Done. Open folder?");
			if (openInExplorer)
				OpenInExplorer();

			MainWindow.DumpEnd();
		}

        public void Dispose()
        {
            _worker.Dispose();
            _worker = null;

            GC.SuppressFinalize(this);
        }

		public static void AddFolder(string nameAndPath)
		{
			var project = Current?._project;
			if (project == null) return;
			project.Folders.Add(new GMFolder(nameAndPath) { order = project.Folders.Count });
		}

		public static MainWindow MainWindow;
        public static Dump Current;
        public static TextureWorker TexWorker => Current?._worker;
		public static DumpOptions Options => Current?._options;
    }
}
