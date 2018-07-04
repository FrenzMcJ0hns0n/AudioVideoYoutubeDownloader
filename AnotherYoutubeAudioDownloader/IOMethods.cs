using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace AnotherYoutubeAudioDownloader
{
    public static class IOMethods
    {




        public static bool DropType_isFile(DragEventArgs e)
        {
            try
            {
                if (File.Exists(((string[])e.Data.GetData(DataFormats.FileDrop))[0]))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }




        public static bool DropType_isURL(DragEventArgs e)
        {
            try
            {
                string text = e.Data.GetData(DataFormats.UnicodeText).ToString();
                if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
                    if (text.Contains("youtube.com/watch"))
                        return true;
                return false;
            }
            catch
            {
                return false;
            }
        }




        public static string GetRootDirPath()
        {
            string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();

            if (!Directory.Exists(rootDirPath))
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            return rootDirPath;
        }
        



        public static void OpenFolder(string contentType)
        {
            string downloadedContentTypeDir = GetRootDirPath() + "\\Downloaded\\" + contentType;

            Process.Start(downloadedContentTypeDir);
        }




        public static bool ValidateFileExtension(FileInfo fileInfo)
        {
            List<string> acceptedExtensions =
                new List<string>() { ".avi", ".mkv", ".mov", ".mp4", ".mpeg", ".ogv", ".webm", ".wmv" };

            if (acceptedExtensions.Contains(fileInfo.Extension.ToLowerInvariant()))
                return true;
            return false;
        }




        public static void WriteToLog(string content)
        {
            using (StreamWriter writer = new StreamWriter(GetRootDirPath() + "\\log.txt", true, Encoding.Unicode))
            {
                writer.WriteLine(content);
            }
        }




    }
}
