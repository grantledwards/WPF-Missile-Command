using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WPFMissileCommand
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private bool[] dataBoolFields;
        private MainWindow parent;
        public Settings(MainWindow p)
        {
            parent = p;

            dataBoolFields = new bool[4];

            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
            
private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            dataBoolFields[0] = (bool)branchMayhemCheckBox.IsChecked;
            dataBoolFields[1] = (bool)missileMayhemCheckBox.IsChecked;
            dataBoolFields[2] = (bool)uAmmoCheckBox.IsChecked;
            dataBoolFields[3] = (bool)planeMayhemCheckBox.IsChecked;

            parent.SetGameSettings(dataBoolFields[0], dataBoolFields[1], dataBoolFields[2],dataBoolFields[3]);
        }
    
    }
}
