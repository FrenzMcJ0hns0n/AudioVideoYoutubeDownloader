using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace AnotherYoutubeAudioDownloader
{
    public static class IOMethods
    {


        // Types Génériques, pour constater de la nature du texte, au lieu de faire ces méthodes bool ?

        public static bool DropType_isFile(DragEventArgs e)
        {
            try
            {
                return File.Exists(((string[])e.Data.GetData(DataFormats.FileDrop))[0]) ? true : false;
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
                    if (text.Contains("youtube.com/watch") | text.Contains("youtu.be/"))
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
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }
        



        public static void OpenFolder(string contentType)
        {
            Process.Start(GetRootDirPath() + @"\Downloaded\" + contentType);
        }




        public static bool ValidateFileExtension(FileInfo fileInfo)
        {
            // TODO : Set better list of supported file formats
            List<string> acceptedExtensions =
                new List<string>() { ".avi", ".mkv", ".mov", ".mp4", ".mpeg", ".ogv", ".webm", ".wmv" };

            return (acceptedExtensions.Contains(fileInfo.Extension.ToLowerInvariant()) ? true : false);
        }




        public static void WriteToLog(string content)
        {
            using (StreamWriter writer = new StreamWriter(GetRootDirPath() + @"\log.txt", true, Encoding.Unicode))
            {
                writer.WriteLine(DateTime.Now + " -> " + content);
            }
        }




    }
}
