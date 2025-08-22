using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Underanalyzer.Decompiler;
using UndertaleModLib;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
using UndertaleModTool.ProjectTool.Resources;

namespace UndertaleModTool.ProjectTool
{
    public partial class Dump
    {
		private DumpOptions _options;
        TextureWorker _worker = new();
		private string _basePath;

		public static Dump Current;
		public static TextureWorker TexWorker => Current?._worker;
		public static GlobalDecompileContext DecompileContext;
		public static DecompileSettings DecompilerSettings;
		public static DumpOptions Options => Current?._options;
		public static string BasePath { get => Current?._basePath; set => Current._basePath = value; }
		public static Dictionary<string, string> ProjectResources = new();

		public Dump()
		{
			_options = new();
			ProjectResources.Clear();

			DecompilerSettings = new DecompileSettings()
			{
				IndentString = "	",
				CreateEnumDeclarations = false,
				UnknownEnumName = "@\r@",
				UnknownEnumValuePattern = "int64({0})",
				UnknownArgumentNamePattern = "_argument{0}",
				RemoveSingleLineBlockBraces = true,
				EmptyLineBeforeSwitchCases = true,
				EmptyLineAroundBranchStatements = true
			};
			DecompilerSettings.MultiPartPredefinedDoubles = new()
			{
				{ 6.283185307179586, "pi * 2" },
				{ 12.566370614359172, "pi * 4" },
				{ 31.41592653589793, "pi * 10" },
				{ 1.5707963267948966, "pi / 2" },
				{ 0.3333333333333333, "1 / 3" },
				{ 0.6666666666666666, "2 / 3" },
				{ 1.3333333333333333, "4 / 3" },
				{ 23.333333333333332, "70 / 3" },
				{ 73.33333333333333, "220 / 3" },
				{ 206.66666666666666, "620 / 3" },
				{ 51.42857142857143, "360 / 7" },
				{ 1.0909090909090908, "12 / 11" },
				{ 0.06666666666666667, "1 / 15" },
				{ 0.9523809523809523, "20 / 21" },
				{ 0.03333333333333333, "1 / 30" },
				{ 0.008333333333333333, "1 / 120" }
			};
		}

		public static string DumpCode(UndertaleCode code)
		{
			string codeString = new DecompileContext(DecompileContext, code, DecompilerSettings).DecompileToString().Trim();
			codeString = codeString.Replace("@\r@.", "/*enum*/");

			// Code tweaks
			if (codeString == "exit;")
				codeString = "";

			if (codeString != "")
				codeString += "\n";

			return codeString;
		}

		public async Task DumpAsset<A, B>(string name, IList<B> list) where A : ISaveable where B : UndertaleNamedResource
		{
			SetupProgress(name, list.Count);

			A.Init();

			await Task.Run(() => Parallel.ForEach(list, (source, state, index) =>
			{
				UpdateStatus(source.Name.Content);

				var asset = (A)Activator.CreateInstance(typeof(A), new object[] { source });
				if (asset.IsValid())
					asset.Save();

				IncrementProgress();
			}));

			A.End();
		}

		public static string NonconflictingAssetName(string name)
		{
			while (Data.ByName(name, true) is not null)
				name += $"_new";
			return name;
		}

        public async Task Start()
        {
			if (Data is null)
				throw new NullReferenceException("Data is null");
			if (Options.data_filename is null)
				throw new NullReferenceException("Data filename is null");

			DecompileContext = new(Data);

			Files.Init();
			Constants.Init();
			TpageAlign.Clear();

			if (Options.asset_texturegroups)
				TpageAlign.Init();

			if (Options.asset_options)
			{
				new Resources.Options.GMMainOptions(Data).Save();
				new Resources.Options.GMWindowsOptions(Data).Save();

				if (Options.options_other_platforms)
				{
					// TODO
				}
			}

			if (Options.asset_shaders)
				await DumpAsset<GMShader, UndertaleShader>("Shaders", Data.Shaders);
			if (Options.asset_sounds)
				await DumpAsset<GMSound, UndertaleSound>("Sounds", Data.Sounds);
			if (Options.asset_scripts)
				await DumpAsset<GMScript, UndertaleScript>("Scripts", Data.Scripts);
			if (Options.asset_objects)
				await DumpAsset<GMObject, UndertaleGameObject>("Objects", Data.GameObjects);
			if (Options.asset_sprites)
				await DumpAsset<GMSprite, UndertaleSprite>("Sprites", Data.Sprites);

			if (Options.asset_project)
				new GMProject(Data).Save();

			if (Options.asset_includedfiles)
				Files.Save();
		}

        public void Dispose()
        {
            _worker.Dispose();
            _worker = null;

            GC.SuppressFinalize(this);
        }
    }
}
