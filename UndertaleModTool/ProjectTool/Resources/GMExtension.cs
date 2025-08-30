using System.Collections.Generic;
using System.IO;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMExtension : ResourceBase, ISaveable
	{
		public GMExtension()
		{
			resourceVersion = "1.2";
			parent = new IdPath("Extensions", "folders/", true);
		}

		/// <summary>
		/// Translate an UndertaleExtension into a new GMExtension
		/// </summary>
		/// <param name="source"></param>
		public GMExtension(UndertaleExtension source) : this()
		{
			name = source.Name.Content;

			// TODO

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "extensions");
		}

		private Dictionary<string, string> _files = new();

		/// <summary>
		/// Saves the resource as a .yy file and any extension files
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"extensions/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			foreach (var file in _files)
				File.WriteAllText(rootFolder + "/" + file.Key, file.Value);
		}
	}
}
