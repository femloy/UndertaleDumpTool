using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ImageMagick;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Util;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMFont : ResourceBase, ISaveable
	{
		public GMFont()
		{
			parent = new IdPath("Fonts", "folders/", true);
		}

		public enum Hinting
		{
			Normal,
			Monochrome,
			Light
		}
		public enum Interpreter
		{
			TTv40,
			TTv35
		}
		public enum Rounding
		{
			Floor,
			Round,
			Ceil
		}
		public enum Kerning
		{
			On,
			Off
		}
		public enum AA
		{
			Off,
			On
		}
		[Flags]
		public enum GlyphOptions
		{
			None = 0,
			NoHinting = 1,
			NoAutoHinter = 2,
			PreferAutoHinter = 4,
			Warping = 8,
			NoScale = 16
		}

		public Hinting hinting { get; set; } = Hinting.Normal;
		public GlyphOptions glyphOperations { get; set; } = GlyphOptions.None;
		public Interpreter interpreter { get; set; } = Interpreter.TTv40;
		public Rounding pointRounding { get; set; } = Rounding.Floor;
		public Kerning applyKerning { get; set; } = Kerning.On;
		public string fontName { get; set; } = "Arial";
		public string styleName { get; set; } = "Regular";
		public float size { get; set; } = 12.0f;
		public bool bold { get; set; } = false;
		public bool italic { get; set; } = false;
		public int charset { get; set; } = 0; // deprecated??? (potentially old GM:S or even GM8 leftover?)
		public AA AntiAlias { get; set; } = AA.On;
		public uint first { get; set; } = 0; // deprecated
		public uint last { get; set; } = 0; // deprecated
		public string sampleText { get; set; } = "abcdef ABCDEF\n0123456789 .,<>\"'&!?\nthe quick brown fox jumps over the lazy dog\nTHE QUICK BROWN FOX JUMPS OVER THE LAZY DOG\nDefault character: ▯ (9647)";
		public bool includeTTF { get; set; } = false;
		public string TTFName { get; set; } = "";
		public IdPath textureGroupId { get; set; } = new IdPath("Default", "texturegroups/");
		public int ascenderOffset { get; set; } = 0;
		public uint ascender { get; set; } = 14;
		public uint lineHeight { get; set; } = 18;
		public Dictionary<int, GMFontGlyph> glyphs { get; set; } = new();
		public List<GMFontKerningPair> kerningPairs { get; set; } = new();
		public List<GMFontRange> ranges { get; set; } = new();
		public bool regenerateBitmap { get; set; } = false;
		public bool canGenerateBitmap { get; set; } = true; // just sets itself back to true anyway
		public bool maintainGms1Font { get; set; } = false;

		/// <summary>
		/// Translate an UndertaleFont into a new GMFont
		/// </summary>
		/// <param name="source"></param>
		public GMFont(UndertaleFont source) : this()
		{
			name = source.Name.Content;
			maintainGms1Font = true; // unfortunately the only way
			fontName = source.DisplayName.Content;
			size = source.EmSize;
			bold = source.Bold;
			italic = source.Italic;
			styleName = "Decompiled";
			charset = source.Charset;
			AntiAlias = source.AntiAliasing > 0 ? AA.On : AA.Off;
			first = source.RangeStart;
			last = source.RangeEnd;
			ascenderOffset = source.AscenderOffset;
			ascender = source.Ascender;
			lineHeight = source.LineHeight;

			if (source.Texture is not null)
				_outputImage = Dump.TexWorker.GetTextureFor(source.Texture, name, true);

			if (Dump.Options.project_texturegroups)
				textureGroupId.SetName(TpageAlign.TextureForOrDefault(source).GetName());

			foreach (var i in source.Glyphs)
			{
				glyphs.Add(i.Character, new GMFontGlyph(i));
				kerningPairs.AddRange(i.Kerning.Select(k => new GMFontKerningPair(i.Character, k)));
			}

			if (glyphs.Count > 0)
			{
				int first = glyphs.First().Key;
				int last = first - 1;

				foreach (var i in glyphs)
				{
					if (i.Key != last + 1)
					{
						ranges.Add(new(first, last));
						first = i.Key;
					}
					last = i.Key;
				}

				ranges.Add(new(first, last));
			}

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "fonts");
		}

		private IMagickImage<byte> _outputImage;

		/// <summary>
		/// Saves the resource as a .yy file and output .png
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"fonts/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			if (_outputImage != null)
				TextureWorker.SaveImageToFile(_outputImage, rootFolder + $"/{name}.png");
		}
	}

	public class GMFontGlyph
	{
		public int x { get; set; }
		public int y { get; set; }
		public int w { get; set; }
		public int h { get; set; }
		public int character { get; set; }
		public int shift { get; set; }
		public int offset { get; set; }

		public GMFontGlyph(UndertaleFont.Glyph source)
		{
			x = source.SourceX;
			y = source.SourceY;
			w = source.SourceWidth;
			h = source.SourceHeight;
			character = source.Character;
			shift = source.Shift;
			offset = source.Offset;
		}
	}

	public class GMFontKerningPair
	{
		public int first { get; set; }
		public int second { get; set; }
		public int amount { get; set; }

		public GMFontKerningPair(int first, UndertaleFont.Glyph.GlyphKerning source)
		{
			this.first = first;
			second = source.Character;
			amount = source.ShiftModifier;
		}
	}

	public class GMFontRange
	{
		public GMFontRange(int lower, int upper)
		{
			if (lower == 32 && upper == 126)
				upper = 127; // DEL character gets ignored.

			this.lower = lower;
			this.upper = upper;
		}

		public int lower { get; set; }
		public int upper { get; set; }
	}
}
