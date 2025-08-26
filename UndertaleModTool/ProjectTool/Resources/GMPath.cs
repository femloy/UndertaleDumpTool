using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMPath : ResourceBase, ISaveable
	{
		public GMPath()
		{
			parent = new IdPath("Paths", "folders/", true);
		}

		public enum Kind
		{
			Linear,
			Curved
		}
		public class PathPoint : Point
		{
			public PathPoint(float x, float y) : base(x, y)
			{

			}
			public PathPoint(float speed, float x, float y) : base(x, y)
			{
				this.speed = speed;
			}

			public float speed { get; set; } = 100.0f;
		}

		public Kind kind { get; set; } = Kind.Linear;
		public bool closed { get; set; } = false;
		public uint precision { get; set; } = 4;
		public List<PathPoint> points { get; set; } = new();

		/// <summary>
		/// Translate an UndertalePath into a new GMPath
		/// </summary>
		/// <param name="source"></param>
		public GMPath(UndertalePath source) : this()
		{
			name = source.Name.Content;
			kind = source.IsSmooth ? Kind.Curved : Kind.Linear;
			closed = source.IsClosed;
			precision = source.Precision;
			points.AddRange(source.Points.Select(p => new PathPoint(p.Speed, p.X, p.Y)));

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "paths");
		}

		/// <summary>
		/// Saves the resource as a .yy file
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"paths/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
		}
	}
}
