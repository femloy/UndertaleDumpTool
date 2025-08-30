using System.IO;
using System.Linq;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMScript : ResourceBase, ISaveable
	{
		public bool isDnD = false;
		public bool isCompatibility = false; // When is this ever not the case??

		/// ---

		private string _code;
		private bool _isValid = false;

		private static string _extensionCode;
		private static string _globalPragmaCode;

		/// <summary>
		/// Runs before dumping any scripts
		/// </summary>
		public static void Init()
		{
			_extensionCode = string.Empty;
			_globalPragmaCode = string.Empty;
		}

		/// <summary>
		/// New custom script, used for the decompiler generated script
		/// </summary>
		public GMScript(string _name, string _code) : base()
		{
			name = Dump.SafeAssetName(_name);
			this._code = _code;
			_isValid = true;
			parent = new IdPath("Scripts", "folders/Scripts.yy");
			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "scripts");
		}

		public GMScript(UndertaleScript source) : base()
		{
			if (source.Code is null)
				return;
			if (source.Code.ParentEntry is not null)
				return;
			
			name = source.Name.Content;
			_code = Dump.DumpCode(source.Code);

			if (!Dump.Data.IsVersionAtLeast(2, 3))
				_code = $"function {name}()\n{{\n\t{_code.Trim().Replace("\n", "\n\t")}\n}}\n";

			if (Dump.Options.script_extra)
			{
				if (Dump.Data.IsVersionAtLeast(2, 3) && !Dump.Data.GlobalInitScripts.Any(i => i.Code == source.Code))
				{
					// This is likely an extension function
					if (!source.Name.Content.Contains("gml_Script_"))
						return;

					string functionName = string.Join("gml_Script_", source.Name.Content.Split("gml_Script_").Skip(1).ToArray());
					// This is more cancerous and diabetic than Pizza Tower United Community Edition

					_extensionCode += $"function {functionName}()\n";
					_extensionCode += "{\n\t";
					_extensionCode += _code.Trim().Replace("\n", "\n\t");
					_extensionCode += "\n}\n";
					return;
				}
			}
			
			_isValid = true;
			parent = new IdPath("Scripts", "folders/Scripts.yy");
			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "scripts");
		}

		/// <summary>
		/// Whether this script will be exported or not
		/// </summary>
		public bool IsValid() => _isValid;

		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"scripts/{name}");
			Directory.CreateDirectory(rootFolder);
			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			File.WriteAllText(rootFolder + $"/{name}.gml", _code);
		}

		/// <summary>
		/// Runs after all scripts have been dumped
		/// </summary>
		public static void End()
		{
			if (Dump.Options.script_extra)
			{
				// Global pragma scripts
				string pragmaCode = "";

				foreach (var i in Dump.Data.GlobalInitScripts)
				{
					if (!Dump.Data.Scripts.Any(j => j.Code == i.Code))
					{
						// In global init but not a script; so a global pragma
						pragmaCode += $"// {i.Code.Name.Content}\n{Dump.DumpCode(i.Code)}\n";
					}
				}

				// Compatibility enums
				string compatCode = "";

				if (Dump.Data.IsGameMaker2() && !Dump.Data.IsYYC())
				{
					if (Dump.Data.Scripts.ByName("__init_view") is not null)
						compatCode += "enum e__VW\n{\n\tXView,\n\tYView,\n\tWView,\n\tHView,\n\tAngle,\n\tHBorder,\n\tVBorder,\n\tHSpeed,\n\tVSpeed,\n\tObject,\n\tVisible,\n\tXPort,\n\tYPort,\n\tWPort,\n\tHPort,\n\tCamera,\n\tSurfaceID,\n};\n\n";
					if (Dump.Data.Scripts.ByName("__init_background") is not null)
						compatCode += "enum e__BG\n{\n\tVisible,\n\tForeground,\n\tIndex,\n\tX,\n\tY,\n\tWidth,\n\tHeight,\n\tHTiled,\n\tVTiled,\n\tXScale,\n\tYScale,\n\tHSpeed,\n\tVSpeed,\n\tBlend,\n\tAlpha,\n};\n\n";
				}

				// Do that
				string finalCode = "";

				if (_extensionCode != "")
					finalCode += $"{_extensionCode.Trim()}\n\n";
				if (pragmaCode != "")
					finalCode += $"{pragmaCode.Trim()}\n\n";
				if (compatCode != "")
					finalCode += $"// Compatibility\n{compatCode.Trim()}\n\n";

				if (finalCode != "")
					new GMScript(Dump.Options.script_extra_name, finalCode.Trim() + "\n").Save();
			}
		}
	}
}
