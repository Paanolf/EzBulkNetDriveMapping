using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace EzBulkNetDriveMapping {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Load_File(object sender, RoutedEventArgs e) {
            var filePath = OpFileDiag();
            dg1.DataContext = LecteurService.ReadFile(filePath);
        }

        private String OpFileDiag() {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true) {
                filePath = openFileDialog.FileName;
            }

            return filePath;
        }

        private void AddRowToGrid(object sender, RoutedEventArgs e) {
            dg1.DataContext = LecteurService.addEmptyRow();
        }

        private void Launch_Net_Drive_Mapping(object sender, RoutedEventArgs e) {
            var lists = dg1.DataContext as List<Lecteur>;
            if (lists != null) {
                foreach (var lect in lists) {
                    var letter = lect.Letter.Trim().Substring(0, 1);
                    var path = lect.Path.Trim();
                    if (!Directory.Exists(letter + ":")) {
                        Debug.WriteLine("Execution de net use");
                        ProcessStartInfo processStartInfo = new ProcessStartInfo {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            FileName = "net.exe",
                            Arguments = "use " + letter + ": " + path

                        };
                        Process p = Process.Start(processStartInfo);
                        string output = p.StandardOutput.ReadToEnd();
                        string error = p.StandardError.ReadToEnd();
                        using (StreamWriter logFile = new StreamWriter("log_" + DateTime.Now.ToString("ddMMyyyy") + ".txt")) {
                            logFile.WriteLine(output);
                            logFile.WriteLine(error);
                        }
                    } else {
                        MessageBoxResult mbr_res = MessageBox.Show("le lecteur " + letter + " est déjà assigné", "Lecteur existant", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                        if (mbr_res == MessageBoxResult.Cancel) {
                            return;
                        }
                    }
                }
            }
            MessageBox.Show(
                "Tâche terminée, consultez le log pour en savoir plus.",
                "Terminé",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void Delete_Mapping_All(object sender, RoutedEventArgs e) {
            MessageBoxResult messageBoxResult = MessageBox.Show(
                "Tout les lecteurs réseaux actuellement mappé sur l'ordinateur vont être supprimés.\r\nÊtes-vous sûr ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
                );
            if (messageBoxResult == MessageBoxResult.Yes) {
                Process.Start("net.exe", "use * /delete");
                MessageBox.Show(
                    "Tous les lecteurs ont été retiré du mapping",
                    "Terminé",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            } else {
                return;
            }
        }
    }
}