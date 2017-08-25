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
    /// Interaction logic for JoinOnlineGame.xaml
    /// </summary>
    public partial class JoinOnlineGame : Page
    {
        public JoinOnlineGame()
        {
            InitializeComponent();

            serverListBox.Items.Add("Server service is currently not implemented.");
        }

        private void JoinBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DirectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            serverListBox.Items.Add("Refreshed.");
        }
    }
}
