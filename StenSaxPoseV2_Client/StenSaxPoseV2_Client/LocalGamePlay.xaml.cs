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
    public class LocalPlayerEntry
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }

    /// <summary>
    /// Interaction logic for LocalGamePlay.xaml
    /// </summary>
    public partial class LocalGamePlay : Page
    {
        LocalGameSave game;
        LocalPlayer currentPlayer;
        Moves chosenMove = Moves.Null;

        LocalPlayerEntry[] score;

        public LocalGamePlay(LocalGameSave g)
        {
            InitializeComponent();
            game = g;
            currentPlayer = game.players[0];
            score = new LocalPlayerEntry[game.playerNum];
            foreach (LocalPlayer p in game.players)
            {
                score[p.id] = new LocalPlayerEntry() { PlayerName = p.name, Score = p.points };
                playerScores.Items.Add(score[p.id]);
            }
        }

        private void RockBtn_Click(object sender, RoutedEventArgs e)
        {
            moveImg.Source = new BitmapImage(new Uri("pack://application:,,,/Images/rock.png"));
            chosenMove = Moves.Rock;
        }

        private void PaperBtn_Click(object sender, RoutedEventArgs e)
        {
            moveImg.Source = new BitmapImage(new Uri("pack://application:,,,/Images/paper.png"));
            chosenMove = Moves.Paper;
        }

        private void ScissorsBtn_Click(object sender, RoutedEventArgs e)
        {
            moveImg.Source = new BitmapImage(new Uri("pack://application:,,,/Images/scissors.png"));
            chosenMove = Moves.Scissors;
        }

        private void SaveQuitBtn_Click(object sender, RoutedEventArgs e)
        {
            game.Save();

            MainMenu mm = new MainMenu();
            NavigationService.Navigate(mm);
        }

        private void TurnBtn_Click(object sender, RoutedEventArgs e)
        {
            currentPlayer.move = chosenMove;
            LocalPlayer nextPlayer = NextPlayer();

            if (nextPlayer == LocalPlayer.Null)
            {
                string doneMoves = "";
                List<int> rock = new List<int>();
                List<int> paper = new List<int>();
                List<int> scissors = new List<int>();


                foreach (LocalPlayer p in game.players)
                {
                    if (!doneMoves.Contains(((int)p.move).ToString()))
                    {
                        doneMoves += ((int)p.move).ToString();
                    }

                    switch (p.move)
                    {
                        case Moves.Rock:
                            rock.Add(p.id);
                            break;
                        case Moves.Paper:
                            paper.Add(p.id);
                            break;
                        case Moves.Scissors:
                            scissors.Add(p.id);
                            break;
                        default:
                            break;
                    }
                }

                if (doneMoves.Length >= 3)
                {
                    List<int> resolveIds = new List<int>();
                    if (rock.Count > paper.Count && rock.Count > scissors.Count)
                    {
                        resolveIds.AddRange(rock);
                    }
                    else if (paper.Count > rock.Count && paper.Count > scissors.Count)
                    {
                        resolveIds.AddRange(paper);
                    }
                    else if (scissors.Count > rock.Count && scissors.Count > rock.Count)
                    {
                        resolveIds.AddRange(scissors);
                    }
                    else if (rock.Count == paper.Count && rock.Count > scissors.Count)
                    {
                        resolveIds.AddRange(rock);
                        resolveIds.AddRange(paper);
                    }
                    else if (rock.Count == scissors.Count && rock.Count > paper.Count)
                    {
                        resolveIds.AddRange(rock);
                        resolveIds.AddRange(scissors);
                    }
                    else if (paper.Count == scissors.Count && paper.Count > rock.Count)
                    {
                        resolveIds.AddRange(rock);
                        resolveIds.AddRange(scissors);
                    }
                    else if (paper.Count == scissors.Count && paper.Count == rock.Count)
                    {
                        resolveIds.AddRange(rock);
                        resolveIds.AddRange(paper);
                        resolveIds.AddRange(scissors);
                    }

                    foreach (LocalPlayer p in game.players)
                    {
                        if (resolveIds.Contains(p.id))
                        {
                            p.move = Moves.Null;
                        }
                        else
                        {
                            p.move = Moves.Idle;
                        }
                    }
                }
                else
                {
                    foreach (LocalPlayer p in game.players)
                    {
                        if (p.move != Moves.Idle)
                        {
                            for (int i = 0; i < game.playerNum; i++)
                            {
                                if (i != p.id)
                                {
                                    switch (p.move)
                                    {
                                        case Moves.Rock:
                                            foreach (LocalPlayer lp in game.players)
                                            {
                                                if (lp.move == Moves.Scissors)
                                                {

                                                }
                                            }
                                            break;
                                        case Moves.Paper:
                                            break;
                                        case Moves.Scissors:
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private LocalPlayer NextPlayer()
        {
            foreach (LocalPlayer lp in game.players)
            {
                if (lp.move == Moves.Null)
                {
                    return lp;
                }
            }

            return LocalPlayer.Null;
        }

        private void Play()
        {

        }
    }

    public class PlayerMove
    {
        public int ID;
        public Moves Move;
        public int[] BeatenBy;
        public int[] Beats;

        public PlayerMove(int id, Moves move)
        {
            ID = id;
            Move = move;
        }
    }
}
