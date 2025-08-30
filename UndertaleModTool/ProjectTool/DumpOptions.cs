using System.IO;

namespace UndertaleModTool.ProjectTool
{
	public class DumpOptions
    {
		public string data_filename { get; set; } = null; // .../DumpTest/data.win
		public string data_filedir => Path.GetDirectoryName(data_filename); // .../DumpTest
		public bool data_do_exe { get; set; } = true; // Whether to recognize the GameMaker runner in the data.win folder for publisher info and such

		public bool asset_sprites { get; set; } = true;
		public bool asset_sounds { get; set; } = true;
		public bool asset_shaders { get; set; } = true;
		public bool asset_scripts { get; set; } = true;
		public bool asset_rooms { get; set; } = true;
		public bool asset_objects { get; set; } = true;
		public bool asset_tilesets { get; set; } = true;
		public bool asset_paths { get; set; } = true;
		public bool asset_timelines { get; set; } = true;
		public bool asset_project { get; set; } = true;
		public bool asset_sequences { get; set; } = true;
		public bool asset_fonts { get; set; } = true;
		public bool asset_extensions { get; set; } = true;
		public bool asset_animcurves { get; set; } = true;

		public bool script_code_fixes { get; set; } = true; // Fix certain regexable things
		public bool script_extra { get; set; } = true; // Whether to generate the extra script for global pragmas and extension functions
		public string script_extra_name { get; set; } = "SCR_DEGEN"; // DE(compiler)GEN(erated) :)

		public bool sprite_shaped_masks { get; set; } = true; // Figure out whether the sprite is using a circle or diamond
												// shaped collision mask by recreating the shape and comparing it
		public double sprite_shaped_mask_precision { get; set; } = 0.97; // Minimum similarity between the recreated shape and
														   // the original mask for the thing above to trigger (0-1)
		public bool sprite_missing_texture { get; set; } = true; // Generates a purple/black checkerboard picture for null textures
		public bool sprite_skip_existing { get; set; } = true; // Skip already dumped frames

		public bool options_other_platforms { get; set; } = false; // Sets up matching settings for every platform other than Windows (no consoles)

		public bool texture_border { get; set; } = true; // Figure out border sizes in texture pages. Otherwise default to 2
		public bool texture_autocrop { get; set; } = true; // Figure out whether "automatically crop" is enabled in texture pages. Otherwise default to true
											 // (will be wrong if all sprites are already pre-cropped in the first place.)
		public bool texture_uppercase_name { get; set; } = false; // User preference. GameMaker doesn't export their capitalization

		public bool project_sort_assets { get; set; } = true;
		public bool project_texturegroups { get; set; } = true;
		public bool project_audiogroups { get; set; } = true;
		public bool project_datafiles { get; set; } = true;
		public bool project_options { get; set; } = true;

		public bool tileset_compress { get; set; } = true; // Whether to use GameMaker's modified RLE for tilemaps
		public bool tileset_reconstruct_sprite { get; set; } = false; // Whether to rebuild the sprite out of the output tileset image (WIP)
	}
}
