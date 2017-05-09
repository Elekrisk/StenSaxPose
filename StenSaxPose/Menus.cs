using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StenSaxPose
{
    public class Player
    {
        public int ID;
        public int Score;

        public Player(int id)
        {
            ID = id;
        }
    }

    enum Menus { MainMenu, LocalChoose, LocalLoad, LocalSave, LocalGameName, LocalSetup, LocalPlayerNum, LocalPlayerScore, OnlineSetup1, InLocalGame, InOnlineGame, Music }

    class Program
    {
        static string path;
        static int localGameID;
        static Menus CurrentMenu = Menus.MainMenu;

        static string nextAdd = "";
        static int localPlayerNum = 2;
        static int localPlayerScoreLimit = 3;
        static string cLocalCharID = "";

        static LocalGamePlay activeLocalGame;

        static Music backgroundMusic;
        
        

        static void Main(string[] args)
        {
            backgroundMusic = new Music("test.wav");
            backgroundMusic.Play();

            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            WriteWelcomeMessage();
            while (true)
            {
                Console.Clear();
                switch (CurrentMenu)
                {
                    case Menus.MainMenu:
                        Console.WriteLine("--- Main Menu ---");
                        Console.WriteLine("1. Play Local Game");
                        Console.WriteLine("2. Play Online Game");
                        Console.WriteLine("3. Music Controller");
                        Console.WriteLine("4. Quit");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalSetup:
                        Console.WriteLine("--- New Local Game ---");
                        Console.WriteLine("1. Number of players: " + localPlayerNum);
                        Console.WriteLine("2. Score limit: " + localPlayerScoreLimit);
                        Console.WriteLine("3. ASCII Name (max 10 char): " + cLocalCharID);
                        Console.WriteLine("4. Start Game");
                        Console.WriteLine("5. Back to Main Menu");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalPlayerNum:
                        Console.Write("Number of players: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalPlayerScore:
                        Console.Write("Score Limit: ");
                        DoCommand(Console.ReadLine());
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalChoose:
                        Console.WriteLine("--- Local Game ---");
                        Console.WriteLine("1. Create new local game");
                        Console.WriteLine("2. Load a local game");
                        Console.WriteLine("3. Back");
                        Console.Write(">");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.InLocalGame:
                        LocalGameFunc();
                        break;
                    case Menus.Music:
                        Console.WriteLine("--- Music Controller ---");
                        Console.WriteLine("1. Start/Stop");
                        Console.WriteLine("2. Back");
                        DoCommand(Console.ReadLine());
                        break;
                    case Menus.LocalGameName:
                        Console.Write("ASCII Name (10 char max): ");
                        string s = Console.ReadLine();
                        if (CheckAscii(s, out s))
                        {
                            cLocalCharID = s;
                        }
                        CurrentMenu = Menus.LocalSetup;
                        break;
                    case Menus.LocalLoad:
                        ListLocalGames();
                        break;
                    default:
                        break;
                }
                if (nextAdd != "")
                {
                    Console.WriteLine(nextAdd);
                    nextAdd = "";
                    Console.WriteLine("Press any button to continue");
                    Console.ReadKey();
                }
            }
        }

        static void ListLocalGames()
        {
            string savePath = path + "savefiles.sspl";
            if (!File.Exists(savePath))
            {
                CurrentMenu = Menus.LocalChoose;
                nextAdd += "No save games found. Create a new local game.";
                return;
            }

            FileStream f = File.Open(savePath, FileMode.Open);

            List<LocalGamePlay> games = new List<LocalGamePlay>();

            while (true)
            {
                if (f.ReadByte() == -1)
                {
                    break;
                }
                int id = f.ReadByte();
                string name = "";
                for (int i = 0; i < 10; i++)
                {
                    name += (char)f.ReadByte();
                }
                int playerNum = f.ReadByte();
                int scoreLimit = f.ReadByte();
                Player[] players = new Player[playerNum];
                for (int i = 0; i < playerNum; i++)
                {
                    players[i] = new Player(f.ReadByte());
                }
                int playerTurn = f.ReadByte();
                f.Close();

                games.Add(new LocalGamePlay(playerNum, players, scoreLimit, id, name));
            }

            int listSize = 10;
            int page = 0;

            Console.WriteLine("--- Load Local Game ---");
            for (int i = page * listSize; i < page * listSize + listSize; i++)
            {
                if (i < games.Count)
                {
                    Console.WriteLine(games[i].localGameID + ". " + games[i].Name + " - " + games[i].playerNum + " players - score limit " + games[i].scoreLimit);
                }
            }

            Console.WriteLine("a. Save game options");
            Console.WriteLine("b. Back");
            Console.Write(">");
            Console.ReadLine();
            
        }

        static bool CheckAscii(string s, out string so)
        {
            char[] c = new char[s.Length <= 10 ? s.Length : 10];
            for (int i = 0; i < (s.Length <= 10 ? s.Length : 10); i++)
            {
                c[i] = s[0];
            }

            if (s.Length < 10)
            {
                for (int i = s.Length; i < 10; i++)
                {
                    s += " ";
                }
            }

            so = s;

            foreach (char ch in c)
            {
                if (ch >= 128)
                {
                    return false;
                }
            }

            return true;
        }

        static void CreateLocalGame()
        {
            string savePath = path + "savefiles.sspl";
            
            if (!File.Exists(savePath))
            {
                File.Create(savePath);
            }
            
            string saveData = "";

            saveData += (char)211;


            FileStream f = File.Open(savePath, FileMode.Open);

            int c = 0;
            while (true)
            {
                int b = f.ReadByte();
                if (b == 211)
                {
                    c++;
                }
                else if (b == -1)
                {
                    break;
                }
            }

            saveData += (char)c;

            saveData += cLocalCharID;

            saveData += (char)localPlayerNum;

            saveData += (char)localPlayerScoreLimit;

            for (int i = 0; i < localPlayerNum; i++)
            {
                saveData += (char)0;
            }
            Random r = new Random();
            saveData += (char)r.Next(0, localPlayerNum);
            byte[] stream = Encoding.ASCII.GetBytes(saveData);
            f.Write(stream, 0, 14 + localPlayerNum);
            f.Close();

            Player[] tpls = new Player[localPlayerNum];

            for (int i = 0; i < localPlayerNum; i++)
            {
                tpls[i] = new Player(i);
            }

            activeLocalGame = new LocalGamePlay(localPlayerNum, tpls, localPlayerScoreLimit, c, cLocalCharID);
        }

        static void LocalGameFunc()
        {
            if (activeLocalGame == null)
            {
                CurrentMenu = Menus.MainMenu;
                nextAdd += "No active game; please load one or create a new one";
                return;
            }

            while (true)
            {
                DrawHUD();
            }
        }

        static void DrawHUD()
        {

        }

        static void WriteWelcomeMessage()
        {
            Console.Write("  ______                      ______                ______                 \n" +
                          " / _____) _                  / _____)              (_____ \\                \n" +
                          "( (____ _| |_ _____ ____    ( (____  _____ _   _    _____) )__   ___ _____ \n" +
                          " \\____ (_   _) ___ |  _ \\    \\____ \\(____ ( \\ / )  |  ____/ _ \\ /___) ___ |\n" +
                          " _____) )| |_| ____| | | |   _____) ) ___ |) X (   | |   | |_| |___ | ____|\n" +
                          "(______/  \\__)_____)_| |_|  (______/\\_____(_/ \\_)  |_|    \\___/(___/|_____)\n" +
                          "===========================================================================\n");

            Console.WriteLine("If you need help with anything, just write '!h' for contextual help!");
            Console.WriteLine("Press any button to continue.");
            Console.ReadKey();
        }

        static void DoCommand(string s)
        {
            if (s == "!h")
            {
                WriteHelp();
            }
            else if (s == "!b" || s == "back")
            {
                switch (CurrentMenu)
                {
                    
                    case Menus.LocalChoose:
                    case Menus.OnlineSetup1:
                        CurrentMenu = Menus.MainMenu;
                        break;
                    case Menus.LocalLoad:
                    case Menus.LocalSetup:
                        CurrentMenu = Menus.LocalChoose;
                        break;
                    default:
                        break;
                }
            }

            switch (CurrentMenu)
            {
                case Menus.MainMenu:

                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalChoose;
                            break;
                        case "2":
                            CurrentMenu = Menus.OnlineSetup1;
                            break;
                        case "3":
                            CurrentMenu = Menus.Music;
                            break;
                        case "4":
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }

                    break;
                case Menus.LocalSetup:

                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalPlayerNum;
                            break;
                        case "2":
                            CurrentMenu = Menus.LocalPlayerScore;
                            break;
                        case "3":
                            CurrentMenu = Menus.LocalGameName;
                            break;
                        case "4":
                            CreateLocalGame();
                            CurrentMenu = Menus.InLocalGame;
                            break;
                        case "5":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }

                    break;
                case Menus.LocalPlayerNum:
                    int.TryParse(s, out localPlayerNum);
                    break;
                case Menus.LocalPlayerScore:
                    int.TryParse(s, out localPlayerScoreLimit);
                    break;
                case Menus.LocalChoose:
                    switch (s)
                    {
                        case "1":
                            CurrentMenu = Menus.LocalSetup;
                            break;
                        case "2":
                            CurrentMenu = Menus.LocalLoad;
                            break;
                        case "3":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }
                    break;
                case Menus.Music:
                    switch (s)
                    {
                        case "1":
                            if (backgroundMusic.Playing())
                                backgroundMusic.Stop();
                            else
                                backgroundMusic.Play();

                            break;
                        case "2":
                            CurrentMenu = Menus.MainMenu;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        static void WriteHelp()
        {
            switch (CurrentMenu)
            {
                case Menus.MainMenu:
                    nextAdd = "\nThis is the main menu. Type one of the numbers shown on screen to choose that option.";
                    break;
                default:
                    break;
            }
        }
    }
}
