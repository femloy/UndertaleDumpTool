using System.Collections.Generic;
using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRLayer : ResourceBase, ISaveable
	{
		public GMRLayer() : base()
		{

		}

		public bool visible { get; set; } = true;
		public int depth { get; set; } = 0;
		public bool userdefinedDepth { get; set; } = false;
		public bool inheritLayerDepth { get; set; } = false;
		public bool inheritLayerSettings { get; set; } = false;
		public bool inheritVisibility { get; set; } = true;
		public bool inheritSubLayers { get; set; } = true;
		public int gridX { get; set; } = 32;
		public int gridY { get; set; } = 32;
		public List<GMRLayer> layers { get; set; } = new();
		public bool hierarchyFrozen { get; set; } = false;
		public bool effectEnabled { get; set; } = true;
		public string effectType { get; set; } = null;
		public List<GMRLayerProperty> properties { get; set; } = new();

		public GMRLayer(UndertaleRoom.Layer source) : this()
		{
			name = source.LayerName.Content;
			visible = source.IsVisible;
			depth = source.LayerDepth;
			effectEnabled = source.EffectEnabled;
			if (source.EffectType is not null)
			{
				effectType = source.EffectType.Content;
				properties = source.EffectProperties.Select(i => new GMRLayerProperty(i)).ToList();
			}
		}

		/// <summary>
		/// Saves any files the layer may need, like instance creation code
		/// </summary>
		public virtual void Save(string rootFolder)
		{
			
		}
	}

	public class GMRLayerProperty
	{
		public enum Type
		{
			Real = 0,
			Colour = 1,
			Texture = 2
		}

		public Type type { get; set; }
		public string name { get; set; }
		public string value { get; set; }

		public GMRLayerProperty(UndertaleRoom.EffectProperty source)
		{
			type = (Type)source.Kind;
			name = source.Name.Content;
			value = source.Value.Content;
		}
	}
}
