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
        }

        public string name { get; set; }
        public string path { get; set; }

        public void SetName(string _name)
        {
            if (path.EndsWith(name))
                path = _base_path + "/" + _name;
			else if (path.EndsWith(name + ".yy"))
				path = _base_path + "/" + _name + ".yy";
			name = _name;
        }

		/// <summary>
		/// Makes a little "sprites/spr_sprite/spr_sprite.yy" for example
		/// </summary>
		public static IdPath From<T>(T source)
		{
			if (source == null) return null;

			string n;

			if (typeof(UndertaleNamedResource).IsAssignableFrom(typeof(T)))
				n = (source as UndertaleNamedResource).Name.Content;
			else if (typeof(ResourceBase).IsAssignableFrom(typeof(T)))
				n = (source as ResourceBase).name;
			else
				throw new System.Exception("Cannot convert");

			return source switch
			{
				UndertaleSprite or GMSprite => Dump.Options.asset_sprites ? new IdPath(n, $"sprites/{n}/{n}.yy") : null,
				UndertaleGameObject or GMObject => Dump.Options.asset_objects ? new IdPath(n, $"objects/{n}/{n}.yy") : null,
				UndertaleRoom => Dump.Options.asset_rooms ? new IdPath(n, $"rooms/{n}/{n}.yy") : null,
				UndertaleAnimationCurve => new IdPath(n, $"animcurves/{n}/{n}.yy"),
				UndertaleExtension => new IdPath(n, $"extensions/{n}/{n}.yy"),
				UndertaleFont => new IdPath(n, $"fonts/{n}/{n}.yy"),
				UndertalePath => new IdPath(n, $"paths/{n}/{n}.yy"),
				UndertaleScript or GMScript => Dump.Options.asset_scripts ? new IdPath(n, $"scripts/{n}/{n}.yy") : null,
				UndertaleSequence or GMSequence => new IdPath(n, $"sequences/{n}/{n}.yy"),
				UndertaleShader or GMShader => Dump.Options.asset_shaders ? new IdPath(n, $"shaders/{n}/{n}.yy") : null,
				UndertaleSound or GMSound => Dump.Options.asset_sounds ? new IdPath(n, $"sounds/{n}/{n}.yy") : null,
				UndertaleTimeline => new IdPath(n, $"timelines/{n}/{n}.yy"),
				UndertaleBackground or GMTileSet => new IdPath(n, $"tilesets/{n}/{n}.yy"),
				_ => throw new System.NotImplementedException()
			};
		}
	}

	public class Point
	{
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public double x { get; set; }
		public double y { get; set; }
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
