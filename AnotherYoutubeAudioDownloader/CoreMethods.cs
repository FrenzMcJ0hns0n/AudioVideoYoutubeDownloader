using System;
using System.Diagnostics;
using System.IO;

namespace AnotherYoutubeAudioDownloader
{
    public static class CoreMethods
    {




        public static string VideoDownload_BuildCLI_From(string url)
        {
            string commandLine = "";

            try
            {
                string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();
                string downloadedDir = rootDirPath + "\\Downloaded";
                string youtubeDlDir = rootDirPath + "\\resources\\youtube-dl";

                //string
                commandLine = String.Format(
                    "/c start \"\" \"{0}\\youtube-dl.exe\" -o {1}\\InProgress\\%(title)s.%(ext)s {2}",
                    youtubeDlDir,
                    downloadedDir,
                    url
                );

                IOMethods.WriteToLog("Command line (video) :\n" + commandLine); // forgotten in v1.0
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans VideoDownload_BuildCLI_From(string url). Exception =\n" + ex.ToString());
            }

            return commandLine;
        }




        public static void ExecuteCLI_With(string cli)
        {
            try
            {
                ProcessStartInfo cmd = new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = cli
                };

                Process.Start(cmd);
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans ExecuteCLI_With(string cli). Exception =\n" + ex.ToString());
            }
        }




        public static string AudioExtraction_BuildCLI_From(string filepath)
        {
            string commandLine = "";

            try
            {
                FileInfo fileInfo = new FileInfo(filepath);
                string filename = fileInfo.Name;
                string filename_noExtension = filename.Substring(0, filename.Length - fileInfo.Extension.Length);
                string ffmpegDir = Properties.Settings.Default["RootDirPath"].ToString() + "\\resources\\ffmpeg";

                //string
                commandLine = String.Format(
                    "/c start \"\" \"{0}\\bin\\ffmpeg.exe\" -i \"{1}\" -f mp3 -ab {2} -vn \"{3}\\Downloaded\\Audio\\{4}.mp3\"",
                    ffmpegDir,
                    filepath,
                    Properties.Settings.Default["AudioBitrate"].ToString(),
                    Properties.Settings.Default["RootDirPath"].ToString(),
                    filename_noExtension
                );

                IOMethods.WriteToLog("Command line (audio) :\n" + commandLine); // incomplete in v1.0
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans AudioExtraction_BuildCLI_From(string filepath). Exception =\n" + ex.ToString());
            }

            return commandLine;
        }


        

    }
}
