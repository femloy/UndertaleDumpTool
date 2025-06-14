using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModTool.ProjectTool
{
	public class DumpOptions
    {
		public bool asset_project { get; set; } = true;
		public bool asset_sprites { get; set; } = true;
		public bool asset_sounds { get; set; } = true;
		public bool asset_rooms { get; set; } = false; // TODO yes we know you don't have to put todo everywhere. what was i thinking
		public bool asset_texturegroups { get; set; } = true;
		public bool asset_audiogroups { get; set; } = true;
		public bool asset_includedfiles { get; set; } = true;
		public bool asset_options { get; set; } = true;

		public bool sprite_shaped_masks { get; set; } = true; // Figure out whether the sprite is using a circle or diamond
												// shaped collision mask by recreating the shape and comparing it
		public double sprite_shaped_mask_precision { get; set; } = 0.97; // Minimum similarity between the recreated shape and
														   // the original mask for the thing above to trigger (0-1)
		public bool sprite_missing_texture { get; set; } = true; // Generates a purple/black checkerboard picture for null textures
		public bool sprite_skip_existing { get; set; } = true; // Skip already dumped frames
		
		public string data_filename { get; set; } = Dump.MainWindow.FilePath; // .../DumpTest/data.win
		public string data_filedir => Path.GetDirectoryName(data_filename); // .../DumpTest
		public bool data_do_exe { get; set; } = true;

		public bool options_other_platforms { get; set; } = false; // Sets up matching settings for every platform other than Windows (no consoles)

		public bool texture_border { get; set; } = true; // Figure out border sizes in texture pages. Otherwise default to 2
		public bool texture_autocrop { get; set; } = true; // Figure out whether "automatically crop" is enabled in texture pages. Otherwise default to true
											 // (will be wrong if all sprites are already pre-cropped in the first place.)
		public bool texture_uppercase_name { get; set; } = false; // User preference. GameMaker doesn't export their capitalization
	}
}
