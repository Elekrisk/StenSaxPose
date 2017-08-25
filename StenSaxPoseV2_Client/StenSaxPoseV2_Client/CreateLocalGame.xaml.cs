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
using System.Text.RegularExpressions;

namespace StenSaxPoseV2_Client
{
    /// <summary>
    /// Interaction logic for CreateLocalGame.xaml
    /// </summary>
    public partial class CreateLocalGame : Page
    {
        public CreateLocalGame()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            LocalGameSave g = new LocalGameSave(nameTextBox.Text, int.Parse(playerTextBox.Text), int.Parse(pointTextBox.Text));

            NamePlayers lg = new NamePlayers(g);
            NavigationService.Navigate(lg);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void NameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("[^a-zA-Z0-9]+");
            e.Handled = r.IsMatch(e.Text);
        }

        private void PlayerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("[^0-9]");
            bool c = r.IsMatch(e.Text);
            if (!c && int.Parse(playerTextBox.Text + e.Text) >= 127)
            {
                playerTextBox.Text = "126";
                e.Handled = true;
            }
            else
            {
                e.Handled = c;
            }
        }

        private void PointTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("[^0-9]");
            bool c = r.IsMatch(e.Text);
            if (!c && int.Parse(pointTextBox.Text + e.Text) >= 127)
            {
                pointTextBox.Text = "126";
                e.Handled = true;
            }
            else
            {
                e.Handled = c;
            }
        }
    }
}
