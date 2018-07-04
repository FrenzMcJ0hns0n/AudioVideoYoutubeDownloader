﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AnotherYoutubeAudioDownloader
{
    public partial class MainWindow : Window
    {




        public MainWindow()
        {
            InitializeComponent();

            DisplayHelpToolTip();
            LoadSettings();
        }




        #region GUI-related methods

        private void DisplayErrorMessage(string content)
        {
            MessageBox.Show("Erreur : " + content);
        }

        private void DisplayHelpToolTip()
        {
            Label_Help.ToolTip = 
                Properties.Resources.Help_AudioBitrate + 
                "\n\n----------\n\n" + 
                Properties.Resources.Help_RetrieveContent;
        }
        
        private void HandleDrop(DragEventArgs e)
        {
            try
            {
                if (IOMethods.DropType_isFile(e))
                {
                    FileInfo fileInfo = new FileInfo(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
                    if (IOMethods.ValidateFileExtension(fileInfo))
                    {
                        TextBox_url.Text = fileInfo.FullName;
                        return;
                    }
                    DisplayErrorMessage("Fichier non supporté.");
                    return;
                }

                if (IOMethods.DropType_isURL(e))
                {
                    TextBox_url.Text = e.Data.GetData(DataFormats.UnicodeText).ToString();
                    return;
                }
                DisplayErrorMessage("Doit être spécifié l'URL d'une vidéo Youtube ou le lien complet vers un fichier vidéo.");
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans HandleDrop(DragEventArgs e). Exception =\n" + ex.ToString());
            }
        }

        private void LoadSettings()
        {
            try
            {
                Properties.Settings.Default["RootDirPath"] = IOMethods.GetRootDirPath();

                bool isReadOnly = Convert.ToBoolean(Properties.Settings.Default["TextBox_readonly"]);
                SetTextBoxReadOnly(isReadOnly);

                int audioBitrate = Convert.ToInt32(Properties.Settings.Default["AudioBitrate"]);
                SetAudioQuality(audioBitrate);
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans LoadSettings(). Exception =\n" + ex.ToString());
            }

        }

        private void DownloadAndExtract()
        {
            // 1. Download video
            string video_builtCli = CoreMethods.VideoDownload_BuildCLI_From(TextBox_url.Text);
            CoreMethods.ExecuteCLI_With(video_builtCli);

            // 2. Handle "InProgress" file
            string inProgressDir = Properties.Settings.Default["RootDirPath"].ToString() + "\\Downloaded\\InProgress";
            string filepath = "";
            FileInfo fileInfo;

            //TODO : Improve
            while (filepath == "") // Wait for the file to be fully downloaded (extension = ".part" during download)
            {
                try
                {
                    filepath = Directory.GetFiles(inProgressDir)[0]; // fails at first, because file may not have been created yet

                    fileInfo = new FileInfo(filepath);
                    if (fileInfo.Extension.ToLowerInvariant() == ".part")
                        continue;

                    filepath = fileInfo.FullName; // while break
                }
                catch
                {
                    continue;
                }
            }
            
            // 3. Move downloaded video
            fileInfo = new FileInfo(filepath);
            string filename = fileInfo.Name;

            string videoDestination = Properties.Settings.Default["RootDirPath"].ToString() + "\\Downloaded\\Video\\" + filename;
            File.Move(filepath, videoDestination);
            
            // 4. Extract audio
            string audio_builtCli = CoreMethods.AudioExtraction_BuildCLI_From(videoDestination);
            CoreMethods.ExecuteCLI_With(audio_builtCli);
        }

        private void SetAudioQuality(int bitrate)
        {
            try
            {
                switch (bitrate)
                {
                    case 128000:
                        RadioButton_mp3_128.IsChecked = true;
                        break;
                    case 256000:
                        RadioButton_mp3_256.IsChecked = true;
                        break;
                    case 320000:
                        RadioButton_mp3_320.IsChecked = true;
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans SetAudioQuality(int bitrate). Exception =\n" + ex.ToString());
            }
        }

        private void SetTextBoxReadOnly(bool readOnly)
        {
            try
            {
                if (readOnly)
                {
                    TextBox_url.IsReadOnly = true;
                    TextBox_url.Cursor = Cursors.Arrow;
                    TextBox_url.Background = (Brush)new BrushConverter().ConvertFrom("#F2F2F2");
                }
                else
                {
                    TextBox_url.IsReadOnly = false;
                    TextBox_url.Cursor = Cursors.IBeam;
                    TextBox_url.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans SetReadOnly(bool flag). Exception =\n" + ex.ToString());
            }
        }

        #endregion




        #region GUI events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void TextBox_url_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_url_Drop(object sender, DragEventArgs e)
        {
            HandleDrop(e);
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            TextBox_url.Text = null;
        }

        private void Button_ToggleReadOnly_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_url.IsReadOnly)
            {
                SetTextBoxReadOnly(false);
                Properties.Settings.Default["TextBox_readonly"] = false;
            }  
            else
            {
                SetTextBoxReadOnly(true);
                Properties.Settings.Default["TextBox_readonly"] = true;
            }
            Properties.Settings.Default.Save();
        }
        
        //private void CheckBox_KeepSourceVideo_Checked(object sender, RoutedEventArgs e)
        //{
        //    Properties.Settings.Default["KeepVideo"] = true;
        //    Properties.Settings.Default.Save();
        //}

        //private void CheckBox_KeepSourceVideo_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Properties.Settings.Default["KeepVideo"] = false;
        //    Properties.Settings.Default.Save();
        //}

        private void RadioButton_mp3_128_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["AudioBitrate"] = 128000;
            Properties.Settings.Default.Save();
        }

        private void RadioButton_mp3_256_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["AudioBitrate"] = 256000;
            Properties.Settings.Default.Save();
        }

        private void RadioButton_mp3_320_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["AudioBitrate"] = 320000;
            Properties.Settings.Default.Save();
        }

        //private void Button_Destination_Click(object sender, RoutedEventArgs e)
        //{
        //    using (System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog())
        //    {
        //        dialog.RootFolder = Environment.SpecialFolder.Desktop;


        //        string destination = Properties.Settings.Default["Destination_path"].ToString();
        //        if (destination != null & destination != "")
        //            dialog.SelectedPath = destination;
        //        else
        //            dialog.SelectedPath = Properties.Settings.Default["RootDirPath"].ToString();


        //        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        //        if (result == System.Windows.Forms.DialogResult.OK)
        //        {
        //            Properties.Settings.Default["Destination_path"] = dialog.SelectedPath;
        //            Properties.Settings.Default.Save();
        //        }
        //    }
        //}

        //private void Button_Test_Click(object sender, RoutedEventArgs e)
        //{
        //    Properties.Settings.Default["KeepVideo"] = false;
        //    Properties.Settings.Default.Save();
        //    MessageBox.Show(String.Format
        //        (
        //            "{0}\n{1}\n{2}\n{3}",
        //            Properties.Settings.Default["TextBox_readonly"].ToString(),
        //            Properties.Settings.Default["RootDirPath"],
        //            Properties.Settings.Default["AudioBitrate"].ToString(),
        //            Properties.Settings.Default["KeepVideo"].ToString()
        //            //Properties.Settings.Default["Destination_path"].ToString()
        //        ));
        //}

        private void Button_ToAudioFolder_Click(object sender, RoutedEventArgs e)
        {
            IOMethods.OpenFolder("Audio");
        }

        private void Button_ToVideoFolder_Click(object sender, RoutedEventArgs e)
        {
            IOMethods.OpenFolder("Video");
        }

        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            string textBoxInput = TextBox_url.Text;


            if (textBoxInput == "")
            {
                DisplayErrorMessage("Pas d'URL ou de fichier vidéo spécifié");
                return;
            }


            if (File.Exists(textBoxInput))
            {
                if (IOMethods.ValidateFileExtension(new FileInfo(textBoxInput)))
                {
                    // It's a video file and it is supported : call audio extraction
                    string audio_builtCli = CoreMethods.AudioExtraction_BuildCLI_From(textBoxInput);
                    CoreMethods.ExecuteCLI_With(audio_builtCli);
                    return;
                }
                DisplayErrorMessage("Le fichier spécifié n'est pas d'un format supporté");
                return;
            }
            
            if (Uri.IsWellFormedUriString(textBoxInput, UriKind.Absolute))
            {
                if (textBoxInput.Contains("youtube.com/watch?"))
                {
                    // It must be a Youtube video URL : Download video & Extract audio
                    DownloadAndExtract();
                    return;
                }
                DisplayErrorMessage("L'URL donnée n'est pas celle d'une vidéo Youtube");
                return;
            }
            DisplayErrorMessage("L'entrée texte ne désigne ni une URL Youtube, ni un fichier vidéo supporté");
        }

        #endregion

        


    }
}
