using System.Text.Json.Serialization;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
    /// <summary>
    /// The base for almost everything.
    /// </summary>
    public class ResourceBase
    {
		/// <summary>
		/// Initializes the resourceType property
		/// </summary>
        public ResourceBase()
        {
            resourceType = GetType().Name;
        }

		/// <summary>
		/// The class name
		/// </summary>
		[JsonProperty(Order = int.MinValue)]
		public string resourceType { get; internal set; } = "!!! FORGOT : base() !!!";

		/// <summary>
		/// Mostly 1.0
		/// </summary>
		[JsonProperty(Order = int.MinValue)]
		public string resourceVersion { get; internal set; } = "1.0";

		/// <summary>
		/// Name. Sometimes null, sometimes and empty string
		/// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = int.MinValue)]
		public string name { get; set; }

		/// <summary>
		/// Most of the time, the folder this resource is in
		/// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IdPath parent { get; set; }
    }

    /// <summary>
    /// Who knows why they do it this way.
    /// </summary>
    public class IdPath
    {
		private string _base_path;
		private string _path_name;

		public IdPath(string name, string path, bool yyExt = false)
        {
			if (path.EndsWith('/'))
            {
                _base_path = path[..(path.Length - 1)];
                path += name;

				if (yyExt)
					path += ".yy";
            }
            else
            {
                var last = path.LastIndexOf('/') - 1;
                if (last < 0)
                    last = path.Length;
                _base_path = path[..last];
            }

            this.name = name;
			this.path = path;
			_path_name = path.Replace(_base_path + "/", "");
        }

        public string name { get; set; }
        public string path { get; set; }

		/// <summary>
		/// Set name and try to change the path to reflect that
		/// </summary>
        public IdPath SetName(string _name)
        {
            if (path.EndsWith(_path_name))
                path = _base_path + "/" + _name;
			else if (path.EndsWith(_path_name + ".yy"))
				path = _base_path + "/" + _name + ".yy";

			name = _name;
			_path_name = _name;

			return this;
        }

		/// <summary>
		/// Only change the name, not the path
		/// </summary>
		public IdPath SetIndependentName(string _name)
        {
			name = _name;
			return this;
        }

		/// <summary>
		/// Makes a little "sprites/spr_sprite/spr_sprite.yy" for example
		/// </summary>
		public static IdPath From<T>(T source, string independent_name = null)
		{
			if (source == null) return null;

			string n;

			if (typeof(UndertaleNamedResource).IsAssignableFrom(typeof(T)))
				n = (source as UndertaleNamedResource).Name.Content;
			else if (typeof(ResourceBase).IsAssignableFrom(typeof(T)))
				n = (source as ResourceBase).name;
			else
				throw new System.Exception("Cannot convert");

			string n2 = independent_name ?? n;

			return source switch
			{
				UndertaleSprite or GMSprite => Dump.Options.asset_sprites ? new IdPath(n2, $"sprites/{n}/{n}.yy") : null,
				UndertaleGameObject or GMObject => Dump.Options.asset_objects ? new IdPath(n2, $"objects/{n}/{n}.yy") : null,
				UndertaleRoom => Dump.Options.asset_rooms ? new IdPath(n2, $"rooms/{n}/{n}.yy") : null,
				UndertaleAnimationCurve => new IdPath(n2, $"animcurves/{n}/{n}.yy"),
				UndertaleExtension => new IdPath(n2, $"extensions/{n}/{n}.yy"),
				UndertaleFont => new IdPath(n2, $"fonts/{n}/{n}.yy"),
				UndertalePath or GMPath => Dump.Options.asset_paths ? new IdPath(n2, $"paths/{n}/{n}.yy") : null,
				UndertaleScript or GMScript => Dump.Options.asset_scripts ? new IdPath(n2, $"scripts/{n}/{n}.yy") : null,
				UndertaleSequence or GMSequence => new IdPath(n2, $"sequences/{n}/{n}.yy"),
				UndertaleShader or GMShader => Dump.Options.asset_shaders ? new IdPath(n2, $"shaders/{n}/{n}.yy") : null,
				UndertaleSound or GMSound => Dump.Options.asset_sounds ? new IdPath(n2, $"sounds/{n}/{n}.yy") : null,
				UndertaleTimeline or GMTimeline => Dump.Options.asset_timelines ? new IdPath(n2, $"timelines/{n}/{n}.yy") : null,
				UndertaleBackground or GMTileSet => Dump.Options.asset_tilesets ? new IdPath(n2, $"tilesets/{n}/{n}.yy") : null,
				_ => throw new System.NotImplementedException()
			};
		}
	}

	public class Point
	{
		public Point(float x, float y)
		{
			this.x = x;
			this.y = y;
		}

		public float x { get; set; }
		public float y { get; set; }
	}

	/// <summary>
	/// A GameMaker resource that can be saved into a .yy file. This was done purely to reduce the amount of boilerplate in Dump.Start()
	/// </summary>
	public interface ISaveable
	{
		/// <summary>
		/// Creates the rootFolder, serializes the resource to JSON and saves it into a .yy file. It may also save other files into the same folder
		/// </summary>
		/// <param name="rootFolder">Full path to the folder the .yy file and others will be saved into. Leave null for default</param>
		public void Save(string rootFolder = null);

		/// <summary>
		/// Whether the dumper will run the Save() method after insantiating the resource
		/// </summary>
		public bool IsValid() => true;

		/// <summary>
		/// Runs before creating any instances of the resource
		/// </summary>
		public virtual static void Init() { }

		/// <summary>
		/// Runs after finishing this particular resource type
		/// </summary>
		public virtual static void End() { }
	}
}
