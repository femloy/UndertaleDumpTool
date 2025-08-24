using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
		public List<int> TileSerialiseData { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public List<int> TileCompressedData { get; set; }

		public GMTileMap(List<int> tileData)
		{
			TileSerialiseData = tileData;
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

			for (int i = 0; i < TileSerialiseData.Count; i++)
			{
				int tile = TileSerialiseData[i];

				int repeats = 1;
				while (i + repeats < TileSerialiseData.Count && TileSerialiseData[i + repeats] == tile)
					++repeats;

				if (repeats < 3)
					tiles.Add(tile);

				if ((i >= TileSerialiseData.Count - 1 || repeats >= 3) && tiles.Count > 0)
				{
					TileCompressedData.Add(tiles.Count);
					TileCompressedData.AddRange(tiles);
					tiles.Clear();
				}

				if (repeats >= 3)
				{
					TileCompressedData.Add(-repeats);
					TileCompressedData.Add(tile);
					i += repeats - 1;
					successful = true;
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
