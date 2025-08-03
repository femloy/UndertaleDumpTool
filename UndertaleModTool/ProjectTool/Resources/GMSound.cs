using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Vorbis;
using NAudio.Wave;
using UndertaleModLib;
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
			ThreeD // No difference with Mono https://manual.gamemaker.io/monthly/en/The_Asset_Editors/Sounds.htm
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

		// from ExportAllSounds.csx
		private byte[] _fileData { get; set; } = null;
		private static readonly byte[] EMPTY_WAV_FILE_BYTES = Convert.FromBase64String("UklGRiQAAABXQVZFZm10IBAAAAABAAIAQB8AAAB9AAAEABAAZGF0YQAAAAA=");

		private IList<UndertaleEmbeddedAudio> GetAudioGroupData(UndertaleSound sound)
		{
			string audioGroupName = sound.AudioGroup is not null ? sound.AudioGroup.Name.Content : "audiogroup_default";
			if (loadedAudioGroups.ContainsKey(audioGroupName))
				return loadedAudioGroups[audioGroupName];

			string relativeAudioGroupPath;
			if (sound.AudioGroup is UndertaleAudioGroup { Path.Content: string customRelativePath })
				relativeAudioGroupPath = customRelativePath;
			else
				relativeAudioGroupPath = $"audiogroup{sound.GroupID}.dat";

			string groupFilePath = Path.Combine(Dump.Options.data_filedir, relativeAudioGroupPath);
			if (!File.Exists(groupFilePath))
				return null;

			Files.InvalidateByFullPath(groupFilePath); // Exclude audiogroup file from included files

			try
			{
				UndertaleData data = null;
				using (var stream = new FileStream(groupFilePath, FileMode.Open, FileAccess.Read))
					data = UndertaleIO.Read(stream, (warning, _) => Dump.Error($"A warning occured while trying to load {audioGroupName}:\n{warning}"));

				loadedAudioGroups[audioGroupName] = data.EmbeddedAudio;
				return data.EmbeddedAudio;
			}
			catch (Exception e)
			{
				Dump.Error($"An error occured while trying to load {audioGroupName}:\n{e.Message}");
				return null;
			}
		}

		private static Dictionary<string, IList<UndertaleEmbeddedAudio>> loadedAudioGroups = new();
		public static List<string> UsedAudioGroups = new();

		public static void InitGroupTracking()
		{
			loadedAudioGroups.Clear();
			UsedAudioGroups.Clear();
		}

		public GMSound(UndertaleSound source)
		{
			name = source.Name.Content;
			volume = source.Volume;
			parent = new IdPath("Sounds", "folders/Sounds.yy");
			soundFile = source.File.Content;

			if (source.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsCompressed))
				compression = Compression.Compressed;
			else if (source.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsDecompressedOnLoad))
				compression = Compression.UncompressOnLoad;
			else if (!source.Flags.HasFlag(UndertaleSound.AudioEntryFlags.IsEmbedded))
				compression = Compression.Streamed;

			if (source.AudioFile is not null && source.AudioFile.Data is not null)
				_fileData = source.AudioFile.Data;
			else if (source.GroupID != 0 && source.AudioID != -1)
			{
				IList<UndertaleEmbeddedAudio> audioGroup = GetAudioGroupData(source);
				if (audioGroup is not null)
					_fileData = audioGroup[source.AudioID].Data;
			}
			else
			{
				string streamedPath = Path.Combine(Dump.Options.data_filedir, source.File.Content);
				if (File.Exists(streamedPath))
					_fileData = File.ReadAllBytes(streamedPath);
				Files.InvalidateByFullPath(streamedPath); // Exclude streamed sound from included files
			}

			if (Dump.Options.asset_audiogroups)
			{
				string agrpName = source.AudioGroup.Name.Content;
				audioGroupId.SetName(agrpName);

				if (_fileData is not null)
				{
					if (!UsedAudioGroups.Contains(agrpName))
						UsedAudioGroups.Add(agrpName);
				}
			}

			if (compression != Compression.Streamed && _fileData != null)
			{
				string fileExt = ".wav";
				if (_fileData[0] == 'R' && _fileData[1] == 'I' && _fileData[2] == 'F' && _fileData[3] == 'F')
					fileExt = ".wav";
				else if (_fileData[0] == 'O' && _fileData[1] == 'g' && _fileData[2] == 'g' && _fileData[3] == 'S')
					fileExt = ".ogg";
				else
					Dump.Error($"Unknown file header for sound {name}");
				soundFile = Path.ChangeExtension(soundFile, fileExt);
			}

			if (_fileData == null)
			{
				_fileData = EMPTY_WAV_FILE_BYTES;
				soundFile = Path.ChangeExtension(soundFile, ".wav");
			}
			else
			{
				string fileExt = Path.GetExtension(soundFile);
				WaveStream reader;

				if (fileExt == ".wav")
					reader = new WaveFileReader(new MemoryStream(_fileData));
				else if (fileExt == ".ogg")
					reader = new VorbisWaveReader(new MemoryStream(_fileData));
				else
				{
					Dump.Error($"Unknown file extension for sound {name}");
					return;
				}

				sampleRate = (uint)reader.WaveFormat.SampleRate;
				duration = reader.TotalTime.TotalSeconds;

				if (reader.WaveFormat.Channels >= 2)
					type = Type.Stereo;
				if (reader.WaveFormat.BitsPerSample == 8)
					bitDepth = BitDepth.EightBits;
				// TODO bitrate reader.WaveFormat.AverageBytesPerSecond

				reader.Dispose();
			}
		}

		public void Save(string rootFolder = null)
		{
			if (rootFolder == null)
				rootFolder = Dump.RelativePath($"sounds/{name}");
			Directory.CreateDirectory(rootFolder);

			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			if (_fileData != null)
				File.WriteAllBytes(rootFolder + $"/{soundFile}", _fileData);
		}
	}
}
