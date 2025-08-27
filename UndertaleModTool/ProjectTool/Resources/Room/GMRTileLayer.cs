using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRTileLayer : GMRLayer
	{
		public GMRTileLayer() : base()
		{
			resourceVersion = "1.1";
		}

		public IdPath tilesetId { get; set; } = null;
		public int x { get; set; } = 0;
		public int y { get; set; } = 0;
		public GMTileMap tiles { get; set; }

		public GMRTileLayer(UndertaleRoom.Layer source) : base(source)
		{
			resourceVersion = "1.1";

			if (source.LayerType != UndertaleRoom.LayerType.Tiles)
				return;

			// TODO
		}
	}
}
