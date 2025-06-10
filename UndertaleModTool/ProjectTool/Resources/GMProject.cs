using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModTool.ProjectTool.Resources
{
	[Flags]
	public enum GMTarget : long
	{
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
		public GMProject()
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

		public enum ScriptType
		{
			Ask,
			GML,
			DND
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

		public List<Resource> resources { get; set; } = new();
		public List<IdPath> Options { get; set; } = new(); // Little YoYo why is this one capitalized and nothing else Little YoYo. I Hate You YoYo Game
		public ScriptType defaultScriptType { get; set; } = ScriptType.Ask;
		public bool isEcma { get; set; } = false;
		public Config configs { get; set; } = new(); // If configs is something other than Default, add that to the children (no way to know the rest though)
		public List<RoomOrderNode> RoomOrderNodes { get; set; } = new();
		public List<GMFolder> Folders { get; set; } = new();
		public List<GMAudioGroup> AudioGroups { get; set; } = new();
		public List<GMTextureGroup> TextureGroups { get; set; } = new();
		public List<IdPath> IncludedFiles { get; set; } = new();
		public MetaDataClass MetaData { get; set; } = new();
	}

	public class GMFolder : ResourceBase
	{
		public string folderPath { get; set; }
		public uint order { get; set; }

		/// <summary>
		/// Create a GMFolder out of a path
		/// </summary>
		/// <param name="nameAndPath">Something like "Sprites/folder" or just "folder"</param>
		/// <returns></returns>
		public static GMFolder From(string nameAndPath)
		{
			var folder = new GMFolder();
			folder.name = Path.GetFileNameWithoutExtension(nameAndPath);
			folder.folderPath = $"folders/{nameAndPath}.yy";
			return folder;
		}
	}

	public class GMAudioGroup : ResourceBase
	{
		public GMAudioGroup()
		{
			resourceVersion = "1.3";
		}

		public GMTarget targets { get; set; } = GMTarget.All;
	}

	public class GMIncludedFile : ResourceBase
	{
		public GMTarget CopyToMask { get; set; } = GMTarget.AllIncludedFile;
		public string filePath { get; set; } = "datafiles";
	}
}
