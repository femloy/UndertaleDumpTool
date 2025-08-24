using System;
using System.IO;
using System.Linq;
using UndertaleModLib.Models;

namespace UndertaleModTool.ProjectTool.Resources
{
	public class GMShader : ResourceBase, ISaveable
	{
		public GMShader() : base()
		{
			parent = new IdPath("Shaders", "folders/", true);
		}

		public enum Type
		{
			GLSL_ES = 1,
			GLSL = 2,
			HLSL_11 = 3
		}

		public Type type = Type.GLSL_ES;

		private string fragment = "";
		private string vertex = "";

		/// <summary>
		/// Translate an UndertaleShader into a new GMShader
		/// </summary>
		/// <param name="source"></param>
		public GMShader(UndertaleShader source) : this()
		{
			name = source.Name.Content;

			switch (source.Type)
			{
				case UndertaleShader.ShaderType.GLSL_ES:
					type = Type.GLSL_ES;
					vertex = source.GLSL_ES_Vertex.Content.Split("#define _YY_GLSLES_ 1\n").Last();
					fragment = source.GLSL_ES_Fragment.Content.Split("#define _YY_GLSLES_ 1\n").Last();
					break;

				case UndertaleShader.ShaderType.GLSL:
					type = Type.GLSL;
					vertex = source.GLSL_Vertex.Content.Split("#define _YY_GLSL_ 1\n").Last();
					fragment = source.GLSL_Fragment.Content.Split("#define _YY_GLSL_ 1\n").Last();
					break;

				case UndertaleShader.ShaderType.HLSL11:
					type = Type.HLSL_11;
					vertex = "//\n// Cannot decompile HLSL11 shaders\n//\n";
					fragment = vertex;
					Dump.Error($"{name} uses HLSL11 which is unsupported");
					break;

				default:
					throw new Exception($"Unsupported shader type {source.Type.ToString()}");
			}

			lock (Dump.ProjectResources)
				Dump.ProjectResources.Add(name, "shaders");
		}

		public void Save(string rootFolder = null)
		{
			rootFolder ??= Dump.RelativePath($"shaders/{name}");
			Directory.CreateDirectory(rootFolder);
			Dump.ToJsonFile(rootFolder + $"/{name}.yy", this);
			File.WriteAllText(rootFolder + $"/{name}.vsh", vertex);
			File.WriteAllText(rootFolder + $"/{name}.fsh", fragment);
		}
	}
}
