using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using UndertaleModTool.ProjectTool.Resources;

namespace UndertaleModTool.ProjectTool
{
    public partial class Dump
    {
        public string BasePath;
		private DumpOptions _options = new();
        TextureWorker _worker = new();
		private GMProject _project = new();

        public void Start()
        {
			if (Options.asset_project)
				_project = new GMProject(MainWindow.Get().Data).Save();

			if (Options.asset_texturegroups)
				TpageAlign.Init();

            if (Options.asset_sprites)
                new GMSprite(MainWindow.Get().Selected as UndertaleSprite)?.Save(); // temporary. still figuring it out Pal.

            Log("Done");
        }

        public void Dispose()
        {
            _worker.Dispose();
            _worker = null;

            GC.SuppressFinalize(this);
        }

		public static void AddFolder(string nameAndPath)
		{
			var project = Get()?._project;
			if (project == null) return;
			project.Folders.Add(new GMFolder(nameAndPath) { order = project.Folders.Count });
		}

        public static Dump Get() => MainWindow.Get().Dump;
        public static TextureWorker TexWorker => Get()?._worker;
		public static DumpOptions Options => Get()?._options;
    }
}
