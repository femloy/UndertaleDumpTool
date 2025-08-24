using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using UndertaleModLib.Models;
using UndertaleModLib.Util;

namespace UndertaleModTool.ProjectTool.Resources
{
	/// <summary>
	/// The fabled Set of Tiles. Dumping these are horribly unreliable as GameMaker completely butchers the the column count for the sake of keeping the textures square shaped.
	/// At least I think that's what it's for. I can't tell because it fails to do so half the time.
	/// </summary>
	public class GMTileSet : ResourceBase, ISaveable
	{
		public GMTileSet()
		{
			parent = new IdPath("Tile Sets", "folders/", true);
		}

		public IdPath spriteId { get; set; }
		public uint tileWidth { get; set; } = 16;
		public uint tileHeight { get; set; } = 16;
		public uint tilexoff { get; set; } = 0;
		public uint tileyoff { get; set; } = 0;
		public uint tilehsep { get; set; } = 0;
		public uint tilevsep { get; set; } = 0;
		public bool spriteNoExport { get; set; } = true; // This is exactly how I name my properties. I should work at opera
		public IdPath textureGroupId { get; set; } = new IdPath("Default", "texturegroups/");
		public uint out_tilehborder { get; set; } = 2; // This and the texture group's border size actually stack.
		public uint out_tilevborder { get; set; } = 2;
		public uint out_columns { get; set; } = 0; // Fuck this shit.
		public uint tile_count { get; set; } = 0;
		public List<GMAutoTileSet> autoTileSets { get; set; } = new();
		public List<GMTileAnimation> tileAnimationFrames { get; set; } = new();
		public float tileAnimationSpeed { get; set; } = 30.0f;
		public GMTileFrames tileAnimation { get; set; } = new();
		public GMTileMap macroPageTiles { get; set; } = new();



		private static Dictionary<string, uint> _tilesetColumns = new();
		public static void Init()
		{
			_tilesetColumns.Clear();
			Dump.ProjectFolders.Add("Tile Sets/Sprites");
		}



		/// <summary>
		/// Check if two lists have equal sequential order but could possibly have wrapped around.
		/// For example 12345 = 34512.
		/// </summary>
		static bool AreRotations<T>(List<T> a, List<T> b)
		{
			return a.Count == b.Count && (a.Count == 0 || Enumerable.Range(0, a.Count).Any(i => a.Concat(a).Skip(i).Take(a.Count).SequenceEqual(b)));
		}

		/// <summary>
		/// Translates an UndertaleBackground into a new GMTileSet
		/// </summary>
		public GMTileSet(UndertaleBackground source) : this()
		{
			name = source.Name.Content;
			tileWidth = source.GMS2TileWidth;
			tileHeight = source.GMS2TileHeight;
			out_tilehborder = source.GMS2OutputBorderX;
			out_tilevborder = source.GMS2OutputBorderY;
			out_columns = source.GMS2TileColumns;
			tile_count = source.GMS2TileCount;
			tileAnimationSpeed = MathF.Round(1000000 / source.GMS2FrameLength, 3);

			if (source.Texture is not null)
				_outputImage = Dump.TexWorker.GetTextureFor(source.Texture, source.Name.Content, true);

			if (Dump.Options.asset_texturegroups)
				textureGroupId.SetName(TpageAlign.TextureForOrDefault(source).GetName());

			var tile_ids = source.GMS2TileIds.Select(i => i.ID).ToList();
			tileAnimation.FrameData.AddRange(tile_ids);
			tileAnimation.SerialiseFrameCount = source.GMS2ItemsPerTileCount;

			#region Detect animations

			if (source.GMS2ItemsPerTileCount > 1 && tile_ids.Count > 1)
			{
				List<List<uint>> alreadyAnimated = new();

				for (int i = 0; i < tile_ids.Count; i += (int)source.GMS2ItemsPerTileCount)
				{
					var tileFrames = tile_ids.GetRange(i, (int)source.GMS2ItemsPerTileCount);
					if (!tileFrames.All(frame => frame == source.GMS2TileIds[i].ID))
					{
						if (!alreadyAnimated.Any(anim => AreRotations(anim, tileFrames)))
						{
							tileAnimationFrames.Add(new()
							{
								name = $"animation_{tileAnimationFrames.Count + 1}",
								frames = tileFrames
							});
							alreadyAnimated.Add(tileFrames);
						}
					}
				}
			}

			#endregion
			#region Sprite

			GMSprite sprite = new(source);
			spriteId = new IdPath(sprite.name, $"sprites/{sprite.name}/", true);
			sprite.Save();

			if (!Dump.Options.tileset_reconstruct_sprite)
			{
				tilexoff = out_tilehborder;
				tileyoff = out_tilevborder;
				tilehsep = out_tilehborder * 2;
				tilevsep = out_tilehborder * 2;
			}

			#endregion

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "tilesets");
		}

		IMagickImage<byte> _outputImage;

		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"tilesets/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			if (_outputImage != null)
				TextureWorker.SaveImageToFile(_outputImage, rootFolder + $"/output_tileset.png");
		}
	}

	public class GMAutoTileSet : ResourceBase
	{
		public List<int> tiles = new();
		public bool closed_edge = false;
	}

	public class GMTileAnimation : ResourceBase
	{
		public List<uint> frames = new();
	}

	public class GMTileFrames
	{
		public List<uint> FrameData = new();
		public uint SerialiseFrameCount = 0;
	}
}
