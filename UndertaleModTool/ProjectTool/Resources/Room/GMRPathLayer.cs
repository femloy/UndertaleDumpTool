using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRPathLayer : GMRLayer
	{
		public IdPath pathId { get; set; } = null;
		public uint colour { get; set; } = uint.MaxValue;

		public GMRPathLayer(UndertaleRoom.Layer source) : base(source)
		{
			// No
		}
	}
}
