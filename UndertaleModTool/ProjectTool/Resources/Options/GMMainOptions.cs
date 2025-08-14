using System;
using System.IO;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Options
{
	public class GMMainOptions : ResourceBase, ISaveable
	{
		public GMMainOptions()
		{
			resourceVersion = "1.4";
			name = "Main";
		}

		public string option_gameguid { get; set; }
		public string option_gameid { get; set; } = "0";
		public int option_game_speed { get; set; } = 60;
		public bool option_mips_for_3d_textures { get; set; } = false;
		public uint option_draw_colour { get; set; } = 4294967295;
		public uint option_window_colour { get; set; } = 255;
		public string option_steam_app_id { get; set; } = "0";
		public bool option_sci_usesci { get; set; } = false;
		public string option_author { get; set; } = "";
		public bool option_collision_compatibility { get; set; } = false;
		public bool option_copy_on_write_enabled { get; set; } = false;
		public bool option_spine_licence { get; set; } = false;
		public string option_template_image { get; set; } = "${base_options_dir}/main/template_image.png";
		public string option_template_icon { get; set; } = "${base_options_dir}/main/template_icon.png";
		public string option_template_description { get; set; } = null;

		public GMMainOptions(UndertaleData source) : this()
		{
			option_gameguid = new Guid(source.GeneralInfo.GMS2GameGUID).ToString();
			option_game_speed = (int)source.GeneralInfo.GMS2FPS; // Why is this a float???

			foreach (var i in TpageAlign.Get3DTextures())
			{
				if (i.Value.TexturePages.Count == 0)
					continue;
				if (i.Value.TexturePages[0].Resource.GeneratedMips > 0)
				{
					option_mips_for_3d_textures = true;
					break;
				}
			}

			option_draw_colour = Constants.DRAW_COLOUR;
			option_window_colour = source.Options.WindowColor;
			option_author = "My Dumpy";

			option_collision_compatibility = source.Options.Info.HasFlag(UndertaleOptions.OptionsFlags.FastCollisionCompatibility);
			option_copy_on_write_enabled = source.Options.Info.HasFlag(UndertaleOptions.OptionsFlags.EnableCopyOnWrite);
		}

		public void Save(string rootFolder = null)
		{
			if (rootFolder == null)
				rootFolder = Dump.RelativePath("options/main");

			Directory.CreateDirectory(rootFolder);
			Dump.ToJsonFile(rootFolder + "/options_main.yy", this);
		}
	}
}
