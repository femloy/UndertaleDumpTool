using System.Collections.Generic;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources.Room
{
	public class GMRInstanceLayer : GMRLayer
	{
		public class GMRInstance : ResourceBase
		{
			public List<GMOverriddenProperty> properties { get; set; } = new();
			public bool isDnd { get; set; } = false;
			public IdPath objectId { get; set; }
			public bool inheritCode { get; set; } = false;
			public bool hasCreationCode { get { return _creationCode != null; } }
			public uint colour { get; set; } = uint.MaxValue; // 4294967295
			public float rotation { get; set; } = 0.0f;
			public float scaleX { get; set; } = 1.0f;
			public float scaleY { get; set; } = 1.0f;
			public int imageIndex { get; set; } = 0;
			public float imageSpeed { get; set; } = 1.0f;
			public IdPath inheritedItemId { get; set; } = null;
			public bool frozen { get; set; } = false;
			public bool ignore { get; set; } = false;
			public bool inheritItemSettings { get; set; } = false;
			public float x { get; set; } = 0.0f;
			public float y { get; set; } = 0.0f;

			private string _creationCode = null;

			public GMRInstance(UndertaleRoom.GameObject source) : base()
			{
				// TODO
			}
		}

		public List<GMRInstance> instances { get; set; } = new();

		public GMRInstanceLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Instances)
				return;

			// TODO
		}
	}
}
