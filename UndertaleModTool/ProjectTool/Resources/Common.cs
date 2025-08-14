using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public string resourceType { get; internal set; }
        public string resourceVersion { get; internal set; } = "1.0";

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
