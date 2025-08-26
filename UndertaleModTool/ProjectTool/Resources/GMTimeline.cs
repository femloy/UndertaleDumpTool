using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMTimeline : ResourceBase, ISaveable
	{
		public GMTimeline()
		{
			parent = new IdPath("Timelines", "folders/", true);
		}

		public List<GMMoment> momentList { get; set; } = new();

		/// <summary>
		/// Translate an UndertaleTimeline into a new GMTimeline
		/// </summary>
		/// <param name="source"></param>
		public GMTimeline(UndertaleTimeline source) : this()
		{
			name = source.Name.Content;

			foreach (var i in source.Moments)
			{
				momentList.Add(new GMMoment() { moment = i.Step });
				_moments[i.Step] = Dump.DumpCode(i.Event);
			}

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "timelines");
		}

		private Dictionary<uint, string> _moments = new();

		/// <summary>
		/// Saves the resource as a .yy file and the code as .gml files
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"timelines/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			foreach (var i in _moments)
				File.WriteAllText(rootFolder + $"/moment_{i.Key}.gml", i.Value);
		}
	}

	public class GMMoment : ResourceBase
	{
		public GMMoment()
		{
			name = "";
		}

		private uint _moment = 0;
		public uint moment { get { return _moment; } set { _moment = value; evnt.eventNum = (int)value; } }
		public GMEvent evnt { get; set; } = new();
	}
}
