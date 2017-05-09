using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StenSaxPose
{
    /// <summary>
    /// The class for a locak player, game specific
    /// </summary>
    public class LocalPlayer
    {
        /// <summary>
        /// The ID of the local player
        /// </summary>
        public int ID;
        /// <summary>
        /// The score of the player
        /// </summary>
        public int Score;

        /// <summary>
        /// Creates a new local player with the specified ID
        /// </summary>
        /// <param name="id">The ID of the local player</param>
        public LocalPlayer(int id)
        {
            ID = id;
        }
    }

    /// <summary>
    /// Menu enum
    /// </summary>
    enum Menus { MainMenu, LocalChoose, LocalLoad, LocalSave, LocalGameName, LocalSetup, LocalPlayerNum, LocalPlayerScore, OnlineSetup1, InLocalGame, InOnlineGame, Music }

    /// <summary>
    /// Main Program, contains the bulk of the game
    /// </summary>
    class Program
    {
        /// <summary>
        /// The path to the .exe at runtime
        /// </summary>
        static string path;
        /// <summary>
        /// The current menu, or the menu to be shown next redraw
        /// </summary>
        static Menus CurrentMenu = Menus.MainMenu;

        /// <summary>
        /// Extra information to add on the next redraw
        /// </summary>
        static string nextAdd = "";
        /// <summary>
        /// Number of players in created local games
        /// </summary>
        static int localPlayerNum = 2;
        /// <summary>
        /// Score limit of created local games
        /// </summary>
        static int localPlayerScoreLimit = 3;
        /// <summary>
        /// NAme of created local games
        /// </summary>
        static string cLocalCharID = "";

        /// <summary>
        /// The currently playing local game, if any
        /// </summary>
        static LocalGamePlay activeLocalGame;

        /// <summary>
        /// The background music. 10/10
        /// </summary>
        static Music backgroundMusic;
        
        
        /// <summary>
        /// Entry point of the program
        /// </summary>
        /// <param name="args">I have no idea what this is</param>
        static void Main(string[] args)
        {
            // Assigning variables
            backgroundMusic = new Music("test.wav");
            backgroundMusic.Play();

            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";

            // Writing the welcome message with ASCII Art by /SaftLasso
            WriteWelcomeMessage();

            // Handles the rewrite and the menus, except for the in-game one and the load menu
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

        /// <summary>
        /// Reads all games from file and displays them for loading.
        /// </summary>
        static void ListLocalGames()
        {
            // Checking if there is a save file and, if there is, opening it
            string savePath = path + "savefiles.sspl";
            if (!File.Exists(savePath))
            {
                CurrentMenu = Menus.LocalChoose;
                nextAdd += "No save games found. Create a new local game.";
                return;
            }

            FileStream f = File.Open(savePath, FileMode.Open);

            // Creating a list to hold all of the saved games
            List<LocalGamePlay> games = new List<LocalGamePlay>();

            // Reading one game each loop, adding to the above list
            // Byte order:
            //
            // Header (211)         1 byte
            // Game ID              1 byte
            // Name in ASCII        10 bytes
            // Number of players    1 bytebyte
            // Score limit          1 byte
            // Player score         1 byte per player
            // Player turn          1 byte
            //
            // Total: 25 + number of players
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
                LocalPlayer[] players = new LocalPlayer[playerNum];
                for (int i = 0; i < playerNum; i++)
                {
                    players[i] = new LocalPlayer(f.ReadByte());
                }
                int playerTurn = f.ReadByte();
                f.Close();

                games.Add(new LocalGamePlay(playerNum, players, scoreLimit, id, name));
            }

            // Initializing variables for use in displaying the above list
            int listSize = 10;
            int page = 0;

            // Displaying the list
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
            string s = Console.ReadLine();

            // Handling input (WIP)
            
        }

        /// <summary>
        /// Function to check if a string is ASCII and also making sure it is exactly 10 characters long
        /// </summary>
        /// <param name="s">The input string</param>
        /// <param name="so">The output string, 10 characters long</param>
        /// <returns></returns>
        static bool CheckAscii(string s, out string so)
        {
            // Limiting the string to 10 characters or less
            string ss = "";
            for (int i = 0; i < (s.Length <= 10 ? s.Length : 10); i++)
            {
                ss += s[i];
            }

            // Filling out the string to 10 characters
            if (s.Length < 10)
            {
                for (int i = s.Length; i < 10; i++)
                {
                    ss += " ";
                }
            }

            // Outputting the string
            so = ss;

            // Turning the string into a char array
            char[] c = new char[10];
            for (int i = 0; i < 10; i++)
                c[i] = ss[i];

            // Checkin ASCII Value
            foreach (char ch in c)
            {
                if (ch >= 128)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creating a local game
        /// </summary>
        static void CreateLocalGame()
        {
            // Setting the path to the save file
            string savePath = path + "savefiles.sspl";
            
            if (!File.Exists(savePath))
            {
                File.Create(savePath);
            }
            
            // Encoding data to a string
            // Byte order can be found in the load function
            string saveData = "";

            saveData += (char)211;


            FileStream f = File.Open(savePath, FileMode.Open);

            // Counting the headers to calculate the next game ID
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

            // Creating local players and setting the active local game to this one
            LocalPlayer[] tpls = new LocalPlayer[localPlayerNum];

            for (int i = 0; i < localPlayerNum; i++)
            {
                tpls[i] = new LocalPlayer(i);
            }

            activeLocalGame = new LocalGamePlay(localPlayerNum, tpls, localPlayerScoreLimit, c, cLocalCharID);
        }

        // Handling in-game logic (WIP)
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
                
            }
        }

        // Writes a welcome message with accompanying ASCII art by /SaftLasso
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

        // Handles menu choices except those in-game and in the load menu
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

        // Writes help (WIP - low priority)
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
