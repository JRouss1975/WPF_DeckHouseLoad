using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Xml.Serialization;
using Microsoft.Win32;

namespace WPF_DeckHouseLoad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ScantlingsCalc> lstScantlingsCalcs = new List<ScantlingsCalc>();
        public MainWindow()
        {
            InitializeComponent();
            dgvCalc.ItemsSource = lstScantlingsCalcs;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _openFileDialog = new OpenFileDialog();
            if (_openFileDialog.ShowDialog() == true)
            {
                XmlSerializer _xmlFormatter = new XmlSerializer(typeof(List<ScantlingsCalc>));
                using (Stream _fileStream = new FileStream(_openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    _fileStream.Position = 0;
                    lstScantlingsCalcs = (List<ScantlingsCalc>)_xmlFormatter.Deserialize(_fileStream);
                }

                dgvCalc.ItemsSource = lstScantlingsCalcs;
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _saveFileDialog = new SaveFileDialog();
            if (_saveFileDialog.ShowDialog() == true)
            {
                XmlSerializer _xmlFormatter = new XmlSerializer(typeof(List<ScantlingsCalc>));
                using (Stream _fileStream = new FileStream(_saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    _fileStream.Position = 0;
                    _xmlFormatter.Serialize(_fileStream, lstScantlingsCalcs);
                }
            }
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in lstScantlingsCalcs)
            {
                i.NotifyChange("");
            }
        }
    }
}
