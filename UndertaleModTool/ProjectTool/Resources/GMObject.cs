using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMObject : ResourceBase, ISaveable
	{
		public enum PhysicsShape
		{
			Circle,
			Box,
			Custom
		}

		public IdPath spriteId { get; set; } = null;
		public bool solid { get; set; } = false;
		public bool visible { get; set; } = true;
		public bool managed { get; set; } = true;
		public IdPath spriteMaskId { get; set; } = null;
		public bool persistent { get; set; } = false;
		public IdPath parentObjectId { get; set; } = null;
		public bool physicsObject { get; set; } = false;
		public bool physicsSensor { get; set; } = false;
		public PhysicsShape physicsShape { get; set; } = PhysicsShape.Box;
		public int physicsGroup { get; set; } = 1;
		public float physicsDensity { get; set; } = 0.5f;
		public float physicsRestitution { get; set; } = 0.1f;
		public float physicsLinearDamping { get; set; } = 0.1f;
		public float physicsAngularDamping { get; set; } = 0.1f;
		public float physicsFriction { get; set; } = 0.2f;
		public bool physicsStartAwake { get; set; } = true;
		public bool physicsKinematic { get; set; } = false;
		public List<Point> physicsShapePoints { get; set; } = new();
		public List<GMEvent> eventList { get; set; } = new();
		public List<GMObjectProperty> properties { get; set; } = new();
		public List<GMOverriddenProperty> overriddenProperties { get; set; } = new();

		public static Dictionary<UndertaleGameObject, Dictionary<string, string>> CachedProperties = new(); // <object, <propname, propvalue>>
		public static void InitProperties()
		{
			CachedProperties.Clear();

			foreach (var obj in Dump.Data.GameObjects)
			{
				if (obj.Events[14].Count == 1 && obj.Events[14][0].Actions.Count == 1)
				{
					Dump.UpdateStatus($"Caching properties - {obj}");
					CachedProperties.Add(obj, ParseProperties(obj.Events[14][0].Actions[0].CodeId));
				}
			}
		}

		public static Dictionary<string, string> ParseProperties(UndertaleCode superCode)
		{
			Dictionary<string, string> properties = new();

			string code = Dump.DumpCode(superCode);

			code = code.Replace(";\n", "\n");

			string[] codeLines = code.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			foreach (var line in codeLines)
			{
				string[] splitLine = line.Split(" = ", 2);
				if (splitLine.Length == 2)
					properties.Add(splitLine[0], splitLine[1]);
			}

			return properties;
		}

		private Dictionary<string, string> _files = new();
		public GMObject(UndertaleGameObject source) : base()
		{
			name = source.Name.Content;
			spriteId = IdPath.From(source.Sprite);
			solid = source.Solid;
			visible = source.Visible;
			managed = source.Managed;
			spriteMaskId = IdPath.From(source.TextureMaskId);
			persistent = source.Persistent;
			parentObjectId = IdPath.From(source.ParentId);

			#region Physics

			physicsObject = source.UsesPhysics;
			physicsSensor = source.IsSensor;
			physicsShape = (PhysicsShape)source.CollisionShape;
			physicsGroup = (int)source.Group;
			physicsDensity = source.Density;
			physicsRestitution = source.Restitution;
			physicsLinearDamping = source.LinearDamping;
			physicsAngularDamping = source.AngularDamping;
			physicsFriction = source.Friction;
			physicsStartAwake = source.Awake;
			physicsKinematic = source.Kinematic;

			foreach (var point in source.PhysicsVertices)
				physicsShapePoints.Add(new Point(point.X, point.Y));

			#endregion
			#region Events

			for (int event_id = 0; event_id < source.Events.Count; ++event_id)
			{
				if (event_id == 14) // PreCreate
				{
					var precreate = GMObjectProperty.FromObject(source);
					properties = precreate.Item1;
					overriddenProperties = precreate.Item2;
					break;
				}

				foreach (var subtype in source.Events[event_id])
				{
					if (subtype.Actions.Count == 0)
						continue;

					GMEvent @event = new();
					@event.eventType = event_id;
					
					string fileName;
					if (event_id == 4) // Collision
					{
						@event.collisionObjectId = IdPath.From(Dump.Data.GameObjects[(int)subtype.EventSubtype]);
						fileName = $"Collision_{@event.collisionObjectId.name}";
					}
					else
					{
						@event.eventNum = (int)subtype.EventSubtype;
						fileName = $"{Enum.GetName(typeof(EventType), event_id)}_{@event.eventNum}";
					}

					_files.Add(fileName, Dump.DumpCode(subtype.Actions));
					eventList.Add(@event);
				}
			}

			#endregion

			parent = new IdPath("Objects", "folders/Objects.yy");
			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "objects");
		}

		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"objects/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			foreach (var file in _files)
				File.WriteAllText(rootFolder + $"/{file.Key}.gml", file.Value);
		}
	}

	public class GMEvent : ResourceBase
	{
		public GMEvent()
		{
			name = "";
		}

		public bool isDnD { get; set; } = false;
		public int eventNum { get; set; }
		public int eventType { get; set; }
		public IdPath collisionObjectId { get; set; } = null;
	}

	public class GMObjectProperty : ResourceBase
	{
		public enum Type
		{
			Real,
			Int,
			String,
			Bool,
			Expression,
			Asset,
			List
		}

		public Type varType { get; set; } = Type.Real;
		public string value { get; set; }
		public bool rangeEnabled { get; set; } = false;
		public double rangeMin { get; set; } = 0.0;
		public double rangeMax { get; set; } = 10.0;
		public List<string> listItems { get; set; } = new();
		public bool multiselect { get; set; } = false;
		public List<string> filters { get; set; } = new(); // ???

		public GMObjectProperty(string propertyName, string propertyValue)
		{
			name = propertyName;
			value = propertyValue;
			varType = Type.Expression;
		}

		public static (List<GMObjectProperty>, List<GMOverriddenProperty>) FromObject(UndertaleGameObject source)
		{
			List<GMObjectProperty> list = new();
			List<GMOverriddenProperty> overrideList = new();

			if (GMObject.CachedProperties.TryGetValue(source, out var props))
			{
				// Get object's family
				Stack<UndertaleGameObject> objectFamily = new();
				UndertaleGameObject parentLookup = source;

				while (parentLookup is not null)
				{
					objectFamily.Push(parentLookup);
					parentLookup = parentLookup.ParentId;
				}

				// Make up all the unique properties
				Dictionary<string, UndertaleGameObject> uniqueProps = new();

				while (objectFamily.Count > 0)
				{
					var obj = objectFamily.Pop();
					if (GMObject.CachedProperties.TryGetValue(obj, out var uniqueProp))
					{
						foreach (var i in uniqueProp)
							uniqueProps.TryAdd(i.Key, obj);
					}
				}

				// Properties
				foreach (var i in props)
				{
					if (uniqueProps[i.Key] != source)
					{
						// Overridden
						overrideList.Add(new(i.Key, i.Value, uniqueProps[i.Key]));
					}
					else
					{
						// Unique
						list.Add(new(i.Key, i.Value));
					}
				}
			}

			return (list, overrideList);
		}

		public static List<GMOverriddenProperty> FromRoomObject(UndertaleRoom.GameObject source)
		{
			if (source.PreCreateCode is null)
				return null;

			List<GMOverriddenProperty> overrideList = new();
			var props = GMObject.ParseProperties(source.PreCreateCode);

			if (props.Count > 0)
			{
				// Get object's family
				Stack<UndertaleGameObject> objectFamily = new();
				UndertaleGameObject parentLookup = source.ObjectDefinition;

				while (parentLookup is not null)
				{
					objectFamily.Push(parentLookup);
					parentLookup = parentLookup.ParentId;
				}

				// Make up all the unique properties
				Dictionary<string, UndertaleGameObject> uniqueProps = new();

				while (objectFamily.Count > 0)
				{
					var obj = objectFamily.Pop();
					if (GMObject.CachedProperties.TryGetValue(obj, out var uniqueProp))
					{
						foreach (var i in uniqueProp)
							uniqueProps.TryAdd(i.Key, obj);
					}
				}

				// Properties
				foreach (var i in props)
					overrideList.Add(new(i.Key, i.Value, uniqueProps[i.Key]));
			}

			return overrideList;
		}
	}

	public class GMOverriddenProperty : ResourceBase
	{
		public IdPath propertyId { get; set; }
		public IdPath objectId { get; set; }
		public string value { get; set; }

		public GMOverriddenProperty(string propertyName, string propertyValue, UndertaleGameObject propertySource)
		{
			if (propertySource is null)
				throw new NullReferenceException("propertySource is null");

			name = "";
			propertyId = IdPath.From(propertySource);
			objectId = IdPath.From(propertySource);
			propertyId.name = propertyName;
			value = propertyValue;
		}
	}
}
