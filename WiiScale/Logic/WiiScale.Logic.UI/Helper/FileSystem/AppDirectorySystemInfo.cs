using System;
using System.IO;

namespace WiiScale.Logic.UI.Helper.FileSystem
{
    public static class AppDirectorySystemInfo
    {
        public static string AppNamePrefix
        {
            get { return "ToPa.WiiScale"; }
        }

        public static string AppPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string AppDataPath(AppSpecialFolder specialFolder)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "ToPa\\WiiScale");

            switch (specialFolder)
            {
                case AppSpecialFolder.Traces:
                    path = Path.Combine(path, "Traces");
                    break;

                case AppSpecialFolder.Configurations:
                    path = Path.Combine(path, "Configurations");
                    break;

                case AppSpecialFolder.Serializations:
                    path = Path.Combine(path, "Serializations");
                    break;

                default:
                    throw new ArgumentOutOfRangeException("specialFolder", specialFolder, null);
            }

            path = path + "\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

    }
}