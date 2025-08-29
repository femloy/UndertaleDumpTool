using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMREffectLayer : GMRLayer
	{
		public GMREffectLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Effect)
				return;

			if (source.EffectData is not null && source.EffectData.EffectType is not null)
			{
				// Pre 2022.1
				effectEnabled = true;
				effectType = source.EffectData.EffectType.Content;
				properties = source.EffectData.Properties.Select(i => new GMRLayerProperty(i)).ToList();
			}
		}
	}
}
