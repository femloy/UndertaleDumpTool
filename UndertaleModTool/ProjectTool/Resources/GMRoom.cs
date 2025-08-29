using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib.Models;
using UndertaleModTool.ProjectTool.Resources.Room;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMRoom : ResourceBase, ISaveable
	{
		public GMRoom()
		{
			parent = new IdPath("Rooms", "folders/", true);
		}

		public bool isDnd { get; set; } = false;
		public float volume { get; set; } = 1.0f;
		public IdPath parentRoom { get; set; }
		public List<GMRView> views { get; set; }
		public List<GMRLayer> layers { get; set; } = new();
		public bool inheritLayers { get; set; } = false;
		public string creationCodeFile { get { return _creationCode == null ? "" : $"rooms/{name}/{RoomCreateCodeFilename}"; } }
		public bool inheritCode { get; set; } = false;
		public List<IdPath> instanceCreationOrder { get; set; } = new();
		public bool inheritCreationOrder { get; set; } = false;
		public IdPath sequenceId { get; set; } = null; // ???
		public GMRoomSettings roomSettings { get; set; } = new();
		public GMRViewSettings viewSettings { get; set; } = new();
		public GMRPhysicsSettings physicsSettings { get; set; } = new();

		public static string InstID(UndertaleRoom.GameObject inst)
		{
			return "inst_" + Dump.ToHexID(inst.InstanceID.ToString());
		}

		/// <summary>
		/// Translate an UndertaleRoom into a new GMRoom
		/// </summary>
		/// <param name="source"></param>
		public GMRoom(UndertaleRoom source) : this()
		{
			name = source.Name.Content;
			views = source.Views.Select(i => new GMRView(i)).ToList();
			instanceCreationOrder = source.GameObjects.Select(i => IdPath.From(source, InstID(i))).ToList();
			roomSettings.Width = source.Width;
			roomSettings.Height = source.Height;
			roomSettings.persistent = source.Persistent;
			viewSettings.clearDisplayBuffer = !source.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.DoNotClearDisplayBuffer);
			viewSettings.clearViewBackground = source.DrawBackgroundColor || source.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.ShowColor);
			viewSettings.enableViews = source.Flags.HasFlag(UndertaleRoom.RoomEntryFlags.EnableViews);
			physicsSettings.PhysicsWorld = source.World;
			physicsSettings.PhysicsWorldGravityX = source.GravityX;
			physicsSettings.PhysicsWorldGravityY = source.GravityY;
			physicsSettings.PhysicsWorldPixToMetres = source.MetersPerPixel;

			if (source.CreationCodeId is not null)
				_creationCode = Dump.DumpCode(source.CreationCodeId);

			var cataclystLayer = source.Layers.MinBy(i => Math.Abs(i.LayerDepth)); // Layer with the lowest depth value
			for (int i = 0; i < source.Layers.Count; i++)
			{
				var sourceLayer = source.Layers[i];
				GMRLayer outputLayer = sourceLayer.LayerType switch
				{
					UndertaleRoom.LayerType.Path => new GMRPathLayer(sourceLayer),
					UndertaleRoom.LayerType.Background => new GMRBackgroundLayer(sourceLayer),
					UndertaleRoom.LayerType.Instances => new GMRInstanceLayer(sourceLayer),
					UndertaleRoom.LayerType.Assets => new GMRAssetLayer(sourceLayer),
					UndertaleRoom.LayerType.Tiles => new GMRTileLayer(sourceLayer),
					UndertaleRoom.LayerType.Effect => new GMREffectLayer(sourceLayer),
					_ => throw new NotImplementedException()
				};
				layers.Add(outputLayer);

				outputLayer.gridX = (int)source.GridWidth;
				outputLayer.gridY = (int)source.GridHeight;

				// User defined depth?
				if (sourceLayer.LayerName.Content.StartsWith("Compatibility_"))
					outputLayer.userdefinedDepth = true;
				else if (i == 0 && sourceLayer.LayerDepth == 0)
					outputLayer.userdefinedDepth = false;
				else if (sourceLayer == cataclystLayer)
					outputLayer.userdefinedDepth = true;
				else
				{
					if (i > 0 && i < source.Layers.Count - 1)
					{
						var upOne = source.Layers[i - 1];
						var downOne = source.Layers[i + 1];

						if (sourceLayer.LayerDepth != (downOne.LayerDepth + upOne.LayerDepth) / 2 || sourceLayer.LayerDepth == downOne.LayerDepth || sourceLayer.LayerDepth == upOne.LayerDepth)
							outputLayer.userdefinedDepth = true;
					}
					else if (i > 0)
					{
						var upOne = source.Layers[i - 1];
						if (sourceLayer.LayerDepth != upOne.LayerDepth + 100)
							outputLayer.userdefinedDepth = true;
					}
					else if (i < source.Layers.Count - 1)
					{
						var downOne = source.Layers[i + 1];
						if (sourceLayer.LayerDepth != downOne.LayerDepth - 100)
							outputLayer.userdefinedDepth = true;
					}
				}
			}

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "rooms");
		}

		private const string RoomCreateCodeFilename = "RoomCreationCode.gml";
		private string _creationCode;

		/// <summary>
		/// Saves the resource as a .yy file and all the .gml files of it all
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"rooms/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);

			if (_creationCode != null)
				File.WriteAllText(rootFolder + "/" + RoomCreateCodeFilename, _creationCode);
			foreach (var i in layers)
				i.Save(rootFolder);
		}
	}

	public class GMRView
	{
		public bool inherit { get; set; } = false;
		public bool visible { get; set; } = false;
		public int xview { get; set; } = 0;
		public int yview { get; set; } = 0;
		public int wview { get; set; } = 1366;
		public int hview { get; set; } = 768;
		public int xport { get; set; } = 0;
		public int yport { get; set; } = 0;
		public int wport { get; set; } = 1366;
		public int hport { get; set; } = 768;
		public int hborder { get; set; } = 32;
		public int vborder { get; set; } = 32;
		public int hspeed { get; set; } = -1;
		public int vspeed { get; set; } = -1;
		public IdPath objectId { get; set; } = null;

		public GMRView(UndertaleRoom.View source)
		{
			visible = source.Enabled;
			xview = source.ViewX;
			yview = source.ViewY;
			wview = source.ViewWidth;
			hview = source.ViewHeight;
			xport = source.PortX;
			yport = source.PortY;
			wport = source.PortWidth;
			hport = source.PortHeight;
			hborder = (int)source.BorderX;
			vborder = (int)source.BorderY;
			hspeed = source.SpeedX;
			vspeed = source.SpeedY;
			objectId = IdPath.From(source.ObjectId);
		}
	}

	public class GMRoomSettings
	{
		public bool inheritRoomSettings { get; set; } = false;
		public uint Width { get; set; } = 1366;
		public uint Height { get; set; } = 768;
		public bool persistent { get; set; } = false;
	}

	public class GMRViewSettings
	{
		public bool inheritViewSettings { get; set; } = false;
		public bool enableViews { get; set; } = false;
		public bool clearViewBackground { get; set; } = false;
		public bool clearDisplayBuffer { get; set; } = true;
	}

	public class GMRPhysicsSettings
	{
		public bool inheritPhysicsSettings { get; set; } = false;
		public bool PhysicsWorld { get; set; } = false;
		public float PhysicsWorldGravityX { get; set; } = 0.0f;
		public float PhysicsWorldGravityY { get; set; } = 10.0f;
		public float PhysicsWorldPixToMetres { get; set; } = 0.1f;
	}
}
