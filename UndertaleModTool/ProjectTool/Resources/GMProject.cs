using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	[Flags]
	public enum GMTarget : long
	{
		None = 0L,
		All = -1L,
		AllIncludedFile = Windows | macOS | iOS | Android | HTML5 | Ubuntu | PS4 | PS5 | Xbox | Switch | tvOS | GXgames,

		Windows = 64L,
		macOS = 2L,
		iOS = 4L,
		Android = 8L,
		HTML5 = 32L,
		Ubuntu = 128L,
		PS4 = 4294967296L,
		PS5 = 576460752303423488L,
		Xbox = 2305843009213693952L,
		Switch = 144115188075855872L,
		tvOS = 9007199254740992L,
		GXgames = 17179869184L

		// There are gaps in your logic, YoYo Game
	}

	public class GMProject : ResourceBase
	{
		public GMProject() : base()
		{
			resourceVersion = "1.6";
		}

		public class Resource
		{
			public Resource(IdPath id, uint order)
			{
				this.id = id;
				this.order = order;
			}

			public IdPath id { get; set; }
			public uint order { get; set; }
		}
		public class Config
		{
			public string name { get; set; } = "Default";
			public List<Config> children { get; set; } = new();
		}
		public class RoomOrderNode
		{
			// Most useless fucking class. Thanks, Little YoYo !
			public IdPath roomId { get; set; }
		}
		public class MetaDataClass
		{
			// Probably just a Dictionary<string, string> but who give shit Truly?
			public string IDEVersion { get; set; } = "2022.0.3.85";
		}
		public enum ScriptType
		{
			Ask,
			GML,
			DND
		}

		public List<Resource> resources { get; set; } = new();
		public List<IdPath> Options { get; set; } = new(); // Little YoYo why is this one capitalized and nothing else Little YoYo. I Hate You YoYo Game
		public ScriptType defaultScriptType { get; set; } = ScriptType.Ask;
		public bool isEcma { get; set; } = false;
		public Config configs { get; set; } = new(); // If configs is something other than Default, add that to the children (no way to know the rest though)
		public List<RoomOrderNode> RoomOrderNodes { get; set; } = new();
		public List<GMFolder> Folders { get; set; } = new();
		public List<GMAudioGroup> AudioGroups { get; set; } = new();
		public List<GMTextureGroup> TextureGroups { get; set; } = new();
		public List<GMIncludedFile> IncludedFiles { get; set; } = new();
		public MetaDataClass MetaData { get; set; } = new();

		public void AddResource(string name, string folder)
		{
			if (Dump.Options.project_sort_assets && Dump.ResourceIndexByName(name, out int id))
				_resourceOrder = (uint)id;
			resources.Add(new Resource(new IdPath(name, $"{folder}/{name}/{name}.yy"), _resourceOrder++));
		}
		private uint _resourceOrder = 0;

		/// <summary>
		/// Create GMProject out of UndertaleData
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public GMProject(UndertaleData source) : this()
		{
			name = source.GeneralInfo.Name.Content;

			if (source.GeneralInfo.Config.Content != "Default")
				configs.children.Add(new Config() { name = source.GeneralInfo.Config.Content });

			foreach (var asset in Dump.ProjectResources)
				AddResource(asset.Key, asset.Value);

			if (Dump.Options.project_sort_assets)
				resources = resources.OrderBy(i => i.order).ToList();

			#region Folders

			foreach (var item in _baseFolders)
				Folders.Add(new GMFolder(item) { order = Folders.Count });
			foreach (var item in Dump.ProjectFolders.Distinct())
				Folders.Add(new GMFolder(item) { order = Folders.Count });

			#endregion
			#region Options

			if (Dump.Options.project_options)
			{
				Options.Add(new IdPath("Main", "options/main/options_main.yy"));
				Options.Add(new IdPath("Windows", "options/windows/options_windows.yy"));

				if (Dump.Options.options_other_platforms)
				{
					Options.Add(new IdPath("macOS", "options/mac/options_mac.yy"));
					Options.Add(new IdPath("Linux", "options/linux/options_linux.yy"));
					Options.Add(new IdPath("HTML5", "options/html5/options_html5.yy"));
					Options.Add(new IdPath("Android", "options/android/options_android.yy"));
					Options.Add(new IdPath("iOS", "options/ios/options_ios.yy"));
					Options.Add(new IdPath("tvOS", "options/tvos/options_tvos.yy")); // Yoyo why?
					Options.Add(new IdPath("operagx", "options/operagx/options_operagx.yy"));
				}
			}

			#endregion
			#region Room Order

			if (Dump.Options.asset_rooms)
			{
				foreach (var node in source.GeneralInfo.RoomOrder)
				{
					string nodeName = node.Resource.Name.Content;
					RoomOrderNodes.Add(new RoomOrderNode() { roomId = IdPath.From(node.Resource) });
				}
			}

			#endregion
			#region Texture Groups

			if (Dump.Options.project_texturegroups)
			{
				foreach (var group in TpageAlign.TextureGroups)
					TextureGroups.Add(new GMTextureGroup(group));
				if (TpageAlign.ConsoleGroup)
					TextureGroups.Add(new GMTextureGroup() { name = TpageAlign.CONSOLE_GROUP_NAME, targets = GMTarget.None });
			}
			else
				TextureGroups.Add(new GMTextureGroup() { name = "Default" });

			#endregion
			#region Audio Groups

			if (Dump.Options.project_audiogroups)
			{
				foreach (var group in source.AudioGroups)
					AudioGroups.Add(new GMAudioGroup(group));
			}
			else
				AudioGroups.Add(new GMAudioGroup() { name = "audiogroup_default" });

			#endregion
			#region Included Files

			if (Dump.Options.project_datafiles)
			{
				foreach (var file in Files.FileList.Where(i => i.Included))
					IncludedFiles.Add(new GMIncludedFile(file));
			}

			#endregion
		}

		private readonly string[] _baseFolders =
		{
			"Sprites",
			"Tile Sets",
			"Sounds",
			"Paths",
			"Scripts",
			"Shaders",
			"Fonts",
			"Timelines",
			"Objects",
			"Rooms",
			"Sequences",
			"Animation Curves",
			"Notes",
			"Extensions"
		};

		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath("");
			Dump.ToJsonFile(rootFolder + $"/{name}.yyp", this);
		}
	}

	public class GMFolder : ResourceBase
	{
		public GMFolder(string nameAndPath) : base()
		{
			name = Path.GetFileNameWithoutExtension(nameAndPath);
			folderPath = $"folders/{nameAndPath}.yy";
		}

		public string folderPath { get; set; }
		public int order { get; set; }
	}

	public class GMAudioGroup : ResourceBase
	{
		public GMAudioGroup()
		{
			resourceVersion = "1.3";
		}

		public GMAudioGroup(UndertaleAudioGroup source) : this()
		{
			name = source.Name.Content;

			if (!GMSound.UsedAudioGroups.Contains(source.Name.Content))
				targets = GMTarget.None;
		}

		public GMTarget targets { get; set; } = GMTarget.All;
	}

	public class GMIncludedFile : ResourceBase
	{
		public GMTarget CopyToMask { get; set; } = GMTarget.AllIncludedFile;
		public string filePath { get; set; } = "datafiles";

		public GMIncludedFile(Files.DumpFile source)
		{
			name = source.NameExt;
			filePath = source.RelativeDirectory;

			if (filePath != "")
				filePath = "datafiles/" + filePath;
			else
				filePath = "datafiles";

			switch (source.Extension.ToLower())
			{
				case ".dll":
					CopyToMask = GMTarget.Windows | GMTarget.Xbox;
					break;
				case ".dylib":
					CopyToMask = GMTarget.macOS;
					break;
				case ".so":
					CopyToMask = GMTarget.Ubuntu;
					break;
			}
		}
	}
}
