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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StenSaxPoseV2_Client
{
    /// <summary>
    /// Interaction logic for SelectLocalGame.xaml
    /// </summary>
    public partial class SelectLocalGame : Page
    {
        public SelectLocalGame()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            //MainMenu mm = new MainMenu();
            //NavigationService.Navigate(mm);

            NavigationService.GoBack();
        }
    }
}
