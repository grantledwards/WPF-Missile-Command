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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private bool[] exitReady;
        private int[] dataFields;
        private MainWindow parent;
        private bool buttonPressed = false;

        public Window1(MainWindow p)
        {
            parent = p;
            exitReady = new bool[2]{ false,false};
            dataFields = new int[3];//bases, cities, difficulty 
            InitializeComponent();
        }
        private void baseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = baseBox.SelectedIndex;
            switch (num)
            {
                case 0:
                    dataFields[0] = 3;//3 base
                    dataFields[1] = 6;//6 cities
                    break;
                case 1:
                    dataFields[0] = 1;//1 base
                    dataFields[1] = 1;//1 cities
                    break;
                case 2:
                    dataFields[0] = 2;//1 base
                    dataFields[1] = 7;//1 cities
                    break;
                default:
                    dataFields[0] = 1;
                    break;
            }

            exitReady[0] = true;
            attemptActivateButton();
        }

        private void difficultyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int num = difficultyBox.SelectedIndex;
            switch (num)
            {
                case 0:
                    dataFields[2] = 1;//difficulty 1 - Easy
                    break;
                case 1:
                    dataFields[2] = 2;//difficulty 2 - Hard
                    break;
                case 2:
                    dataFields[2] = 3;//difficulty 3 - Constant
                    break;
                default:
                    dataFields[2] = 1;
                    break;
            }

            exitReady[1] = true;
            attemptActivateButton();
        }

        private void attemptActivateButton()
        {
            if (exitReady[0] && exitReady[1])
                beginGameButton.IsEnabled = true;
        }

        private void beginGameButton_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            this.Close();
        }

        private void newGameDialog_Closed(object sender, EventArgs e)
        {
            if (buttonPressed)
            {
                parent.SetGameParams(dataFields[0], dataFields[1], dataFields[2]);
                parent.beginGame();
            }
        }

        private void setDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            buttonPressed = true;
            difficultyBox.SelectedIndex = 1;
            baseBox.SelectedIndex = 0;
        }
    }
}
