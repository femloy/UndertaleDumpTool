using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModTool.ProjectTool.Resources;

namespace UndertaleModTool.ProjectTool
{
    [Flags]
    public enum DumpAssets
    {
        Sprites = 1,
        Test1 = 2,
        Test2 = 4,
        Test3 = 8,
        Test4 = 16
    }

    public class Dump
    {
        public string basePath;
        public DumpAssets toDump = 0;

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
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            settings.Converters.Add(new YYJson());
            settings.Converters.Add(new YYFlatJson());

            return JsonConvert.SerializeObject(obj, settings);
        }

        public void Start()
        {
            TpageAlign.Init();

            if (toDump.HasFlag(DumpAssets.Sprites))
            {
                var a = GMSprite.From(MainWindow.Get().Selected as UndertaleSprite); // temporary. still figuring it out Pal.
                Info(ToJson(a));
            }

            Log("Done");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Thank me later BUDDY
        /// </summary>
        public static void Info(string message) => MainWindow.Get().ScriptMessage(message);
        public static void Error(string message) => MainWindow.Get().ScriptError(message);
        public static void Log(string message) => MainWindow.Get().SetUMTConsoleText(message);
        public static UndertaleData Data => MainWindow.Get().Data;
    }
}
