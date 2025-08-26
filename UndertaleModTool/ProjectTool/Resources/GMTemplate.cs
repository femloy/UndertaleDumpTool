using System.IO;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMTemplate : ResourceBase, ISaveable
	{
		public GMTemplate()
		{
			parent = new IdPath("Xs", "folders/", true);
		}

		/// <summary>
		/// Translate an Undertale_ into a new GM_
		/// </summary>
		/// <param name="source"></param>
		public GMTemplate(UndertaleNamedResource source) : this()
		{
			name = source.Name.Content;



			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "xs");
		}

		/// <summary>
		/// Saves the resource as a .yy file (and anything else)
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"xs/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
		}
	}
}
