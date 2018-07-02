using System;
using System.IO;
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

            LoadSettings();
        }




        #region GUI-related methods

        private void LoadSettings()
        {
            try
            {
                bool isReadOnly = Convert.ToBoolean(Properties.Settings.Default["TextBox_readonly"]);
                SetTextBoxReadOnly(isReadOnly);

                Properties.Settings.Default["RootDirPath"] = IOMethods.GetRootDirPath();

                int audioBitrate = Convert.ToInt32(Properties.Settings.Default["AudioBitrate"]);
                if (audioBitrate != 0)
                    SetAudioQuality(audioBitrate);
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans LoadSettings(). Exception =\n" + ex.ToString());
            }
            
        }

        private void HandleStringDrop(DragEventArgs e)
        {
            try
            {
                string drop = e.Data.GetData(DataFormats.UnicodeText).ToString(); // failure => catch
                TextBox_url.Text = null;
                TextBox_url.Text = drop;
            }
            catch
            {
                DisplayErrorMessage("Ne peut être glissé/déposé que du texte");
            }
        }

        private void SetAudioQuality(int bitrate)
        {
            try
            {
                if (bitrate == 128000)
                    RadioButton_mp3_128.IsChecked = true;

                else if (bitrate == 256000)
                    RadioButton_mp3_256.IsChecked = true;

                else
                    RadioButton_mp3_320.IsChecked = true;
            }
            catch (Exception ex)
            {
                IOMethods.WriteToLog(DateTime.Now + " -> Erreur dans SetAudioQuality(int bitrate). Exception =\n" + ex.ToString());
            }
        }

        private void SetTextBoxReadOnly(bool flag)
        {
            try
            {
                if (flag)
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

        private void DisplayErrorMessage(string content)
        {
            MessageBox.Show("Erreur : " + content);
        }

        #endregion




        #region GUI events

        private void TextBox_url_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_url_Drop(object sender, DragEventArgs e)
        {
            HandleStringDrop(e);
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

        private void Button_Download_Click(object sender, RoutedEventArgs e)
        {
            //TODO ? Check URL validity here ? In case user write it manually
            if (TextBox_url.Text == "")
            {
                DisplayErrorMessage("Aucune URL n'est spécifiée");
                return;
            }

            //TODO : chunk into several functions ?

            // 1. Download video --------------------------------------------------

            string built_cli = CoreMethods.VideoDownload_BuildCLI_From(TextBox_url.Text);
            CoreMethods.ExecuteCLI_With(built_cli);



            // 2. Handle "InProgress" file --------------------------------------------------

            string inProgressDir = Properties.Settings.Default["RootDirPath"].ToString() + "\\Downloaded\\InProgress";
            string filepath = "";
            FileInfo fileInfo;



            while (true) // Wait for the file to be fully downloaded (extension = ".part" during download)
            {
                try
                {
                    filepath = Directory.GetFiles(inProgressDir)[0]; // fails at first, because download hasn't been created yet
                    fileInfo = new FileInfo(filepath);

                    if (fileInfo.Extension.ToLowerInvariant() == ".part")
                    {
                        continue;
                    }
                    
                    filepath = fileInfo.FullName;
                    break;
                }
                catch
                {
                    continue;
                }
                    
            }
            
            //if (CheckBox_KeepSourceVideo.IsChecked == true) // handle it elsewhere ! -> Prop & settings
            //{
            fileInfo = new FileInfo(filepath);
            string filename = fileInfo.Name;
            string destination = Properties.Settings.Default["RootDirPath"].ToString() + "\\Downloaded\\Video\\" + filename;

            File.Move(filepath, destination);
            //}

            

            // 3. Extract audio --------------------------------------------------

            string built_cli2 = CoreMethods.AudioExtraction_BuildCLI_From(destination);
            CoreMethods.ExecuteCLI_With(built_cli2);
            //File.Delete(file);
        }




        #endregion




    }
}
