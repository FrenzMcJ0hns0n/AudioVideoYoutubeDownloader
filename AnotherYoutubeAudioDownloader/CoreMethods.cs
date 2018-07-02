using System;
using System.Diagnostics;
using System.IO;

namespace AnotherYoutubeAudioDownloader
{
    public static class CoreMethods
    {




        // TEST DE RÉCUPÉRATION DE L'OUTPUT DE LA CLI
        // Pour récupérer le nom du fichier, grâce à l'expression '%(title)s.%(ext)s' de youtube-dl
        // Et accessoirement pour obtenir la fin du téléchargement (= début de l'extraction audio)
        // Infructueux. Et trop chiant.

        //public static string SetCLIInput_From(string url)
        //{
        //    string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();
        //    string youtubeDlDir = String.Format("{0}\\{1}", rootDirPath, "Resources\\youtube-dl"); //TODO : put in c# settings


        //    string input_test = String.Format(
        //        "/c for /f \"delims=\" %%a in ('\"{0}\\youtube-dl.exe\" --get-filename -o \"%(title)s.%(ext)s\" {1}') do @set my_variable=%%a",
        //        youtubeDlDir,
        //        url
        //    );


        //    // "/c for /f \"delims=\" %%a in ('\"{0}\\youtube-dl.exe\" --get-filename -o \"%(title)s.%(ext)s\" {1}') do @set my_variable=%%a"


        //    string input = String.Format(
        //        "/c start \"\" \"{0}\\youtube-dl.exe\" --get-filename -o %(title)s.%(ext)s {1}", 
        //        youtubeDlDir,
        //        url
        //    );

        //    return input_test;
        //}




        //public static string GetCLIOutput_With(string input)
        //{

        //    Process process = new Process
        //    {
        //        StartInfo = new ProcessStartInfo("cmd.exe")
        //        {
        //            UseShellExecute = false,
        //            CreateNoWindow = true,
        //            RedirectStandardOutput = true,
        //            Arguments = input
        //        }
        //    };

        //    process.Start();
        //    string output = process.StandardOutput.ReadToEnd();
        //    //while (!proc.StandardOutput.EndOfStream)
        //    //{
        //    //    string line = proc.StandardOutput.ReadLine();
        //    //    output += line;
        //    //}

        //    return output;
        //}




        public static string VideoDownload_BuildCLI_From(string url)
        {
            string commandLine = "";

            try
            {
                string rootDirPath = Properties.Settings.Default["RootDirPath"].ToString();
                string youtubeDlDir = rootDirPath + "\\resources\\youtube-dl";

                //string
                commandLine = String.Format(
                    "/c start \"\" \"{0}\\youtube-dl.exe\" -o {1}\\Downloaded\\InProgress\\%(title)s.%(ext)s {2}",
                    youtubeDlDir,
                    rootDirPath,
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
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = cli
                };

                Process.Start(psi);
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
                string filename_noExt = filename.Substring(0, filename.Length - fileInfo.Extension.Length);
                string ffmpegDir = Properties.Settings.Default["RootDirPath"].ToString() + "\\resources\\ffmpeg";

                //string
                commandLine = String.Format(
                    "/c start \"\" \"{0}\\bin\\ffmpeg.exe\" -i \"{1}\" -f mp3 -ab {2} -vn \"{3}\\Downloaded\\Audio\\{4}.mp3\"",
                    ffmpegDir,
                    filepath,
                    Properties.Settings.Default["AudioBitrate"].ToString(),
                    Properties.Settings.Default["RootDirPath"].ToString(),
                    filename_noExt
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
