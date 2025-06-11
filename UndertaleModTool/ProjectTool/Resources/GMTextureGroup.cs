using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
    public class GMTextureGroup : ResourceBase
    {
        public GMTextureGroup()
        {
            resourceVersion = "1.3";
        }

        public bool isScaled { get; set; } = true;
        public string compressFormat { get; set; } = "bz2";
        public string loadType { get; set; } = "default";
        public string directory { get; set; } = "";
        public bool autocrop { get; set; } = true;
        public uint border { get; set; } = 2;
        public uint mipsToGenerate { get; set; } = 0;
        public IdPath groupParent { get; set; } = null;
		public GMTarget targets { get; set; } = GMTarget.All;

		/// <summary>
		/// Translate UndertaleTextureGroupInfo to GMTextureGroup
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public GMTextureGroup(UndertaleTextureGroupInfo source) : this()
		{
			name = source.Name.Content;

			if (source.TexturePages != null && source.TexturePages.Count > 0)
			{
				var texData = source.TexturePages[0].Resource.TextureData;

				// Easy ones
				mipsToGenerate = source.TexturePages[0].Resource.GeneratedMips;
				isScaled = source.TexturePages[0].Resource.Scaled == 1; // "i think this is wrong?" thanks utmt devs.

				// Format ones
				if (texData.FormatBZ2)
					compressFormat = "bz2";
				else if (texData.FormatQOI)
					compressFormat = "qoi";
				else
					compressFormat = "png";

				// Disgustingly slow ones
				bool doBorder = Dump.Options.texture_border;
				bool doCrop = Dump.Options.texture_autocrop;

				foreach (var texPage in source.TexturePages)
				{
					if (!doBorder && !doCrop)
						break;

					var items = Dump.Data.TexturePageItems.Where(i => i.TexturePage == texPage.Resource).ToList();
					if (items.Count > 0)
					{
						if (doBorder)
						{
							items.Sort((x, y) => x.SourceX.CompareTo(y.SourceX)); // Get the left-most item in this texture page
							border = items[0].SourceX; // Its position on the texture *should* in theory be the border size
							doBorder = false;
						}

						if (doCrop)
						{
							autocrop = false;
							foreach (var item in items)
							{
								if (item.TargetWidth != item.BoundingWidth || item.TargetHeight != item.BoundingHeight)
								{
									doCrop = false;
									autocrop = true;
									break;
								}
							}
						}
					}
				}
			}

			// TODO: loadType, directory
		}
	}

	/// <summary>
	/// Helps align sprites to their respective groups during a dump
	/// Because UndertaleModTool is dogshit and doesn't just have a texture group dropdown list in the sprite itself
	/// Even though it really fucking should
	/// </summary>
    public static class TpageAlign
    {
        private static Dictionary<string, UndertaleTextureGroupInfo> _align = new();
        public static List<UndertaleTextureGroupInfo> TextureGroups = new();

        public static void Init()
        {
            _align.Clear();
			TextureGroups.Clear();

			foreach (var tpage in Dump.Data.TextureGroupInfo)
            {
                if (!tpage.Name.Content.Contains("_yyg_auto_gen_tex_group_name_"))
                    TextureGroups.Add(tpage);

                foreach (var i in tpage.Sprites)
                    _align.Add(i.Resource.Name.Content, tpage);
                foreach (var i in tpage.Fonts)
                    _align.Add(i.Resource.Name.Content, tpage);

				// If there's no sprites, then add the texture pages instead (look at IsSeparateTexture() below)
				if (tpage.Sprites.Count == 0)
                {
                    foreach (var i in tpage.TexturePages)
                        _align.Add(i.Resource.Name.Content, tpage);
                }
            }
        }

        public static UndertaleTextureGroupInfo DefaultGroup()
        {
            if (TextureGroups.Count == 0)
                throw new Exception("You fucked up so badly that this data.win has no texture groups please fix this");
            return TextureGroups[0];
        }
        public static UndertaleTextureGroupInfo TextureFor(UndertaleNamedResource source)
        {
            return _align.GetValueOrDefault(source.Name.Content);
        }
        public static UndertaleTextureGroupInfo TextureForOrDefault(UndertaleNamedResource source)
        {
            return TextureFor(source) ?? DefaultGroup();
        }
        public static bool IsSeparateTexture(UndertaleSprite sprite)
        {
            if (sprite.Textures.Count == 0)
                return false;

			// Texture pages aren't usually added to _align, unless there's no sprites.
			// When this happens it means that sprite is using the "separate texture page" setting.
			// So no other checks needed
			if (TextureFor(sprite.Textures[0].Texture.TexturePage) is not null)
                return true;

            return false;
        }
    }
}
