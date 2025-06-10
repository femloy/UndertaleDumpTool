using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace UndertaleModTool.ProjectTool.Resources.GMOptions
{
	public class GMWindowsOptions : ResourceBase
	{
		public GMWindowsOptions()
		{
			name = "Windows";
			resourceVersion = "1.1";
		}

		public enum SaveLocation
		{
			LocalAppData,
			AppData
		}
		public enum Scaling
		{
			KeepAspectRatio,
			FullScale
		}

		public string option_windows_display_name { get; set; } = "Created with GameMaker";
		public string option_windows_executable_name { get; set; } = "${project_name}.exe";
		public string option_windows_version { get; set; } = "1.0.0.0";
		public string option_windows_company_info { get; set; } = "YoYo Games Ltd";
		public string option_windows_product_info { get; set; } = "Created with GameMaker";
		public string option_windows_copyright_info { get; set; } = "";
		public string option_windows_description_info { get; set; } = "A GameMaker Game";
		public bool option_windows_display_cursor { get; set; } = true;
		public string option_windows_icon { get; set; } = "${base_options_dir}/windows/icons/icon.ico";
		public SaveLocation option_windows_save_location { get; set; } = SaveLocation.LocalAppData;
		public string option_windows_splash_screen { get; set; } = "${base_options_dir}/windows/splash/splash.png";
		public bool option_windows_use_splash { get; set; } = false;
		public bool option_windows_start_fullscreen { get; set; } = false;
		public bool option_windows_allow_fullscreen_switching { get; set; } = false;
		public bool option_windows_interpolate_pixels { get; set; } = false;
		public bool option_windows_vsync { get; set; } = false;
		public bool option_windows_resize_window { get; set; } = false;
		public bool option_windows_borderless { get; set; } = false;
		public Scaling option_windows_scale { get; set; } = Scaling.KeepAspectRatio;
		public bool option_windows_copy_exe_to_dest { get; set; } = false;
		public int option_windows_sleep_margin { get; set; } = 10;
		public string option_windows_texture_page { get; set; } = "2048x2048";
		public string option_windows_installer_finished { get; set; } = "${base_options_dir}/windows/installer/finished.bmp";
		public string option_windows_installer_header { get; set; } = "${base_options_dir}/windows/installer/header.bmp";
		public string option_windows_license { get; set; } = "${base_options_dir}/windows/installer/license.txt";
		public string option_windows_nsis_file { get; set; } = "${base_options_dir}/windows/installer/nsis_script.nsi";
		public bool option_windows_enable_steam { get; set; } = false;
		public bool option_windows_disable_sandbox { get; set; } = false;
		public bool option_windows_steam_use_alternative_launcher { get; set; } = false;
	}
}
