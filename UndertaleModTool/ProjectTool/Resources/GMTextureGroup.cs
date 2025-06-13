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
			name = source.GetName();

			if (source.TexturePages != null && source.TexturePages.Count > 0)
			{
				var texData = source.TexturePages[0].Resource.TextureData;

				// Easy ones
				mipsToGenerate = source.TexturePages[0].Resource.GeneratedMips;
				isScaled = source.TexturePages[0].Resource.Scaled == 1; // "i think this is wrong?" thanks utmt devs.

				if (source.LoadType != UndertaleTextureGroupInfo.TextureGroupLoadType.InFile)
				{
					directory = source.Directory.Content;
					loadType = "dynamicpages";
				}

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
							items.Sort((x, y) => x.SourceY.CompareTo(y.SourceY)); // Get the top-most item in this texture page
							border = items[0].SourceY; // Its position on the texture *should* in theory be the border size
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

		public static void Clear()
		{
			_align.Clear();
			TextureGroups.Clear();
			ConsoleGroup = false;
			TexturePageSize = 2048;
		}
        public static void Init()
        {
			foreach (var source in Dump.Data.TextureGroupInfo)
            {
				// 3D sprites aka separate texture pages (see IsSeparateTexture() below)
				bool addPages = source.Sprites.Count == 0 && source.SpineSprites.Count == 0 && source.Fonts.Count == 0 && source.Tilesets.Count == 0;
				if (source.Name.Content.Contains("_yyg_auto_gen_tex_group_name_") && addPages && source.TexturePages.Count > 0)
				{
					foreach (var i in source.TexturePages)
						_align.Add(i.Resource.Name.Content, source);
					continue;
				}

				// Default or userdefined groups
				TextureGroups.Add(source);
				if (source.TexturePages.Count == 0)
					continue;

                foreach (var i in source.Sprites)
                    _align.Add(i.Resource.Name.Content, source);
                foreach (var i in source.Fonts)
                    _align.Add(i.Resource.Name.Content, source);
				foreach (var i in source.Tilesets)
					_align.Add(i.Resource.Name.Content, source);

				// Exclude external textures (.yytex and such) from included files
				if (source.TexturePages[0].Resource.TextureExternal)
				{
					foreach (var i in source.TexturePages)
					{
						var dumpFile = Files.ByName($"{source.Name.Content}_{i.Resource.IndexInGroup}");
						if (dumpFile is not null)
							dumpFile.Included = false;
					}
				}
            }

			TexturePageSize = 256;
			foreach (var tpage in Dump.Data.EmbeddedTextures)
			{
				var size = Math.Max(tpage.TextureData.Width, tpage.TextureData.Height);
				if (size > TexturePageSize)
					TexturePageSize = size;
			}
        }

		/// <summary>
		/// Define the maximum texture page size now for options later
		/// </summary>
		public static int TexturePageSize = 2048;

		/// <summary>
		/// Whether to add an extra "DecompiledConsoleOnly" group
		/// </summary>
		public static bool ConsoleGroup = false;

		/// <summary>
		/// Name for the console group
		/// </summary>
		public const string CONSOLE_GROUP_NAME = "RedactedGroup";

		public static string GetName(this UndertaleTextureGroupInfo tpage)
		{
			if (tpage.Name.Content == "default")
				return "Default";
			if (Dump.Options.texture_uppercase_name)
				return tpage.Name.Content.ToUpper();
			return tpage.Name.Content;
		}
        public static UndertaleTextureGroupInfo DefaultGroup()
        {
            if (TextureGroups.Count == 0)
                throw new Exception("TpageAlign.TextureGroups is empty.\n\nTpageAlign.Init() was not run, or you fucked up so badly that this data.win has no texture groups.");
            return TextureGroups.ByName("default") ?? TextureGroups[0];
        }
		public static UndertaleTextureGroupInfo TextureFor(UndertaleNamedResource source) => _align.GetValueOrDefault(source.Name.Content);
		public static UndertaleTextureGroupInfo TextureForOrDefault(UndertaleNamedResource source) => TextureFor(source) ?? DefaultGroup();
        public static bool IsSeparateTexture(UndertaleSprite sprite)
        {
            if (sprite is null || sprite.Textures.Count == 0 || sprite.Textures[0].Texture is null)
                return false;

			// Texture pages aren't usually added to _align, unless there's no sprites.
			// When this happens it means that sprite is using the "separate texture page" setting.
			// So no other checks needed
			if (TextureFor(sprite.Textures[0].Texture.TexturePage) is not null)
                return true;

            return false;
        }
		public static Dictionary<string, UndertaleTextureGroupInfo> Get3DTextures() => _align.Where(i => Dump.Data.EmbeddedTextures.ByName(i.Key) is not null).ToList().ToDictionary();
    }
}
