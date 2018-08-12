using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AnotherYoutubeAudioDownloader
{
    public static class CoreMethods
    {



        //public static List<string> Audio_BuildCLI_WithTEST(string filepath)
        //{
        //    List<string> ls = new List<string>();


        //}





        public static string AudioExtraction_BuildCLI_With(string filepath)
        {
            string commandLine = "";

            try
            {
                FileInfo fileInfo = new FileInfo(filepath);
                string filename = fileInfo.Name;
                string filename_noExtension = filename.Substring(0, filename.Length - fileInfo.Extension.Length);
                string ffmpegDir = Properties.Settings.Default["RootDirPath"].ToString() + @"\resources\ffmpeg";

                // FFmpeg parameters :
                // -i         -> ignore error
                // -f         -> output format
                // -ab        -> audio bitrate
                // -vn        -> don't include video

                // In cmd.exe, the command line would be :
                // start "" "path/to/youtube-dl.exe" [-options] "path/as/destination/name.extension" <youtube video url>

                commandLine = String.Format(
                    "/c start \"\" \"{0}\\bin\\ffmpeg.exe\" -i \"{1}\" -f mp3 -ab {2} -vn \"{3}\\Downloaded\\Audio\\{4}.mp3\"",
                    ffmpegDir,
                    filepath,
                    Properties.Settings.Default["AudioBitrate"].ToString(),
                    Properties.Settings.Default["RootDirPath"].ToString(),
                    filename_noExtension
                );
                IOMethods.WriteToLog("Command line (audio) :\n" + commandLine);
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog("Erreur dans AudioExtraction_BuildCLI_From(string filepath). Exception =\n" + ex.ToString());
            }

            return commandLine; // TODO : return filename too ?
        }




        public static ProcessStartInfo PrepareProcess(string cli)
        {
            try
            {
                return new ProcessStartInfo("cmd.exe")
                {
                    Arguments = cli,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog("Erreur dans ExecuteCLI_With(string cli). Exception =\n" + ex.ToString());
                return null;
            }
        }




        public static string VideoDownload_BuildCLI_With(string url)
        {
            string commandLine = "";

            try
            {
                string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();
                string downloadedDir = rootDirPath + @"\Downloaded";
                string youtubeDlDir = rootDirPath + @"\resources\youtube-dl";

                // youtube-dl parameters :
                // -o                   -> Output filename template
                // -q                   -> Quiet mode
                // --console-title      -> Display progression as windows title

                // In cmd.exe, the command line would be :
                // start "" "path/to/youtube-dl.exe" [-option] "path/as/destination/nom.extension" <youtube video url>

                commandLine = String.Format(
                    "/c start \"\" \"{0}\\youtube-dl.exe\" --console-title -o \"{1}\\InProgress\\%(title)s.%(ext)s\" {2}", // -q
                    youtubeDlDir,
                    downloadedDir,
                    url
                );

                IOMethods.WriteToLog("Command line (video) :\n" + commandLine);
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog("Erreur dans VideoDownload_BuildCLI_From(string url). Exception =\n" + ex.ToString());
            }

            return commandLine;
        }


       

    }
}
