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
    /// Interaction logic for NamePlayers.xaml
    /// </summary>
    public partial class NamePlayers : Page
    {
        LocalGameSave game;
        Playerz[] names;

        public NamePlayers(LocalGameSave g)
        {
            InitializeComponent();
            game = g;

            names = new Playerz[g.playerNum];
            for (int i = 0; i < g.playerNum; i++)
            {
                names[i] = new Playerz() { Player = "Player" + (i + 1) };
                playerList.Items.Add(names[i]);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < game.playerNum; i++)
            {
                LocalPlayer lp = new LocalPlayer(i, names[i].Player);
                game.AddLocalPlayer(lp);
            }

            LocalGamePlay lgp = new LocalGamePlay(game);
            NavigationService.Navigate(lgp);
        }
    }

    public class Playerz
    {
        public string Player { get; set; }
    }
}
