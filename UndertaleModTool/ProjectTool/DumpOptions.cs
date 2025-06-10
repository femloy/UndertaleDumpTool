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
		public bool asset_texturegroups = true;
		public bool asset_audiogroups = true;
		public bool asset_includedfiles = true;

		public bool texture_border = true; // Figure out border sizes in texture pages. Otherwise default to 2
		public bool texture_autocrop = true; // Figure out whether "automatically crop" is enabled in texture pages. Otherwise default to true
											 // (will be wrong if all sprites are already pre-cropped in the first place.)
	}
}
