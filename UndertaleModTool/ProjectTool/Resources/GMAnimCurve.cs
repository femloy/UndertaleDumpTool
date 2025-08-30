using System.Collections.Generic;
using System.IO;
using System.Linq;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMAnimCurve : ResourceBase, ISaveable
	{
		public GMAnimCurve()
		{
			resourceVersion = "1.2";
			parent = new IdPath("Animation Curves", "folders/", true);
		}

		readonly uint[] ChannelColors = { 0xFF0C69C0, 0xFFCE262C, 0xFF3FC00C, 0xFFEDF00F, 0xFFF1980C, 0xFFF10CE9, 0xFFAE3CE9 };

		public enum Function
		{
			Linear,
			Smooth,
			Bezier
		}
		public Function function { get; set; } = Function.Linear;
		public List<GMAnimCurveChannel> channels { get; set; } = new();

		/// <summary>
		/// Translate an UndertaleAnimationCurve into a new GMAnimCurve
		/// </summary>
		/// <param name="source"></param>
		public GMAnimCurve(UndertaleAnimationCurve source) : this()
		{
			name = source.Name.Content;
			channels.AddRange(source.Channels.Select(i => new GMAnimCurveChannel(i)));

			int count = 0;
			foreach (var i in channels)
			{
				i.colour = ChannelColors[count];
				if (++count >= ChannelColors.Length)
					count = 0;
			}

			if (source.Channels.Count > 0)
				function = (Function)source.Channels[0].Curve;

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "animcurves");
		}

		/// <summary>
		/// Saves the resource as a .yy file
		/// </summary>
		/// <param name="rootFolder"></param>
		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"animcurves/{name}");
			Directory.CreateDirectory(rootFolder);
			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
		}
	}

	public class GMAnimCurveChannel : ResourceBase
	{
		public class CurvePoint
		{
			public float th0 { get; set; } = 0.0f;
			public float th1 { get; set; } = 0.0f;
			public float tv0 { get; set; } = 0.0f;
			public float tv1 { get; set; } = 0.0f;
			public float x { get; set; } = 0.0f;
			public float y { get; set; } = 0.0f;

			public CurvePoint(UndertaleAnimationCurve.Channel.Point source)
			{
				x = source.X;
				th0 = source.BezierX0;
				th1 = source.BezierX1;
				tv0 = source.BezierY0;
				tv1 = source.BezierY1;
				y = source.Value;
			}
		}

		public uint colour { get; set; } = uint.MaxValue;
		public bool visible { get; set; } = true;
		public List<CurvePoint> points { get; set; } = new();

		public GMAnimCurveChannel(UndertaleAnimationCurve.Channel source)
		{
			name = source.Name.Content;
			points.AddRange(source.Points.Select(i => new CurvePoint(i)));
		}
	}
}
