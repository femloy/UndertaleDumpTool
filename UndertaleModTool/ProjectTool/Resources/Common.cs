using System.Text.Json.Serialization;
using Newtonsoft.Json;

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

        private string _base_path;

        public string name { get; set; }
        public string path { get; set; }

        public void SetName(string _name)
        {
            if (path.EndsWith(name))
                path = _base_path + "/" + _name;
            name = _name;
        }
    }

	public interface ISaveable
	{		
		public void Save(string rootFolder = null);
		public bool IsValid() => true;
		public virtual static void Init() { }
		public virtual static void End() { }
	}
}
