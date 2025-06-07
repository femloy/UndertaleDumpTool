using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using UndertaleModLib;
using UndertaleModLib.Models;
using UndertaleModLib.Util;
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

    public partial class Dump
    {
        public string basePath;
        public DumpAssets toDump = 0;
        TextureWorker worker = new();

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
            worker.Dispose();
            worker = null;

            GC.SuppressFinalize(this);
        }

        public static Dump Get() => MainWindow.Get().Dump;
    }
}
