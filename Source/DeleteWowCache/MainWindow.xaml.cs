using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace DeleteWowCache
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var key = Registry.CurrentUser.OpenSubKey("WoWCacheDelete");
            if (key == null) return;
            textBox1.Text =  key.GetValue("Path").ToString();
            key.Close();
        }

        private void Button2Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text.Equals("")) return;
            var path = textBox1.Text;
            var key = Registry.CurrentUser.CreateSubKey("WoWCacheDelete");
            key.SetValue("Path",path);
            key.Close();
            try
            {
                var drive = new DriveInfo(textBox1.Text.Substring(0, 1));
                var freeSpaceInBytes = ((drive.TotalFreeSpace/1024)/1024);
                Directory.Delete(path + @"\Cache", true);
                Directory.Delete(path + @"\Data\Cache", true);
                var freeSpaceInBytesafter = ((drive.TotalFreeSpace/1024)/1024);
                var recovered = freeSpaceInBytesafter - freeSpaceInBytes;

                SetStatusGood("  Space Before: " + freeSpaceInBytes + "MB. Space After: " +
                              freeSpaceInBytesafter + "MB. Total Saving: " + recovered + "MB. All Done.  ");
            }
            catch (DirectoryNotFoundException direx)
            {
                SetStatusBad("  Cache Already Cleared!    No space to save.  ",null);
            }
            catch (Exception ex)
            {
             SetStatusBad(ex.Message,ex.StackTrace);
             return;
            }
           
        }

        private void Button1Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result==System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = dialog.SelectedPath;
            }
        }

        private void SetStatusGood(string message)
        {
            statusmsg.Foreground = Brushes.GreenYellow;
            statusmsg.Text = message;
        }

        private void SetStatusBad(string message,string stack)
        {
            statusmsg.Foreground = Brushes.Red;
            statusmsg.Text = message;

            using (var log = new StreamWriter("Log.txt", true))
            {
                log.WriteLine("");
                log.WriteLine("Error Occured: " + DateTime.Today.ToLongDateString() + " - " +
                              DateTime.Now.ToLongTimeString());
                log.WriteLine(message);
                if (stack != null)
                {
                    log.WriteLine(stack);
                }
            }
        }
    }
}
