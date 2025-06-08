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
        TextureWorker _worker = new();

        public void Start()
        {
            TpageAlign.Init();

            if (toDump.HasFlag(DumpAssets.Sprites))
            {
                GMSprite.From(MainWindow.Get().Selected as UndertaleSprite)?.Save(); // temporary. still figuring it out Pal.
            }

            Log("Done");
        }

        public void Dispose()
        {
            _worker.Dispose();
            _worker = null;

            GC.SuppressFinalize(this);
        }

        public static Dump Get() => MainWindow.Get().Dump;
        public static TextureWorker texWorker => Get()._worker;
    }
}
