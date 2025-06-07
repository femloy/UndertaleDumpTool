using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public int border { get; set; } = 2;
        public int mipsToGenerate { get; set; } = 0;
        public IdPath groupParent { get; set; } = null;
        public int targets { get; set; } = -1; // TODO
    }

    public static class TpageAlign
    {
        private static Dictionary<string, UndertaleTextureGroupInfo> _align = new();
        public static List<UndertaleTextureGroupInfo> TextureGroups = new();

        public static void Init()
        {
            _align.Clear();

            foreach (var tpage in Dump.GetMainWindow().Data.TextureGroupInfo)
            {
                if (!tpage.Name.Content.Contains("_yyg_auto_gen_tex_group_name_"))
                    TextureGroups.Add(tpage);

                foreach (var i in tpage.Sprites)
                    _align.Add(i.Resource.Name.Content, tpage);
                foreach (var i in tpage.Fonts)
                    _align.Add(i.Resource.Name.Content, tpage);
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
                throw new Exception();
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

            if (TextureFor(sprite.Textures[0].Texture.TexturePage) is not null)
                return true;

            return false;
        }
    }
}
