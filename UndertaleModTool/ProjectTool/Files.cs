using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModTool.ProjectTool
{
	public static class Files
	{
		public static List<DumpFile> FileList = new();
		
		public static DumpFile PROGRAM_EXE;
		public static DumpFile SPLASH_PNG;

		/// <summary>
		/// Represents a file inside of where the data.win is also located
		/// To copy them over as an included file, extension files, or deal with the exe
		/// </summary>
		public class DumpFile
		{
			public DumpFile(string path)
			{
				FullPath = path;
			}
			public bool Included { get; set; } = true; // Whether to treat as an included file
			public string FullPath { get; set; }
			public string Name => Path.GetFileNameWithoutExtension(FullPath);
			public string NameExt => Path.GetFileName(FullPath);
			public string Directory => Path.GetDirectoryName(FullPath);
			public string RelativeDirectory
			{
				get
				{
					string path = Path.GetDirectoryName(FullPath);
					path = path.Replace(Dump.Options.data_filedir, "", StringComparison.CurrentCultureIgnoreCase);
					path = path.TrimStart('\\');
					return path;
				}
			}
			public string Extension => Path.GetExtension(FullPath);
		}

		public static void Init()
		{
			FileList.Clear();
			bool retarded = false;

			foreach (var source in Directory.GetFiles(Dump.Options.data_filedir, "*", SearchOption.AllDirectories))
			{
				DumpFile target = new(source);

				if (target.FullPath == Dump.Options.data_filename)
					target.Included = false; // data.win

				if (target.Directory == Dump.Options.data_filedir)
				{
					if (Dump.Options.data_do_exe && target.Extension == ".exe" && File.ReadAllLines(source).Last().Contains("YoYoGames.GameMaker.Runner"))
					{
						if (PROGRAM_EXE != null)
						{
							// User has multiple gamemaker runners in the same fucking folder
							retarded = true;
							PROGRAM_EXE = null;
						}
						if (!retarded)
							PROGRAM_EXE = target;
						target.Included = false;
					}

					if (target.NameExt == "options.ini") // Could be wrong but genuinely who puts their own custom options.ini in the root folder
						target.Included = false;

					if (target.NameExt == "splash.png")
					{
						SPLASH_PNG = target;
						target.Included = false;
					}
				}

				if (target.FullPath.Contains("donotdumpthis"))
					target.Included = false;

				FileList.Add(target);
			}
		}

		public static void Save()
		{
			if (!Dump.Options.asset_includedfiles)
				return;

			string path = Dump.RelativePath("datafiles");
			Directory.CreateDirectory(path);

			foreach (var file in FileList.Where(i => i.Included))
			{
				string filePath = path + "/" + file.RelativeDirectory;
				if (file.RelativeDirectory != "")
					Directory.CreateDirectory(filePath);
				File.Copy(file.FullPath, filePath + "/" + file.NameExt, true);
			}
		}

		/// <summary>
		/// Find an included file by its extensionless name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DumpFile ByName(string name) => FileList.FirstOrDefault(i => i.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
	}
}
