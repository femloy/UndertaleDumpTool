using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMREffectLayer : GMRLayer
	{
		public GMREffectLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Effect)
				return;

			// TODO
		}
	}
}
