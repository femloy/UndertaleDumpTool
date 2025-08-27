using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRBackgroundLayer : GMRLayer
	{
		public IdPath spriteId { get; set; } = null;
		public uint colour { get; set; } = uint.MaxValue;
		public int x { get; set; } = 0;
		public int y { get; set; } = 0;
		public bool htiled { get; set; } = false;
		public bool vtiled { get; set; } = false;
		public float hspeed { get; set; } = 0.0f;
		public float vspeed { get; set; } = 0.0f;
		public bool stretch { get; set; } = false;
		public float animationFPS { get; set; } = 1.0f;
		public GMSequence.PlaybackSpeedType animationSpeedType { get; set; } = GMSequence.PlaybackSpeedType.FramesPerGameFrame;
		public bool userdefinedAnimFPS { get; set; } = false;

		public GMRBackgroundLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Background)
				return;

			// TODO
		}
	}
}
