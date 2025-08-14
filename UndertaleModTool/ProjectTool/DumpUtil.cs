using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UndertaleModLib;

namespace UndertaleModTool.ProjectTool
{
    public partial class Dump
    {
        public static string ToGUID(string seed)
        {
            if (string.IsNullOrEmpty(seed))
                return new Guid().ToString();
            return new Guid(MD5.HashData(Encoding.UTF8.GetBytes(seed))).ToString();
        }
        public static string ToJson(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            return YYJson.Format(JsonConvert.SerializeObject(obj, settings));
        }
        public static void ToJsonFile(string absolutePath, object obj)
        {
            File.WriteAllText(absolutePath, ToJson(obj));
        }
		public static string RelativePath(string path)
		{
			return Path.Combine(BasePath, path);
		}

		/// <summary>
		/// Thank me later BUDDY
		/// </summary>
		public static MainWindow MainWindow;
		public Dump(MainWindow _window) : this()
		{
			MainWindow = _window;
			_options.data_filename = MainWindow.FilePath;
			Data = MainWindow.Data;
		}

		public static void Info(string message) => MainWindow.ScriptMessage(message);
        public static void Error(string message) => MainWindow.ScriptError(message);
		public static bool YesNoQuestion(string message) => MainWindow.ScriptQuestion(message);
		public static void Log(string message) => MainWindow.SetUMTConsoleText(message);
		public static void SetupProgress(string current, double maxValue) => MainWindow.SetProgressBar(null, current, 0, maxValue);
		public static void UpdateStatus(string status) => Log(status);
		public static void IncrementProgress() => MainWindow.IncrementProgressParallel();
		public static void OpenInExplorer() => Process.Start("explorer.exe", BasePath.Replace('/', '\\'));
        public static UndertaleData Data;
    }
}
