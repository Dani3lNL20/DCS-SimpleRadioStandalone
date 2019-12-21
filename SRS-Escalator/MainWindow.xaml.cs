using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRS_Escalator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show(GetArgsString(true),
                "UAC Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            RequireAdmin();

          

        }

        private void RequireAdmin()
        {
            Task.Factory.StartNew(() =>
                {
                    var location = AppDomain.CurrentDomain.BaseDirectory;

                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        WorkingDirectory = "\"" + location+"\"",
                        FileName = "SRS Escalator.exe",
                        Verb = "runas",
                        Arguments = GetArgsString(false) + " -allowMultiple -cfg=\"C:\\Some path\\pathp ath\"  "
                    };
                    try
                    {
                        Process p = Process.Start(startInfo);

                        //shutdown this process as another has started
                        Dispatcher?.BeginInvoke(new Action(() => Application.Current.Shutdown(0)));
                    }
                    catch (System.ComponentModel.Win32Exception ex)
                    {
                        MessageBox.Show(
                            "SRS Requires admin rights to be able to read keyboard input in the background. \n\nIf you do not use any keyboard binds for SRS and want to stop this message - Disable Require Admin Rights in SRS Settings\n\nSRS will continue without admin rights but keyboard binds will not work!",
                            "UAC Error", MessageBoxButton.OK, MessageBoxImage.Warning);

                        startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            WorkingDirectory = "\"" + location + "\"",
                            FileName = "\"" + location + "SRS Escalator.exe" + "\"",
                            Verb = "runas",
                            Arguments = GetArgsString(false) + " -allowMultiple -cfg=\"C:\\Some path\\pathp ath\"  "
                        };

                        Process p = Process.Start(startInfo);
                    }
                });
        }

        private string GetArgsString(bool newline)
        {
            StringBuilder builder = new StringBuilder();
            var args = Environment.GetCommandLineArgs();
            foreach (var s in args)
            {
                if (builder.Length > 0)
                {
                  
                    builder.Append(" ");

                }

                if (s.Contains("-cfg=") && !newline)
                {
                    var str= s.Replace("-cfg=", "-cfg=\"");

                    builder.Append(str);
                    builder.Append("\"");

                }
                else
                {
                    builder.Append(s);
                }

               
                if (newline)
                {
                    builder.Append("\n");
                }
            }

            return builder.ToString();
        }
    }
}
