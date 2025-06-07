using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using UndertaleModLib.Models;
using UndertaleModTool.ProjectTool.Resources;

namespace UndertaleModTool.ProjectTool
{
    public class Dump
    {
        MainWindow w = GetMainWindow();

        public bool DoSprites;

        public static MainWindow GetMainWindow() => Application.Current.MainWindow as MainWindow;

        public static string ToGUID(string? seed)
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

            var a = GMSprite.From(w.Selected as UndertaleSprite);
            w.ScriptMessage(ToJson(a));

            //w.ScriptMessage($"{ToGUID($"spr0")}\n{ToGUID($"spr0")}\n{ToGUID($"spr0")}\n\n{ToGUID($"spr1")}\n{ToGUID($"spr2")}\n{ToGUID($"spr3")}");

            w.SetUMTConsoleText("Done");
        }
    }
}
