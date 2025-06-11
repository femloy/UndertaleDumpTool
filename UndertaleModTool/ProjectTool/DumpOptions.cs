using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndertaleModTool.ProjectTool
{
	public class DumpOptions
    {
		public bool asset_project = true;
		public bool asset_sprites = true;
		public bool asset_rooms = false; // TODO
		public bool asset_texturegroups = true;
		public bool asset_audiogroups = true;
		public bool asset_includedfiles = true;
		public bool asset_options = true;

		public bool sprite_shaped_masks = true; // Figure out whether the sprite is using a circle or diamond
												// shaped collision mask by recreating the shape and comparing it
		public double sprite_shaped_mask_precision = 0.97; // Minimum similarity between the recreated shape and
														   // the original mask for the thing above to trigger (0-1)
		public bool sprite_missing_texture = true; // Generates a purple/black checkerboard picture for null textures
		public bool sprite_skip_existing = true; // Skip already dumped frames
		
		public string data_filename; // .../DumpTest/data.win

		public bool options_other_platforms = false; // Sets up matching settings for every platform other than Windows (no consoles)

		public bool texture_border = true; // Figure out border sizes in texture pages. Otherwise default to 2
		public bool texture_autocrop = true; // Figure out whether "automatically crop" is enabled in texture pages. Otherwise default to true
											 // (will be wrong if all sprites are already pre-cropped in the first place.)
		public bool texture_uppercase_name = false; // User preference. GameMaker doesn't export their capitalization
	}
}
