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
    public class LoadedGame
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Players { get; set; }
        public int PointCap { get; set; }
        public LocalGameSave gameSave;
    }

    /// <summary>
    /// Interaction logic for LoadLocalGame.xaml
    /// </summary>
    public partial class LoadLocalGame : Page
    {
        LoadedGame[] lgames;

        public LoadLocalGame()
        {
            InitializeComponent();

            _refresh();
        }

        private void _refresh()
        {
            while (loadBox.Items.Count > 0)
            {
                loadBox.Items.RemoveAt(0);
            }
            FileHandler f = new FileHandler();
            LocalGameSave[] games = f.Read(Vars.path);
            lgames = new LoadedGame[games.Length];

            for (int i = 0; i < games.Length; i++)
            {
                lgames[i] = new LoadedGame() { ID = games[i].id, Name = games[i].name, Players = games[i].playerNum, PointCap = games[i].pointCap, gameSave = games[i] };
                loadBox.Items.Add(lgames[i]);
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (loadBox.SelectedItem != null && loadBox.SelectedItems.Count == 1)
            {
                LocalGamePlay lgp = new LocalGamePlay(lgames[loadBox.SelectedIndex].gameSave);
                NavigationService.Navigate(lgp);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (loadBox.SelectedItem != null && loadBox.SelectedItems.Count == 1)
            {
                lgames[loadBox.SelectedIndex].gameSave.Delete();
                _refresh();
            }
        }
    }
}
