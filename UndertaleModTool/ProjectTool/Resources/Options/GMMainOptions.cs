using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib;

namespace UndertaleModTool.ProjectTool.Resources.Options
{
	public class GMMainOptions : ResourceBase
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		public GMMainOptions(UndertaleData source) : this()
		{
			// TODO
		}

		public GMMainOptions Save()
		{
			return this;
		}
	}
}
