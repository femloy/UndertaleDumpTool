using System.IO;
using System.Linq;
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

			if (Dump.Options.script_extra)
			{
				if (!Dump.Data.GlobalInitScripts.Any(i => i.Code == source.Code))
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

				// Do that
				string finalCode = "";

				if (_extensionCode != "")
					finalCode += $"{_extensionCode.Trim()}\n\n";
				if (pragmaCode != "")
					finalCode += $"{pragmaCode.Trim()}\n\n";

				if (finalCode != "")
					new GMScript(Dump.Options.script_extra_name, finalCode.Trim() + "\n").Save();
			}
		}
	}
}
