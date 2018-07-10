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
    /// Interaction logic for EnterNameWindow.xaml
    /// </summary>
    public partial class EnterNameWindow : Window
    {
        private MainWindow parent;
        private string name;
        public EnterNameWindow(MainWindow p)
        {
            parent = p;
            InitializeComponent();
        }

        private void enterNameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string getName()
        {
            return ""+name;
        }
        
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            name = textBox.Text;
            if (name.Trim().Length > 3)
            {
                MessageBox.Show("Must enter a three-letter name");
                name = "";
                enterNameButton.IsEnabled = false;
            }
            else if (name.Trim().Length == 3)
                enterNameButton.IsEnabled = true;
            else
                enterNameButton.IsEnabled = false;
        }
    }
}
