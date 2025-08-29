using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using static UndertaleModLib.Models.UndertaleRoom;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMTileMap
	{
		public GMTileMap()
		{
			TileSerialiseData = new();
		}

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? TileDataFormat { get; private set; } = null;

		public int SerialiseWidth { get; set; } = 0;
		public int SerialiseHeight { get; set; } = 0;

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<uint> TileSerialiseData { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<int> TileCompressedData { get; set; }

		public GMTileMap(uint[][] tileData)
		{
			TileSerialiseData = new();

			foreach (var row in tileData)
			{
				SerialiseWidth = row.Length;
				TileSerialiseData.AddRange(row);
			}
			SerialiseHeight = tileData.Length;

			if (Dump.Options.tileset_compress)
				Compress();
		}

		public void Compress()
		{
			// Switches to compressed whenever there's a line of at least THREE (3) of the same tile
			// Negative number, Repeating tile, ...
			// Positive number, non, repeating, tiles, ...
			// -5, 0 ---> five of tile 0 in a row
			// 5, 0, 1, 2, 3, 4 ---> five unique tiles in a row

			if (TileDataFormat == 1)
				return;
			if (TileSerialiseData == null)
				throw new NullReferenceException("You must fill TileSerialiseData first");

			TileCompressedData = new();
			List<int> tiles = new();
			bool successful = false;
			bool lastWasRepeat = false;

			for (int i = 0; i < TileSerialiseData.Count; i++)
			{
				int tile = (int)TileSerialiseData[i];
				int repeatThreshold = lastWasRepeat ? 2 : 3; // wtf? this makes it accurate, apparently.

				int repeats = 1;
				while (i + repeats < TileSerialiseData.Count && TileSerialiseData[i + repeats] == tile)
					++repeats;

				if (repeats < repeatThreshold)
				{
					tiles.Add(tile == 0 ? int.MinValue : tile);
					lastWasRepeat = false;
				}

				if ((i >= TileSerialiseData.Count - 1 || repeats >= repeatThreshold) && tiles.Count > 0)
				{
					TileCompressedData.Add(tiles.Count);
					TileCompressedData.AddRange(tiles);
					tiles.Clear();
				}

				if (repeats >= repeatThreshold)
				{
					TileCompressedData.Add(-repeats);
					TileCompressedData.Add(tile == 0 ? int.MinValue : tile);
					i += repeats - 1;
					successful = true;
					lastWasRepeat = true;
				}
			}

			if (!successful)
			{
				TileCompressedData = null;
				TileDataFormat = null;
			}
			else
			{
				TileSerialiseData = null;
				TileDataFormat = 1;
			}
		}
	}
}
