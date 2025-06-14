using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndertaleModLib.Models;
using static UndertaleModTool.ProjectTool.Resources.GMProject;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMSound : ResourceBase, ISaveable
	{
		public enum ConversionMode
		{
			Automatic, // Possible file and gamemaker sound property mismatch, it just copies the ogg to the build
			Required // Forces a new OGG
		}
		public enum Compression
		{
			Uncompressed,
			Compressed,
			UncompressOnLoad,
			Streamed
		}
		public enum Type
		{
			Mono,
			Stereo,
			ThreeD
		}
		public enum BitDepth
		{
			EightBits,
			SixteenBits
		}

		public ConversionMode conversionMode { get; set; } = ConversionMode.Automatic;
		public Compression compression { get; set; } = Compression.Uncompressed;
		public Type type { get; set; } = Type.Mono;
		public uint sampleRate { get; set; } = 44100;
		public BitDepth bitDepth { get; set; } = BitDepth.SixteenBits;
		public uint bitRate { get; set; } = 128;
		public float volume { get; set; } = 1.0f;
		public bool preload { get; set; } = false;
		public IdPath audioGroupId { get; set; } = new IdPath("audiogroup_default", "audiogroups/");
		public string soundFile { get; set; }
		public double duration { get; set; }

		public GMSound(UndertaleSound source)
		{
			
		}

		public void Save(string rootFolder = null)
		{
			if (rootFolder == null)
				rootFolder = Dump.RelativePath($"sounds/{name}");

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
		}
	}
}
