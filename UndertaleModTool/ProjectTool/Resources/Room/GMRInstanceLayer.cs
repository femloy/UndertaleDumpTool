using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			private const string InstanceCreateCodeFilename = "InstanceCreationCode_{0}.gml";

			public GMRInstance(UndertaleRoom.GameObject source) : base()
			{
				name = GMRoom.InstID(source);
				properties = GMObjectProperty.FromRoomObject(source) ?? properties;
				objectId = IdPath.From(source.ObjectDefinition);
				colour = source.Color;
				rotation = source.Rotation;
				scaleX = source.ScaleX;
				scaleY = source.ScaleY;
				imageIndex = source.ImageIndex;
				imageSpeed = source.ImageSpeed;
				x = source.X;
				y = source.Y;

				if (source.CreationCode is not null)
					_creationCode = Dump.DumpCode(source.CreationCode);
			}

			public void Save(string rootFolder)
			{
				if (_creationCode is not null)
					File.WriteAllText(rootFolder + "/" + string.Format(InstanceCreateCodeFilename, name), _creationCode);
			}
		}

		public List<GMRInstance> instances { get; set; } = new();

		public GMRInstanceLayer(UndertaleRoom.Layer source) : base(source)
		{
			if (source.LayerType != UndertaleRoom.LayerType.Instances)
				return;
			if (!Dump.Options.asset_objects && Dump.Options.asset_project)
				return;

			instances = source.InstancesData.Instances.Select(i => new GMRInstance(i)).ToList();
		}

		public override void Save(string rootFolder)
		{
			foreach (var i in instances)
				i.Save(rootFolder);
		}
	}
}
