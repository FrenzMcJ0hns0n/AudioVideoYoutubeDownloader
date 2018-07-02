using System.IO;
using System.Reflection;
using System.Text;

namespace AnotherYoutubeAudioDownloader
{
    public static class IOMethods
    {




        public static string GetRootDirPath()
        {
            string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();

            if (!Directory.Exists(rootDirPath))
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            return rootDirPath;
        }
        



        public static void WriteToLog(string content)
        {
            string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();

            using (StreamWriter writer = new StreamWriter(rootDirPath + "\\log.txt", true, Encoding.Unicode))
            {
                writer.WriteLine(content);
            }
        }




    }
}
