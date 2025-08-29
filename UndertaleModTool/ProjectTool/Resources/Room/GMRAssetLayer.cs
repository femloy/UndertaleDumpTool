using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRAssetLayer : GMRLayer
	{
		public class GMRAsset : ResourceBase
		{
			public uint colour { get; set; } = uint.MaxValue;
			public IdPath inheritedItemId { get; set; } = null;
			public bool frozen { get; set; } = false;
			public bool ignore { get; set; } = false;
			public bool inheritItemSettings { get; set; } = false;
			public float x { get; set; } = 0.0f;
			public float y { get; set; } = 0.0f;
		}
		public class GMRSpriteGraphic : GMRAsset
		{
			public IdPath spriteId { get; set; } = null;
			public float headPosition { get; set; } = 0.0f;
			public float rotation { get; set; } = 0.0f;
			public float scaleX { get; set; } = 0.0f;
			public float scaleY { get; set; } = 0.0f;
			public float animationSpeed { get; set; } = 0.0f;

			public GMRSpriteGraphic(UndertaleRoom.SpriteInstance source)
			{
				spriteId = IdPath.From(source.Sprite);
				headPosition = source.FrameIndex;
				rotation = source.Rotation;
				scaleX = source.ScaleX;
				scaleY = source.ScaleY;
				animationSpeed = source.AnimationSpeed;
				colour = source.Color;
				x = source.X;
				y = source.Y;
				name = source.Name.Content;
			}
		}
		public class GMRGraphic : GMRAsset
		{
			// GMS1 compatibility tile
			public IdPath spriteId { get; set; } = null;
			public int w { get; set; } = 0;
			public int h { get; set; } = 0;
			public int u0 { get; set; } = 0; // Left
			public int v0 { get; set; } = 0; // Top
			public int u1 { get; set; } = 0; // Right
			public int v1 { get; set; } = 0; // Bottom

			public GMRGraphic(UndertaleRoom.Tile source)
			{
				spriteId = IdPath.From(source.SpriteDefinition);
				u0 = source.SourceX;
				v0 = source.SourceY;
				u1 = u0 + (int)source.Width;
				v1 = v0 + (int)source.Height;
				w = (int)(source.Width * source.ScaleX);
				h = (int)(source.Width * source.ScaleX);
				colour = source.Color;
				x = source.X;
				y = source.Y;
				name = "tile_" + Dump.ToHexID($"tile{source.InstanceID}");
			}
		}
		public class GMRSequenceGraphic : GMRAsset
		{
			// TODO didnt test this
			public IdPath sequenceId { get; set; }
			public float headPosition { get; set; } = 0.0f;
			public float rotation { get; set; } = 0.0f;
			public float scaleX { get; set; } = 0.0f;
			public float scaleY { get; set; } = 0.0f;
			public float animationSpeed { get; set; } = 0.0f;

			public GMRSequenceGraphic(UndertaleRoom.SequenceInstance source)
			{
				sequenceId = IdPath.From(source.Sequence);
				headPosition = source.FrameIndex;
				rotation = source.Rotation;
				scaleX = source.ScaleX;
				scaleY = source.ScaleY;
				animationSpeed = source.AnimationSpeed;
				colour = source.Color;
				x = source.X;
				y = source.Y;
				name = source.Name.Content;
			}
		}

		public List<GMRAsset> assets { get; set; } = new();

		public GMRAssetLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Assets)
				return;

			assets.AddRange(source.AssetsData.LegacyTiles.Select(i => new GMRGraphic(i)));
			assets.AddRange(source.AssetsData.Sprites.Select(i => new GMRSpriteGraphic(i)));

			if (source.AssetsData.NineSlices is not null)
				assets.AddRange(source.AssetsData.NineSlices.Select(i => new GMRSpriteGraphic(i)));
			if (Dump.Options.asset_sequences && source.AssetsData.Sequences is not null)
				assets.AddRange(source.AssetsData.Sequences.Select(i => new GMRSequenceGraphic(i)));

			if (source.AssetsData.ParticleSystems is not null)
				Dump.Error($"Asset layer {source.LayerName.Content} contains ParticleSystems which are not supported");
			if (source.AssetsData.TextItems is not null)
				Dump.Error($"Asset layer {source.LayerName.Content} contains TextItems which are not supported");
		}
	}
}
