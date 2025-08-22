using System.Text.Json.Serialization;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
    /// <summary>
    /// The base for almost everything
    /// Objects that have a "resourceType" are usually derivative of this
    /// </summary>
    public class ResourceBase
    {
        public ResourceBase()
        {
            resourceType = GetType().Name;
        }

		[JsonProperty(Order = int.MinValue)]
		public string resourceType { get; internal set; }

		[JsonProperty(Order = int.MinValue)]
		public string resourceVersion { get; internal set; } = "1.0";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = int.MinValue)]
		public string name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IdPath parent { get; set; }
    }

    /// <summary>
    /// Who knows why they do it this way.
    /// </summary>
    public class IdPath
    {
		private string _base_path;
		public IdPath(string name, string path)
        {
            if (path.EndsWith('/'))
            {
                _base_path = path[..(path.Length - 1)];
                path += name;
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
            name = _name;
        }

		public static IdPath From<T>(T source) where T : UndertaleNamedResource
		{
			if (source == null) return null;
			string n = source.Name.Content;
			return source switch
			{
				UndertaleSprite => Dump.Options.asset_sprites ? new IdPath(n, $"sprites/{n}/{n}.yy") : null,
				UndertaleGameObject => Dump.Options.asset_objects ? new IdPath(n, $"objects/{n}/{n}.yy") : null,
				UndertaleRoom => Dump.Options.asset_rooms ? new IdPath(n, $"rooms/{n}/{n}.yy") : null,
				UndertaleAnimationCurve => new IdPath(n, $"animcurves/{n}/{n}.yy"),
				UndertaleExtension => new IdPath(n, $"extensions/{n}/{n}.yy"),
				UndertaleFont => new IdPath(n, $"fonts/{n}/{n}.yy"),
				UndertalePath => new IdPath(n, $"paths/{n}/{n}.yy"),
				UndertaleScript => Dump.Options.asset_scripts ? new IdPath(n, $"scripts/{n}/{n}.yy") : null,
				UndertaleSequence => new IdPath(n, $"sequences/{n}/{n}.yy"),
				UndertaleShader => Dump.Options.asset_shaders ? new IdPath(n, $"shaders/{n}/{n}.yy") : null,
				UndertaleSound => Dump.Options.asset_sounds ? new IdPath(n, $"sounds/{n}/{n}.yy") : null,
				UndertaleTimeline => new IdPath(n, $"timelines/{n}/{n}.yy"),
				UndertaleBackground => new IdPath(n, $"tilesets/{n}/{n}.yy"),
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

	public interface ISaveable
	{		
		public void Save(string rootFolder = null);
		public bool IsValid() => true;
		public virtual static void Init() { }
		public virtual static void End() { }
	}
}
