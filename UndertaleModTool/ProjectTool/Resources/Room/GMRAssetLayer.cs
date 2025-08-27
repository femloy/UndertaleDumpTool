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
		}
		public class GMRSequenceGraphic : GMRAsset
		{
			public IdPath sequenceId { get; set; }
			public float headPosition { get; set; } = 0.0f;
			public float rotation { get; set; } = 0.0f;
			public float scaleX { get; set; } = 0.0f;
			public float scaleY { get; set; } = 0.0f;
			public float animationSpeed { get; set; } = 0.0f;
		}

		public GMRAssetLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Assets)
				return;

			// TODO
		}
	}
}
