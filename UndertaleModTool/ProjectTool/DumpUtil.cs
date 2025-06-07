using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
                Formatting = Formatting.Indented,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            settings.Converters.Add(new YYJson());
            settings.Converters.Add(new YYFlatJson());

            return JsonConvert.SerializeObject(obj, settings);
        }
        public static void ToJsonFile(object obj, string relativePath = "")
        {
            File.WriteAllText(Get().basePath + relativePath, ToJson(obj));
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
