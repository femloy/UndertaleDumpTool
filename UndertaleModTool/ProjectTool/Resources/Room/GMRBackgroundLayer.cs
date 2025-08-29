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

			spriteId = IdPath.From(source.BackgroundData.Sprite);
			colour = source.BackgroundData.Color;
			x = (int)source.XOffset;
			y = (int)source.XOffset;
			htiled = source.BackgroundData.TiledHorizontally;
			vtiled = source.BackgroundData.TiledVertically;
			hspeed = source.HSpeed;
			vspeed = source.VSpeed;
			stretch = source.BackgroundData.Stretch;

			if (source.BackgroundData.Sprite is not null)
			{
				if (source.BackgroundData.AnimationSpeed != source.BackgroundData.Sprite.GMS2PlaybackSpeed || (int)source.BackgroundData.AnimationSpeedType != (int)source.BackgroundData.Sprite.GMS2PlaybackSpeedType)
				{
					userdefinedAnimFPS = true;
					animationFPS = source.BackgroundData.AnimationSpeed;
					animationSpeedType = (GMSequence.PlaybackSpeedType)source.BackgroundData.AnimationSpeedType;
				}
			}
		}
	}
}
