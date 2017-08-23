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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {

        public MainMenu()
        {
            InitializeComponent();
        }

        private void LocalBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectLocalGame sl = new SelectLocalGame();
            NavigationService.Navigate(sl);
        }

        private void OnlineBtn_Click(object sender, RoutedEventArgs e)
        {
            JoinOnlineGame jg = new JoinOnlineGame();
            NavigationService.Navigate(jg);
        }

        private void OptionsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
