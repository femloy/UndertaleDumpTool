using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using UndertaleModLib;
using UndertaleModTool.ProjectTool.Resources.Options;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.GMOptions
{
	public class GMWindowsOptions : ResourceBase, ISaveable
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

		public GMWindowsOptions(UndertaleData source) : this()
		{
			option_windows_display_name = source.GeneralInfo.DisplayName.Content;

			if (Files.PROGRAM_EXE != null)
			{
				option_windows_executable_name = Files.PROGRAM_EXE.NameExt.Replace(source.GeneralInfo.Name.Content, "${project_name}");

				var info = FileVersionInfo.GetVersionInfo(Files.PROGRAM_EXE.FullPath);
				option_windows_version = info.FileVersion ?? option_windows_version;
				option_windows_company_info = info.CompanyName ?? option_windows_company_info;
				option_windows_product_info = info.ProductName ?? option_windows_product_info;
				option_windows_copyright_info = info.LegalCopyright ?? option_windows_copyright_info;
				option_windows_description_info = info.FileDescription ?? option_windows_description_info;

				// TODO: exe icon
			}

			// Options flags (DEFAULT): UseNewAudio, ShowCursor, VariableErrors
			// Options flags (INVERSE): FullScreen, InterpolatePixels, UseNewAudio, Sizeable, ScreenKey, VariableErrors, FastCollisionCompatibility, DisableSandbox, EnableCopyOnWrite
			// Generalinfo flags (DEFAULT): Scale, ShowCursor
			// Generalinfo flags (INVERSE): Fullscreen, SyncVertex1, Interpolate, Sizeable, ScreenKey, LocalDataEnabled, BorderlessWindow

			option_windows_display_cursor = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.ShowCursor);
			
			if (!source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.LocalDataEnabled)) // This is the opposite...?
				option_windows_save_location = SaveLocation.LocalAppData;

			if (Files.SPLASH_PNG != null)
			{
				option_windows_use_splash = true;
				option_windows_splash_screen = "splash/splash.png";
			}

			option_windows_start_fullscreen = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.Fullscreen);
			option_windows_allow_fullscreen_switching = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.ScreenKey); // "???" thanks again utmt devs
			option_windows_interpolate_pixels = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.Interpolate);
			option_windows_vsync = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.SyncVertex1) || source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.SyncVertex2);
			option_windows_resize_window = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.Sizeable);
			option_windows_borderless = source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.BorderlessWindow);

			if (!source.GeneralInfo.Info.HasFlag(UndertaleGeneralInfo.InfoFlags.Scale))
				option_windows_scale = Scaling.FullScale;

			option_windows_sleep_margin = Constants.SLEEP_MARGIN;
			option_windows_texture_page = $"{TpageAlign.TexturePageSize}x{TpageAlign.TexturePageSize}";
			option_windows_disable_sandbox = source.Options.Info.HasFlag(UndertaleOptions.OptionsFlags.DisableSandbox);
		}

		public void Save(string rootFolder = null)
		{
			if (rootFolder == null)
				rootFolder = Dump.RelativePath("options/windows");

			Directory.CreateDirectory(rootFolder);
			Dump.ToJsonFile($"{rootFolder}/options_windows.yy", this);

			if (Files.SPLASH_PNG != null)
			{
				Directory.CreateDirectory($"{rootFolder}/splash");
				File.Copy(Files.SPLASH_PNG.FullPath, $"{rootFolder}/splash/splash.png");
			}
		}
	}
}
