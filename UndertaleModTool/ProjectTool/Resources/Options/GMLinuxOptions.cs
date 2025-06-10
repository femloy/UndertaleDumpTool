using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModTool.ProjectTool.Resources.Options
{
	public class GMLinuxOptions : ResourceBase
	{
		public GMLinuxOptions()
		{
			name = "Linux";
		}

		public enum Scaling
		{
			KeepAspectRatio,
			FullScale
		}

		public string option_linux_display_name { get; set; } = "Created with GameMaker";
		public string option_linux_version { get; set; } = "1.0.0.0";
		public string option_linux_maintainer_email { get; set; } = "";
		public string option_linux_homepage { get; set; } = "http://www.yoyogames.com";
		public string option_linux_short_desc { get; set; } = "";
		public string option_linux_long_desc { get; set; } = "";
		public string option_linux_splash_screen { get; set; } = "${base_options_dir}/linux/splash/splash.png";
		public bool option_linux_display_splash { get; set; } = false;
		public string option_linux_icon { get; set; } = "${base_options_dir}/linux/icons/64.png";
		public bool option_linux_start_fullscreen { get; set; } = false;
		public bool option_linux_allow_fullscreen { get; set; } = false;
		public bool option_linux_interpolate_pixels { get; set; } = true;
		public bool option_linux_display_cursor { get; set; } = true;
		public bool option_linux_sync { get; set; } = false;
		public bool option_linux_resize_window { get; set; } = false;
		public Scaling option_linux_scale { get; set; } = Scaling.KeepAspectRatio;
		public string option_linux_texture_page { get; set; } = "2048x2048";
		public bool option_linux_enable_steam { get; set; } = false;
		public bool option_linux_disable_sandbox { get; set; } = false;
	}
}
